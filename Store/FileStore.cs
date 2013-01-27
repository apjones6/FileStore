using System;
using System.IO;

namespace Store
{
    public class SimpleFileStore : IFileStore
    {
        private readonly IHandleStore handleStore;
        private readonly string root;

        public SimpleFileStore(string root)
            : this(root, new MemoryHandleStore())
        {
        }

        public SimpleFileStore(string root, IHandleStore handleStore)
        {
            this.handleStore = handleStore;
            this.root = root;
        }

        public FileHandle Retrieve(Guid id)
        {
            return handleStore.Get(id);
        }

        public FileHandle Insert(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            var id = Guid.NewGuid();
            var destination = Path.Combine(root, id.ToString());

            using (var filestream = File.Create(destination))
            {
                stream.CopyTo(filestream);
            }

            var handle = new FileHandle(id, destination);
            handleStore.Insert(handle);

            return handle;
        }

        public FileHandle Insert(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!File.Exists(path)) throw new FileNotFoundException("The file could not be found", path);

            var id = Guid.NewGuid();
            var destination = Path.Combine(root, string.Concat(id, Path.GetExtension(path)));

            File.Copy(path, destination);

            var handle = new FileHandle(id, destination);
            handleStore.Insert(handle);

            return handle;
        }

        public FileHandle Duplicate(Guid id)
        {
            var handle = handleStore.Get(id);
            if (handle == null)
            {
                throw new ArgumentException("No handle with id found.", "id");
            }

            var duplicate = new FileHandle(Guid.NewGuid(), handle.Path);

            handleStore.Insert(duplicate);

            return duplicate;
        }

        public FileHandle Replace(Guid id, Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            var handle = handleStore.Get(id);
            if (handle == null)
            {
                throw new ArgumentException("No handle with id found.", "id");
            }

            var destination = Path.Combine(root, Guid.NewGuid().ToString());

            using (var filestream = File.Create(destination))
            {
                stream.CopyTo(filestream);
            }

            var replacement = new FileHandle(id, destination);
            handleStore.Update(replacement);

            if (handleStore.Count(handle.Path) == 0)
            {
                File.Delete(handle.Path);
            }

            return replacement;
        }

        public FileHandle Replace(Guid id, string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!File.Exists(path)) throw new FileNotFoundException("The file could not be found", path);

            var handle = handleStore.Get(id);
            if (handle == null)
            {
                throw new ArgumentException("No handle with id found.", "id");
            }

            var destination = Path.Combine(root, string.Concat(Guid.NewGuid(), Path.GetExtension(path)));
            File.Copy(path, destination);

            var replacement = new FileHandle(id, destination);
            handleStore.Update(replacement);

            if (handleStore.Count(handle.Path) == 0)
            {
                File.Delete(handle.Path);
            }

            return replacement;
        }

        public void Remove(Guid id)
        {
            var handle = handleStore.Get(id);
            if (handle == null)
            {
                throw new ArgumentException("No handle with id found.", "id");
            }

            handleStore.Remove(id);

            if (handleStore.Count(handle.Path) == 0)
            {
                File.Delete(handle.Path);
            }
        }
    }
}

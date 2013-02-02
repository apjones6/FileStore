using System;
using System.IO;

namespace Store
{
    public class SimpleFileStore : IFileStore
    {
        private readonly IHandleStore handleStore;
        private readonly IFileLocator fileLocator;
        private readonly string root;

        public SimpleFileStore(string root)
            : this(root, new MemoryHandleStore())
        {
        }

        public SimpleFileStore(string root, IHandleStore handleStore)
            : this(root, handleStore, new SimpleFileLocator())
        {
        }

        public SimpleFileStore(string root, IHandleStore handleStore, IFileLocator fileLocator)
        {
            this.handleStore = handleStore;
            this.handleStore.LastHandleRemoved += OnLastHandleRemoved;
            this.fileLocator = fileLocator;
            this.root = root;
        }

        public void Initialize()
        {
            fileLocator.PrepareDirectory(root);
        }

        public FileHandle Retrieve(Guid id)
        {
            return FixPath(handleStore.Get(id));
        }

        public FileHandle Insert(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            var id = Guid.NewGuid();
            var destination = fileLocator.GetPath(id);

            using (var filestream = File.Create(Path.Combine(root, destination)))
            {
                stream.CopyTo(filestream);
            }

            var handle = new FileHandle(id, destination);
            handleStore.Insert(handle);

            return FixPath(handle);
        }

        public FileHandle Insert(string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!File.Exists(path)) throw new FileNotFoundException("The file could not be found", path);

            var id = Guid.NewGuid();
            var destination = fileLocator.GetPath(path);

            File.Copy(path, Path.Combine(root, destination));

            var handle = new FileHandle(id, destination);
            handleStore.Insert(handle);

            return FixPath(handle);
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

            return FixPath(duplicate);
        }

        public FileHandle Replace(Guid id, Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            var destination = fileLocator.GetPath(Guid.NewGuid());

            using (var filestream = File.Create(Path.Combine(root, destination)))
            {
                stream.CopyTo(filestream);
            }

            var replacement = new FileHandle(id, destination);
            handleStore.Update(replacement);

            return FixPath(replacement);
        }

        public FileHandle Replace(Guid id, string path)
        {
            if (path == null) throw new ArgumentNullException("path");
            if (!File.Exists(path)) throw new FileNotFoundException("The file could not be found", path);

            var destination = fileLocator.GetPath(path);
            File.Copy(path, Path.Combine(root, destination));

            var replacement = new FileHandle(id, destination);
            handleStore.Update(replacement);

            return FixPath(replacement);
        }

        public void Remove(Guid id)
        {
            handleStore.Remove(id);
        }

        private FileHandle FixPath(FileHandle handle)
        {
            return new FileHandle(handle.Id, Path.Combine(root, handle.Path));
        }

        private void OnLastHandleRemoved(object sender, FileHandleEventArgs e)
        {
            var path = Path.Combine(root, e.Handle.Path);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}

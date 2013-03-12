using System;
using System.IO;
using System.Linq;

namespace Store
{
    public class SimpleFileStore : IFileStore
    {
        private readonly IHandleStore handleStore;
        private readonly string root;

        private long? totalSize;

        public SimpleFileStore(string root)
            : this(root, new MemoryHandleStore())
        {
        }

        public SimpleFileStore(string root, IHandleStore handleStore)
        {
            this.handleStore = handleStore;
            this.handleStore.LastHandleRemoved += OnLastHandleRemoved;
            this.root = root;
        }

        public long TotalSize
        {
            get
            {
                if (totalSize == null)
                {
                    totalSize = Directory.EnumerateFiles(root, string.Empty, SearchOption.AllDirectories).Sum(x => new FileInfo(x).Length);
                }

                return totalSize.Value;
            }
        }

        public void Initialize()
        {
            for (var i = 0; i < 16 * 16; ++i)
            {
                Directory.CreateDirectory(Path.Combine(root, i.ToString("X2")));
            }
        }

        public FileHandle Retrieve(Guid id)
        {
            return handleStore.Get(id);
        }
        
        public FileHandle Insert(Stream stream, string filename)
        {
            var handle = new FileHandle(Guid.NewGuid(), Locate(filename));
            using (var filestream = File.Create(handle.Path))
            {
                stream.CopyTo(filestream);
            }

            handleStore.Insert(handle);
            if (totalSize != null)
            {
                totalSize += handle.Size;
            }

            return handle;
        }
        
        public FileHandle Insert(string path)
        {
            var handle = new FileHandle(Guid.NewGuid(), Locate(path));
            File.Copy(path, handle.Path);

            handleStore.Insert(handle);
            if (totalSize != null)
            {
                totalSize += handle.Size;
            }

            return handle;
        }

        public FileHandle Duplicate(Guid id)
        {
            var handle = handleStore.Get(id);
            var duplicate = new FileHandle(Guid.NewGuid(), handle.Path);

            handleStore.Insert(duplicate);

            return duplicate;
        }
        
        public FileHandle Replace(Guid id, Stream stream, string filename)
        {
            var replacement = new FileHandle(id, Locate(filename));
            using (var filestream = File.Create(replacement.Path))
            {
                stream.CopyTo(filestream);
            }

            handleStore.Update(replacement);
            if (totalSize != null)
            {
                totalSize += replacement.Size;
            }

            return replacement;
        }
        
        public FileHandle Replace(Guid id, string path)
        {
            var replacement = new FileHandle(id, Locate(path));
            File.Copy(path, replacement.Path);

            handleStore.Update(replacement);
            if (totalSize != null)
            {
                totalSize += replacement.Size;
            }

            return replacement;
        }

        public void Remove(Guid id)
        {
            handleStore.Remove(id);
        }

        private string Locate(string path)
        {
            var id = Guid.NewGuid().ToString();
            return Path.Combine(root, id.Substring(0, 2), string.Concat(id.Substring(2), Path.GetExtension(path)));
        }

        private void OnLastHandleRemoved(object sender, FileHandleEventArgs e)
        {
            if (File.Exists(e.Handle.Path))
            {
                if (totalSize != null)
                {
                    totalSize -= e.Handle.Size;
                }

                File.Delete(e.Handle.Path);
            }
        }
    }
}

using System;
using System.IO;

namespace Store
{
    public class FileStoreLogWrapper : IFileStore
    {
        private readonly IFileStore store;

        public FileStoreLogWrapper(IFileStore store)
        {
            this.store = store;
        }

        public long TotalSize
        {
            get { return store.TotalSize; }
        }

        public void Initialize()
        {
            store.Initialize();
        }

        public FileHandle Retrieve(Guid id)
        {
            return store.Retrieve(id);
        }
        
        public FileHandle Insert(Stream stream, string filename)
        {
            return store.Insert(stream, filename);
        }
        
        public FileHandle Insert(string path)
        {
            var handle = store.Insert(path);

            Write("INSERT", handle);

            return handle;
        }

        public FileHandle Duplicate(Guid id)
        {
            var handle = store.Duplicate(id);

            Write("DUPLICATE", handle);

            return handle;
        }
        
        public FileHandle Replace(Guid id, Stream stream, string filename)
        {
            return store.Replace(id, stream, filename);
        }
        
        public FileHandle Replace(Guid id, string path)
        {
            var handle = store.Replace(id, path);

            Write("REPLACE", handle);

            return handle;
        }

        public void Remove(Guid id)
        {
            Write("REMOVE", id);
            store.Remove(id);
        }

        private static void Write(string action, Guid id)
        {
            Console.Write(action + " [id=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(id);
            Console.ResetColor();
            Console.Write("]");

            Console.WriteLine();
        }

        private static void Write(string action, FileHandle handle)
        {
            Console.Write(action + " [id=");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(handle.Id);
            Console.ResetColor();
            Console.Write("|path=");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(handle.Path);
            Console.ResetColor();
            Console.Write("]");

            Console.WriteLine();
        }
    }
}

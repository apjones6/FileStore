using System;
using System.Collections.Generic;
using System.Linq;

namespace Store
{
    public class MemoryHandleStore : IHandleStore
    {
        private readonly List<FileHandle> handles = new List<FileHandle>();

        public FileHandle Get(Guid id)
        {
            return handles.FirstOrDefault(x => x.Id == id);
        }

        public int Count(string path)
        {
            return handles.Count(x => x.Path == path);
        }

        public void Insert(FileHandle handle)
        {
            handles.Add(handle);
        }

        public void Update(FileHandle handle)
        {
            Remove(handle.Id);
            Insert(handle);
        }

        public void Remove(Guid id)
        {
            handles.RemoveAll(x => x.Id == id);
        }
    }
}

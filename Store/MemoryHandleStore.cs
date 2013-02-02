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
            var handle = handles.FirstOrDefault(x => x.Id == id);
            if (handle != null)
            {
                handles.RemoveAll(x => x.Id == id);

                // Notify if last handle removed
                if (LastHandleRemoved != null && Count(handle.Path) == 0)
                {
                    var e = new FileHandleEventArgs(handle);
                    LastHandleRemoved(this, e);
                }
            }
        }

        public event EventHandler<FileHandleEventArgs> LastHandleRemoved;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Cdsm.FileStorage
{
    public class MemoryHandleStore : IHandleStore
    {
        private readonly Dictionary<Guid, FileHandle> handles = new Dictionary<Guid, FileHandle>();

        public event EventHandler<FileHandleEventArgs> LastHandleRemoved;

        public IDictionary<Guid, FileHandle> Get(IEnumerable<Guid> ids)
        {
            return handles.Where(x => ids.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
        }

        public FileHandle Get(Guid id)
        {
            FileHandle handle;
            return handles.TryGetValue(id, out handle) ? handle : null;
        }

        public void Insert(FileHandle handle)
        {
            if (handles.ContainsKey(handle.Id))
            {
                throw new InvalidOperationException("Key already exists.");
            }

            handles[handle.Id] = handle;
        }

        public void Update(FileHandle handle)
        {
            FileHandle previous;
            if (!handles.TryGetValue(handle.Id, out previous))
            {
                throw new InvalidOperationException("Key not found.");
            }

            handles[handle.Id] = handle;

            CheckLast(previous);
        }

        public void Remove(Guid id)
        {
            FileHandle previous;
            if (!handles.TryGetValue(id, out previous))
            {
                throw new InvalidOperationException("Key not found.");
            }

            handles.Remove(id);

            CheckLast(previous);
        }

        private void CheckLast(FileHandle handle)
        {
            var handler = LastHandleRemoved;
            if (handler != null)
            {
                if (handles.Values.Count(x => x.Uri == handle.Uri && x.Repository == handle.Repository) == 0)
                {
                    var e = new FileHandleEventArgs(handle);
                    handler(this, e);
                }
            }
        }
    }
}

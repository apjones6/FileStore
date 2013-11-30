using System;
using System.Collections.Generic;

namespace Cdsm.FileStorage
{
    public interface IHandleStore
    {
        event EventHandler<FileHandleEventArgs> LastHandleRemoved;

        IDictionary<Guid, FileHandle> Get(IEnumerable<Guid> ids);
        FileHandle Get(Guid id);

        void Insert(FileHandle handle);
        void Update(FileHandle handle);
        void Remove(Guid id);
    }
}

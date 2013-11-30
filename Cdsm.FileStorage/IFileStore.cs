using System;
using System.Collections.Generic;
using System.IO;

namespace Cdsm.FileStorage
{
    public interface IFileStore
    {
        IDictionary<Guid, FileHandle> Get(IEnumerable<Guid> ids);
        FileHandle Get(Guid id);

        FileHandle Insert(Stream stream, string filename);
        FileHandle Insert(string filename);

        FileHandle Duplicate(Guid id);

        FileHandle Replace(Guid id, Stream stream, string filename);
        FileHandle Replace(Guid id, string filename);

        void Remove(Guid id);
    }
}

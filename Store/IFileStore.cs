using System;
using System.IO;

namespace Store
{
    public interface IFileStore
    {
        FileHandle Retrieve(Guid id);

        FileHandle Insert(Stream stream);
        FileHandle Insert(string path);

        FileHandle Duplicate(Guid id);

        FileHandle Replace(Guid id, Stream stream);
        FileHandle Replace(Guid id, string path);

        void Remove(Guid id);
    }
}

using System;
using System.IO;

namespace Store
{
    public interface IFileStore
    {
        long TotalSize { get; }

        void Initialize();

        FileHandle Retrieve(Guid id);

        FileHandle Insert(Stream stream, string filename);
        FileHandle Insert(string path);

        FileHandle Duplicate(Guid id);

        FileHandle Replace(Guid id, Stream stream, string filename);
        FileHandle Replace(Guid id, string path);

        void Remove(Guid id);
    }
}

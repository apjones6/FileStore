using System;

namespace Store
{
    public interface IHandleStore
    {
        FileHandle Get(Guid id);

        int Count(string path);

        void Insert(FileHandle handle);
        void Update(FileHandle handle);
        void Remove(Guid id);
    }
}

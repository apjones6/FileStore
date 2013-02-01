using System;

namespace Store
{
    public interface IFileLocator
    {
        void PrepareDirectory(string directory);

        string GetPath(string filename);
        string GetPath(Guid id);
    }
}

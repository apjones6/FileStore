using System.IO;

namespace Cdsm.FileStorage
{
    public interface IFileRepository
    {
        string Name { get; }

        FileHandle Insert(Stream stream, string filename);
        FileHandle Insert(string filename);

        void Remove(FileHandle handle);
    }
}

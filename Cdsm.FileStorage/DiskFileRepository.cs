using System;
using System.IO;

namespace Cdsm.FileStorage
{
    public class DiskFileRepository : IFileRepository
    {
        private readonly string rootDirectory;
        private readonly string name;

        public DiskFileRepository(string name, string rootDirectory)
        {
            this.rootDirectory = rootDirectory;
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public FileHandle Insert(Stream stream, string filename)
        {
            var uri = Locate(filename);
            var path = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (var filestream = File.Create(path))
            {
                stream.CopyTo(filestream);
            }

            return new FileHandle(Guid.NewGuid(), uri, filename, new FileInfo(path).Length, name);
        }

        public FileHandle Insert(string filename)
        {
            var uri = Locate(filename);
            var path = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.Copy(filename, path);

            return new FileHandle(Guid.NewGuid(), uri, Path.GetFileName(filename), new FileInfo(filename).Length, name);
        }

        public void Remove(FileHandle handle)
        {
            var path = handle.Uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private Uri Locate(string filename)
        {
            var id = Guid.NewGuid().ToString();
            var path = Path.GetFullPath(Path.Combine(rootDirectory, id.Substring(0, 2), string.Concat(id.Substring(2), Path.GetExtension(filename))));
            return new Uri("file:///" + path, UriKind.Absolute);
        }
    }
}

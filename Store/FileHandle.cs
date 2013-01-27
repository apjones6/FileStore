using System;
using System.IO;

namespace Store
{
    public sealed class FileHandle
    {
        private readonly Guid id;
        private readonly string path;
        private readonly long size;

        public FileHandle(Guid id, string path)
        {
            this.id = id;
            this.path = path;
            if (File.Exists(path))
            {
                this.size = new FileInfo(path).Length;
            }
        }

        public FileHandle(Guid id, string path, long size)
        {
            this.id = id;
            this.path = path;
            this.size = size;
        }

        public Guid Id
        {
            get { return id; }
        }

        public string Path
        {
            get { return path; }
        }

        public long Size
        {
            get { return size; }
        }
    }
}

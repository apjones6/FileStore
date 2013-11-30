using System;
using System.IO;

namespace Cdsm.FileStorage
{
    public sealed class FileHandle
    {
        private readonly Guid id;
        private readonly string filename;
        private readonly long length;
        private readonly string repository;
        private readonly Uri uri;

        public FileHandle(Guid id, Uri uri, string filename, long length, string repository)
        {
            this.id = id;
            this.filename = filename;
            this.length = length;
            this.repository = repository;
            this.uri = uri;
        }

        public FileHandle(Guid id, FileHandle source)
            : this(id, source.uri, source.filename, source.length, source.repository)
        {
        }

        public Guid Id
        {
            get { return id; }
        }

        public string FileName
        {
            get { return filename; }
        }

        public long Length
        {
            get { return length; }
        }

        public string Repository
        {
            get { return repository; }
        }

        public Stream Stream
        {
            get { throw new NotImplementedException(); }
        }

        public Uri Uri
        {
            get { return uri; }
        }
    }
}

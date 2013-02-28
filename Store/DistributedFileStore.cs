using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Store
{
    public class DistributedFileStore : IFileStore
    {
        private readonly Dictionary<Location, IFileStore> stores;

        public DistributedFileStore(Location[] locations)
        {
            this.stores = locations.ToDictionary(x => x, x => (IFileStore)new SimpleFileStore(x.Path));
        }

        public long TotalSize
        {
            get { return stores.Values.Sum(x => x.TotalSize); }
        }

        public void Initialize()
        {
            foreach (var store in stores.Values)
            {
                store.Initialize();
            }
        }

        public FileHandle Retrieve(Guid id)
        {
            return stores.Values.Select(x => x.Retrieve(id)).FirstOrDefault();
        }

        public FileHandle Insert(Stream stream, string filename)
        {
            return StoreFor(stream.Length).Insert(stream, filename);
        }

        public FileHandle Insert(string path)
        {
            return StoreFor(path).Insert(path);
        }

        public FileHandle Duplicate(Guid id)
        {
            return StoreFor(id).Duplicate(id);
        }

        public FileHandle Replace(Guid id, Stream stream, string filename)
        {
            return StoreFor(id).Duplicate(id);
        }

        public FileHandle Replace(Guid id, string path)
        {
            return StoreFor(id).Replace(id, path);
        }

        public void Remove(Guid id)
        {
            StoreFor(id).Remove(id);
        }

        private IFileStore StoreFor(string path)
        {
            return StoreFor(new FileInfo(path).Length);
        }

        private IFileStore StoreFor(long length)
        {
            var weights = stores.ToDictionary(x => x.Key, x => 1 - (double)x.Value.TotalSize / x.Key.MaxSize);

            var store = stores
                .OrderByDescending(x => weights[x.Key])
                .Where(x => (x.Key.MaxSize - x.Value.TotalSize) >= length)
                .Select(x => x.Value)
                .FirstOrDefault();

            if (store == null)
            {
                throw new IOException("No FileStores have sufficient available space.");
            }

            return store;
        }

        private IFileStore StoreFor(Guid id)
        {
            return stores.Values.FirstOrDefault(x => x.Retrieve(id) != null);
        }

        public class Location
        {
            private readonly string path;
            private readonly long maxSize;

            public Location(string path, long maxSize)
            {
                this.path = path;
                this.maxSize = maxSize;
            }

            public string Path
            {
                get { return path; }
            }

            public long MaxSize
            {
                get { return maxSize; }
            }
        }
    }
}

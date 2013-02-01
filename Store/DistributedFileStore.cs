using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Store
{
    public class DistributedFileStore : IFileStore
    {
        private readonly Dictionary<Location, IFileStore> stores;
        private readonly IHandleStore handleStore;

        public DistributedFileStore(Location[] locations)
            : this(locations, new MemoryHandleStore())
        {
        }

        public DistributedFileStore(Location[] locations, IHandleStore handleStore)
        {
            if (locations == null) throw new ArgumentNullException("locations");
            if (locations.Length == 0) throw new ArgumentException("At least one location is required.", "locations");
            if (locations.GroupBy(x => x.Path).Any(x => x.Count() > 1))
            {
                throw new ArgumentException("All locations must be distinct.", "locations");
            }

            this.stores = locations.ToDictionary(x => x, x => (IFileStore)new SimpleFileStore(x.Path, handleStore));
            this.handleStore = handleStore;
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
            throw new NotImplementedException();
        }

        public FileHandle Insert(Stream stream)
        {
            throw new NotImplementedException();
        }

        public FileHandle Insert(string path)
        {
            throw new NotImplementedException();
        }

        public FileHandle Duplicate(Guid id)
        {
            throw new NotImplementedException();
        }

        public FileHandle Replace(Guid id, Stream stream)
        {
            throw new NotImplementedException();
        }

        public FileHandle Replace(Guid id, string path)
        {
            throw new NotImplementedException();
        }

        public void Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public class Location
        {
            private readonly string path;
            private readonly long? maxSize;

            public Location(string path)
            {
                this.path = path;
                this.maxSize = null;
            }

            public Location(string path, long maxSize)
            {
                this.path = path;
                this.maxSize = maxSize;
            }

            public string Path
            {
                get { return path; }
            }

            public long? MaxSize
            {
                get { return maxSize; }
            }
        }
    }
}

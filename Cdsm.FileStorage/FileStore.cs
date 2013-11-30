using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cdsm.FileStorage
{
    public class FileStore : IFileStore
    {
        private readonly IFileRepository[] repositories;
        private readonly IHandleStore handles;

        public FileStore(IFileRepository[] repositories, IHandleStore handles)
        {
            this.handles = handles;
            this.handles.LastHandleRemoved += OnLastHandleRemoved;
            this.repositories = repositories;
        }

        public IDictionary<Guid, FileHandle> Get(IEnumerable<Guid> ids)
        {
            return handles.Get(ids);
        }

        public FileHandle Get(Guid id)
        {
            return handles.Get(id);
        }
        
        public FileHandle Insert(Stream stream, string filename)
        {
            var repository = PickRepository(filename);
            var handle = repository.Insert(stream, filename);
            handles.Insert(handle);
            return handle;
        }
        
        public FileHandle Insert(string filename)
        {
            var repository = PickRepository(filename);
            var handle = repository.Insert(filename);
            handles.Insert(handle);
            return handle;
        }

        public FileHandle Duplicate(Guid id)
        {
            var handle = handles.Get(id);
            var duplicate = new FileHandle(Guid.NewGuid(), handle);
            handles.Insert(duplicate);
            return duplicate;
        }
        
        public FileHandle Replace(Guid id, Stream stream, string filename)
        {
            var repository = PickRepository(filename);
            var handle = new FileHandle(id, repository.Insert(stream, filename));
            handles.Update(handle);
            return handle;
        }

        public FileHandle Replace(Guid id, string filename)
        {
            var repository = PickRepository(filename);
            var handle = new FileHandle(id, repository.Insert(filename));
            handles.Update(handle);
            return handle;
        }

        public void Remove(Guid id)
        {
            handles.Remove(id);
        }

        protected virtual IFileRepository PickRepository(string filename, IFileRepository[] repositories)
        {
            // TODO: Pick which repository based on extension and other stuff...
            return repositories.First();
        }

        private IFileRepository PickRepository(string filename)
        {
            return PickRepository(filename, repositories);
        }

        private void OnLastHandleRemoved(object sender, FileHandleEventArgs e)
        {
            IFileRepository repository;
            if ((repository = repositories.SingleOrDefault(x => x.Name == e.Handle.Repository)) != null)
            {
                repository.Remove(e.Handle);
            }
            else
            {
                // TODO: Log a warning
            }
        }
    }
}

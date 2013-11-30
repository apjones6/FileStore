using System.IO;
using Cdsm.FileStorage;

namespace Demo
{
    class TestFileStore : FileStore
    {
        public TestFileStore(IFileRepository[] repositories, IHandleStore handles)
            : base(repositories, handles)
        {
        }

        protected override IFileRepository PickRepository(string filename, IFileRepository[] repositories)
        {
            var start = Path.GetFileName(filename)[0];
            if (start >= 'a' && start <= 'm')
            {
                return repositories[0];
            }
            else
            {
                return repositories[1];
            }
        }
    }
}

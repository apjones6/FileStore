using System;
using System.IO;
using System.Linq;

namespace Store
{
    public class SimpleFileLocator : IFileLocator
    {
        public void PrepareDirectory(string directory)
        {
            // Create all the folders
            for (var i = 0; i < 16; ++i)
            {
                for (var j = 0; j < 16; ++j)
                {
                    Directory.CreateDirectory(Path.Combine(directory, string.Concat(i.ToString("X"), j.ToString("X"))));
                }
            }
        }

        public string GetPath(string filename)
        {
            var extension = Path.GetExtension(filename);
            var id = Guid.NewGuid();
            return Path.Combine(id.ToString().Substring(0, 2), string.Concat(id, extension));
        }

        public string GetPath(Guid id)
        {
            return Path.Combine(id.ToString().Substring(0, 2), id.ToString());
        }
    }
}

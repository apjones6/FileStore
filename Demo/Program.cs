using System;
using System.IO;
using System.Linq;
using Cdsm.FileStorage;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(180, 50);

            var files = Directory.GetFiles("Content");
            if (files.Length < 5)
            {
                Console.WriteLine("You need at least 5 files.");
                Console.ReadKey();
                return;
            }

            // Initialize
            var store = new TestFileStore(new[] { new DiskFileRepository("A-M", "A-M Storage"), new DiskFileRepository("N-Z", "N-Z Storage") }, new MemoryHandleStore());

            // Test
            var handles = files.Select(x => store.Insert(x)).ToArray();
            var copies = handles.Select(x => store.Duplicate(x.Id)).ToArray();

            var handle = handles[0];
            var handleReplaced = store.Replace(handle.Id, files[1]);
            var copy = copies[0];
            using (var fs = File.OpenRead(files[2]))
            {
                var copyReplaced = store.Replace(copy.Id, fs, files[2]);
            }

            store.Remove(handle.Id);

            // Pause while inspecting input
            Console.WriteLine("Press any key to exit.");
            Console.WriteLine("This will delete the 'storage' directory, to ensure future tests are unaffected.");
            Console.ReadKey();

            // Cleanup
            Directory.Delete("A-M Storage", true);
            Directory.Delete("N-Z Storage", true);
        }
    }
}

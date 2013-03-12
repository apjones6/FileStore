using System;
using System.IO;

namespace Store
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(180, 50);

            var current = Directory.GetCurrentDirectory();
            var files = new[]
                {
                    Path.Combine(current, "Content/Karei's Healing Circle.jpg"),
                    Path.Combine(current, "Content/Radiant Scythe.jpg"),
                    Path.Combine(current, "Content/Searing Flames.jpg")
                };

            // Recycle output directory
            var root = Path.Combine(current, "Store");
            Directory.CreateDirectory(root);
            foreach (var path in Directory.EnumerateFiles(root, string.Empty, SearchOption.AllDirectories))
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }

            // Initialize
            var store = new FileStoreLogWrapper(new SimpleFileStore(root));
            store.Initialize();

            // Test
            var handle1 = store.Insert(files[0]);
            var handle2 = store.Duplicate(handle1.Id);
            store.Replace(handle1.Id, files[1]);
            store.Replace(handle2.Id, files[2]);
            store.Remove(handle1.Id);

            // Pause while inspecting input
            Console.ReadKey();
        }
    }
}

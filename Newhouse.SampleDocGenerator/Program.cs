using Microsoft.Extensions.Configuration;
using Newhouse.SampleDocGenerator.Models;

namespace Newhouse.SampleDocGenerator
{
    class Program
    {
        private static void WaitForAnyKey()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("*************************************");
            Console.WriteLine("***** SAMPLE DOCUMENT GENERATOR *****");
            Console.WriteLine("*************************************");
            Console.WriteLine();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);
            var config = builder.Build();
            var appSettings = config.GetSection("AppSettings").Get<AppSettings>();

            if (string.IsNullOrWhiteSpace(appSettings?.OutputLocation)
                || appSettings.NumberOfFiles < 1
                || appSettings.FileSizeKB < 1
                || appSettings.FileTypes == null
                || appSettings.FileTypes.Length < 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please ensure that all entries in appsettings.json have been populated with valid values.");
                WaitForAnyKey();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Using the following App Settings:");
            Console.WriteLine($"\tOutput Path: {appSettings.OutputLocation}");
            Console.WriteLine($"\tNumber of Files: {appSettings.NumberOfFiles}");
            Console.WriteLine($"\tFile Size (KB): {appSettings.FileSizeKB}");
            Console.WriteLine($"\tFile Types Enabled: {string.Join(", ", appSettings.FileTypes)}");
            Console.WriteLine();

            // Check if the output location is created, create it if it doesn't 
            if (!Directory.Exists(appSettings.OutputLocation))
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"Directory at path {appSettings.OutputLocation} does not exist, creating...");

                Directory.CreateDirectory(appSettings.OutputLocation);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("done.");
            }

            // Create sample files with random data
            var random = new Random(Guid.NewGuid().GetHashCode());
            for (var i = 0; i < appSettings.NumberOfFiles; i++)
            {
                // Pick a random extension from the list
                var ext = appSettings.FileTypes[random.Next(0, appSettings.FileTypes.Length)];
                var filename = Path.Combine(appSettings.OutputLocation, $"Sample Document {i}.{ext}");

                // Check if the file already exists
                if (File.Exists(filename))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"The document {filename} already exists skipping...");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write($"Creating document {filename}...");

                    var data = new byte[appSettings.FileSizeKB * 1024];
                    using (var fs = File.OpenWrite(filename))
                    {
                        random.NextBytes(data);
                        fs.Write(data, 0, data.Length);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("done.");
            }

            WaitForAnyKey();
        }
    }
}
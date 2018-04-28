using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MetadataExtractor;
using Directory = System.IO.Directory;

namespace MetadataInfo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
                Console.WriteLine("What directory?");

            foreach (var dir in args)
            {
                var filePaths = Directory.GetFiles(dir, "*.jp*", SearchOption.AllDirectories);

                Console.WriteLine($"Collected {filePaths.Length:N0} JPEG file paths");

                var scanned = 0;

                foreach (var filePath in filePaths)
                {
                    try
                    {
                        var directories = ImageMetadataReader.ReadMetadata(filePath);

                        var items = new List<string>();

                        foreach (var directory in directories)
                        {
                            // Each directory stores values in tags
                            items.AddRange(directory.Tags.Select(tag => $"'{tag.DirectoryName}'.'{tag.Name}' = {tag.Description}"));

                            // Each directory may also contain error messages
                            items.AddRange(directory.Errors.Select((error, i) => $"'{directory.Name}'.Errors[${i}] = {error}"));
                        }

                        Console.WriteLine(items.Count > 0
                            ? $"{Environment.NewLine}{filePath}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", items)}"
                            : $"{Environment.NewLine}{filePath} contains no metadata");

                        ++scanned;

                        if (scanned % 100 == 0)
                        {
                            Console.WriteLine($"Scanned {scanned:N0} of {filePaths.Length:N0}");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"{ex.GetType().Name} '{ex.Message}' thrown processing {filePath}");
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Type any key to continue...");
            Console.ReadLine();
        }
    }
}

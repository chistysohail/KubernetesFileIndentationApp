using System;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the full path to the folder containing the Kubernetes file:");
        string folderPath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
        {
            Console.WriteLine("Invalid folder path. Please try again.");
            return;
        }

        Console.WriteLine("Enter the name of the Kubernetes file (with extension):");
        string fileName = Console.ReadLine();

        string inputFilePath = Path.Combine(folderPath, fileName);

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine($"The file '{fileName}' does not exist in the specified folder.");
            return;
        }

        try
        {
            // Read all lines from the file
            var lines = File.ReadAllLines(inputFilePath);

            // Extract only the lines with '=' and calculate max key length
            var keyValuePairs = lines
                .Where(line => line.Contains('='))
                .Select(line =>
                {
                    var parts = line.Split('=', 2);
                    return new
                    {
                        Key = parts[0].Trim(),
                        Value = parts.Length > 1 ? parts[1].Trim() : string.Empty
                    };
                })
                .ToList();

            // Determine the max length of the keys for alignment
            int maxKeyLength = keyValuePairs.Max(kvp => kvp.Key.Length);

            // Format all lines, aligning keys and '='
            var formattedLines = lines.Select(line =>
            {
                if (line.Contains('='))
                {
                    var parts = line.Split('=', 2);
                    string key = parts[0].Trim();
                    string value = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                    return key.PadRight(maxKeyLength) + " = " + value;
                }
                return line; // Unchanged for lines without '='
            });

            // Generate output file name with timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputFileName = Path.GetFileNameWithoutExtension(fileName) + $"_formatted_{timestamp}" + Path.GetExtension(fileName);
            string outputFilePath = Path.Combine(folderPath, outputFileName);

            // Write the formatted lines to the output file
            File.WriteAllLines(outputFilePath, formattedLines);

            Console.WriteLine("File formatted successfully!");
            Console.WriteLine($"Formatted file saved at: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

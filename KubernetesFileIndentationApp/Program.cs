using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the full path to the folder containing the file:");
        string folderPath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
        {
            Console.WriteLine("Invalid folder path. Please try again.");
            return;
        }

        Console.WriteLine("Enter the name of the file (with extension):");
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

            // Process lines, respecting single-line and multi-line blocks
            var processedLines = ProcessLinesWithInlineBlocks(lines);

            // Generate output file name with timestamp
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputFileName = Path.GetFileNameWithoutExtension(fileName) + $"_processed_{timestamp}" + Path.GetExtension(fileName);
            string outputFilePath = Path.Combine(folderPath, outputFileName);

            // Write the processed lines to the output file
            File.WriteAllLines(outputFilePath, processedLines);

            Console.WriteLine("File processed successfully!");
            Console.WriteLine($"Processed file saved at: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static string[] ProcessLinesWithInlineBlocks(string[] lines)
    {
        int maxKeyLength = 0; // Max key length for alignment outside of blocks

        // First pass: Calculate max key length for alignment
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // Skip if it's a comment or a single-line block
            if (trimmedLine.StartsWith("#") || IsSingleLineBlock(trimmedLine))
                continue;

            // Calculate max key length for alignment
            if (trimmedLine.Contains("="))
            {
                var parts = trimmedLine.Split('=', 2);
                string key = parts[0].Trim();
                maxKeyLength = Math.Max(maxKeyLength, key.Length);
            }
        }

        // Second pass: Format the lines
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string trimmedLine = line.Trim();

            // Skip comments and single-line blocks
            if (trimmedLine.StartsWith("#") || IsSingleLineBlock(trimmedLine))
                continue;

            // Align assignments outside of blocks
            if (trimmedLine.Contains("="))
            {
                var parts = trimmedLine.Split('=', 2);
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    lines[i] = key.PadRight(maxKeyLength) + " = " + value;
                }
            }
        }

        return lines;
    }

    static bool IsSingleLineBlock(string line)
    {
        // Detect single-line blocks like `if (condition) { key = value }`
        return line.Contains("if") && line.Contains("{") && line.Contains("}");
    }
}

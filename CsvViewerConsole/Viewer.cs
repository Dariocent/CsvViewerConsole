using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CsvViewerConsole
{
    internal class Viewer
    {
        private readonly string _csvFilePath;
        private readonly int _pageLength;
        private string[] _csvData;
        private string _firstLine;

        public Viewer(string filename, int pagelength)
        {
            _csvFilePath = filename;
            _pageLength = pagelength;
        }

        // Entry point
        public void Run()
        {
            // Read file and separate first line from the rest. (local function)
            (string first, string[] rest) ReadAndSeparateLines(string path)
            {
                var lines = File.ReadAllLines(path);
                return (lines.First(), lines.Skip(1).ToArray());
            }

            // Read file and separate first line from the rest
            (_firstLine, _csvData) = ReadAndSeparateLines(_csvFilePath);

            // Display the CSV data and the menu
            Show(_csvData, _pageLength);
        }

        // Display the CSV data and the menu 
        private void Show(string[] csvData, int pageLength)
        {
            // Variables to keep track of the current page and total pages.
            int currentPageIndex = 0;
            int totalPages = (csvData.Length + pageLength - 1) / pageLength;  // Calculate total pages using ceiling division.

            // Local function to get the current page of CSV data.
            string[] GetCurrentPage() => csvData.Skip(currentPageIndex).Take(pageLength).ToArray();

            // Dictionary to map user commands to corresponding actions.
            Dictionary<char, Action> commands = new Dictionary<char, Action>
            {
                { 'e', () => Environment.Exit(0) },  // Exit the application
                { 'f', () => currentPageIndex = 0 },  // Move to the first page
                { 'l', () => currentPageIndex = (totalPages - 1) * pageLength },  // Move to the last page
                { 'n', () => currentPageIndex = Math.Min((totalPages - 1) * pageLength, currentPageIndex + pageLength) },  // Move to the next page
                { 'p', () => currentPageIndex = Math.Max(0, currentPageIndex - pageLength) }  // Move to the previous page
            };

            // Local functions for clarity
            void DisplayCurrentPageAndOptions()
            {
                var currentPage = GetCurrentPage();
                DisplayCsvPage(currentPage, _firstLine);
                Console.Write("F)irst, L)ast, N)ext, P)rev, E)xit: ");
            }

            char GetUserCommand() => char.ToLower(Console.ReadKey().KeyChar);

            void ExecuteCommand(char command)
            {
                Console.WriteLine("\n");
                if (commands.ContainsKey(command))
                {
                    commands[command]();
                }
            }

            // Main loop for pagination and user interaction.
            while (true)
            {
                DisplayCurrentPageAndOptions();
                var command = GetUserCommand();
                ExecuteCommand(command);
            }
        }

        // Method to display a single page of CSV data.
        private static void DisplayCsvPage(IReadOnlyList<string> pageData, string firstLine)
        {
            // Local function to generate the separator line based on column widths.
            string GenerateSeparator(List<int> widths) =>
                "+" + string.Join("+", widths.Select(w => new string('-', w + 1))) + "+";

            // Calculate the column widths based on the first line and data.
            var columnWidths = GetColumnWidths(pageData, firstLine);

            // Generate the separator line.
            var separator = GenerateSeparator(columnWidths);

            // Clear the console for fresh output.
            Console.Clear();

            // Action to print a line in a formatted manner.
            Action<string> printLine = line => Console.WriteLine(CreateDisplayLine(line, columnWidths));

            // Display the header and separator.
            new[] { separator, firstLine }.ToList().ForEach(printLine);
            Console.WriteLine(separator);

            // Display each data line.
            pageData.ToList().ForEach(printLine);

            // Display the closing separator.
            Console.WriteLine(separator);
        }

        // Method to calculate the maximum width for each column.
        private static List<int> GetColumnWidths(IReadOnlyList<string> pageData, string firstLine)
        {
            // Local function to split and trim each field in a line.
            var headerFields = firstLine.Split(';').Select(field => field.Trim()).ToList();

            // Calculate the maximum width for each column.
            return headerFields.Select((header, index) =>
                    pageData.Select(row => row.Split(';').ElementAtOrDefault(index)?.Trim().Length ?? 0)
                        .Append(header.Length)
                        .Max())
                        .ToList();
        }

        // Method to format a line for display.
        private static string CreateDisplayLine(string dataLine, IReadOnlyList<int> columnWidths)
        {
            // Split and trim each field in the line.
            var fields = dataLine.Split(';').Select(field => field.Trim());

            // Pad each field to match the corresponding column width.
            var paddedFields = fields.Zip(columnWidths, (field, width) => field.PadRight(width + 1));

            // Join the padded fields with pipe characters to create the display line.
            return "|" + string.Join("|", paddedFields) + "|";
        }
    }
}

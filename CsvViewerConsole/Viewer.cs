using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvViewerConsole
{
    class Viewer 
    {
        //declare filename and linlength
        private string csvFilePath;
        private int pageLength;
        private string[] csvData;
        private string firstLine;
        public Viewer(string filename, int pagelength)
        {
            csvFilePath = filename;
            pageLength = pagelength;
        }
        public void Run()
        {
            csvData = File.ReadAllLines(csvFilePath);
            firstLine = csvData[0];
            csvData = csvData.Skip(1).ToArray();
            Show(csvData, pageLength);
        } 

        private void Show(string[] csvData, int pageLength)
        {
            int currentPageStart = 0;

            while (true)
            {
                var currentPage = csvData.Skip(currentPageStart).Take(pageLength).ToList();

                DisplayCsvPage(currentPage, firstLine);

                Console.Write("F)irst, L)ast, N)ext, P)rev, E)xit: ");
                var command = char.ToLower(Console.ReadKey().KeyChar);
                Console.WriteLine("\n");

                switch (command)
                {
                    case 'e': // exit
                        return;
                    case 'f': // first page
                        currentPageStart = 0;
                        break;
                    case 'l': // last page
                        currentPageStart = Math.Max(0, csvData.Length - pageLength);
                        break;
                    case 'n': // next page
                        currentPageStart += pageLength;
                        currentPageStart = Math.Min(csvData.Length - pageLength, currentPageStart);
                        break;
                    case 'p': // previous page
                        currentPageStart -= pageLength;
                        currentPageStart = Math.Max(0, currentPageStart);
                        break;
                }
            }
        }

        private static void DisplayCsvPage(List<string> pageData, string firstLine)
        {
            Console.Clear();
            var columnWidths = GetColumnWidths(pageData, firstLine);

            var separatorFields = columnWidths.Select(width => new string('-', width+1));
            var separatorLine = "+" + string.Join("+", separatorFields) + "+";

            Console.WriteLine(separatorLine);
            Console.WriteLine(CreateDisplayLine(firstLine, columnWidths));
            Console.WriteLine(separatorLine);

           
            for (int i = 0; i < pageData.Count; i++)
            {
                Console.WriteLine(CreateDisplayLine(pageData[i], columnWidths));
            }

            Console.WriteLine(separatorLine);
        }

        private static List<int> GetColumnWidths(List<string> pageData, string firstLine)
        {
            var headerFields = firstLine.Split(';').Select(field => field.Trim()).ToList();
            var columnWidths = new List<int>();

            for (int i = 0; i < headerFields.Count; i++)
            {
                int maxWidth = headerFields[i].Length;
                for (int j = 1; j < pageData.Count; j++)
                {
                    var rowData = pageData[j].Split(';').Select(field => field.Trim()).ToList();
                    maxWidth = Math.Max(maxWidth, rowData[i].Length);
                }
                columnWidths.Add(maxWidth);
            }

            return columnWidths;
        }

        private static string CreateDisplayLine(string dataLine, List<int> columnWidths)
        {
            var fields = dataLine.Split(';').Select(field => field.Trim()).ToList();
            var displayFields = new List<string>();

            for (int i = 0; i < fields.Count; i++)
            {
                displayFields.Add(fields[i].PadRight(columnWidths[i]+1));
            }

            return "|" + string.Join("|", displayFields) + "|";
        }       
    }
}
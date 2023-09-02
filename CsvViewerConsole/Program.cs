namespace CsvViewerConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || !int.TryParse(args[1], out var pagelength))
            {
                pagelength = 3;
            }
            // first argument is filename

            var csvViewer = new Viewer(args[0], pagelength);
            csvViewer.Run();
        }
    }
}
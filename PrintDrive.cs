namespace PrintDriveToTxt;

internal class PrintDrive
{
    public static void Run()
    {
        Console.WriteLine("Input drive:\r\n");
        char drive = Console.ReadKey().KeyChar;

        EnumerationOptions options = new()
        {
            MaxRecursionDepth = 69,
            AttributesToSkip = FileAttributes.System | FileAttributes.Hidden,
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
        };

        DirectoryInfo hd = new(@$"{drive}:\");

        string[] extensions = [".txt"];

        IEnumerable<FileInfo> kak = hd.EnumerateFiles("*.*", options).Where(x => extensions.Contains(x.Extension));
        IEnumerator<FileInfo> iter = kak.GetEnumerator();

        List<string> temp = [];
        while (iter.MoveNext())
        {
            temp.Add(iter.Current.FullName);
        }

        string outPath = Path.Combine(AppContext.BaseDirectory, $"{drive}_fileinfo_{DateTime.Now:yyyy-MM-dd}.txt");
        File.WriteAllLines(outPath, temp);
        Console.Clear();
        Console.WriteLine($"output saved: {outPath}");
        _ = Console.ReadLine();
        Console.Clear();
    }
}

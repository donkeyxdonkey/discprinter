namespace PrintDriveToTxt;

internal class PrintDrive
{
    private static long _minFileSize = 0;

    public static void Run(string[] args)
    {
        List<string> extensions = [];

        try
        {
            extensions = (args.Length > 0) switch
            {
                true => ParseArgs(args),
                false => LoadConfig(),
            };

            if (extensions.Count == 0)
                throw new ArgumentException("No extensions provided.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
            Console.ReadLine();
            return;
        }

        Console.WriteLine("Input drive:\r\n");
        char drive = Console.ReadKey().KeyChar;
        Console.Clear();

        EnumerationOptions options = new()
        {
            MaxRecursionDepth = 69,
            AttributesToSkip = FileAttributes.System | FileAttributes.Hidden,
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
        };

        DirectoryInfo hd = new(@$"{drive}:\");
        if (!hd.Exists)
        {
            Console.WriteLine($"Drive not found! {hd.FullName}");
            Console.ReadLine();
            return;
        }

        IEnumerable<FileInfo> kak = hd.EnumerateFiles("*.*", options).Where(x => extensions.Contains(x.Extension) && x.Length >= _minFileSize);
        IEnumerator<FileInfo> iter = kak.GetEnumerator();

        Console.WriteLine($"Loading... {drive}:\\ - Extensions: {string.Join(',', extensions)}");

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

    private enum ARGS
    {
        none,
        help,
        ext,
        limit,
    }

    private static List<string> ParseArgs(string[] args)
    {
        const string IS_ARG = "--";
        const char DELIMITER = '~';

        ARGS type = ARGS.none;

        string[] temp = [];

        foreach (string arg in args)
        {
            if (arg.StartsWith(IS_ARG))
            {
                if (type == ARGS.none && Enum.TryParse(arg[2..], out type))
                {
                    if (type == ARGS.help)
                    {
                        Console.WriteLine($"ARGUMENTS: --help --ext{Environment.NewLine}        --ext [ext-name]+{DELIMITER}[pipe] to add extensions{Environment.NewLine}        --limit set minimum file size in bytes{Environment.NewLine}{Environment.NewLine}If no arguments provided, set extensions in config.yaml");
                        Console.ReadLine();
                        System.Environment.Exit(1);
                    }
                }

                continue;
            }

            // any non-valid args simply do nothing

            switch (type)
            {
                case ARGS.ext:
                    if (!arg.Contains(DELIMITER))
                        return [arg];

                    temp = arg.Split(DELIMITER, StringSplitOptions.RemoveEmptyEntries);
                    const char DOT = '.';
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i].StartsWith(DOT))
                            continue;

                        temp[i] = $"{DOT}{temp[i]}";
                    }
                    break;
                case ARGS.limit:
                    if (!long.TryParse(arg, out _minFileSize))
                    {
                        // just warn, default is 0
                        Console.WriteLine("Invalid format provided for filesizelimit.");
                        Console.ReadLine();
                    }
                    break;
            }

            // reset
            type = ARGS.none;
        }
        return [.. temp];
    }

    private static List<string> LoadConfig()
    {
        try
        {
            FileInfo configPath = new(Path.Combine(AppContext.BaseDirectory, "config.yaml"));
            Config config = YamlParser.ParseConfig<Config>(configPath);
            _minFileSize = config.MinFileSize;
            return config.Extensions;
        }
        catch (Exception)
        {
            throw new ArgumentException("Invalid config provided");
        }
    }
}

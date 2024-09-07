namespace PrintDriveToTxt;

public class Config
{
    public long MinFileSize { get => _minFileSize; set => _minFileSize = value; }

    public List<string> Extensions { get => _extensions; set => _extensions = value; }

    List<string> _extensions;
    long _minFileSize;

    public Config()
    {
        _extensions = [];
        _minFileSize = 0;
    }
}

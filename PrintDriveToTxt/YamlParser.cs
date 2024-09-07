using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PrintDriveToTxt;

public class YamlParser
{
    public static T ParseConfig<T>(FileInfo config)
    {
        if (!config.Exists)
            throw new FileNotFoundException($"Config not found: {config.FullName}");

        string contents = File.ReadAllText(config.FullName);

        IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<T>(contents);
    }

    public static void SaveConfig<T>(T content, FileInfo config)
    {
        if (content is null)
            throw new ArgumentNullException("Config contained no content.");

        ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();

        string yaml = serializer.Serialize(content);
        File.WriteAllText(config.FullName, yaml);
    }
}

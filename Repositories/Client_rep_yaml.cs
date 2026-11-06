using JewelryWorkshop.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью Client в формате YAML (наследует Client_rep_base).
/// </summary>
public class Client_rep_yaml : Client_rep_base
{
    private static readonly ISerializer YamlSerializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    private sealed class YamlClientModel
    {
        public int Id { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string PassportSeriesNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
    }

    public Client_rep_yaml(string filePath)
        : base(filePath)
    {
        ReadAll();
    }

    /// <summary>
    /// a. Чтение всех значений из YAML-файла.
    /// </summary>
    public override List<Client> ReadAll()
    {
        if (!File.Exists(FilePath))
        {
            Clients = new List<Client>();
            return Clients;
        }

        string yaml = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(yaml))
        {
            Clients = new List<Client>();
            return Clients;
        }

        var rawItems = YamlDeserializer.Deserialize<List<YamlClientModel>>(yaml) ?? new List<YamlClientModel>();
        Clients = rawItems
            .Select(item => new Client(
                item.Id,
                item.LastName,
                item.FirstName,
                item.MiddleName,
                item.Phone,
                item.Email,
                item.PassportSeriesNumber,
                item.Address))
            .ToList();

        return Clients;
    }

    /// <summary>
    /// b. Запись всех значений в YAML-файл.
    /// </summary>
    public override void WriteAll()
    {
        EnsureDirectoryExists();

        var rawItems = Clients
            .Select(c => new YamlClientModel
            {
                Id = c.Id,
                LastName = c.LastName,
                FirstName = c.FirstName,
                MiddleName = c.MiddleName,
                Phone = c.Phone,
                Email = c.Email,
                PassportSeriesNumber = c.PassportSeriesNumber,
                Address = c.Address
            })
            .ToList();

        string yaml = YamlSerializer.Serialize(rawItems);
        File.WriteAllText(FilePath, yaml);
    }
}

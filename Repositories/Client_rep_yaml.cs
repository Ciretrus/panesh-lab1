using JewelryWorkshop.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Вспомогательный DTO класс для сериализации/десериализации клиента в YAML.
/// </summary>
public class ClientYamlDto
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

/// <summary>
/// Репозиторий сущности Client для работы с YAML-файлом (ЛР2, Пункт 2 и 3).
/// Наследует общий базовый класс Client_rep_base, реализуя специфику чтения и записи YAML.
/// </summary>
public class Client_rep_yaml : Client_rep_base
{
    private static readonly ISerializer Serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Конструктор репозитория YAML.
    /// </summary>
    /// <param name="filePath">Путь к файлу YAML.</param>
    public Client_rep_yaml(string filePath) : base(filePath)
    {
        Initialize();
    }

    /// <summary>
    /// a. Чтение всех значений из YAML-файла.
    /// </summary>
    public override List<Client> ReadFromFile()
    {
        _clients.Clear();

        if (!File.Exists(_filePath))
            return new List<Client>(_clients);

        string yaml = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(yaml))
            return new List<Client>(_clients);

        var dtos = Deserializer.Deserialize<List<ClientYamlDto>?>(yaml);
        if (dtos != null)
        {
            foreach (var dto in dtos)
            {
                _clients.Add(new Client(
                    id: dto.Id,
                    lastName: dto.LastName,
                    firstName: dto.FirstName,
                    middleName: dto.MiddleName,
                    phone: dto.Phone,
                    email: dto.Email,
                    passportSeriesNumber: dto.PassportSeriesNumber,
                    address: dto.Address
                ));
            }
        }

        return new List<Client>(_clients);
    }

    /// <summary>
    /// b. Запись всех значений в YAML-файл.
    /// </summary>
    public override void WriteToFile()
    {
        EnsureDirectoryExists();

        var dtos = _clients.Select(c => new ClientYamlDto
        {
            Id = c.Id,
            LastName = c.LastName,
            FirstName = c.FirstName,
            MiddleName = c.MiddleName,
            Phone = c.Phone,
            Email = c.Email,
            PassportSeriesNumber = c.PassportSeriesNumber,
            Address = c.Address
        }).ToList();

        string yaml = Serializer.Serialize(dtos);
        File.WriteAllText(_filePath, yaml);
    }
}

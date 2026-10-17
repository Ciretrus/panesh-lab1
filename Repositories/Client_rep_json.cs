using System.Text.Encodings.Web;
using System.Text.Json;
using JewelryWorkshop.Models;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Репозиторий сущности Client для работы с JSON-файлом (ЛР2, Пункт 1 и 3).
/// Наследует общий базовый класс Client_rep_base, реализуя специфику чтения и записи JSON.
/// </summary>
public class Client_rep_json : Client_rep_base
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    /// <summary>
    /// Конструктор репозитория JSON.
    /// </summary>
    /// <param name="filePath">Путь к файлу JSON.</param>
    public Client_rep_json(string filePath) : base(filePath)
    {
        Initialize();
    }

    /// <summary>
    /// a. Чтение всех значений из JSON-файла.
    /// </summary>
    public override List<Client> ReadFromFile()
    {
        _clients.Clear();

        if (!File.Exists(_filePath))
            return new List<Client>(_clients);

        string json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new List<Client>(_clients);

        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                _clients.Add(new Client(element));
            }
        }

        return new List<Client>(_clients);
    }

    /// <summary>
    /// b. Запись всех значений в JSON-файл.
    /// </summary>
    public override void WriteToFile()
    {
        EnsureDirectoryExists();

        var dtoList = _clients.Select(c => new
        {
            id = c.Id,
            lastName = c.LastName,
            firstName = c.FirstName,
            middleName = c.MiddleName,
            phone = c.Phone,
            email = c.Email,
            passportSeriesNumber = c.PassportSeriesNumber,
            address = c.Address
        }).ToList();

        string json = JsonSerializer.Serialize(dtoList, JsonOptions);
        File.WriteAllText(_filePath, json);
    }
}

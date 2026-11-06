using System.Text.Encodings.Web;
using System.Text.Json;
using JewelryWorkshop.Models;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью Client в формате JSON (наследует Client_rep_base).
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

    public Client_rep_json(string filePath)
        : base(filePath)
    {
        ReadAll();
    }

    /// <summary>
    /// a. Чтение всех значений из JSON-файла.
    /// </summary>
    public override List<Client> ReadAll()
    {
        if (!File.Exists(FilePath))
        {
            Clients = new List<Client>();
            return Clients;
        }

        string json = File.ReadAllText(FilePath);
        if (string.IsNullOrWhiteSpace(json))
        {
            Clients = new List<Client>();
            return Clients;
        }

        Clients = JsonSerializer.Deserialize<List<Client>>(json, JsonOptions) ?? new List<Client>();
        return Clients;
    }

    /// <summary>
    /// b. Запись всех значений в JSON-файл.
    /// </summary>
    public override void WriteAll()
    {
        EnsureDirectoryExists();
        string json = JsonSerializer.Serialize(Clients, JsonOptions);
        File.WriteAllText(FilePath, json);
    }
}

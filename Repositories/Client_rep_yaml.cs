using JewelryWorkshop.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью Client в формате YAML (ЛР2, пункт 2).
/// </summary>
public class Client_rep_yaml
{
    private readonly string _filePath;
    private List<Client> _clients = new();

    private static readonly ISerializer YamlSerializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Вспомогательная структура для сериализации/десериализации YAML с последующей валидацией через конструктор Client.
    /// </summary>
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
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));

        _filePath = filePath;
        ReadAll();
    }

    /// <summary>
    /// a. Чтение всех значений из файла.
    /// </summary>
    public List<Client> ReadAll()
    {
        if (!File.Exists(_filePath))
        {
            _clients = new List<Client>();
            return _clients;
        }

        string yaml = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(yaml))
        {
            _clients = new List<Client>();
            return _clients;
        }

        var rawItems = YamlDeserializer.Deserialize<List<YamlClientModel>>(yaml) ?? new List<YamlClientModel>();
        _clients = rawItems
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

        return _clients;
    }

    /// <summary>
    /// b. Запись всех значений в файл.
    /// </summary>
    public void WriteAll()
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var rawItems = _clients
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
        File.WriteAllText(_filePath, yaml);
    }

    /// <summary>
    /// c. Получить объект по ID.
    /// </summary>
    public Client? GetById(int id)
    {
        return _clients.FirstOrDefault(c => c.Id == id);
    }

    /// <summary>
    /// d. get_k_n_short_list: Получить список k по счету n объектов класса ClientShort.
    /// </summary>
    public List<ClientShort> Get_k_n_short_list(int k, int n)
    {
        if (k < 1)
            throw new ArgumentOutOfRangeException(nameof(k), "Номер страницы (k) должен быть больше или равен 1.");
        if (n < 1)
            throw new ArgumentOutOfRangeException(nameof(n), "Количество элементов (n) должно быть больше или равно 1.");

        return _clients
            .Skip((k - 1) * n)
            .Take(n)
            .Select(c => new ClientShort(c))
            .ToList();
    }

    /// <summary>
    /// e. Сортировать элементы по выбранному полю предметной области (по фамилии и имени).
    /// </summary>
    public void SortByField()
    {
        _clients = _clients
            .OrderBy(c => c.LastName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.FirstName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.MiddleName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// f. Добавить объект в список (при добавлении сформировать новый ID).
    /// </summary>
    public Client Add(Client client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        int newId = _clients.Count == 0 ? 1 : _clients.Max(c => c.Id) + 1;
        client.Id = newId;
        _clients.Add(client);
        return client;
    }

    /// <summary>
    /// g. Заменить элемент списка по ID.
    /// </summary>
    public bool ReplaceById(int id, Client updatedClient)
    {
        if (updatedClient is null)
            throw new ArgumentNullException(nameof(updatedClient));

        int index = _clients.FindIndex(c => c.Id == id);
        if (index == -1)
            return false;

        updatedClient.Id = id;
        _clients[index] = updatedClient;
        return true;
    }

    /// <summary>
    /// h. Удалить элемент списка по ID.
    /// </summary>
    public bool DeleteById(int id)
    {
        Client? existing = GetById(id);
        if (existing is null)
            return false;

        return _clients.Remove(existing);
    }

    /// <summary>
    /// i. get_count: Получить количество элементов.
    /// </summary>
    public int GetCount()
    {
        return _clients.Count;
    }
}


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
/// Репозиторий сущности Client для работы с YAML-файлом (ЛР2, Пункт 2).
/// Выполняет чтение, запись и CRUD-операции над коллекцией клиентов в формате YAML.
/// </summary>
public class Client_rep_yaml
{
    private readonly string _filePath;
    private readonly List<Client> _clients = new();

    private static readonly ISerializer Serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Конструктор репозитория. Загружает данные из файла, если он существует.
    /// </summary>
    /// <param name="filePath">Путь к файлу YAML.</param>
    public Client_rep_yaml(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));

        _filePath = filePath;

        if (File.Exists(_filePath))
        {
            ReadFromFile();
        }
        else
        {
            string? dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            WriteToFile();
        }
    }

    /// <summary>
    /// a. Чтение всех значений из файла.
    /// </summary>
    public List<Client> ReadFromFile()
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
    /// Чтение всех значений (из оперативной памяти).
    /// </summary>
    public List<Client> ReadAll() => new(_clients);

    /// <summary>
    /// b. Запись всех значений в файл.
    /// </summary>
    public void WriteToFile()
    {
        string? dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

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

    /// <summary>
    /// c. Получить объект по ID.
    /// </summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <returns>Объект Client или null, если не найден.</returns>
    public Client? GetById(int id)
    {
        return _clients.FirstOrDefault(c => c.Id == id);
    }

    /// <summary>
    /// d. get_k_n_short_list: Получить список k по счету n объектов класса short.
    /// </summary>
    /// <param name="k">Номер страницы (начиная с 1).</param>
    /// <param name="n">Размер страницы (количество элементов на странице).</param>
    /// <returns>Список объектов ClientShort.</returns>
    public List<ClientShort> get_k_n_short_list(int k, int n)
    {
        if (k < 1)
            throw new ArgumentOutOfRangeException(nameof(k), "Номер страницы k должен быть >= 1.");
        if (n < 1)
            throw new ArgumentOutOfRangeException(nameof(n), "Размер страницы n должен быть >= 1.");

        return _clients
            .Skip((k - 1) * n)
            .Take(n)
            .Select(c => new ClientShort(c))
            .ToList();
    }

    /// <summary>
    /// d. PascalCase-синоним для get_k_n_short_list.
    /// </summary>
    public List<ClientShort> GetKNShortList(int k, int n) => get_k_n_short_list(k, n);

    /// <summary>
    /// e. Сортировать элементы по выбранному полю (по фамилии, затем имени).
    /// </summary>
    /// <param name="ascending">Направление сортировки (по возрастанию по умолчанию).</param>
    public void SortByField(bool ascending = true)
    {
        if (ascending)
        {
            _clients.Sort((a, b) =>
            {
                int cmp = string.Compare(a.LastName, b.LastName, StringComparison.CurrentCultureIgnoreCase);
                return cmp != 0 ? cmp : string.Compare(a.FirstName, b.FirstName, StringComparison.CurrentCultureIgnoreCase);
            });
        }
        else
        {
            _clients.Sort((a, b) =>
            {
                int cmp = string.Compare(b.LastName, a.LastName, StringComparison.CurrentCultureIgnoreCase);
                return cmp != 0 ? cmp : string.Compare(b.FirstName, a.FirstName, StringComparison.CurrentCultureIgnoreCase);
            });
        }
    }

    /// <summary>
    /// f. Добавить объект в список (при добавлении сформировать новый ID).
    /// </summary>
    /// <param name="client">Добавляемый клиент.</param>
    /// <returns>Присвоенный новый ID.</returns>
    public int Add(Client client)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));

        int newId = _clients.Count > 0 ? _clients.Max(c => c.Id) + 1 : 1;
        client.Id = newId;

        _clients.Add(client);
        WriteToFile();

        return newId;
    }

    /// <summary>
    /// g. Заменить элемент списка по ID.
    /// </summary>
    /// <param name="id">ID заменяемого элемента.</param>
    /// <param name="newClient">Новый объект клиента.</param>
    /// <returns>True, если замена успешна; иначе false.</returns>
    public bool ReplaceById(int id, Client newClient)
    {
        if (newClient == null)
            throw new ArgumentNullException(nameof(newClient));

        int index = _clients.FindIndex(c => c.Id == id);
        if (index == -1)
            return false;

        newClient.Id = id;
        _clients[index] = newClient;
        WriteToFile();

        return true;
    }

    /// <summary>
    /// h. Удалить элемент списка по ID.
    /// </summary>
    /// <param name="id">ID удаляемого элемента.</param>
    /// <returns>True, если удален; иначе false.</returns>
    public bool DeleteById(int id)
    {
        int index = _clients.FindIndex(c => c.Id == id);
        if (index == -1)
            return false;

        _clients.RemoveAt(index);
        WriteToFile();

        return true;
    }

    /// <summary>
    /// i. get_count: Получить количество элементов.
    /// </summary>
    /// <returns>Количество клиентов в репозитории.</returns>
    public int get_count()
    {
        return _clients.Count;
    }

    /// <summary>
    /// i. PascalCase-синоним для get_count.
    /// </summary>
    public int GetCount() => get_count();
}

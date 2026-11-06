using JewelryWorkshop.Models;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Базовый абстрактный класс репозитория клиентов (ЛР2, пункт 3).
/// Инкапсулирует всю общую логику работы со списком сущностей, устраняя дублирование кода между JSON и YAML.
/// </summary>
public abstract class Client_rep_base
{
    protected readonly string FilePath = string.Empty;
    protected List<Client> Clients = new();

    protected Client_rep_base()
    {
    }

    protected Client_rep_base(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));

        FilePath = filePath;
    }

    /// <summary>
    /// a. Чтение всех значений из файла (реализуется в конкретном формате наследника).
    /// </summary>
    public abstract List<Client> ReadAll();

    /// <summary>
    /// b. Запись всех значений в файл (реализуется в конкретном формате наследника).
    /// </summary>
    public abstract void WriteAll();

    /// <summary>
    /// c. Получить объект по ID.
    /// </summary>
    public virtual Client? GetById(int id)
    {
        return Clients.FirstOrDefault(c => c.Id == id);
    }

    /// <summary>
    /// d. get_k_n_short_list: Получить список k по счету n объектов класса ClientShort.
    /// </summary>
    public virtual List<ClientShort> Get_k_n_short_list(int k, int n)
    {
        if (k < 1)
            throw new ArgumentOutOfRangeException(nameof(k), "Номер страницы (k) должен быть больше или равен 1.");
        if (n < 1)
            throw new ArgumentOutOfRangeException(nameof(n), "Количество элементов (n) должно быть больше или равно 1.");

        return Clients
            .Skip((k - 1) * n)
            .Take(n)
            .Select(c => new ClientShort(c))
            .ToList();
    }

    /// <summary>
    /// e. Сортировать элементы по выбранному полю предметной области (по фамилии и имени).
    /// </summary>
    public virtual void SortByField()
    {
        Clients = Clients
            .OrderBy(c => c.LastName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.FirstName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(c => c.MiddleName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    /// f. Добавить объект в список (при добавлении сформировать новый ID).
    /// </summary>
    public virtual Client Add(Client client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        int newId = Clients.Count == 0 ? 1 : Clients.Max(c => c.Id) + 1;
        client.Id = newId;
        Clients.Add(client);
        return client;
    }

    /// <summary>
    /// g. Заменить элемент списка по ID.
    /// </summary>
    public virtual bool ReplaceById(int id, Client updatedClient)
    {
        if (updatedClient is null)
            throw new ArgumentNullException(nameof(updatedClient));

        int index = Clients.FindIndex(c => c.Id == id);
        if (index == -1)
            return false;

        updatedClient.Id = id;
        Clients[index] = updatedClient;
        return true;
    }

    /// <summary>
    /// h. Удалить элемент списка по ID.
    /// </summary>
    public virtual bool DeleteById(int id)
    {
        Client? existing = GetById(id);
        if (existing is null)
            return false;

        return Clients.Remove(existing);
    }

    /// <summary>
    /// i. get_count: Получить количество элементов.
    /// </summary>
    public virtual int GetCount()
    {
        return Clients.Count;
    }

    protected void EnsureDirectoryExists()
    {
        string? directory = Path.GetDirectoryName(FilePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}


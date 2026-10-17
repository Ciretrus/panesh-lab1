using JewelryWorkshop.Models;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Базовый абстрактный класс для файловых репозиториев сущности Client (ЛР2, Пункт 3).
/// Объединяет общую логику работы с коллекцией в памяти, сортировки, CRUD-операций
/// и постраничной выборки. Конкретные классы-наследники определяют формат хранения
/// данных через методы ReadFromFile() и WriteToFile() (паттерн «Шаблонный метод»).
/// </summary>
public abstract class Client_rep_base
{
    protected readonly string _filePath;
    protected readonly List<Client> _clients = new();

    /// <summary>
    /// Конструктор базового репозитория.
    /// </summary>
    /// <param name="filePath">Путь к файлу хранилища.</param>
    protected Client_rep_base(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу не может быть пустым.", nameof(filePath));

        _filePath = filePath;
    }

    /// <summary>
    /// Инициализация репозитория: загрузка из файла при его наличии
    /// или создание пустого файла хранилища.
    /// </summary>
    protected void Initialize()
    {
        if (File.Exists(_filePath))
        {
            ReadFromFile();
        }
        else
        {
            EnsureDirectoryExists();
            WriteToFile();
        }
    }

    /// <summary>
    /// Проверка и создание директории для файла, если она не существует.
    /// </summary>
    protected void EnsureDirectoryExists()
    {
        string? dir = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    /// <summary>
    /// a. Чтение всех значений из файла (определяется форматом данных в наследниках).
    /// </summary>
    public abstract List<Client> ReadFromFile();

    /// <summary>
    /// Чтение всех значений из оперативной памяти.
    /// </summary>
    public virtual List<Client> ReadAll() => new(_clients);

    /// <summary>
    /// b. Запись всех значений в файл (определяется форматом данных в наследниках).
    /// </summary>
    public abstract void WriteToFile();

    /// <summary>
    /// c. Получить объект по ID.
    /// </summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <returns>Найденный клиент или null.</returns>
    public virtual Client? GetById(int id)
    {
        return _clients.FirstOrDefault(c => c.Id == id);
    }

    /// <summary>
    /// d. get_k_n_short_list: Получить список k по счету n объектов класса short.
    /// </summary>
    /// <param name="k">Номер страницы (начиная с 1).</param>
    /// <param name="n">Размер страницы.</param>
    /// <returns>Список объектов ClientShort.</returns>
    public virtual List<ClientShort> get_k_n_short_list(int k, int n)
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
    /// PascalCase-синоним для get_k_n_short_list.
    /// </summary>
    public List<ClientShort> GetKNShortList(int k, int n) => get_k_n_short_list(k, n);

    /// <summary>
    /// e. Сортировать элементы по выбранному полю (по фамилии, затем имени).
    /// </summary>
    /// <param name="ascending">По возрастанию (true) или по убыванию (false).</param>
    public virtual void SortByField(bool ascending = true)
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
    public virtual int Add(Client client)
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
    /// <returns>True, если успешно заменен; иначе false.</returns>
    public virtual bool ReplaceById(int id, Client newClient)
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
    /// <returns>True, если успешно удален; иначе false.</returns>
    public virtual bool DeleteById(int id)
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
    /// <returns>Количество клиентов в коллекции.</returns>
    public virtual int get_count()
    {
        return _clients.Count;
    }

    /// <summary>
    /// PascalCase-синоним для get_count.
    /// </summary>
    public int GetCount() => get_count();
}

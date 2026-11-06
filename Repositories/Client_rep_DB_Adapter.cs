using JewelryWorkshop.Models;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Паттерн Адаптер (Adapter): включает класс Client_rep_DB в общую иерархию Client_rep_base (ЛР2, пункт 6).
/// Target — Client_rep_base, Adaptee — Client_rep_DB, Adapter — Client_rep_DB_Adapter.
/// </summary>
public class Client_rep_DB_Adapter : Client_rep_base
{
    private readonly Client_rep_DB _dbRepository;

    public Client_rep_DB_Adapter(Client_rep_DB dbRepository)
    {
        _dbRepository = dbRepository ?? throw new ArgumentNullException(nameof(dbRepository));
    }

    public Client_rep_DB_Adapter(string? connectionString = null)
        : this(new Client_rep_DB(connectionString))
    {
    }

    /// <summary>
    /// a. Чтение всех значений (адаптирует GetAll() из БД к контракту Client_rep_base).
    /// </summary>
    public override List<Client> ReadAll()
    {
        Clients = _dbRepository.GetAll();
        return Clients;
    }

    /// <summary>
    /// b. Запись всех значений (для БД изменения применяются транзакционно в каждом запросе).
    /// </summary>
    public override void WriteAll()
    {
        // В реляционной СУБД запись происходит непосредственно при вызовах Add / ReplaceById / DeleteById.
    }

    /// <summary>
    /// c. Получить объект по ID (делегируется в Client_rep_DB).
    /// </summary>
    public override Client? GetById(int id)
    {
        return _dbRepository.GetById(id);
    }

    /// <summary>
    /// d. get_k_n_short_list: Получить список k по счету n объектов класса ClientShort (делегируется в Client_rep_DB).
    /// </summary>
    public override List<ClientShort> Get_k_n_short_list(int k, int n)
    {
        return _dbRepository.Get_k_n_short_list(k, n);
    }

    /// <summary>
    /// e. Сортировать элементы по фамилии.
    /// </summary>
    public override void SortByField()
    {
        ReadAll();
        base.SortByField();
    }

    /// <summary>
    /// f. Добавить объект (делегируется в Client_rep_DB).
    /// </summary>
    public override Client Add(Client client)
    {
        return _dbRepository.Add(client);
    }

    /// <summary>
    /// g. Заменить элемент по ID (делегируется в Client_rep_DB).
    /// </summary>
    public override bool ReplaceById(int id, Client updatedClient)
    {
        return _dbRepository.ReplaceById(id, updatedClient);
    }

    /// <summary>
    /// h. Удалить элемент по ID (делегируется в Client_rep_DB).
    /// </summary>
    public override bool DeleteById(int id)
    {
        return _dbRepository.DeleteById(id);
    }

    /// <summary>
    /// i. get_count: Получить количество элементов (делегируется в Client_rep_DB).
    /// </summary>
    public override int GetCount()
    {
        return _dbRepository.GetCount();
    }
}


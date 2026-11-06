using System.Data;
using System.Globalization;
using JewelryWorkshop.Models;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью Client в БД PostgreSQL (ЛР2, пункт 5).
/// Использует делегацию обращения к БД через одиночку DatabaseConnection.
/// </summary>
public class Client_rep_DB
{
    private readonly DatabaseConnection _db;

    public Client_rep_DB(string? connectionString = null)
    {
        _db = string.IsNullOrWhiteSpace(connectionString)
            ? DatabaseConnection.GetInstance()
            : DatabaseConnection.GetInstance(connectionString);
    }

    public Client_rep_DB(DatabaseConnection db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    /// <summary>
    /// a. Получить объект по ID.
    /// </summary>
    public Client? GetById(int id)
    {
        const string sql = """
            SELECT id, last_name, first_name, middle_name, phone, email, passport_series_number, address
            FROM clients
            WHERE id = @id;
            """;

        var parameters = new Dictionary<string, object?> { ["@id"] = id };
        DataTable table = _db.ExecuteQuery(sql, parameters);

        if (table.Rows.Count == 0)
            return null;

        return MapClientFromRow(table.Rows[0]);
    }

    /// <summary>
    /// b. get_k_n_short_list: Получить список k по счету n объектов класса ClientShort.
    /// </summary>
    public List<ClientShort> Get_k_n_short_list(int k, int n)
    {
        if (k < 1)
            throw new ArgumentOutOfRangeException(nameof(k), "Номер страницы (k) должен быть больше или равен 1.");
        if (n < 1)
            throw new ArgumentOutOfRangeException(nameof(n), "Количество элементов (n) должно быть больше или равно 1.");

        const string sql = """
            SELECT id, last_name, first_name, middle_name, phone, email, passport_series_number, address
            FROM clients
            ORDER BY id
            LIMIT @limit OFFSET @offset;
            """;

        var parameters = new Dictionary<string, object?>
        {
            ["@limit"] = n,
            ["@offset"] = (k - 1) * n
        };

        DataTable table = _db.ExecuteQuery(sql, parameters);
        var result = new List<ClientShort>(table.Rows.Count);

        foreach (DataRow row in table.Rows)
        {
            Client fullClient = MapClientFromRow(row);
            result.Add(new ClientShort(fullClient));
        }

        return result;
    }

    /// <summary>
    /// c. Добавить объект в список (при добавлении сформировать новый ID средствами БД).
    /// </summary>
    public Client Add(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);

        const string sql = """
            INSERT INTO clients (last_name, first_name, middle_name, phone, email, passport_series_number, address)
            VALUES (@lastName, @firstName, @middleName, @phone, @email, @passport, @address)
            RETURNING id;
            """;

        int newId = _db.ExecuteScalar<int>(sql, CreateClientParameters(client));
        client.Id = newId;
        return client;
    }

    /// <summary>
    /// d. Заменить элемент списка по ID.
    /// </summary>
    public bool ReplaceById(int id, Client updatedClient)
    {
        ArgumentNullException.ThrowIfNull(updatedClient);

        const string sql = """
            UPDATE clients
            SET last_name = @lastName,
                first_name = @firstName,
                middle_name = @middleName,
                phone = @phone,
                email = @email,
                passport_series_number = @passport,
                address = @address
            WHERE id = @id;
            """;

        var parameters = CreateClientParameters(updatedClient);
        parameters["@id"] = id;

        int affected = _db.ExecuteNonQuery(sql, parameters);
        if (affected > 0)
        {
            updatedClient.Id = id;
            return true;
        }

        return false;
    }

    /// <summary>
    /// e. Удалить элемент списка по ID.
    /// </summary>
    public bool DeleteById(int id)
    {
        const string sql = "DELETE FROM clients WHERE id = @id;";
        var parameters = new Dictionary<string, object?> { ["@id"] = id };

        int affected = _db.ExecuteNonQuery(sql, parameters);
        return affected > 0;
    }

    /// <summary>
    /// f. get_count: Получить количество элементов.
    /// </summary>
    public int GetCount()
    {
        const string sql = "SELECT COUNT(*) FROM clients;";
        return _db.ExecuteScalar<int>(sql);
    }

    /// <summary>
    /// Получение всех клиентов из БД.
    /// </summary>
    public List<Client> GetAll()
    {
        const string sql = """
            SELECT id, last_name, first_name, middle_name, phone, email, passport_series_number, address
            FROM clients
            ORDER BY id;
            """;

        DataTable table = _db.ExecuteQuery(sql);
        var result = new List<Client>(table.Rows.Count);
        foreach (DataRow row in table.Rows)
        {
            result.Add(MapClientFromRow(row));
        }

        return result;
    }

    /// <summary>
    /// Очистка таблицы (вспомогательный метод для демонстрации).
    /// </summary>
    public void ClearAll()
    {
        const string sql = "TRUNCATE TABLE clients RESTART IDENTITY CASCADE;";
        _db.ExecuteNonQuery(sql);
    }

    private static Dictionary<string, object?> CreateClientParameters(Client client)
    {
        return new Dictionary<string, object?>
        {
            ["@lastName"] = client.LastName,
            ["@firstName"] = client.FirstName,
            ["@middleName"] = client.MiddleName,
            ["@phone"] = client.Phone,
            ["@email"] = client.Email,
            ["@passport"] = client.PassportSeriesNumber,
            ["@address"] = client.Address
        };
    }

    private static Client MapClientFromRow(DataRow row)
    {
        int id = Convert.ToInt32(row["id"], CultureInfo.InvariantCulture);
        string lastName = Convert.ToString(row["last_name"], CultureInfo.InvariantCulture)!;
        string firstName = Convert.ToString(row["first_name"], CultureInfo.InvariantCulture)!;
        string? middleName = row.IsNull("middle_name") ? null : Convert.ToString(row["middle_name"], CultureInfo.InvariantCulture);
        string phone = Convert.ToString(row["phone"], CultureInfo.InvariantCulture)!;
        string? email = row.IsNull("email") ? null : Convert.ToString(row["email"], CultureInfo.InvariantCulture);
        string passport = Convert.ToString(row["passport_series_number"], CultureInfo.InvariantCulture)!;
        string? address = row.IsNull("address") ? null : Convert.ToString(row["address"], CultureInfo.InvariantCulture);

        return new Client(id, lastName, firstName, middleName, phone, email, passport, address);
    }
}

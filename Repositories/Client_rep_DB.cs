using System.Data;
using JewelryWorkshop.Models;
using Npgsql;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Репозиторий сущности Client для работы с реляционной базой данных PostgreSQL (ЛР2, Пункт 4).
/// Реализует CRUD-операции исключительно через сырые SQL-запросы (без ORM и фреймворков).
/// </summary>
public class Client_rep_DB
{
    private readonly string _connectionString;

    /// <summary>
    /// Конструктор репозитория БД.
    /// </summary>
    /// <param name="connectionString">Строка подключения к базе данных PostgreSQL.</param>
    public Client_rep_DB(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Строка подключения не может быть пустой.", nameof(connectionString));

        _connectionString = connectionString;
        EnsureTableExists();
    }

    /// <summary>
    /// Создание вспомогательного открытого подключения.
    /// </summary>
    private NpgsqlConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>
    /// Проверка и создание таблицы clients, если она отсутствует в БД.
    /// </summary>
    public void EnsureTableExists()
    {
        using var conn = CreateConnection();
        const string sql = """
            CREATE TABLE IF NOT EXISTS clients (
                id SERIAL PRIMARY KEY,
                last_name VARCHAR(60) NOT NULL,
                first_name VARCHAR(60) NOT NULL,
                middle_name VARCHAR(60),
                phone VARCHAR(20) NOT NULL UNIQUE,
                email VARCHAR(100),
                passport_series_number VARCHAR(20) NOT NULL UNIQUE,
                address VARCHAR(255)
            );
            """;
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }

    /// <summary>
    /// a. Получить объект по ID.
    /// </summary>
    /// <param name="id">Идентификатор клиента.</param>
    /// <returns>Объект Client или null, если запись не найдена.</returns>
    public Client? GetById(int id)
    {
        using var conn = CreateConnection();
        const string sql = """
            SELECT id, last_name, first_name, middle_name, phone, email, passport_series_number, address
            FROM clients
            WHERE id = @id;
            """;

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

        return MapRowToClient(reader);
    }

    /// <summary>
    /// b. get_k_n_short_list: Получить список k по счету n объектов класса short.
    /// </summary>
    /// <param name="k">Номер страницы (начиная с 1).</param>
    /// <param name="n">Размер страницы (количество элементов).</param>
    /// <returns>Список объектов ClientShort.</returns>
    public List<ClientShort> get_k_n_short_list(int k, int n)
    {
        if (k < 1)
            throw new ArgumentOutOfRangeException(nameof(k), "Номер страницы k должен быть >= 1.");
        if (n < 1)
            throw new ArgumentOutOfRangeException(nameof(n), "Размер страницы n должен быть >= 1.");

        int offset = (k - 1) * n;

        using var conn = CreateConnection();
        const string sql = """
            SELECT id, last_name, first_name, middle_name, phone, passport_series_number
            FROM clients
            ORDER BY id
            OFFSET @offset LIMIT @limit;
            """;

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@offset", offset);
        cmd.Parameters.AddWithValue("@limit", n);

        using var reader = cmd.ExecuteReader();
        var result = new List<ClientShort>();

        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string lastName = reader.GetString(1);
            string firstName = reader.GetString(2);
            string? middleName = reader.IsDBNull(3) ? null : reader.GetString(3);
            string phone = reader.GetString(4);
            string passport = reader.GetString(5);

            string shortName = string.IsNullOrWhiteSpace(middleName)
                ? $"{lastName} {char.ToUpperInvariant(firstName[0])}."
                : $"{lastName} {char.ToUpperInvariant(firstName[0])}. {char.ToUpperInvariant(middleName[0])}.";

            result.Add(new ClientShort(id, shortName, phone, passport));
        }

        return result;
    }

    /// <summary>
    /// PascalCase-синоним для get_k_n_short_list.
    /// </summary>
    public List<ClientShort> GetKNShortList(int k, int n) => get_k_n_short_list(k, n);

    /// <summary>
    /// c. Добавить объект в БД (при добавлении сформировать и вернуть новый ID).
    /// </summary>
    /// <param name="client">Клиент для добавления.</param>
    /// <returns>Присвоенный СУБД новый идентификатор.</returns>
    public int Add(Client client)
    {
        if (client == null)
            throw new ArgumentNullException(nameof(client));

        using var conn = CreateConnection();
        const string sql = """
            INSERT INTO clients (last_name, first_name, middle_name, phone, email, passport_series_number, address)
            VALUES (@lastName, @firstName, @middleName, @phone, @email, @passport, @address)
            RETURNING id;
            """;

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@lastName", client.LastName);
        cmd.Parameters.AddWithValue("@firstName", client.FirstName);
        cmd.Parameters.AddWithValue("@middleName", (object?)client.MiddleName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@phone", client.Phone);
        cmd.Parameters.AddWithValue("@email", (object?)client.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@passport", client.PassportSeriesNumber);
        cmd.Parameters.AddWithValue("@address", (object?)client.Address ?? DBNull.Value);

        object? result = cmd.ExecuteScalar();
        if (result == null || !int.TryParse(result.ToString(), out int generatedId))
        {
            throw new InvalidOperationException("Не удалось получить сгенерированный ID при вставке в БД.");
        }

        client.Id = generatedId;
        return generatedId;
    }

    /// <summary>
    /// d. Заменить элемент списка по ID.
    /// </summary>
    /// <param name="id">ID обновляемой записи.</param>
    /// <param name="newClient">Новые данные клиента.</param>
    /// <returns>True, если запись была найдена и обновлена; иначе false.</returns>
    public bool ReplaceById(int id, Client newClient)
    {
        if (newClient == null)
            throw new ArgumentNullException(nameof(newClient));

        using var conn = CreateConnection();
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

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@lastName", newClient.LastName);
        cmd.Parameters.AddWithValue("@firstName", newClient.FirstName);
        cmd.Parameters.AddWithValue("@middleName", (object?)newClient.MiddleName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@phone", newClient.Phone);
        cmd.Parameters.AddWithValue("@email", (object?)newClient.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@passport", newClient.PassportSeriesNumber);
        cmd.Parameters.AddWithValue("@address", (object?)newClient.Address ?? DBNull.Value);

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected > 0)
        {
            newClient.Id = id;
            return true;
        }

        return false;
    }

    /// <summary>
    /// e. Удалить элемент списка по ID.
    /// </summary>
    /// <param name="id">Идентификатор удаляемого клиента.</param>
    /// <returns>True, если строка удалена; иначе false.</returns>
    public bool DeleteById(int id)
    {
        using var conn = CreateConnection();
        const string sql = "DELETE FROM clients WHERE id = @id;";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        return cmd.ExecuteNonQuery() > 0;
    }

    /// <summary>
    /// f. get_count: Получить количество элементов в БД.
    /// </summary>
    /// <returns>Количество записей в таблице clients.</returns>
    public int get_count()
    {
        using var conn = CreateConnection();
        const string sql = "SELECT COUNT(*) FROM clients;";

        using var cmd = new NpgsqlCommand(sql, conn);
        object? result = cmd.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    /// <summary>
    /// PascalCase-синоним для get_count.
    /// </summary>
    public int GetCount() => get_count();

    /// <summary>
    /// Вспомогательный метод маппинга строки IDataReader в объект Client.
    /// </summary>
    private static Client MapRowToClient(IDataReader reader)
    {
        int id = reader.GetInt32(reader.GetOrdinal("id"));
        string lastName = reader.GetString(reader.GetOrdinal("last_name"));
        string firstName = reader.GetString(reader.GetOrdinal("first_name"));
        string? middleName = reader.IsDBNull(reader.GetOrdinal("middle_name"))
            ? null
            : reader.GetString(reader.GetOrdinal("middle_name"));
        string phone = reader.GetString(reader.GetOrdinal("phone"));
        string? email = reader.IsDBNull(reader.GetOrdinal("email"))
            ? null
            : reader.GetString(reader.GetOrdinal("email"));
        string passport = reader.GetString(reader.GetOrdinal("passport_series_number"));
        string? address = reader.IsDBNull(reader.GetOrdinal("address"))
            ? null
            : reader.GetString(reader.GetOrdinal("address"));

        return new Client(id, lastName, firstName, middleName, phone, email, passport, address);
    }
}

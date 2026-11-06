using JewelryWorkshop.Models;
using Npgsql;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Репозиторий для работы с сущностью Client в БД PostgreSQL с использованием чистых SQL-запросов (ЛР2, пункт 4).
/// </summary>
public class Client_rep_DB
{
    private readonly string _connectionString;

    public Client_rep_DB(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Строка подключения не может быть пустой.", nameof(connectionString));

        _connectionString = connectionString;
        EnsureDatabaseAndTableExist();
    }

    /// <summary>
    /// Автоматическое создание БД (если не существует) и таблицы clients.
    /// </summary>
    private void EnsureDatabaseAndTableExist()
    {
        var builder = new NpgsqlConnectionStringBuilder(_connectionString);
        string? targetDatabase = builder.Database;

        if (!string.IsNullOrWhiteSpace(targetDatabase) &&
            !string.Equals(targetDatabase, "postgres", StringComparison.OrdinalIgnoreCase))
        {
            var adminBuilder = new NpgsqlConnectionStringBuilder(_connectionString)
            {
                Database = "postgres"
            };

            using var adminConn = new NpgsqlConnection(adminBuilder.ConnectionString);
            adminConn.Open();

            using var checkCmd = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @dbName;", adminConn);
            checkCmd.Parameters.AddWithValue("@dbName", targetDatabase);
            object? exists = checkCmd.ExecuteScalar();

            if (exists is null)
            {
                using var createDbCmd = new NpgsqlCommand($"CREATE DATABASE \"{targetDatabase}\";", adminConn);
                createDbCmd.ExecuteNonQuery();
            }
        }

        const string createTableSql = """
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

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        using var cmd = new NpgsqlCommand(createTableSql, conn);
        cmd.ExecuteNonQuery();
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

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

        return MapClientFromReader(reader);
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

        int offset = (k - 1) * n;
        var result = new List<ClientShort>();

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@limit", n);
        cmd.Parameters.AddWithValue("@offset", offset);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Client fullClient = MapClientFromReader(reader);
            result.Add(new ClientShort(fullClient));
        }

        return result;
    }

    /// <summary>
    /// c. Добавить объект в список (при добавлении сформировать новый ID средствами БД).
    /// </summary>
    public Client Add(Client client)
    {
        if (client is null)
            throw new ArgumentNullException(nameof(client));

        const string sql = """
            INSERT INTO clients (last_name, first_name, middle_name, phone, email, passport_series_number, address)
            VALUES (@lastName, @firstName, @middleName, @phone, @email, @passport, @address)
            RETURNING id;
            """;

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(sql, conn);
        AddClientParameters(cmd, client);

        object? scalar = cmd.ExecuteScalar();
        int newId = Convert.ToInt32(scalar);
        client.Id = newId;
        return client;
    }

    /// <summary>
    /// d. Заменить элемент списка по ID.
    /// </summary>
    public bool ReplaceById(int id, Client updatedClient)
    {
        if (updatedClient is null)
            throw new ArgumentNullException(nameof(updatedClient));

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

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        AddClientParameters(cmd, updatedClient);

        int affected = cmd.ExecuteNonQuery();
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

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        int affected = cmd.ExecuteNonQuery();
        return affected > 0;
    }

    /// <summary>
    /// f. get_count: Получить количество элементов.
    /// </summary>
    public int GetCount()
    {
        const string sql = "SELECT COUNT(*) FROM clients;";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(sql, conn);
        object? scalar = cmd.ExecuteScalar();
        return Convert.ToInt32(scalar);
    }

    /// <summary>
    /// Очистка таблицы (вспомогательный метод для демонстрации).
    /// </summary>
    public void ClearAll()
    {
        const string sql = "TRUNCATE TABLE clients RESTART IDENTITY CASCADE;";

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.ExecuteNonQuery();
    }

    private static void AddClientParameters(NpgsqlCommand cmd, Client client)
    {
        cmd.Parameters.AddWithValue("@lastName", client.LastName);
        cmd.Parameters.AddWithValue("@firstName", client.FirstName);
        cmd.Parameters.AddWithValue("@middleName", (object?)client.MiddleName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@phone", client.Phone);
        cmd.Parameters.AddWithValue("@email", (object?)client.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@passport", client.PassportSeriesNumber);
        cmd.Parameters.AddWithValue("@address", (object?)client.Address ?? DBNull.Value);
    }

    private static Client MapClientFromReader(NpgsqlDataReader reader)
    {
        int id = reader.GetInt32(0);
        string lastName = reader.GetString(1);
        string firstName = reader.GetString(2);
        string? middleName = reader.IsDBNull(3) ? null : reader.GetString(3);
        string phone = reader.GetString(4);
        string? email = reader.IsDBNull(5) ? null : reader.GetString(5);
        string passport = reader.GetString(6);
        string? address = reader.IsDBNull(7) ? null : reader.GetString(7);

        return new Client(id, lastName, firstName, middleName, phone, email, passport, address);
    }
}


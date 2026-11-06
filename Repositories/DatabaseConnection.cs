using System.Data;
using System.Globalization;
using Npgsql;

namespace JewelryWorkshop.Repositories;

/// <summary>
/// Класс для работы с БД PostgreSQL, реализованный по паттерну Одиночка (Singleton) (ЛР2, пункт 5).
/// Отвечает за подключение к СУБД и выполнение параметризованных SQL-запросов.
/// </summary>
public sealed class DatabaseConnection
{
    private static DatabaseConnection? _instance;
    private static readonly object LockObj = new();

    private readonly string _connectionString;

    /// <summary>
    /// Закрытый конструктор запрещает создание экземпляров через оператор new извне.
    /// </summary>
    private DatabaseConnection(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Строка подключения не может быть пустой.", nameof(connectionString));

        _connectionString = connectionString;
        EnsureDatabaseAndTableExist();
    }

    /// <summary>
    /// Глобальная точка доступа к единственному экземпляру класса (Singleton).
    /// </summary>
    public static DatabaseConnection GetInstance(
        string connectionString = "Host=localhost;Port=5432;Database=jewelry_workshop;Username=postgres;Password=1234")
    {
        if (_instance is null)
        {
            lock (LockObj)
            {
                _instance ??= new DatabaseConnection(connectionString);
            }
        }

        return _instance;
    }

    /// <summary>
    /// Выполнение SQL-запроса SELECT. Возвращает результат в виде таблицы DataTable.
    /// </summary>
    public DataTable ExecuteQuery(string sql, Dictionary<string, object?>? parameters = null)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = CreateCommand(sql, conn, parameters);
        using var reader = cmd.ExecuteReader();

        var table = new DataTable();
        table.Load(reader);
        return table;
    }

    /// <summary>
    /// Выполнение запроса, возвращающего одно скалярное значение (например, COUNT(*) или RETURNING id).
    /// </summary>
    public T ExecuteScalar<T>(string sql, Dictionary<string, object?>? parameters = null)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = CreateCommand(sql, conn, parameters);
        object? result = cmd.ExecuteScalar();
        return (T)Convert.ChangeType(result!, typeof(T), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Выполнение команды модификации данных (INSERT, UPDATE, DELETE, TRUNCATE).
    /// Возвращает количество затронутых строк.
    /// </summary>
    public int ExecuteNonQuery(string sql, Dictionary<string, object?>? parameters = null)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = CreateCommand(sql, conn, parameters);
        return cmd.ExecuteNonQuery();
    }

    private static NpgsqlCommand CreateCommand(
        string sql,
        NpgsqlConnection connection,
        Dictionary<string, object?>? parameters)
    {
        var cmd = new NpgsqlCommand(sql, connection);
        if (parameters is not null)
        {
            foreach (var (key, value) in parameters)
            {
                cmd.Parameters.AddWithValue(key, value ?? DBNull.Value);
            }
        }

        return cmd;
    }

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

        ExecuteNonQuery(createTableSql);
    }
}


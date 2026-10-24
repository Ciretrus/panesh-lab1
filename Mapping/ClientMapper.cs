using JewelryWorkshop.Models;

namespace JewelryWorkshop.Mapping;

/// <summary>
/// Маппер для преобразования объектов между полным классом (Client) и кратким (ClientShort).
/// Предоставляет как реализацию интерфейса IClientMapper, так и статические методы.
/// </summary>
public class ClientMapper : IClientMapper
{
    /// <summary>
    /// Экземпляр маппера по умолчанию (Singleton).
    /// </summary>
    public static ClientMapper Instance { get; } = new ClientMapper();

    /// <summary>
    /// Преобразование полного объекта Client в краткий ClientShort.
    /// </summary>
    public ClientShort ToShort(Client client) => MapToShort(client);

    /// <summary>
    /// Преобразование краткого объекта ClientShort в полный Client (с извлечением инициалов).
    /// </summary>
    public Client ToFull(ClientShort clientShort) => MapToFull(clientShort);

    /// <summary>
    /// Преобразование краткого объекта ClientShort в полный Client с указанием полных данных.
    /// </summary>
    public Client ToFull(
        ClientShort clientShort,
        string firstName,
        string? middleName = null,
        string? email = null,
        string? address = null) =>
        MapToFull(clientShort, firstName, middleName, email, address);

    // --- Статические методы прямого вызова ---

    /// <summary>
    /// Преобразование полного объекта Client в краткий ClientShort.
    /// </summary>
    /// <param name="client">Полный объект клиента.</param>
    /// <returns>Краткий объект ClientShort.</returns>
    public static ClientShort MapToShort(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);
        client.ThrowIfDisposed();

        return new ClientShort(client);
    }

    /// <summary>
    /// Преобразование краткого объекта ClientShort в полный Client.
    /// Фамилия и инициалы извлекаются из ShortName.
    /// Автоматически связывает жизненные циклы объектов (удаление одного удалит второй).
    /// </summary>
    /// <param name="clientShort">Краткий объект клиента.</param>
    /// <param name="email">Необязательный email.</param>
    /// <param name="address">Необязательный адрес.</param>
    /// <returns>Полный объект Client.</returns>
    public static Client MapToFull(
        ClientShort clientShort,
        string? email = null,
        string? address = null)
    {
        ArgumentNullException.ThrowIfNull(clientShort);
        clientShort.ThrowIfDisposed();

        var (lastName, firstName, middleName) = ParseShortName(clientShort.ShortName);

        var fullClient = new Client(
            id: clientShort.Id,
            lastName: lastName,
            firstName: firstName,
            middleName: middleName,
            phone: clientShort.Phone,
            email: email,
            passportSeriesNumber: clientShort.PassportSeriesNumber,
            address: address
        );

        fullClient.LinkWith(clientShort);
        return fullClient;
    }

    /// <summary>
    /// Преобразование краткого объекта ClientShort в полный Client с указанием полных имени и отчества.
    /// Автоматически связывает жизненные циклы объектов (удаление одного удалит второй).
    /// </summary>
    /// <param name="clientShort">Краткий объект клиента.</param>
    /// <param name="firstName">Полное имя.</param>
    /// <param name="middleName">Необязательное отчество.</param>
    /// <param name="email">Необязательный email.</param>
    /// <param name="address">Необязательный адрес.</param>
    /// <returns>Полный объект Client.</returns>
    public static Client MapToFull(
        ClientShort clientShort,
        string firstName,
        string? middleName,
        string? email = null,
        string? address = null)
    {
        ArgumentNullException.ThrowIfNull(clientShort);
        clientShort.ThrowIfDisposed();

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Имя не может быть пустым.", nameof(firstName));

        var (lastName, _, _) = ParseShortName(clientShort.ShortName);

        var fullClient = new Client(
            id: clientShort.Id,
            lastName: lastName,
            firstName: firstName.Trim(),
            middleName: string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim(),
            phone: clientShort.Phone,
            email: email,
            passportSeriesNumber: clientShort.PassportSeriesNumber,
            address: address
        );

        fullClient.LinkWith(clientShort);
        return fullClient;
    }

    /// <summary>
    /// Преобразование коллекции полных объектов Client в список кратких ClientShort.
    /// </summary>
    public static List<ClientShort> MapToShortList(IEnumerable<Client> clients)
    {
        ArgumentNullException.ThrowIfNull(clients);
        return clients.Select(MapToShort).ToList();
    }

    /// <summary>
    /// Преобразование коллекции кратких объектов ClientShort в список полных Client.
    /// </summary>
    public static List<Client> MapToFullList(IEnumerable<ClientShort> clients)
    {
        ArgumentNullException.ThrowIfNull(clients);
        return clients.Select(c => MapToFull(c)).ToList();
    }

    /// <summary>
    /// Разбор строки краткого имени ('Фамилия И.' или 'Фамилия И. О.') на компоненты.
    /// </summary>
    private static (string LastName, string FirstName, string? MiddleName) ParseShortName(string shortName)
    {
        if (string.IsNullOrWhiteSpace(shortName))
            throw new ArgumentException("Краткое имя не может быть пустым.", nameof(shortName));

        string[] parts = shortName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
            throw new ArgumentException($"Не удалось разобрать краткое имя '{shortName}'. Ожидался формат 'Фамилия И.' или 'Фамилия И. О.'.", nameof(shortName));

        string lastName = parts[0];
        string firstName = parts[1].TrimEnd('.');
        string? middleName = parts.Length > 2 ? parts[2].TrimEnd('.') : null;

        return (lastName, firstName, middleName);
    }
}

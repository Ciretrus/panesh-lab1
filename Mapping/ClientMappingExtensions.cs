using JewelryWorkshop.Models;

namespace JewelryWorkshop.Mapping;

/// <summary>
/// Методы расширения для быстрого вызова маппинга у объектов Client и ClientShort.
/// </summary>
public static class ClientMappingExtensions
{
    /// <summary>
    /// Преобразовать полный объект Client в краткий ClientShort.
    /// </summary>
    public static ClientShort ToShort(this Client client) =>
        ClientMapper.MapToShort(client);

    /// <summary>
    /// Преобразовать краткий объект ClientShort в полный Client (с извлечением инициалов).
    /// </summary>
    public static Client ToFull(this ClientShort clientShort, string? email = null, string? address = null) =>
        ClientMapper.MapToFull(clientShort, email, address);

    /// <summary>
    /// Преобразовать краткий объект ClientShort в полный Client с указанием полного имени.
    /// </summary>
    public static Client ToFull(
        this ClientShort clientShort,
        string firstName,
        string? middleName = null,
        string? email = null,
        string? address = null) =>
        ClientMapper.MapToFull(clientShort, firstName, middleName, email, address);

    /// <summary>
    /// Преобразовать коллекцию Client в список ClientShort.
    /// </summary>
    public static List<ClientShort> ToShortList(this IEnumerable<Client> clients) =>
        ClientMapper.MapToShortList(clients);

    /// <summary>
    /// Преобразовать коллекцию ClientShort в список Client.
    /// </summary>
    public static List<Client> ToFullList(this IEnumerable<ClientShort> clients) =>
        ClientMapper.MapToFullList(clients);
}

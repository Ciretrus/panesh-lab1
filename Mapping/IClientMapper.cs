using JewelryWorkshop.Models;

namespace JewelryWorkshop.Mapping;

/// <summary>
/// Интерфейс маппера для преобразования между полным (Client) и кратким (ClientShort) классами.
/// </summary>
public interface IClientMapper
{
    /// <summary>
    /// Преобразование полного объекта Client в краткий ClientShort.
    /// </summary>
    ClientShort ToShort(Client client);

    /// <summary>
    /// Преобразование краткого объекта ClientShort в полный Client (с извлечением инициалов).
    /// </summary>
    Client ToFull(ClientShort clientShort);

    /// <summary>
    /// Преобразование краткого объекта ClientShort в полный Client с указанием недостающих данных.
    /// </summary>
    Client ToFull(ClientShort clientShort, string firstName, string? middleName = null, string? email = null, string? address = null);
}

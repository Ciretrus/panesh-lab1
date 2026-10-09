using System.Text.RegularExpressions;

namespace JewelryWorkshop.Models;

/// <summary>
/// Краткая версия данных клиента: ID, Фамилия Инициалы, контактный телефон и номер документа.
/// </summary>
public class ClientShort : IEquatable<ClientShort>
{
    private const string PhonePattern = @"^\+?[0-9]{10,15}$";
    private const string PassportPattern = @"^\d{4}\s?\d{6}$";
    private const string ShortNamePattern = @"^[a-zA-Zа-яА-ЯёЁ\-]+\s+[a-zA-Zа-яА-ЯёЁ]\.(?:\s*[a-zA-Zа-яА-ЯёЁ]\.)?$";

    // Закрытые поля
    private int _id;
    private string _shortName = string.Empty;
    private string _phone = string.Empty;
    private string _passportSeriesNumber = string.Empty;

    // --- Конструкторы ---

    /// <summary>
    /// Конструктор со всеми параметрами краткой сущности.
    /// </summary>
    public ClientShort(int id, string shortName, string phone, string passportSeriesNumber)
    {
        Id = id;
        ShortName = shortName;
        Phone = phone;
        PassportSeriesNumber = passportSeriesNumber;
    }

    /// <summary>
    /// Конструктор, создающий краткую версию на основе полного объекта Client.
    /// </summary>
    public ClientShort(Client client)
        : this(
            client == null ? throw new ArgumentNullException(nameof(client)) : client.Id,
            client.GetShortName(),
            client.Phone,
            client.PassportSeriesNumber)
    {
    }

    /// <summary>
    /// Конструктор из строки с разделителем ';' (формат: "id;Фамилия И. О.;телефон;паспорт").
    /// </summary>
    public ClientShort(string rawData)
        : this(ParseString(rawData))
    {
    }

    private ClientShort((int id, string shortName, string phone, string passport) data)
        : this(data.id, data.shortName, data.phone, data.passport)
    {
    }

    // --- Свойства с валидацией ---

    public int Id
    {
        get => _id;
        set
        {
            if (!IsValidId(value))
                throw new ArgumentException("ID клиента должен быть положительным числом.", nameof(value));
            _id = value;
        }
    }

    public string ShortName
    {
        get => _shortName;
        set
        {
            if (!IsValidShortName(value))
                throw new ArgumentException("Краткое имя должно иметь формат 'Фамилия И.' или 'Фамилия И. О.'.", nameof(value));
            _shortName = value.Trim();
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            if (!IsValidPhone(value))
                throw new ArgumentException("Номер телефона имеет неверный формат.", nameof(value));
            _phone = value.Trim();
        }
    }

    public string PassportSeriesNumber
    {
        get => _passportSeriesNumber;
        set
        {
            if (!IsValidPassportSeriesNumber(value))
                throw new ArgumentException("Паспортные данные должны содержать 10 цифр (серия и номер).", nameof(value));
            _passportSeriesNumber = value.Trim();
        }
    }

    // --- Методы строкового представления ---

    public override string ToString()
    {
        return $"Краткий клиент [ID={Id}]: {ShortName} | Тел: {Phone} | Паспорт: {PassportSeriesNumber}";
    }

    // --- Сравнение на равенство ---

    public bool Equals(ClientShort? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id == other.Id &&
               string.Equals(ShortName, other.ShortName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Phone, other.Phone, StringComparison.Ordinal) &&
               string.Equals(PassportSeriesNumber, other.PassportSeriesNumber, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as ClientShort);

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, StringComparer.OrdinalIgnoreCase.GetHashCode(ShortName), Phone, PassportSeriesNumber);
    }

    public static bool operator ==(ClientShort? left, ClientShort? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(ClientShort? left, ClientShort? right) => !(left == right);

    // --- Статические методы парсинга и валидации ---

    private static (int, string, string, string) ParseString(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
            throw new ArgumentException("Строка не может быть пустой.", nameof(rawData));

        string[] parts = rawData.Trim().Split(';');
        if (parts.Length != 4)
            throw new ArgumentException("Строка должна содержать 4 поля через ';': id;краткое_имя;телефон;паспорт", nameof(rawData));

        if (!int.TryParse(parts[0].Trim(), out int id))
            throw new ArgumentException("ID должен быть целым числом.", nameof(rawData));

        return (id, parts[1].Trim(), parts[2].Trim(), parts[3].Trim());
    }

    public static bool IsValidId(int id) => id > 0;

    public static bool IsValidShortName(string? shortName)
    {
        if (string.IsNullOrWhiteSpace(shortName))
            return false;
        return Regex.IsMatch(shortName.Trim(), ShortNamePattern);
    }

    public static bool IsValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;
        return Regex.IsMatch(phone.Trim(), PhonePattern);
    }

    public static bool IsValidPassportSeriesNumber(string? passport)
    {
        if (string.IsNullOrWhiteSpace(passport))
            return false;
        return Regex.IsMatch(passport.Trim(), PassportPattern);
    }
}

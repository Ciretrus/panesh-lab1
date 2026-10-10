using System.Text.RegularExpressions;

namespace JewelryWorkshop.Models;

/// <summary>
/// Краткая версия данных клиента: ID, Фамилия Инициалы, контактный телефон и номер документа.
/// Наследует общий базовый класс ClientBase, устраняя дублирование кода.
/// </summary>
public class ClientShort : ClientBase, IEquatable<ClientShort>, IDisposable
{
    private const string ShortNamePattern = @"^[a-zA-Zа-яА-ЯёЁ\-]+\s+[a-zA-Zа-яА-ЯёЁ]\.(?:\s*[a-zA-Zа-яА-ЯёЁ]\.)?$";

    // Закрытое поле, специфичное только для краткой версии
    private string _shortName = string.Empty;

    // --- Конструкторы ---

    public ClientShort(int id, string shortName, string phone, string passportSeriesNumber)
        : base(id, phone, passportSeriesNumber)
    {
        ShortName = shortName;
    }

    public ClientShort(Client client)
        : this(
            client == null ? throw new ArgumentNullException(nameof(client)) : client.Id,
            client.GetShortName(),
            client.Phone,
            client.PassportSeriesNumber)
    {
        client.OnDestroy += Dispose;
    }

    public ClientShort(string rawData)
        : this(ParseString(rawData))
    {
    }

    private ClientShort((int id, string shortName, string phone, string passport) data)
        : this(data.id, data.shortName, data.phone, data.passport)
    {
    }

    // --- Свойства ---

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

    // --- Переопределение методов базового класса ---

    public override string GetShortName() => ShortName;

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

    // --- Парсинг и валидация специфичных полей ---

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

    public static bool IsValidShortName(string? shortName)
    {
        if (string.IsNullOrWhiteSpace(shortName))
            return false;
        return Regex.IsMatch(shortName.Trim(), ShortNamePattern);
    }

    public void Dispose()
    {

    }
}

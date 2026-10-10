using System.Text.Json;
using System.Text.RegularExpressions;

namespace JewelryWorkshop.Models;

/// <summary>
/// Полная сущность "Клиент" ювелирной мастерской.
/// Наследует базовый класс ClientBase, устраняя дублирование общих полей и логики.
/// </summary>
public class Client : ClientBase, IEquatable<Client>, IDisposable
{
    private const string NamePattern = @"^[a-zA-Zа-яА-ЯёЁ\-]+$";
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

    // Закрытые поля, специфичные для полной сущности
    private string _lastName = string.Empty;
    private string _firstName = string.Empty;
    private string? _middleName;
    private string? _email;
    private string? _address;
    public event Action OnDestroy;

    // --- Конструкторы ---

    /// <summary>
    /// 1. Основной канонический конструктор.
    /// </summary>
    public Client(
        int id,
        string lastName,
        string firstName,
        string? middleName,
        string phone,
        string? email,
        string passportSeriesNumber,
        string? address)
        : base(id, phone, passportSeriesNumber)
    {
        LastName = lastName;
        FirstName = firstName;
        MiddleName = middleName;
        Email = email;
        Address = address;
    }

    /// <summary>
    /// 2. Краткий конструктор (только обязательные поля).
    /// </summary>
    public Client(int id, string lastName, string firstName, string phone, string passportSeriesNumber)
        : this(id, lastName, firstName, middleName: null, phone, email: null, passportSeriesNumber, address: null)
    {
    }

    /// <summary>
    /// 3. Перегруженный конструктор из форматированной строки (CSV/разделитель ';' или JSON-строка).
    /// </summary>
    public Client(string rawData)
        : this(ParseString(rawData))
    {
    }

    /// <summary>
    /// 4. Перегруженный конструктор из объекта JsonElement.
    /// </summary>
    public Client(JsonElement jsonElement)
        : this(ParseJsonElement(jsonElement))
    {
    }

    private Client((int id, string lastName, string firstName, string? middleName, string phone, string? email, string passport, string? address) data)
        : this(data.id, data.lastName, data.firstName, data.middleName, data.phone, data.email, data.passport, data.address)
    {
    }

    // --- Свойства специфичных полей ---

    public string LastName
    {
        get => _lastName;
        set
        {
            if (!IsValidLastName(value))
                throw new ArgumentException("Фамилия не может быть пустой и должна содержать только буквы.", nameof(value));
            _lastName = value.Trim();
        }
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            if (!IsValidFirstName(value))
                throw new ArgumentException("Имя не может быть пустым и должно содержать только буквы.", nameof(value));
            _firstName = value.Trim();
        }
    }

    public string? MiddleName
    {
        get => _middleName;
        set
        {
            if (!IsValidMiddleName(value))
                throw new ArgumentException("Отчество должно содержать только буквы.", nameof(value));
            _middleName = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }

    public string? Email
    {
        get => _email;
        set
        {
            if (!IsValidEmail(value))
                throw new ArgumentException("Email имеет неверный формат.", nameof(value));
            _email = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }

    public string? Address
    {
        get => _address;
        set
        {
            if (!IsValidAddress(value))
                throw new ArgumentException("Адрес не может превышать 255 символов.", nameof(value));
            _address = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }

    // --- Переопределения методов базового класса ---

    /// <summary>
    /// Формирование краткого имени (Фамилия И. О. или Фамилия И.)
    /// </summary>
    public override string GetShortName()
    {
        char firstInitial = char.ToUpperInvariant(FirstName[0]);
        if (!string.IsNullOrWhiteSpace(MiddleName))
        {
            char middleInitial = char.ToUpperInvariant(MiddleName[0]);
            return $"{LastName} {firstInitial}. {middleInitial}.";
        }

        return $"{LastName} {firstInitial}.";
    }

    /// <summary>
    /// Полная версия строкового представления объекта.
    /// </summary>
    public string ToFullString()
    {
        string middle = string.IsNullOrWhiteSpace(MiddleName) ? string.Empty : $" {MiddleName}";
        string mail = Email ?? "—";
        string addr = Address ?? "—";
        return $"Клиент [ID={Id}]: {LastName} {FirstName}{middle} | Тел: {Phone} | Email: {mail} | Паспорт: {PassportSeriesNumber} | Адрес: {addr}";
    }

    public override string ToString() => ToFullString();

    // --- Сравнение объектов на равенство ---

    public bool Equals(Client? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id == other.Id &&
               string.Equals(LastName, other.LastName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(MiddleName, other.MiddleName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(Phone, other.Phone, StringComparison.Ordinal) &&
               string.Equals(Email, other.Email, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(PassportSeriesNumber, other.PassportSeriesNumber, StringComparison.Ordinal) &&
               string.Equals(Address, other.Address, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Client);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Id);
        hash.Add(LastName, StringComparer.OrdinalIgnoreCase);
        hash.Add(FirstName, StringComparer.OrdinalIgnoreCase);
        hash.Add(MiddleName, StringComparer.OrdinalIgnoreCase);
        hash.Add(Phone, StringComparer.Ordinal);
        hash.Add(Email, StringComparer.OrdinalIgnoreCase);
        hash.Add(PassportSeriesNumber, StringComparer.Ordinal);
        hash.Add(Address, StringComparer.OrdinalIgnoreCase);
        return hash.ToHashCode();
    }

    public static bool operator ==(Client? left, Client? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Client? left, Client? right) => !(left == right);

    // --- Вспомогательные методы парсинга ---

    private static (int, string, string, string?, string, string?, string, string?) ParseString(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
            throw new ArgumentException("Входная строка не может быть пустой.", nameof(rawData));

        string trimmed = rawData.Trim();

        if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
        {
            using var doc = JsonDocument.Parse(trimmed);
            return ParseJsonElement(doc.RootElement);
        }

        string[] parts = trimmed.Split(';');
        if (parts.Length != 8)
        {
            throw new ArgumentException(
                "Строка должна содержать 8 полей, разделенных точкой с запятой: id;фамилия;имя;отчество;телефон;email;паспорт;адрес",
                nameof(rawData));
        }

        if (!int.TryParse(parts[0].Trim(), out int id))
            throw new ArgumentException("Первое поле (ID) должно быть целым числом.", nameof(rawData));

        string lastName = parts[1].Trim();
        string firstName = parts[2].Trim();
        string? middleName = string.IsNullOrWhiteSpace(parts[3]) ? null : parts[3].Trim();
        string phone = parts[4].Trim();
        string? email = string.IsNullOrWhiteSpace(parts[5]) ? null : parts[5].Trim();
        string passport = parts[6].Trim();
        string? address = string.IsNullOrWhiteSpace(parts[7]) ? null : parts[7].Trim();

        return (id, lastName, firstName, middleName, phone, email, passport, address);
    }

    private static (int, string, string, string?, string, string?, string, string?) ParseJsonElement(JsonElement element)
    {
        if (element.ValueKind != JsonValueKind.Object)
            throw new ArgumentException("JSON должен представлять объект.", nameof(element));

        int id = element.TryGetProperty("id", out var idProp) && idProp.TryGetInt32(out int parsedId)
            ? parsedId
            : throw new ArgumentException("JSON должен содержать числовое поле 'id'.");

        string lastName = element.TryGetProperty("lastName", out var lnProp) && lnProp.GetString() is string ln
            ? ln
            : throw new ArgumentException("JSON должен содержать строковое поле 'lastName'.");

        string firstName = element.TryGetProperty("firstName", out var fnProp) && fnProp.GetString() is string fn
            ? fn
            : throw new ArgumentException("JSON должен содержать строковое поле 'firstName'.");

        string? middleName = element.TryGetProperty("middleName", out var mnProp) && mnProp.ValueKind != JsonValueKind.Null
            ? mnProp.GetString()
            : null;

        string phone = element.TryGetProperty("phone", out var phProp) && phProp.GetString() is string ph
            ? ph
            : throw new ArgumentException("JSON должен содержать строковое поле 'phone'.");

        string? email = element.TryGetProperty("email", out var emProp) && emProp.ValueKind != JsonValueKind.Null
            ? emProp.GetString()
            : null;

        string passport = element.TryGetProperty("passportSeriesNumber", out var psProp) && psProp.GetString() is string ps
            ? ps
            : throw new ArgumentException("JSON должен содержать строковое поле 'passportSeriesNumber'.");

        string? address = element.TryGetProperty("address", out var adProp) && adProp.ValueKind != JsonValueKind.Null
            ? adProp.GetString()
            : null;

        return (id, lastName, firstName, middleName, phone, email, passport, address);
    }

    // --- Валидация специфичных для полной сущности полей ---

    private static bool ValidateNamePart(string? value, bool isRequired, int maxLength = 60)
    {
        if (string.IsNullOrWhiteSpace(value))
            return !isRequired;

        string trimmed = value.Trim();
        return trimmed.Length <= maxLength && Regex.IsMatch(trimmed, NamePattern);
    }

    public static bool IsValidLastName(string? lastName) =>
        ValidateNamePart(lastName, isRequired: true);

    public static bool IsValidFirstName(string? firstName) =>
        ValidateNamePart(firstName, isRequired: true);

    public static bool IsValidMiddleName(string? middleName) =>
        ValidateNamePart(middleName, isRequired: false);

    public static bool IsValidEmail(string? email) =>
        ValidateRegex(email, EmailPattern, isRequired: false);

    public static bool IsValidAddress(string? address) =>
        ValidateLength(address, maxLength: 255, isRequired: false);

    public void Dispose()
    {
        OnDestroy?.Invoke();
    }
}

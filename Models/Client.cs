using System.Text.Json;
using System.Text.RegularExpressions;

namespace JewelryWorkshop.Models;

/// <summary>
/// Сущность "Клиент" ювелирной мастерской с валидацией полей и перегруженными конструкторами.
/// </summary>
public class Client
{
    // Шаблоны регулярных выражений для проверки
    private const string NamePattern = @"^[a-zA-Zа-яА-ЯёЁ\-]+$";
    private const string PhonePattern = @"^\+?[0-9]{10,15}$";
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    private const string PassportPattern = @"^\d{4}\s?\d{6}$";

    // Закрытые поля (инкапсуляция)
    private int _id;
    private string _lastName = string.Empty;
    private string _firstName = string.Empty;
    private string? _middleName;
    private string _phone = string.Empty;
    private string? _email;
    private string _passportSeriesNumber = string.Empty;
    private string? _address;

    // --- Конструкторы ---

    /// <summary>
    /// 1. Основной канонический конструктор со всеми полями.
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
    {
        Id = id;
        LastName = lastName;
        FirstName = firstName;
        MiddleName = middleName;
        Phone = phone;
        Email = email;
        PassportSeriesNumber = passportSeriesNumber;
        Address = address;
    }

    /// <summary>
    /// 2. Краткий конструктор (только обязательные поля, опциональные поля равны null).
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

    /// <summary>
    /// Вспомогательный закрытый конструктор для распаковки кортежа данных.
    /// </summary>
    private Client((int id, string lastName, string firstName, string? middleName, string phone, string? email, string passport, string? address) data)
        : this(data.id, data.lastName, data.firstName, data.middleName, data.phone, data.email, data.passport, data.address)
    {
    }

    // Свойства с валидацией
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

    public string Phone
    {
        get => _phone;
        set
        {
            if (!IsValidPhone(value))
                throw new ArgumentException("Номер телефона имеет неверный формат (ожидается от 10 до 15 цифр, опционально '+').", nameof(value));
            _phone = value.Trim();
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

    // --- Вспомогательные методы парсинга для нетривиальных конструкторов ---

    private static (int, string, string, string?, string, string?, string, string?) ParseString(string rawData)
    {
        if (string.IsNullOrWhiteSpace(rawData))
            throw new ArgumentException("Входная строка не может быть пустой.", nameof(rawData));

        string trimmed = rawData.Trim();

        // Если передана строка в формате JSON
        if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
        {
            using var doc = JsonDocument.Parse(trimmed);
            return ParseJsonElement(doc.RootElement);
        }

        // Если передана строка с разделителем ';'
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

    // --- Обобщенные методы валидации (устранение дублирования кода) ---

    private static bool ValidateRegex(string? value, string pattern, bool isRequired)
    {
        if (string.IsNullOrWhiteSpace(value))
            return !isRequired;

        return Regex.IsMatch(value.Trim(), pattern);
    }

    private static bool ValidateNamePart(string? value, bool isRequired, int maxLength = 60)
    {
        if (string.IsNullOrWhiteSpace(value))
            return !isRequired;

        string trimmed = value.Trim();
        return trimmed.Length <= maxLength && Regex.IsMatch(trimmed, NamePattern);
    }

    private static bool ValidateLength(string? value, int maxLength, bool isRequired)
    {
        if (string.IsNullOrWhiteSpace(value))
            return !isRequired;

        return value.Trim().Length <= maxLength;
    }

    // --- Публичные статические методы валидации (методы класса) ---

    public static bool IsValidId(int id) => id > 0;

    public static bool IsValidLastName(string? lastName) =>
        ValidateNamePart(lastName, isRequired: true);

    public static bool IsValidFirstName(string? firstName) =>
        ValidateNamePart(firstName, isRequired: true);

    public static bool IsValidMiddleName(string? middleName) =>
        ValidateNamePart(middleName, isRequired: false);

    public static bool IsValidPhone(string? phone) =>
        ValidateRegex(phone, PhonePattern, isRequired: true);

    public static bool IsValidEmail(string? email) =>
        ValidateRegex(email, EmailPattern, isRequired: false);

    public static bool IsValidPassportSeriesNumber(string? passport) =>
        ValidateRegex(passport, PassportPattern, isRequired: true);

    public static bool IsValidAddress(string? address) =>
        ValidateLength(address, maxLength: 255, isRequired: false);
}

using System.Text.RegularExpressions;

namespace JewelryWorkshop.Models;

/// <summary>
/// Сущность "Клиент" ювелирной мастерской с валидацией полей без дублирования кода.
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

    /// <summary>
    /// Конструктор со всеми параметрами.
    /// Инициализация через свойства гарантирует валидацию всех полей.
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

    // --- Обобщенные методы валидации (устранение дублирования кода) ---

    /// <summary>
    /// Проверка соответствия строки регулярному выражению с учётом обязательности.
    /// </summary>
    private static bool ValidateRegex(string? value, string pattern, bool isRequired)
    {
        if (string.IsNullOrWhiteSpace(value))
            return !isRequired;

        return Regex.IsMatch(value.Trim(), pattern);
    }

    /// <summary>
    /// Проверка части ФИО (длина и допустимые символы) с учётом обязательности.
    /// </summary>
    private static bool ValidateNamePart(string? value, bool isRequired, int maxLength = 60)
    {
        if (string.IsNullOrWhiteSpace(value))
            return !isRequired;

        string trimmed = value.Trim();
        return trimmed.Length <= maxLength && Regex.IsMatch(trimmed, NamePattern);
    }

    /// <summary>
    /// Проверка ограничения по максимальной длине строки.
    /// </summary>
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

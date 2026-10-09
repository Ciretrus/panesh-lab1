using System.Text.RegularExpressions;

namespace JewelryWorkshop.Models;

/// <summary>
/// Сущность "Клиент" ювелирной мастерской с валидацией полей.
/// Существование объекта с некорректными данными невозможно.
/// </summary>
public class Client
{
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
    /// Инициализация через свойства гарантирует валидацию.
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

    // Свойства с валидацией при установке значений
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

    // --- Статические методы валидации (методы класса) ---

    public static bool IsValidId(int id)
    {
        return id > 0;
    }

    public static bool IsValidLastName(string? lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return false;
        if (lastName.Trim().Length > 60)
            return false;
        return Regex.IsMatch(lastName.Trim(), @"^[a-zA-Zа-яА-ЯёЁ\-]+$");
    }

    public static bool IsValidFirstName(string? firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return false;
        if (firstName.Trim().Length > 60)
            return false;
        return Regex.IsMatch(firstName.Trim(), @"^[a-zA-Zа-яА-ЯёЁ\-]+$");
    }

    public static bool IsValidMiddleName(string? middleName)
    {
        if (string.IsNullOrWhiteSpace(middleName))
            return true;
        if (middleName.Trim().Length > 60)
            return false;
        return Regex.IsMatch(middleName.Trim(), @"^[a-zA-Zа-яА-ЯёЁ\-]+$");
    }

    public static bool IsValidPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;
        return Regex.IsMatch(phone.Trim(), @"^\+?[0-9]{10,15}$");
    }

    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return true;
        return Regex.IsMatch(email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    public static bool IsValidPassportSeriesNumber(string? passport)
    {
        if (string.IsNullOrWhiteSpace(passport))
            return false;
        return Regex.IsMatch(passport.Trim(), @"^\d{4}\s?\d{6}$");
    }

    public static bool IsValidAddress(string? address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return true;
        return address.Trim().Length <= 255;
    }
}

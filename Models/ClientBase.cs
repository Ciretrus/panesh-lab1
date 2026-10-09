using System.Text.RegularExpressions;

namespace JewelryWorkshop.Models;

/// <summary>
/// Базовый абстрактный класс клиента ювелирной мастерской.
/// Инкапсулирует общие поля, свойства и логику валидации для устранения дублирования кода.
/// </summary>
public abstract class ClientBase
{
    // Общие константы валидации
    protected const string PhonePattern = @"^\+?[0-9]{10,15}$";
    protected const string PassportPattern = @"^\d{4}\s?\d{6}$";

    // Закрытые поля общих данных
    private int _id;
    private string _phone = string.Empty;
    private string _passportSeriesNumber = string.Empty;

    /// <summary>
    /// Защищенный конструктор для использования в классах-наследниках.
    /// </summary>
    protected ClientBase(int id, string phone, string passportSeriesNumber)
    {
        Id = id;
        Phone = phone;
        PassportSeriesNumber = passportSeriesNumber;
    }

    // --- Общие свойства с инкапсуляцией и валидацией ---

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

    // --- Абстрактные и виртуальные методы для полиморфизма ---

    /// <summary>
    /// Получение краткого имени клиента (фамилия и инициалы).
    /// Реализуется по-разному в полном и кратком классах.
    /// </summary>
    public abstract string GetShortName();

    /// <summary>
    /// Краткая строковая версия данных клиента.
    /// </summary>
    public virtual string ToShortString()
    {
        return $"[ID={Id}] {GetShortName()} | Тел: {Phone} | Паспорт: {PassportSeriesNumber}";
    }

    public override string ToString() => ToShortString();

    // --- Общие методы валидации (методы класса) ---

    public static bool IsValidId(int id) => id > 0;

    public static bool IsValidPhone(string? phone) =>
        ValidateRegex(phone, PhonePattern, isRequired: true);

    public static bool IsValidPassportSeriesNumber(string? passport) =>
        ValidateRegex(passport, PassportPattern, isRequired: true);

    /// <summary>
    /// Проверка соответствия строки регулярному выражению с учётом обязательности.
    /// </summary>
    protected static bool ValidateRegex(string? value, string pattern, bool isRequired)
    {
        if (string.IsNullOrWhiteSpace(value))
            return !isRequired;

        return Regex.IsMatch(value.Trim(), pattern);
    }

    /// <summary>
    /// Проверка ограничения по максимальной длине строки.
    /// </summary>
    protected static bool ValidateLength(string? value, int maxLength, bool isRequired)
    {
        if (string.IsNullOrWhiteSpace(value))
            return !isRequired;

        return value.Trim().Length <= maxLength;
    }
}

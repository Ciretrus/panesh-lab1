using System.Text.RegularExpressions;

namespace JewelryWorkshop.Models;

/// <summary>
/// Базовый абстрактный класс клиента ювелирной мастерской.
/// Инкапсулирует общие поля, свойства и логику валидации для устранения дублирования кода.
/// </summary>
public abstract class ClientBase : IDisposable
{
    // Общие константы валидации
    protected const string PhonePattern = @"^\+?[0-9]{10,15}$";
    protected const string PassportPattern = @"^\d{4}\s?\d{6}$";

    // Закрытые поля общих данных
    protected int _id;
    protected string _phone = string.Empty;
    protected string _passportSeriesNumber = string.Empty;

    // Поля управления жизненным циклом и связью объектов
    protected bool _isDisposed;
    private readonly List<WeakReference<ClientBase>> _linkedClients = new();
    private readonly object _linkLock = new();

    /// <summary>
    /// Флаг уничтожения объекта.
    /// </summary>
    public virtual bool IsDisposed => _isDisposed;

    /// <summary>
    /// Событие, вызываемое при уничтожении объекта.
    /// </summary>
    public event Action<ClientBase>? OnDestroy;

    /// <summary>
    /// Связать жизненный цикл данного объекта со вторым объектом ClientBase (двусторонняя связь).
    /// При удалении одного экземпляра автоматически уничтожается и второй.
    /// </summary>
    public void LinkWith(ClientBase other)
    {
        if (other == null || ReferenceEquals(this, other)) return;

        lock (_linkLock)
        {
            bool alreadyLinked = _linkedClients.Any(w => w.TryGetTarget(out var t) && ReferenceEquals(t, other));
            if (!alreadyLinked)
            {
                _linkedClients.Add(new WeakReference<ClientBase>(other));
            }
        }

        lock (other._linkLock)
        {
            bool otherAlreadyLinked = other._linkedClients.Any(w => w.TryGetTarget(out var t) && ReferenceEquals(t, this));
            if (!otherAlreadyLinked)
            {
                other._linkedClients.Add(new WeakReference<ClientBase>(this));
            }
        }
    }

    /// <summary>
    /// Явное удаление объекта (синоним Dispose).
    /// </summary>
    public void Delete() => Dispose();

    /// <summary>
    /// Проверка, не уничтожен ли объект.
    /// </summary>
    public void ThrowIfDisposed()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(GetType().Name, $"Объект {GetType().Name} [ID={_id}] уничтожен.");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        _isDisposed = true;

        OnDestroy?.Invoke(this);

        // Каскадно уничтожаем все связанные экземпляры
        List<ClientBase> toDispose = new();
        lock (_linkLock)
        {
            foreach (var wr in _linkedClients)
            {
                if (wr.TryGetTarget(out var target) && !target.IsDisposed)
                {
                    toDispose.Add(target);
                }
            }
            _linkedClients.Clear();
        }

        foreach (var linked in toDispose)
        {
            linked.Dispose();
        }
    }

    ~ClientBase()
    {
        Dispose(false);
    }

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
        if (IsDisposed)
            return $"[{GetType().Name} ID={_id}] (ОБЪЕКТ УНИЧТОЖЕН)";

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

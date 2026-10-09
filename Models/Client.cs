namespace JewelryWorkshop.Models;

/// <summary>
/// Сущность "Клиент" ювелирной мастерской с полной инкапсуляцией всех полей.
/// </summary>
public class Client
{
    // Закрытые поля (инкапсуляция внутреннего состояния)
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

    // Свойства доступа к закрытым полям (геттеры и сеттеры)
    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public string LastName
    {
        get => _lastName;
        set => _lastName = value;
    }

    public string FirstName
    {
        get => _firstName;
        set => _firstName = value;
    }

    public string? MiddleName
    {
        get => _middleName;
        set => _middleName = value;
    }

    public string Phone
    {
        get => _phone;
        set => _phone = value;
    }

    public string? Email
    {
        get => _email;
        set => _email = value;
    }

    public string PassportSeriesNumber
    {
        get => _passportSeriesNumber;
        set => _passportSeriesNumber = value;
    }

    public string? Address
    {
        get => _address;
        set => _address = value;
    }
}

using JewelryWorkshop.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 1: Пункт 4 ===");
Console.WriteLine("1. Создание корректного объекта Client:");

try
{
    Client validClient = new Client(
        id: 1,
        lastName: "Иванов",
        firstName: "Иван",
        middleName: "Иванович",
        phone: "+79991234567",
        email: "ivanov@example.com",
        passportSeriesNumber: "0315 123456",
        address: "г. Краснодар, ул. Красная, д. 10"
    );
    Console.WriteLine($"[УСПЕХ] Клиент создан: {validClient.LastName} {validClient.FirstName}, тел: {validClient.Phone}");
}
catch (Exception ex)
{
    Console.WriteLine($"[ОШИБКА] {ex.Message}");
}

Console.WriteLine("\n2. Проверка невозможности создания объекта с некорректными данными:");

// Тест 1: Некорректный телефон
try
{
    Console.Write("Попытка создать клиента с неверным телефоном ('123'): ");
    var invalidClient = new Client(2, "Петров", "Петр", null, "123", null, "0315 123456", null);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Поймано исключение: {ex.Message}");
}

// Тест 2: Некорректный ID
try
{
    Console.Write("Попытка создать клиента с отрицательным ID (-5): ");
    var invalidClient = new Client(-5, "Петров", "Петр", null, "+79998887766", null, "0315 123456", null);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Поймано исключение: {ex.Message}");
}

// Тест 3: Некорректный паспорт
try
{
    Console.Write("Попытка создать клиента с некорректным паспортом ('паспорт'): ");
    var invalidClient = new Client(3, "Петров", "Петр", null, "+79998887766", null, "паспорт", null);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Поймано исключение: {ex.Message}");
}

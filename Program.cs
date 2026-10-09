using JewelryWorkshop.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 1: Пункт 5 (Устранение дублирования кода валидации) ===");

// 1. Проверка работы рефакторинговой валидации на валидных данных
Client client = new Client(
    id: 1,
    lastName: "Смирнов",
    firstName: "Алексей",
    middleName: "Викторович",
    phone: "+79001234567",
    email: "smirnov@mail.ru",
    passportSeriesNumber: "0314 987654",
    address: "г. Москва, ул. Арбат, 15"
);
Console.WriteLine($"[УСПЕХ] Клиент успешно создан: {client.LastName} {client.FirstName} {client.MiddleName}");

// 2. Проверка граничных условий через статические методы класса
Console.WriteLine("\nТестирование статических методов валидации после устранения дублирования:");
Console.WriteLine($"IsValidLastName('Иванов'): {Client.IsValidLastName("Иванов")}");
Console.WriteLine($"IsValidLastName(''): {Client.IsValidLastName("")}");
Console.WriteLine($"IsValidLastName('Иванов123'): {Client.IsValidLastName("Иванов123")}");

Console.WriteLine($"IsValidMiddleName(null) (опционально): {Client.IsValidMiddleName(null)}");
Console.WriteLine($"IsValidMiddleName('Иванович'): {Client.IsValidMiddleName("Иванович")}");

Console.WriteLine($"IsValidPhone('+79991234567'): {Client.IsValidPhone("+79991234567")}");
Console.WriteLine($"IsValidPhone('invalid'): {Client.IsValidPhone("invalid")}");

Console.WriteLine($"IsValidEmail(null) (опционально): {Client.IsValidEmail(null)}");
Console.WriteLine($"IsValidEmail('client@test.com'): {Client.IsValidEmail("client@test.com")}");
Console.WriteLine($"IsValidEmail('bad_email'): {Client.IsValidEmail("bad_email")}");

Console.WriteLine($"IsValidPassportSeriesNumber('1234 567890'): {Client.IsValidPassportSeriesNumber("1234 567890")}");
Console.WriteLine($"IsValidPassportSeriesNumber('123'): {Client.IsValidPassportSeriesNumber("123")}");

using JewelryWorkshop.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 1: Пункт 9 (Иерархия наследования ClientBase -> Client / ClientShort) ===\n");

// 1. Создание объектов наследников
Client fullClient = new Client(
    id: 1,
    lastName: "Иванов",
    firstName: "Иван",
    middleName: "Иванович",
    phone: "+79991112233",
    email: "ivanov@mail.ru",
    passportSeriesNumber: "0315 111222",
    address: "г. Краснодар, ул. Красная, 10"
);

ClientShort shortClient = new ClientShort(
    id: 2,
    shortName: "Петров П. С.",
    phone: "+79992223344",
    passportSeriesNumber: "0316 222333"
);

ClientShort shortFromFull = new ClientShort(fullClient);

// 2. Демонстрация полиморфизма через базовый класс ClientBase
List<ClientBase> allClients = new List<ClientBase> { fullClient, shortClient, shortFromFull };

Console.WriteLine("Полиморфный вывод объектов через базовый тип ClientBase (метод ToShortString()):");
foreach (ClientBase c in allClients)
{
    Console.WriteLine($"  - Тип: {c.GetType().Name,-12} | Вывод: {c.ToShortString()}");
}

// 3. Проверка работы базовой валидации в обоих классах
Console.WriteLine("\nПроверка единой статической валидации базового класса ClientBase:");
Console.WriteLine($"  ClientBase.IsValidPhone('+79991112233'): {ClientBase.IsValidPhone("+79991112233")}");
Console.WriteLine($"  ClientBase.IsValidPhone('не_телефон'):    {ClientBase.IsValidPhone("не_телефон")}");
Console.WriteLine($"  ClientBase.IsValidPassportSeriesNumber('0315 111222'): {ClientBase.IsValidPassportSeriesNumber("0315 111222")}");

// 4. Проверка полнофункционального вывода полной сущности
Console.WriteLine("\nПолная информация о клиенте (метод ToFullString()):");
Console.WriteLine($"  {fullClient}");
Console.WriteLine($"  {shortFromFull}");
fullClient.Dispose();
Console.WriteLine($"  {shortFromFull}");

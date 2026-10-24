using JewelryWorkshop.Models;
using JewelryWorkshop.Mapping;

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
Console.WriteLine($"------------------------");
Console.WriteLine($"  До удаления: {shortFromFull}");
Console.WriteLine("\n[Удаление] Вызываем fullClient.Delete() (или Dispose):");
fullClient.Delete();
Console.WriteLine($"  Состояние fullClient: {fullClient}");
Console.WriteLine($"  Состояние shortFromFull (автоматически уничтожен): {shortFromFull}");

allClients.Remove(fullClient);

// Демонстрация обратного удаления: удаляем короткий -> уничтожается длинный
Console.WriteLine("\n[Обратное удаление] Создаем пару full <-> short через маппер и удаляем короткий:");
Client full2 = new Client(5, "Смирнов", "Олег", "Игоревич", "+79998887766", "smirnov@mail.ru", "0319 777888", "г. Анапа");
ClientShort short2 = full2.ToShort();
Console.WriteLine($"  До удаления: {full2}");
Console.WriteLine($"  Удаляем short2.Delete()...");
short2.Delete();
Console.WriteLine($"  Состояние short2: {short2}");
Console.WriteLine($"  Состояние full2 (автоматически уничтожен): {full2}");

// 5. Демонстрация работы ClientMapper
Console.WriteLine("\n=== Тестирование ClientMapper (Маппинг между Client и ClientShort) ===");

Client sampleFull = new Client(
    id: 10,
    lastName: "Сидоров",
    firstName: "Алексей",
    middleName: "Сергеевич",
    phone: "+79001234567",
    email: "sidorov@example.com",
    passportSeriesNumber: "0317 333444",
    address: "г. Москва, ул. Арбат, 15"
);

// Маппинг Client -> ClientShort через метод расширения
ClientShort mappedShort = sampleFull.ToShort();
Console.WriteLine("\n[1] Маппинг Client -> ClientShort (через .ToShort()):");
Console.WriteLine($"  Исходный: {sampleFull}");
Console.WriteLine($"  Результат: {mappedShort}");

// Маппинг ClientShort -> Client (через ClientMapper)
ClientShort sampleShort = new ClientShort(
    id: 20,
    shortName: "Кузнецов В. П.",
    phone: "+79059876543",
    passportSeriesNumber: "0318 555666"
);

Client mappedFull = sampleShort.ToFull(email: "kuznetsov@example.com", address: "г. Сочи, ул. Морская, 5");
Console.WriteLine("\n[2] Маппинг ClientShort -> Client (через .ToFull()):");
Console.WriteLine($"  Исходный: {sampleShort}");
Console.WriteLine($"  Результат: {mappedFull}");

// Маппинг ClientShort -> Client с передачей полного имени
Client mappedFullNamed = sampleShort.ToFull(firstName: "Владимир", middleName: "Петрович");
Console.WriteLine("\n[3] Маппинг ClientShort -> Client с указанием полных ФИО:");
Console.WriteLine($"  Результат: {mappedFullNamed}");

// Маппинг списков
List<Client> clientList = new() { sampleFull, mappedFullNamed };
List<ClientShort> shortList = clientList.ToShortList();
Console.WriteLine($"\n[4] Маппинг коллекции ({clientList.Count} шт.) через .ToShortList():");
foreach (var s in shortList)
{
    Console.WriteLine($"  - {s}");
}


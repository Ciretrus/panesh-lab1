using JewelryWorkshop.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 1: Пункт 7 (Вывод и сравнение объектов) ===");

var client1 = new Client(
    id: 1,
    lastName: "Иванов",
    firstName: "Иван",
    middleName: "Иванович",
    phone: "+79991112233",
    email: "ivanov@mail.ru",
    passportSeriesNumber: "0315 111222",
    address: "г. Краснодар, ул. Красная, 10"
);

var client2 = new Client(
    id: 1,
    lastName: "Иванов",
    firstName: "Иван",
    middleName: "Иванович",
    phone: "+79991112233",
    email: "ivanov@mail.ru",
    passportSeriesNumber: "0315 111222",
    address: "г. Краснодар, ул. Красная, 10"
);

var client3 = new Client(
    id: 2,
    lastName: "Петров",
    firstName: "Петр",
    middleName: null,
    phone: "+79998887766",
    email: null,
    passportSeriesNumber: "0316 333444",
    address: null
);

Console.WriteLine("\n--- 1. Вывод полной версии объекта (ToFullString / ToString) ---");
Console.WriteLine(client1.ToFullString());
Console.WriteLine(client3.ToFullString());

Console.WriteLine("\n--- 2. Вывод краткой версии объекта (ToShortString) ---");
Console.WriteLine(client1.ToShortString());
Console.WriteLine(client3.ToShortString());

Console.WriteLine("\n--- 3. Сравнение объектов на равенство ---");
Console.WriteLine($"client1 и client2 (одинаковые данные в разных объектах):");
Console.WriteLine($"  client1.Equals(client2): {client1.Equals(client2)}");
Console.WriteLine($"  client1 == client2:      {client1 == client2}");
Console.WriteLine($"  Хэш-коды равны:          {client1.GetHashCode() == client2.GetHashCode()}");

Console.WriteLine($"\nclient1 и client3 (разные данные):");
Console.WriteLine($"  client1.Equals(client3): {client1.Equals(client3)}");
Console.WriteLine($"  client1 == client3:      {client1 == client3}");
Console.WriteLine($"  client1 != client3:      {client1 != client3}");

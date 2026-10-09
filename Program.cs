using JewelryWorkshop.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 1: Пункт 3 ===");
Console.WriteLine("Создание объекта класса Client с инкапсулированными полями:\n");

Client client = new Client(
    id: 1,
    lastName: "Иванов",
    firstName: "Иван",
    middleName: "Иванович",
    phone: "+79991234567",
    email: "ivanov@example.com",
    passportSeriesNumber: "0315 123456",
    address: "г. Краснодар, ул. Красная, д. 10"
);

Console.WriteLine($"ID: {client.Id}");
Console.WriteLine($"ФИО: {client.LastName} {client.FirstName} {client.MiddleName}");
Console.WriteLine($"Телефон: {client.Phone}");
Console.WriteLine($"Email: {client.Email}");
Console.WriteLine($"Паспорт: {client.PassportSeriesNumber}");
Console.WriteLine($"Адрес: {client.Address}");

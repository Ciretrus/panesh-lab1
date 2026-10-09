using System.Text.Json;
using JewelryWorkshop.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 1: Пункт 6 (Перегрузка конструкторов) ===");

// 1. Канонический конструктор
var client1 = new Client(1, "Иванов", "Иван", "Иванович", "+79991112233", "ivan@mail.ru", "0315 111222", "ул. Ленина 1");
Console.WriteLine($"1. Канонический конструктор: {client1.LastName} {client1.FirstName}, Паспорт: {client1.PassportSeriesNumber}");

// 2. Краткий конструктор (только обязательные поля)
var client2 = new Client(2, "Петров", "Петр", "+79992223344", "0316 222333");
Console.WriteLine($"2. Краткий конструктор: {client2.LastName} {client2.FirstName}, Отчество: '{client2.MiddleName ?? "отсутствует"}', Email: '{client2.Email ?? "отсутствует"}'");

// 3. Конструктор из форматированной строки (CSV с разделителем ';')
string csvRow = "3;Сидоров;Алексей;Сергеевич;+79993334455;sidorov@mail.ru;0317 333444;ул. Мира 5";
var client3 = new Client(csvRow);
Console.WriteLine($"3. Из строки (разделитель ';'): ID={client3.Id}, {client3.LastName} {client3.FirstName}, Тел: {client3.Phone}");

// 4. Конструктор из строки JSON
string jsonString = """
{
    "id": 4,
    "lastName": "Кузнецова",
    "firstName": "Анна",
    "middleName": null,
    "phone": "+79994445566",
    "email": "kuznetsova@gmail.com",
    "passportSeriesNumber": "0318 444555",
    "address": "ул. Гагарина 12"
}
""";
var client4 = new Client(jsonString);
Console.WriteLine($"4. Из JSON-строки: ID={client4.Id}, {client4.LastName} {client4.FirstName}, Email: {client4.Email}");

// 5. Конструктор из JsonElement
using var doc = JsonDocument.Parse(jsonString);
var client5 = new Client(doc.RootElement);
Console.WriteLine($"5. Из JsonElement: ID={client5.Id}, {client5.LastName} {client5.FirstName}, Тел: {client5.Phone}");

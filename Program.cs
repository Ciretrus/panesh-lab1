using JewelryWorkshop.Models;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 1: Пункт 8 (Класс с краткой версией данных ClientShort) ===");

// 1. Исходный полный объект Client
var fullClient = new Client(
    id: 10,
    lastName: "Васильев",
    firstName: "Василий",
    middleName: "Васильевич",
    phone: "+79181112233",
    email: "vasiliev@mail.ru",
    passportSeriesNumber: "0310 998877",
    address: "г. Сочи, Курортный проспект, 5"
);
Console.WriteLine($"Полный клиент:\n  {fullClient.ToFullString()}");

// 2. Создание ClientShort на основе полного Client
var shortFromFull = new ClientShort(fullClient);
Console.WriteLine($"\nClientShort, созданный из полного Client:\n  {shortFromFull}");

// 3. Создание ClientShort через конструктор с параметрами
var shortExplicit = new ClientShort(11, "Николаев Н. Н.", "+79182223344", "0311 887766");
Console.WriteLine($"\nClientShort, созданный напрямую:\n  {shortExplicit}");

// 4. Создание ClientShort из строки
var shortFromString = new ClientShort("12;Романов Р. Р.;+79183334455;0312 776655");
Console.WriteLine($"\nClientShort, созданный из строки:\n  {shortFromString}");

// 5. Проверка равенства
var shortCopy = new ClientShort(10, "Васильев В. В.", "+79181112233", "0310 998877");
Console.WriteLine($"\nСравнение shortFromFull и shortCopy: {shortFromFull == shortCopy}");

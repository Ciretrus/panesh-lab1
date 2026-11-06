using JewelryWorkshop.Models;
using JewelryWorkshop.Repositories;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 2: Пункт 5 (Singleton DatabaseConnection и делегация в Client_rep_DB) ===\n");

// 1. Проверка работы паттерна Одиночка (Singleton)
DatabaseConnection db1 = DatabaseConnection.GetInstance();
DatabaseConnection db2 = DatabaseConnection.GetInstance();
Console.WriteLine($"1. Проверка паттерна Одиночка (Singleton):");
Console.WriteLine($"   ReferenceEquals(db1, db2): {ReferenceEquals(db1, db2)} (оба вызова возвращают один и тот же экземпляр)\n");

// 2. Работа с Client_rep_DB через делегацию к DatabaseConnection
var dbRepo = new Client_rep_DB(db1);
dbRepo.ClearAll();

dbRepo.Add(new Client(1, "Яковлев", "Яков", "Яковлевич", "+79990001122", "yakov@mail.ru", "0315 111222", "г. Краснодар"));
dbRepo.Add(new Client(1, "Алексеев", "Алексей", "Алексеевич", "+79990002233", "alex@mail.ru", "0315 222333", "г. Сочи"));
dbRepo.Add(new Client(1, "Борисов", "Борис", null, "+79990003344", null, "0315 333444", null));

Console.WriteLine($"2. Количество записей в БД через делегацию (GetCount): {dbRepo.GetCount()}");
Console.WriteLine("   Краткий список клиентов (Get_k_n_short_list):");
foreach (ClientShort item in dbRepo.Get_k_n_short_list(k: 1, n: 10))
{
    Console.WriteLine($"     {item}");
}

using JewelryWorkshop.Models;
using JewelryWorkshop.Repositories;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 2: Пункт 4 (Репозиторий Client_rep_DB для PostgreSQL) ===\n");

const string connectionString = "Host=localhost;Port=5432;Database=jewelry_workshop;Username=postgres;Password=1234";

var dbRepo = new Client_rep_DB(connectionString);
dbRepo.ClearAll();

// c. Добавление объектов (с автоматическим формированием нового ID через SERIAL в PostgreSQL)
Console.WriteLine("1. Добавление клиентов в БД PostgreSQL (Add):");
Client c1 = dbRepo.Add(new Client(1, "Яковлев", "Яков", "Яковлевич", "+79990001122", "yakov@mail.ru", "0315 111222", "г. Краснодар"));
Client c2 = dbRepo.Add(new Client(1, "Алексеев", "Алексей", "Алексеевич", "+79990002233", "alex@mail.ru", "0315 222333", "г. Сочи"));
Client c3 = dbRepo.Add(new Client(1, "Борисов", "Борис", null, "+79990003344", null, "0315 333444", null));
Client c4 = dbRepo.Add(new Client(1, "Викторов", "Виктор", "Викторович", "+79990004455", "viktor@mail.ru", "0315 444555", "г. Новороссийск"));

Console.WriteLine($"Добавлено клиентов (GetCount): {dbRepo.GetCount()}");
Console.WriteLine($"  {c1}");
Console.WriteLine($"  {c2}");
Console.WriteLine($"  {c3}");
Console.WriteLine($"  {c4}");

// a. Получение объекта по ID
Console.WriteLine("\n2. Получение объекта по ID = 2 (GetById):");
Console.WriteLine($"  {dbRepo.GetById(2)}");

// b. Получение списка k по счету n объектов класса ClientShort
Console.WriteLine("\n3. Пагинация get_k_n_short_list (k = 2, n = 2 — вторые 2 элемента из БД):");
foreach (ClientShort item in dbRepo.Get_k_n_short_list(k: 2, n: 2))
{
    Console.WriteLine($"  {item}");
}

// d. Замена элемента по ID
Console.WriteLine("\n4. Замена клиента с ID = 3 (ReplaceById):");
dbRepo.ReplaceById(3, new Client(1, "Борисов", "Борис", "Борисович", "+79997778899", "boris_db@mail.ru", "0315 333444", "г. Анапа"));
Console.WriteLine($"  После обновления: {dbRepo.GetById(3)}");

// e. Удаление элемента по ID и f. Получение количества элементов
Console.WriteLine("\n5. Удаление клиента с ID = 1 (DeleteById) и подсчет количества (GetCount):");
bool deleted = dbRepo.DeleteById(1);
Console.WriteLine($"  Удален ID=1: {deleted}, осталось в БД: {dbRepo.GetCount()}");

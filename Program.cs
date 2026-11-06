using JewelryWorkshop.Models;
using JewelryWorkshop.Repositories;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 2: Пункт 1 (Репозиторий Client_rep_json) ===\n");

string jsonPath = Path.Combine(AppContext.BaseDirectory, "data", "clients.json");
if (File.Exists(jsonPath))
{
    File.Delete(jsonPath);
}

var repo = new Client_rep_json(jsonPath);

// f. Добавление объектов (с автоматическим формированием ID)
Console.WriteLine("1. Добавление клиентов (Add):");
repo.Add(new Client(1, "Яковлев", "Яков", "Яковлевич", "+79990001122", "yakov@mail.ru", "0315 111222", "г. Краснодар, ул. Северная, 1"));
repo.Add(new Client(1, "Алексеев", "Алексей", "Алексеевич", "+79990002233", "alex@mail.ru", "0315 222333", "г. Краснодар, ул. Красная, 2"));
repo.Add(new Client(1, "Борисов", "Борис", null, "+79990003344", null, "0315 333444", null));
repo.Add(new Client(1, "Викторов", "Виктор", "Викторович", "+79990004455", "viktor@mail.ru", "0315 444555", "г. Сочи, ул. Морская, 10"));

// b. Запись в файл и a. Чтение из файла
repo.WriteAll();
var allFromFile = repo.ReadAll();
Console.WriteLine($"Записано и прочитано из JSON ({jsonPath}): {repo.GetCount()} клиентов.");
foreach (var c in allFromFile)
{
    Console.WriteLine($"  {c}");
}

// c. Получение объекта по ID
Console.WriteLine("\n2. Получение объекта по ID = 2 (GetById):");
Client? found = repo.GetById(2);
Console.WriteLine($"  Найдено: {found}");

// d. Получение k-го по счету списка из n объектов класса ClientShort
Console.WriteLine("\n3. Пагинация get_k_n_short_list (k = 2, n = 2 — вторые 2 элемента):");
List<ClientShort> page2 = repo.Get_k_n_short_list(k: 2, n: 2);
foreach (var sc in page2)
{
    Console.WriteLine($"  {sc}");
}

// e. Сортировка по выбранному полю (Фамилия)
Console.WriteLine("\n4. Сортировка по фамилии (SortByField):");
repo.SortByField();
foreach (var sc in repo.Get_k_n_short_list(k: 1, n: repo.GetCount()))
{
    Console.WriteLine($"  {sc}");
}

// g. Замена элемента по ID
Console.WriteLine("\n5. Замена клиента с ID = 3 (ReplaceById):");
repo.ReplaceById(3, new Client(1, "Борисов", "Борис", "Борисович", "+79997778899", "boris_new@mail.ru", "0315 333444", "г. Анапа"));
Console.WriteLine($"  Обновленный ID=3: {repo.GetById(3)}");

// h. Удаление элемента по ID и i. Получение количества
Console.WriteLine("\n6. Удаление клиента с ID = 1 (DeleteById) и получение количества (GetCount):");
bool deleted = repo.DeleteById(1);
repo.WriteAll();
Console.WriteLine($"  Удален: {deleted}, текущее количество элементов: {repo.GetCount()}");

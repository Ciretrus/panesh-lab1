using JewelryWorkshop.Models;
using JewelryWorkshop.Repositories;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 2: Пункт 2 (Репозиторий Client_rep_yaml) ===\n");

string yamlPath = Path.Combine(AppContext.BaseDirectory, "data", "clients.yaml");
if (File.Exists(yamlPath))
{
    File.Delete(yamlPath);
}

var yamlRepo = new Client_rep_yaml(yamlPath);

// f. Добавление объектов (с автоматическим формированием ID)
Console.WriteLine("1. Добавление клиентов в YAML-репозиторий (Add):");
yamlRepo.Add(new Client(1, "Яковлев", "Яков", "Яковлевич", "+79990001122", "yakov@mail.ru", "0315 111222", "г. Краснодар, ул. Северная, 1"));
yamlRepo.Add(new Client(1, "Алексеев", "Алексей", "Алексеевич", "+79990002233", "alex@mail.ru", "0315 222333", "г. Краснодар, ул. Красная, 2"));
yamlRepo.Add(new Client(1, "Борисов", "Борис", null, "+79990003344", null, "0315 333444", null));
yamlRepo.Add(new Client(1, "Викторов", "Виктор", "Викторович", "+79990004455", "viktor@mail.ru", "0315 444555", "г. Сочи, ул. Морская, 10"));

// b. Запись в файл и a. Чтение из файла
yamlRepo.WriteAll();
var allFromYaml = yamlRepo.ReadAll();
Console.WriteLine($"Записано и прочитано из YAML ({yamlPath}): {yamlRepo.GetCount()} клиентов.");
foreach (var c in allFromYaml)
{
    Console.WriteLine($"  {c}");
}

// c. Получение объекта по ID
Console.WriteLine("\n2. Получение объекта по ID = 2 (GetById):");
Client? found = yamlRepo.GetById(2);
Console.WriteLine($"  Найдено: {found}");

// d. Получение k-го по счету списка из n объектов класса ClientShort
Console.WriteLine("\n3. Пагинация get_k_n_short_list (k = 2, n = 2 — вторые 2 элемента):");
List<ClientShort> page2 = yamlRepo.Get_k_n_short_list(k: 2, n: 2);
foreach (var sc in page2)
{
    Console.WriteLine($"  {sc}");
}

// e. Сортировка по выбранному полю (Фамилия)
Console.WriteLine("\n4. Сортировка по фамилии (SortByField):");
yamlRepo.SortByField();
foreach (var sc in yamlRepo.Get_k_n_short_list(k: 1, n: yamlRepo.GetCount()))
{
    Console.WriteLine($"  {sc}");
}

// g. Замена элемента по ID
Console.WriteLine("\n5. Замена клиента с ID = 3 (ReplaceById):");
yamlRepo.ReplaceById(3, new Client(1, "Борисов", "Борис", "Борисович", "+79997778899", "boris_new@mail.ru", "0315 333444", "г. Анапа"));
Console.WriteLine($"  Обновленный ID=3: {yamlRepo.GetById(3)}");

// h. Удаление элемента по ID и i. Получение количества
Console.WriteLine("\n6. Удаление клиента с ID = 1 (DeleteById) и получение количества (GetCount):");
bool deleted = yamlRepo.DeleteById(1);
yamlRepo.WriteAll();
Console.WriteLine($"  Удален: {deleted}, текущее количество элементов: {yamlRepo.GetCount()}");

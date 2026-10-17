using JewelryWorkshop.Models;
using JewelryWorkshop.Repositories;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("======================================================================");
Console.WriteLine("  Лабораторная работа 2. Пункт 1: Репозиторий Client_rep_json");
Console.WriteLine("======================================================================\n");

string testJsonPath = Path.Combine(AppContext.BaseDirectory, "data", "clients_test.json");
if (File.Exists(testJsonPath))
{
    File.Delete(testJsonPath);
}

// 1. Инициализация репозитория
Console.WriteLine("1. Инициализация репозитория Client_rep_json...");
var rep = new Client_rep_json(testJsonPath);
Console.WriteLine($"   Файл: {testJsonPath}");
Console.WriteLine($"   Начальное количество элементов (get_count): {rep.get_count()}\n");

// 2. Добавление клиентов (пункт f: формирование нового ID и запись)
Console.WriteLine("2. Добавление клиентов (Add):");
var client1 = new Client(1, "Иванов", "Иван", "Иванович", "+79991112233", "ivanov@mail.ru", "0315 111222", "г. Краснодар, ул. Красная, 10");
var client2 = new Client(1, "Алексеев", "Алексей", "Алексеевич", "+79992223344", "alex@mail.ru", "0316 222333", "г. Москва, ул. Арбат, 5");
var client3 = new Client(1, "Сергеев", "Сергей", null, "+79993334455", null, "0317 333444", "г. Сочи, ул. Морская, 12");
var client4 = new Client(1, "Борисов", "Борис", "Борисович", "+79994445566", "borya@mail.ru", "0318 444555", null);
var client5 = new Client(1, "Дмитриев", "Дмитрий", "Дмитриевич", "+79995556677", "dmitry@mail.ru", "0319 555666", "г. Ростов, ул. Ленина, 20");

Console.WriteLine($"   Добавлен клиент ID={rep.Add(client1)}: {client1.LastName} {client1.FirstName}");
Console.WriteLine($"   Добавлен клиент ID={rep.Add(client2)}: {client2.LastName} {client2.FirstName}");
Console.WriteLine($"   Добавлен клиент ID={rep.Add(client3)}: {client3.LastName} {client3.FirstName}");
Console.WriteLine($"   Добавлен клиент ID={rep.Add(client4)}: {client4.LastName} {client4.FirstName}");
Console.WriteLine($"   Добавлен клиент ID={rep.Add(client5)}: {client5.LastName} {client5.FirstName}");
Console.WriteLine($"   Текущее количество (get_count): {rep.get_count()}\n");

// 3. Чтение всех значений из файла (пункт a)
Console.WriteLine("3. Чтение всех значений из файла (ReadFromFile):");
var repReloaded = new Client_rep_json(testJsonPath);
var loadedClients = repReloaded.ReadFromFile();
foreach (var c in loadedClients)
{
    Console.WriteLine($"   - {c.ToFullString()}");
}
Console.WriteLine();

// 4. Получить объект по ID (пункт c)
Console.WriteLine("4. Получить объект по ID (GetById):");
var found = rep.GetById(2);
Console.WriteLine($"   Поиск ID=2: {(found != null ? found.ToFullString() : "Не найден")}");
var notFound = rep.GetById(999);
Console.WriteLine($"   Поиск ID=999: {(notFound != null ? notFound.ToFullString() : "Не найден")}\n");

// 5. Сортировка по выбранному полю (пункт e: по фамилии)
Console.WriteLine("5. Сортировка по фамилии (SortByField):");
rep.SortByField(ascending: true);
foreach (var c in rep.ReadAll())
{
    Console.WriteLine($"   - [ID={c.Id}] {c.LastName} {c.FirstName}");
}
Console.WriteLine();

// 6. Получить список k по счету n объектов класса short (пункт d: get_k_n_short_list)
Console.WriteLine("6. Постраничная выборка кратких объектов (get_k_n_short_list):");
Console.WriteLine("   Страница 1 (k=1, n=2):");
var page1 = rep.get_k_n_short_list(k: 1, n: 2);
foreach (var cs in page1)
{
    Console.WriteLine($"     * {cs.ToShortString()}");
}

Console.WriteLine("   Страница 2 (k=2, n=2):");
var page2 = rep.get_k_n_short_list(k: 2, n: 2);
foreach (var cs in page2)
{
    Console.WriteLine($"     * {cs.ToShortString()}");
}
Console.WriteLine();

// 7. Заменить элемент списка по ID (пункт g: ReplaceById)
Console.WriteLine("7. Замена элемента по ID (ReplaceById):");
var replacementClient = new Client(1, "Алексеев-Обновленный", "Алексей", "Алексеевич", "+79997778899", "new_alex@mail.ru", "0316 222333", "г. Москва, Новый адрес");
bool replaced = rep.ReplaceById(2, replacementClient);
Console.WriteLine($"   Замена ID=2 успешна: {replaced}");
Console.WriteLine($"   Обновленный объект: {rep.GetById(2)?.ToFullString()}\n");

// 8. Удалить элемент списка по ID (пункт h: DeleteById)
Console.WriteLine("8. Удаление элемента по ID (DeleteById):");
bool deleted = rep.DeleteById(3);
Console.WriteLine($"   Удаление ID=3 успешно: {deleted}");
Console.WriteLine($"   Количество после удаления (get_count): {rep.get_count()}\n");

// 9. Итоговый список
Console.WriteLine("9. Итоговое состояние репозитория (после записи в файл и перезагрузки):");
var repFinal = new Client_rep_json(testJsonPath);
foreach (var c in repFinal.ReadAll())
{
    Console.WriteLine($"   - {c.ToFullString()}");
}
Console.WriteLine($"\nТест Client_rep_json успешно завершен! Файл {testJsonPath} корректно сохранен и прочитан.\n");

Console.WriteLine("======================================================================");
Console.WriteLine("  Лабораторная работа 2. Пункт 2: Репозиторий Client_rep_yaml");
Console.WriteLine("======================================================================\n");

string testYamlPath = Path.Combine(AppContext.BaseDirectory, "data", "clients_test.yaml");
if (File.Exists(testYamlPath))
{
    File.Delete(testYamlPath);
}

// 1. Инициализация репозитория YAML
Console.WriteLine("1. Инициализация репозитория Client_rep_yaml...");
var repYaml = new Client_rep_yaml(testYamlPath);
Console.WriteLine($"   Файл: {testYamlPath}");
Console.WriteLine($"   Начальное количество элементов (get_count): {repYaml.get_count()}\n");

// 2. Добавление клиентов в YAML (Add)
Console.WriteLine("2. Добавление клиентов в YAML (Add):");
var yamlClient1 = new Client(1, "Смирнов", "Сергей", "Сергеевич", "+79181112233", "smirnov@mail.ru", "0310 123456", "г. Краснодар, ул. Северная, 1");
var yamlClient2 = new Client(1, "Васильев", "Василий", "Васильевич", "+79182223344", "vasya@mail.ru", "0311 234567", "г. Москва, ул. Тверская, 15");
var yamlClient3 = new Client(1, "Антонов", "Антон", null, "+79183334455", null, "0312 345678", "г. Санкт-Петербург, Невский пр., 20");
var yamlClient4 = new Client(1, "Григорьев", "Григорий", "Григорьевич", "+79184445566", "grigory@mail.ru", "0313 456789", null);

Console.WriteLine($"   Добавлен клиент ID={repYaml.Add(yamlClient1)}: {yamlClient1.LastName} {yamlClient1.FirstName}");
Console.WriteLine($"   Добавлен клиент ID={repYaml.Add(yamlClient2)}: {yamlClient2.LastName} {yamlClient2.FirstName}");
Console.WriteLine($"   Добавлен клиент ID={repYaml.Add(yamlClient3)}: {yamlClient3.LastName} {yamlClient3.FirstName}");
Console.WriteLine($"   Добавлен клиент ID={repYaml.Add(yamlClient4)}: {yamlClient4.LastName} {yamlClient4.FirstName}");
Console.WriteLine($"   Текущее количество (get_count): {repYaml.get_count()}\n");

// 3. Чтение всех значений из файла (ReadFromFile)
Console.WriteLine("3. Чтение всех значений из файла YAML (ReadFromFile):");
var repYamlReloaded = new Client_rep_yaml(testYamlPath);
var loadedYamlClients = repYamlReloaded.ReadFromFile();
foreach (var c in loadedYamlClients)
{
    Console.WriteLine($"   - {c.ToFullString()}");
}
Console.WriteLine();

// 4. Получить объект по ID (GetById)
Console.WriteLine("4. Получить объект по ID в YAML (GetById):");
var foundYaml = repYaml.GetById(2);
Console.WriteLine($"   Поиск ID=2: {(foundYaml != null ? foundYaml.ToFullString() : "Не найден")}");
var notFoundYaml = repYaml.GetById(999);
Console.WriteLine($"   Поиск ID=999: {(notFoundYaml != null ? notFoundYaml.ToFullString() : "Не найден")}\n");

// 5. Сортировка по выбранному полю (по фамилии)
Console.WriteLine("5. Сортировка по фамилии в YAML (SortByField):");
repYaml.SortByField(ascending: true);
foreach (var c in repYaml.ReadAll())
{
    Console.WriteLine($"   - [ID={c.Id}] {c.LastName} {c.FirstName}");
}
Console.WriteLine();

// 6. get_k_n_short_list
Console.WriteLine("6. Постраничная выборка кратких объектов из YAML (get_k_n_short_list):");
var yamlPage1 = repYaml.get_k_n_short_list(k: 1, n: 2);
foreach (var cs in yamlPage1)
{
    Console.WriteLine($"   [Стр 1] * {cs.ToShortString()}");
}
var yamlPage2 = repYaml.get_k_n_short_list(k: 2, n: 2);
foreach (var cs in yamlPage2)
{
    Console.WriteLine($"   [Стр 2] * {cs.ToShortString()}");
}
Console.WriteLine();

// 7. Заменить элемент списка по ID (ReplaceById)
Console.WriteLine("7. Замена элемента по ID в YAML (ReplaceById):");
var updatedVasya = new Client(1, "Васильев-Модифицированный", "Василий", "Васильевич", "+79189998877", "vasya_new@mail.ru", "0311 234567", "г. Казань, ул. Баумана, 5");
bool yamlReplaced = repYaml.ReplaceById(2, updatedVasya);
Console.WriteLine($"   Замена ID=2 успешна: {yamlReplaced}");
Console.WriteLine($"   Обновленный объект: {repYaml.GetById(2)?.ToFullString()}\n");

// 8. Удалить элемент списка по ID (DeleteById)
Console.WriteLine("8. Удаление элемента по ID в YAML (DeleteById):");
bool yamlDeleted = repYaml.DeleteById(3);
Console.WriteLine($"   Удаление ID=3 успешно: {yamlDeleted}");
Console.WriteLine($"   Количество после удаления (get_count): {repYaml.get_count()}\n");

// 9. Итоговое состояние YAML
Console.WriteLine("9. Итоговое состояние репозитория YAML после перезагрузки:");
var repYamlFinal = new Client_rep_yaml(testYamlPath);
foreach (var c in repYamlFinal.ReadAll())
{
    Console.WriteLine($"   - {c.ToFullString()}");
}
Console.WriteLine($"\nТест Client_rep_yaml успешно завершен! Файл {testYamlPath} корректно сохранен и прочитан.");

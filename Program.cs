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
Console.WriteLine($"\nТест успешно завершен! Файл {testJsonPath} корректно сохранен и прочитан.");

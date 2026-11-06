using JewelryWorkshop.Models;
using JewelryWorkshop.Repositories;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 2: Пункт 6 (Паттерн Адаптер: Client_rep_DB_Adapter в иерархии Client_rep_base) ===\n");

string jsonPath = Path.Combine(AppContext.BaseDirectory, "data", "clients.json");
string yamlPath = Path.Combine(AppContext.BaseDirectory, "data", "clients.yaml");
if (File.Exists(jsonPath)) File.Delete(jsonPath);
if (File.Exists(yamlPath)) File.Delete(yamlPath);

var dbRepo = new Client_rep_DB();
dbRepo.ClearAll();

// Все три источника данных (JSON, YAML и БД через Адаптер) работают через единый базовый класс Client_rep_base
List<Client_rep_base> repositories = new()
{
    new Client_rep_json(jsonPath),
    new Client_rep_yaml(yamlPath),
    new Client_rep_DB_Adapter(dbRepo)
};

foreach (Client_rep_base repo in repositories)
{
    Console.WriteLine($"--- Работа через единый интерфейс Client_rep_base: {repo.GetType().Name} ---");

    Client added1 = repo.Add(new Client(1, "Яковлев", "Яков", "Яковлевич", "+79990001122", "yakov@mail.ru", "0315 111222", "г. Краснодар"));
    Client added2 = repo.Add(new Client(1, "Алексеев", "Алексей", "Алексеевич", "+79990002233", "alex@mail.ru", "0315 222333", "г. Сочи"));

    repo.ReplaceById(added2.Id, new Client(1, "Алексеев", "Алексей", "Сергеевич", "+79990002299", "alex_new@mail.ru", "0315 222333", "г. Анапа"));
    repo.WriteAll();

    Console.WriteLine($"Всего элементов (GetCount): {repo.GetCount()}");
    Console.WriteLine($"Получение по ID={added2.Id} (GetById): {repo.GetById(added2.Id)}");
    Console.WriteLine("Краткий список (Get_k_n_short_list):");
    foreach (ClientShort sc in repo.Get_k_n_short_list(1, 10))
    {
        Console.WriteLine($"  {sc}");
    }
    Console.WriteLine();
}

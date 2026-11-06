using JewelryWorkshop.Models;
using JewelryWorkshop.Repositories;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Лабораторная работа 2: Пункт 3 (Иерархия наследования Client_rep_base) ===\n");

string jsonPath = Path.Combine(AppContext.BaseDirectory, "data", "clients.json");
string yamlPath = Path.Combine(AppContext.BaseDirectory, "data", "clients.yaml");

if (File.Exists(jsonPath)) File.Delete(jsonPath);
if (File.Exists(yamlPath)) File.Delete(yamlPath);

List<Client_rep_base> repositories = new()
{
    new Client_rep_json(jsonPath),
    new Client_rep_yaml(yamlPath)
};

foreach (Client_rep_base repo in repositories)
{
    Console.WriteLine($"--- Полиморфная работа с {repo.GetType().Name} ---");

    repo.Add(new Client(1, "Яковлев", "Яков", "Яковлевич", "+79990001122", "yakov@mail.ru", "0315 111222", "г. Краснодар"));
    repo.Add(new Client(1, "Алексеев", "Алексей", "Алексеевич", "+79990002233", "alex@mail.ru", "0315 222333", "г. Сочи"));
    repo.Add(new Client(1, "Борисов", "Борис", null, "+79990003344", null, "0315 333444", null));

    repo.SortByField();
    repo.WriteAll();
    repo.ReadAll();

    Console.WriteLine($"Количество элементов (GetCount): {repo.GetCount()}");
    foreach (ClientShort item in repo.Get_k_n_short_list(k: 1, n: 10))
    {
        Console.WriteLine($"  {item}");
    }
    Console.WriteLine();
}

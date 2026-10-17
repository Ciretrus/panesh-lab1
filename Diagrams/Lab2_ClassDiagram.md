# Диаграмма классов — Лабораторная работа 2

## Пункт 4: Репозиторий Client_rep_DB (работа с PostgreSQL)

```mermaid
classDiagram
    direction TB

    %% Сущности предметной области
    class ClientBase {
        <<abstract>>
        # PhonePattern: string
        # PassportPattern: string
        - _id: int
        - _phone: string
        - _passportSeriesNumber: string
        + Id: int
        + Phone: string
        + PassportSeriesNumber: string
        + GetShortName()* string
        + ToShortString() string
        + ToString() string
        + IsValidId(id: int)$ bool
        + IsValidPhone(phone: string)$ bool
        + IsValidPassportSeriesNumber(passport: string)$ bool
    }

    class Client {
        - _lastName: string
        - _firstName: string
        - _middleName: string?
        - _email: string?
        - _address: string?
        + OnDestroy: Action?
        + LastName: string
        + FirstName: string
        + MiddleName: string?
        + Email: string?
        + Address: string?
        + GetShortName() string
        + ToFullString() string
        + ToString() string
        + Equals(other: Client?) bool
        + GetHashCode() int
        + Dispose() void
    }

    class ClientShort {
        - _shortName: string
        + ShortName: string
        + GetShortName() string
        + Equals(other: ClientShort?) bool
        + GetHashCode() int
        + Dispose() void
    }

    %% Файловая иерархия репозиториев
    class Client_rep_base {
        <<abstract>>
        # _filePath: string
        # _clients: List~Client~
        # Client_rep_base(filePath: string)
        # Initialize() void
        # EnsureDirectoryExists() void
        + ReadFromFile()* List~Client~
        + ReadAll() List~Client~
        + WriteToFile()* void
        + GetById(id: int) Client?
        + get_k_n_short_list(k: int, n: int) List~ClientShort~
        + GetKNShortList(k: int, n: int) List~ClientShort~
        + SortByField(ascending: bool) void
        + Add(client: Client) int
        + ReplaceById(id: int, newClient: Client) bool
        + DeleteById(id: int) bool
        + get_count() int
        + GetCount() int
    }

    class Client_rep_json {
        - JsonOptions: JsonSerializerOptions$
        + Client_rep_json(filePath: string)
        + ReadFromFile() List~Client~
        + WriteToFile() void
    }

    class ClientYamlDto {
        + Id: int
        + LastName: string
        + FirstName: string
        + MiddleName: string?
        + Phone: string
        + Email: string?
        + PassportSeriesNumber: string
        + Address: string?
    }

    class Client_rep_yaml {
        - Serializer: ISerializer$
        - Deserializer: IDeserializer$
        + Client_rep_yaml(filePath: string)
        + ReadFromFile() List~Client~
        + WriteToFile() void
    }

    %% Репозиторий для работы с базой данных (Пункт 4)
    class Client_rep_DB {
        - _connectionString: string
        + Client_rep_DB(connectionString: string)
        - CreateConnection() NpgsqlConnection
        + EnsureTableExists() void
        + GetById(id: int) Client?
        + get_k_n_short_list(k: int, n: int) List~ClientShort~
        + GetKNShortList(k: int, n: int) List~ClientShort~
        + Add(client: Client) int
        + ReplaceById(id: int, newClient: Client) bool
        + DeleteById(id: int) bool
        + get_count() int
        + GetCount() int
        - MapRowToClient(reader: IDataReader)$ Client
    }

    %% Связи
    ClientBase <|-- Client : наследует
    ClientBase <|-- ClientShort : наследует
    ClientShort ..> Client : создает из Client

    Client_rep_base <|-- Client_rep_json : наследует
    Client_rep_base <|-- Client_rep_yaml : наследует

    Client_rep_base o-- Client : агрегирует коллекцию
    Client_rep_base ..> ClientShort : возвращает в get_k_n_short_list
    Client_rep_yaml ..> ClientYamlDto : использует для сериализации

    Client_rep_DB ..> Client : создает/читает
    Client_rep_DB ..> ClientShort : возвращает в get_k_n_short_list
```

Dice Microservices
==================

Backend система с два микросервиза, изградена с ASP.NET Core 8 Web API,
Entity Framework Core и SQLite.


Архитектура
-----------

Solution-ът съдържа два независими микросервиза:

| Микросервиз       | Порт | База       | Отговаря за                     |
|-------------------|------|------------|---------------------------------|
| UserService       | 7276 | users.db   | Регистрация, логин, JWT токени  |
| OperativeService  | 7291 | rolls.db   | Хвърляне на зарове, история     |

Всеки микросервиз има собствена база данни и може да се стартира независимо.

Комуникацията между тях става чрез JWT токен, издаден от UserService
и валидиран от OperativeService.


Изисквания
----------

- .NET 8 SDK (или ASP.NET Core Runtime 8.0)
- Visual Studio 2022 (17.8+) с workload "ASP.NET and web development"
- SQLite (идва с .NET, не изисква инсталация)


Как се стартира
---------------

Вариант 1: Visual Studio
------------------------

1. Отвори MySolution01.sln.
2. Десен клик върху Solution -> Configure Startup Projects.
3. Избери "Multiple startup projects".
4. За UserService и OperativeService избери "Start".
5. Натисни F5.

Ще се отворят два таба в браузъра:
- https://localhost:7276/swagger - UserService
- https://localhost:7291/swagger - OperativeService


Вариант 2: Command line
-----------------------

Терминал 1:
    cd UserService
    dotnet run

Терминал 2:
    cd OperativeService
    dotnet run


API Endpoints
=============

UserService (https://localhost:7276)
------------------------------------

POST /api/ItsAuth/register
Регистрира нов потребител.

Тяло (multipart/form-data):
- FirstName  (string, задължително)
- LastName   (string, задължително)
- Email      (string, задължително)
- Password   (string, задължително)
- Image      (file, по избор)

Отговор 200 OK:
    { "message": "User created successfully" }

Грешки:
- 400 Bad Request - невалидни данни или съществуващ имейл


POST /api/ItsAuth/login
Връща JWT токен.

Тяло (application/json):
    {
      "email": "ivan@test.com",
      "password": "123456"
    }

Отговор 200 OK:
    { "token": "eyJhbGciOiJIUzI1NiIs..." }

Грешки:
- 400 Bad Request - невалидни данни
- 401 Unauthorized - грешен имейл или парола


OperativeService (https://localhost:7291)
-----------------------------------------

ВАЖНО: Всички endpoints изискват JWT токен в header-а:
    Authorization: Bearer <token>


POST /api/ItsRolls
Хвърля два зара и записва резултата.

Отговор 200 OK:
    {
      "die1": 3,
      "die2": 5,
      "sum": 8,
      "rolledAt": "2026-09-22T14:45:46"
    }


GET /api/ItsRolls
Връща хвърлянията на логнатия потребител.

Query параметри:
- Filter    : all | year | month | day    (default: all)
- Year      : 1900-2100
- Month     : 1-12
- Day       : 1-31
- SortBy    : id | date | sum            (default: id)
- Order     : asc | desc                 (default: asc)
- SortBy2   : id | date | sum            (по избор)
- Order2    : asc | desc                 (по избор)
- Page      : >= 1                       (default: 1)
- PageSize  : 1-100                      (default: 10)

Сортиране:
- Ако sum участва - той е винаги с по-висок приоритет (секция ii).
- Ако само date участва - тя е primary.
- Ако нищо не участва - id ascending.

Отговор 200 OK:
    {
      "data": [
        { "id": 1, "userId": 1, "die1": 3, "die2": 5, "sum": 8, "rolledAt": "..." }
      ],
      "page": 1,
      "pageSize": 10,
      "totalRecords": 47,
      "totalPages": 5
    }

Примери:
    GET /api/ItsRolls?Filter=Year&Year=2026
    GET /api/ItsRolls?Filter=Month&Year=2026&Month=9
    GET /api/ItsRolls?SortBy=sum&Order=desc
    GET /api/ItsRolls?SortBy=sum&Order=desc&SortBy2=date&Order2=asc
    GET /api/ItsRolls?Page=2&PageSize=5


Технологии
----------

- ASP.NET Core 8 - Web API framework
- Entity Framework Core 8 - ORM
- SQLite - база данни
- JWT (JSON Web Tokens) - автентикация
- BCrypt.Net - хеширане на пароли
- Swashbuckle - Swagger/OpenAPI документация
- Data Annotations - валидация


Принципи
--------

- Microservices architecture - два независими сървиса, две бази.
- RESTful API - правилни HTTP методи, статус кодове, ресурси.
- Clean code - ясна структура, коментари, отделени отговорности.
- Error handling - global exception middleware, стандартизирани отговори.
- Request validation - Data Annotations в DTO-тата.
- Security - JWT, BCrypt хеширане, уникален Email.


Структура
---------

MySolution01/
  UserService/
    Controllers/
      ItsAuthController.cs
    Data/
      ItsAppDbContext.cs
    DTOs/
      ItsRegisterDto.cs
      ItsLoginDto.cs
      ItsAuthResponseDto.cs
    Middleware/
      ItsExceptionMiddleware.cs
    Models/
      ItsUser.cs
    Program.cs
    appsettings.json

  OperativeService/
    Controllers/
      ItsRollsController.cs
    Data/
      ItsAppDbContext.cs
    DTOs/
      ItsRollsQueryDto.cs
      ItsPagedResponseDto.cs
    Middleware/
      ItsExceptionMiddleware.cs
    Models/
      ItsDiceRoll.cs
    Program.cs
    appsettings.json


Тестване с curl
---------------

Регистрация:
    curl -X POST https://localhost:7276/api/ItsAuth/register \
      -F "FirstName=Ivan" \
      -F "LastName=Petrov" \
      -F "Email=ivan@test.com" \
      -F "Password=123456" -k

Логин:
    curl -X POST https://localhost:7276/api/ItsAuth/login \
      -H "Content-Type: application/json" \
      -d "{\"email\":\"ivan@test.com\",\"password\":\"123456\"}" -k

Хвърляне (замести ТОКЕН):
    curl -X POST https://localhost:7291/api/ItsRolls \
      -H "Authorization: Bearer ТОКЕН" -k

История:
    curl "https://localhost:7291/api/ItsRolls?SortBy=sum&Order=desc" \
      -H "Authorization: Bearer ТОКЕН" -k
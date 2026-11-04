# Лабораторна робота №15: LINQ в дії: фільтри, join, агрегації. Продуктивність.
**Курс:** Об'єктно-орієнтоване програмування  
**Навчальний заклад:** Рівненський фаховий коледж інформаційних технологій  
**Варіант:** 7 (Управління проєктами)  

---

## Опис сутностей та тестових даних
В рамках варіанту 7 створено класи `Manager` та `Project`:
- `Manager`: `Id`, `Name`, `Department`.
- `Project`: `Id`, `Name`, `StartDate`, `EndDate`, `ManagerId`, обчислювана властивість `DurationDays`.

Базу даних заповнено тестовим набором із 10 менеджерів та 15 проєктів.

---

## Виконанні LINQ-запити та порівняння синтаксисів

### 1. Пошук проєктів 2023 року у відділі "IT"
- **Query Syntax:**
  ```csharp
  var result = (from p in projects
                join m in managers on p.ManagerId equals m.Id
                where p.StartDate.Year == 2023 && m.Department == "IT"
                select new { p.Name, p.StartDate, m.Department }).ToList();
Method Syntax:

C#
var result = projects.Join(managers, p => p.ManagerId, m => m.Id, (p, m) => new { p, m })
                     .Where(x => x.p.StartDate.Year == 2023 && x.m.Department == "IT")
                     .Select(x => new { x.p.Name, x.p.StartDate, x.m.Department })
                     .ToList();
2. Об'єднання проєктів з менеджерами (Join)
Query Syntax:

C#
var result = (from p in projects
              join m in managers on p.ManagerId equals m.Id
              select new { ProjectName = p.Name, ManagerName = m.Name }).ToList();
Method Syntax:

C#
var result = projects.Join(managers, p => p.ManagerId, m => m.Id, (p, m) => new { ProjectName = p.Name, ManagerName = m.Name }).ToList();
3. Підрахунок кількості проєктів для кожного менеджера (GroupJoin / Агрегація)
Query Syntax:

C#
var result = (from m in managers
              join p in projects on m.Id equals p.ManagerId into projGroup
              select new { ManagerName = m.Name, ProjectCount = projGroup.Count() }).ToList();
Method Syntax:

C#
var result = managers.GroupJoin(projects, m => m.Id, p => p.ManagerId, (m, projGroup) => new { ManagerName = m.Name, ProjectCount = projGroup.Count() }).ToList();
4. Пошук проєкту з найдовшою тривалістю
Query Syntax:

C#
int maxDuration = (from p in projects select p.DurationDays).Max();
var result = (from p in projects where p.DurationDays == maxDuration select p).FirstOrDefault();
Method Syntax:

C#
var result = projects.OrderByDescending(p => p.DurationDays).FirstOrDefault();
Приклад виводу в консоль
Plaintext
=== Лабораторна робота №15 | Варіант 7: Управління проєктами ===

--- 1. Проєкти, що починаються у 2023 році та належать відділу 'IT' ---
 - Проєкт: CRM System Redesign | Старт: 2023-02-15 | Відділ: IT
 - Проєкт: Mobile Banking App | Старт: 2023-05-01 | Відділ: IT
 - Проєкт: Cloud Infrastructure Migration | Старт: 2023-01-10 | Відділ: IT
 - Проєкт: AI Chatbot Integration | Старт: 2023-09-15 | Відділ: IT
 - Проєкт: Data Warehouse Setup | Старт: 2023-06-01 | Відділ: IT
 - Проєкт: DevOps Pipeline Automation | Старт: 2023-08-10 | Відділ: IT
[Час виконання] Query Syntax: 1450 ticks | Method Syntax: 820 ticks

--- 2. Об'єднання проєктів з менеджерами (Назва проєкту + Ім'я менеджера) ---
 - Проєкт: CRM System Redesign ---> Менеджер: Олександр Петренко
 - Проєкт: Mobile Banking App ---> Менеджер: Марія Ковальчук
 - Проєкт: HR Onboarding Portal ---> Менеджер: Іван Бондаренко
 - Проєкт: SMM Campaign Q3 ---> Менеджер: Олена Шевченко
 - Проєкт: Cloud Infrastructure Migration ---> Менеджер: Дмитро Бойко
   ... та ще 10 записів.
[Час виконання] Query Syntax: 310 ticks | Method Syntax: 240 ticks

--- 3. Кількість проєктів для кожного менеджера ---
 - Олександр Петренко: 2 проєкт(и)
 - Марія Ковальчук: 2 проєкт(и)
 - Іван Бондаренко: 1 проєкт(и)
 - Олена Шевченко: 2 проєкт(и)
 - Дмитро Бойко: 2 проєкт(и)
 - Анна Ткаченко: 1 проєкт(и)
 - Сергій Кравченко: 2 проєкт(и)
 - Наталія Лисенко: 1 проєкт(и)
 - Василь Гуменюк: 1 проєкт(и)
 - Ірина Мельник: 1 проєкт(и)
[Час виконання] Query Syntax: 290 ticks | Method Syntax: 210 ticks

--- 4. Проєкт з найдовшою тривалістю ---
 - Найдовший проєкт: 'ERP System Rollout'
 - Тривалість: 1095 днів (з 2021-01-15 по 2024-01-15)
[Час виконання] Query Syntax: 180 ticks | Method Syntax: 150 ticks

=== Лабораторну роботу №15 успішно виконано! ===
Висновки щодо продуктивності
1.Method Syntax проти Query Syntax: Синтаксис методів виконується трохи швидше через відсутність додаткової трансляції виразів компілятором у виклики методів розширення.

2.Найдорожчі операції: Операції об'єднання (Join, GroupJoin) вимагають найбільше часу через порівняння ключів та побудову хеш-таблиць.

3.Матеріалізація: Метод .ToList() виконує запит негайно, що дозволяє точніше виміряти час за допомогою Stopwatch.
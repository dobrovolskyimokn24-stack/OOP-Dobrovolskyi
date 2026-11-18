# Лабораторна робота №16: LINQ: SelectMany, пагінація й custom extensions.
**Курс:** Об'єктно-орієнтоване програмування  
**Навчальний заклад:** Рівненський фаховий коледж інформаційних технологій  
**Варіант:** 7 (Університети та їхні факультети з кафедрами)  

---

## Опис сутностей та тестових даних
У рамках варіанту 7 створено ієрархію класів із вкладеними колекціями:
- `University`: `Id`, `Name`, `List<Faculty> Faculties`.
- `Faculty`: `Id`, `Name`, `List<Department> Departments`.
- `Department`: `Id`, `Name`, `NumStudents`.

Базу даних заповнено тестовим набором із 3 університетів, 8 факультетів та 15 кафедр.

---

## Реалізовані операції LINQ

### 1. Використання `SelectMany` (розгортання вкладених колекцій)
Оператор `SelectMany` дозволяє перетворити багаторівневу ієрархію `University -> Faculty -> Department` у плоский список усіх кафедр:
```csharp
var allDepartments = universities
    .SelectMany(u => u.Faculties)
    .SelectMany(f => f.Departments)
    .ToList();
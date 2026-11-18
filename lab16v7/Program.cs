using System;
using System.Collections.Generic;
using System.Linq;

namespace lab16v7
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NumStudents { get; set; }
    }

    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Department> Departments { get; set; } = new List<Department>();
    }

    public class University
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Faculty> Faculties { get; set; } = new List<Faculty>();
    }

    public static class DepartmentExtensions
    {
        public static int TotalStudents(this IEnumerable<Department> departments)
        {
            if (departments == null) return 0;
            return departments.Sum(d => d.NumStudents);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №16 | Варіант 7: Університети та факультети ===\n");

            var universities = new List<University>
            {
                new University
                {
                    Id = 1,
                    Name = "Національний університет водного господарства та природокористування",
                    Faculties = new List<Faculty>
                    {
                        new Faculty
                        {
                            Id = 101,
                            Name = "Навчально-науковий інститут автоматики, кібернетики та обчислювальної техніки",
                            Departments = new List<Department>
                            {
                                new Department { Id = 1, Name = "Кафедра комп'ютерних наук та прикладної математики", NumStudents = 320 },
                                new Department { Id = 2, Name = "Кафедра автоматизації, електротехнічних та комп'ютерно-інтегрованих технологій", NumStudents = 240 }
                            }
                        },
                        new Faculty
                        {
                            Id = 102,
                            Name = "Навчально-науковий інститут будівництва та архітектури",
                            Departments = new List<Department>
                            {
                                new Department { Id = 3, Name = "Кафедра промислового, цивільного будівництва та інженерних споруд", NumStudents = 290 },
                                new Department { Id = 4, Name = "Кафедра архітектури та середовищного дизайну", NumStudents = 180 }
                            }
                        },
                        new Faculty
                        {
                            Id = 103,
                            Name = "Навчально-науковий інститут економіки та менеджменту",
                            Departments = new List<Department>
                            {
                                new Department { Id = 5, Name = "Кафедра економічної кібернетики", NumStudents = 150 },
                                new Department { Id = 6, Name = "Кафедра менеджменту", NumStudents = 210 }
                            }
                        }
                    }
                },
                new University
                {
                    Id = 2,
                    Name = "Рівненський державний гуманітарний університет",
                    Faculties = new List<Faculty>
                    {
                        new Faculty
                        {
                            Id = 104,
                            Name = "Факультет математики та інформатики",
                            Departments = new List<Department>
                            {
                                new Department { Id = 7, Name = "Кафедра інформатики та прикладної математики", NumStudents = 260 },
                                new Department { Id = 8, Name = "Кафедра вищої математики", NumStudents = 110 }
                            }
                        },
                        new Faculty
                        {
                            Id = 105,
                            Name = "Факультет іноземної філології",
                            Departments = new List<Department>
                            {
                                new Department { Id = 9, Name = "Кафедра практики англійської мови", NumStudents = 340 },
                                new Department { Id = 10, Name = "Кафедра німецької філології", NumStudents = 140 }
                            }
                        }
                    }
                },
                new University
                {
                    Id = 3,
                    Name = "Національний університет 'Острозька академія'",
                    Faculties = new List<Faculty>
                    {
                        new Faculty
                        {
                            Id = 106,
                            Name = "Економічний факультет",
                            Departments = new List<Department>
                            {
                                new Department { Id = 11, Name = "Кафедра фінансів, обліку і аудиту", NumStudents = 195 },
                                new Department { Id = 12, Name = "Кафедра економіко-математичного моделювання та інформаційних технологій", NumStudents = 175 }
                            }
                        },
                        new Faculty
                        {
                            Id = 107,
                            Name = "Факультет політико-інформаційного менеджменту",
                            Departments = new List<Department>
                            {
                                new Department { Id = 13, Name = "Кафедра журналістики та PR-менеджменту", NumStudents = 230 },
                                new Department { Id = 14, Name = "Кафедра політології та національної безпеки", NumStudents = 160 }
                            }
                        },
                        new Faculty
                        {
                            Id = 108,
                            Name = "Навчально-науковий інститут права ім. І. Малиновського",
                            Departments = new List<Department>
                            {
                                new Department { Id = 15, Name = "Кафедра кримінально-правових дисциплін", NumStudents = 280 }
                            }
                        }
                    }
                }
            };

            Console.WriteLine("--- 1. SelectMany: Усі кафедри всіх факультетів всіх університетів ---");
            var allDepartments = universities
                .SelectMany(u => u.Faculties)
                .SelectMany(f => f.Departments)
                .ToList();

            foreach (var dept in allDepartments)
            {
                Console.WriteLine($" - [{dept.Id:D2}] {dept.Name} (Студентів: {dept.NumStudents})");
            }
            Console.WriteLine($"Усього кафедр: {allDepartments.Count}\n");

            Console.WriteLine("--- 2. Пагінація: Усі факультети по 3 факультети на сторінку ---");
            var allFaculties = universities
                .SelectMany(u => u.Faculties)
                .ToList();

            int pageSize = 3;
            int totalPages = (int)Math.Ceiling((double)allFaculties.Count / pageSize);

            for (int page = 1; page <= totalPages; page++)
            {
                Console.WriteLine($"\n[Сторінка {page} з {totalPages}]");
                var pageItems = allFaculties
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                foreach (var fac in pageItems)
                {
                    Console.WriteLine($" * {fac.Name} (Кафедр: {fac.Departments.Count})");
                }
            }
            Console.WriteLine();

            Console.WriteLine("--- 3. Custom Extension: TotalStudents() для IEnumerable<Department> ---");
            int totalStudentsAll = allDepartments.TotalStudents();
            Console.WriteLine($"Загальна кількість студентів на всіх кафедрах: {totalStudentsAll}");

            var firstUniversityDepts = universities[0].Faculties.SelectMany(f => f.Departments);
            Console.WriteLine($"Студентів в університеті '{universities[0].Name}': {firstUniversityDepts.TotalStudents()}\n");

            Console.WriteLine("=== Лабораторну роботу №16 успішно виконано! ===");
        }
    }
}
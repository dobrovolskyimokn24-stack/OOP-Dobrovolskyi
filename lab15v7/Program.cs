using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace lab15v7
{
    public class Manager
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ManagerId { get; set; }

        public int DurationDays => (EndDate - StartDate).Days;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №15 | Варіант 7: Управління проєктами ===\n");

            var managers = new List<Manager>
            {
                new Manager { Id = 1, Name = "Олександр Петренко", Department = "IT" },
                new Manager { Id = 2, Name = "Марія Ковальчук", Department = "IT" },
                new Manager { Id = 3, Name = "Іван Бондаренко", Department = "HR" },
                new Manager { Id = 4, Name = "Олена Шевченко", Department = "Marketing" },
                new Manager { Id = 5, Name = "Дмитро Бойко", Department = "IT" },
                new Manager { Id = 6, Name = "Анна Ткаченко", Department = "Finance" },
                new Manager { Id = 7, Name = "Сергій Кравченко", Department = "IT" },
                new Manager { Id = 8, Name = "Наталія Лисенко", Department = "Marketing" },
                new Manager { Id = 9, Name = "Василь Гуменюк", Department = "IT" },
                new Manager { Id = 10, Name = "Ірина Мельник", Department = "HR" }
            };

            var projects = new List<Project>
            {
                new Project { Id = 101, Name = "CRM System Redesign", StartDate = new DateTime(2023, 02, 15), EndDate = new DateTime(2023, 10, 20), ManagerId = 1 },
                new Project { Id = 102, Name = "Mobile Banking App", StartDate = new DateTime(2023, 05, 01), EndDate = new DateTime(2024, 06, 01), ManagerId = 2 },
                new Project { Id = 103, Name = "HR Onboarding Portal", StartDate = new DateTime(2022, 11, 10), EndDate = new DateTime(2023, 04, 15), ManagerId = 3 },
                new Project { Id = 104, Name = "SMM Campaign Q3", StartDate = new DateTime(2023, 07, 01), EndDate = new DateTime(2023, 09, 30), ManagerId = 4 },
                new Project { Id = 105, Name = "Cloud Infrastructure Migration", StartDate = new DateTime(2023, 01, 10), EndDate = new DateTime(2024, 12, 31), ManagerId = 5 },
                new Project { Id = 106, Name = "Annual Financial Audit", StartDate = new DateTime(2023, 03, 01), EndDate = new DateTime(2023, 05, 01), ManagerId = 6 },
                new Project { Id = 107, Name = "AI Chatbot Integration", StartDate = new DateTime(2023, 09, 15), EndDate = new DateTime(2024, 03, 15), ManagerId = 1 },
                new Project { Id = 108, Name = "Cybersecurity Upgrade", StartDate = new DateTime(2022, 08, 01), EndDate = new DateTime(2023, 08, 01), ManagerId = 7 },
                new Project { Id = 109, Name = "Brand Awareness Strategy", StartDate = new DateTime(2023, 04, 10), EndDate = new DateTime(2023, 11, 10), ManagerId = 8 },
                new Project { Id = 110, Name = "ERP System Rollout", StartDate = new DateTime(2021, 01, 15), EndDate = new DateTime(2024, 01, 15), ManagerId = 9 },
                new Project { Id = 111, Name = "Data Warehouse Setup", StartDate = new DateTime(2023, 06, 01), EndDate = new DateTime(2023, 12, 01), ManagerId = 2 },
                new Project { Id = 112, Name = "SEO Optimization", StartDate = new DateTime(2023, 02, 01), EndDate = new DateTime(2023, 06, 01), ManagerId = 4 },
                new Project { Id = 113, Name = "DevOps Pipeline Automation", StartDate = new DateTime(2023, 08, 10), EndDate = new DateTime(2024, 02, 10), ManagerId = 5 },
                new Project { Id = 114, Name = "Employee Satisfaction Survey", StartDate = new DateTime(2023, 10, 01), EndDate = new DateTime(2023, 11, 15), ManagerId = 10 },
                new Project { Id = 115, Name = "Legacy Code Refactoring", StartDate = new DateTime(2023, 03, 15), EndDate = new DateTime(2023, 08, 15), ManagerId = 7 }
            };

            Stopwatch sw = new Stopwatch();

            // 1. Проєкти 2023 року у відділі "IT"
            Console.WriteLine("--- 1. Проєкти, що починаються у 2023 році та належать відділу 'IT' ---");

            sw.Restart();
            var q1Query = (from p in projects
                           join m in managers on p.ManagerId equals m.Id
                           where p.StartDate.Year == 2023 && m.Department == "IT"
                           select new { p.Name, p.StartDate, m.Department }).ToList();
            sw.Stop();
            long t1Query = sw.ElapsedTicks;

            sw.Restart();
            var q1Method = projects.Join(managers,
                                         p => p.ManagerId,
                                         m => m.Id,
                                         (p, m) => new { p, m })
                                   .Where(x => x.p.StartDate.Year == 2023 && x.m.Department == "IT")
                                   .Select(x => new { x.p.Name, x.p.StartDate, x.m.Department })
                                   .ToList();
            sw.Stop();
            long t1Method = sw.ElapsedTicks;

            foreach (var item in q1Method)
            {
                Console.WriteLine($" - Проєкт: {item.Name} | Старт: {item.StartDate:yyyy-MM-dd} | Відділ: {item.Department}");
            }
            Console.WriteLine($"[Час виконання] Query Syntax: {t1Query} ticks | Method Syntax: {t1Method} ticks\n");

            // 2. Об'єднання проєктів з менеджерами
            Console.WriteLine("--- 2. Об'єднання проєктів з менеджерами (Назва проєкту + Ім'я менеджера) ---");

            sw.Restart();
            var q2Query = (from p in projects
                           join m in managers on p.ManagerId equals m.Id
                           select new { ProjectName = p.Name, ManagerName = m.Name }).ToList();
            sw.Stop();
            long t2Query = sw.ElapsedTicks;

            sw.Restart();
            var q2Method = projects.Join(managers,
                                         p => p.ManagerId,
                                         m => m.Id,
                                         (p, m) => new { ProjectName = p.Name, ManagerName = m.Name })
                                   .ToList();
            sw.Stop();
            long t2Method = sw.ElapsedTicks;

            foreach (var item in q2Method.Take(5))
            {
                Console.WriteLine($" - Проєкт: {item.ProjectName} ---> Менеджер: {item.ManagerName}");
            }
            Console.WriteLine($"   ... та ще {q2Method.Count - 5} записів.");
            Console.WriteLine($"[Час виконання] Query Syntax: {t2Query} ticks | Method Syntax: {t2Method} ticks\n");

            // 3. Кількість проєктів для кожного менеджера
            Console.WriteLine("--- 3. Кількість проєктів для кожного менеджера ---");

            sw.Restart();
            var q3Query = (from m in managers
                           join p in projects on m.Id equals p.ManagerId into projGroup
                           select new { ManagerName = m.Name, ProjectCount = projGroup.Count() }).ToList();
            sw.Stop();
            long t3Query = sw.ElapsedTicks;

            sw.Restart();
            var q3Method = managers.GroupJoin(projects,
                                              m => m.Id,
                                              p => p.ManagerId,
                                              (m, projGroup) => new { ManagerName = m.Name, ProjectCount = projGroup.Count() })
                                   .ToList();
            sw.Stop();
            long t3Method = sw.ElapsedTicks;

            foreach (var item in q3Method)
            {
                Console.WriteLine($" - {item.ManagerName}: {item.ProjectCount} проєкт(и)");
            }
            Console.WriteLine($"[Час виконання] Query Syntax: {t3Query} ticks | Method Syntax: {t3Method} ticks\n");

            // 4. Проєкт з найдовшою тривалістю
            Console.WriteLine("--- 4. Проєкт з найдовшою тривалістю ---");

            sw.Restart();
            int maxDurationQuery = (from p in projects select p.DurationDays).Max();
            var q4Query = (from p in projects
                           where p.DurationDays == maxDurationQuery
                           select p).FirstOrDefault();
            sw.Stop();
            long t4Query = sw.ElapsedTicks;

            sw.Restart();
            var q4Method = projects.OrderByDescending(p => p.DurationDays).FirstOrDefault();
            sw.Stop();
            long t4Method = sw.ElapsedTicks;

            if (q4Method != null)
            {
                Console.WriteLine($" - Найдовший проєкт: '{q4Method.Name}'");
                Console.WriteLine($" - Тривалість: {q4Method.DurationDays} днів (з {q4Method.StartDate:yyyy-MM-dd} по {q4Method.EndDate:yyyy-MM-dd})");
            }
            Console.WriteLine($"[Час виконання] Query Syntax: {t4Query} ticks | Method Syntax: {t4Method} ticks\n");

            Console.WriteLine("=== Лабораторну роботу №15 успішно виконано! ===");
        }
    }
}

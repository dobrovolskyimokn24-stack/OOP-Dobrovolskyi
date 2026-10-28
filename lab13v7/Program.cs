using System;
using System.Collections.Generic;

namespace lab13v7
{
    public static class FunctionalUtils
    {
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in list)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static List<T> Filter<T>(List<T> list, Func<T, bool> predicate)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static TAccumulate Reduce<T, TAccumulate>(List<T> list, TAccumulate initialValue, Func<TAccumulate, T, TAccumulate> accumulator)
        {
            TAccumulate result = initialValue;
            foreach (var item in list)
            {
                result = accumulator(result, item);
            }
            return result;
        }

        public static List<T> Skip<T>(List<T> list, int count)
        {
            var result = new List<T>();
            for (int i = count; i < list.Count; i++)
            {
                if (i >= 0)
                {
                    result.Add(list[i]);
                }
            }
            return result;
        }

        public static List<T> Take<T>(List<T> list, int count)
        {
            var result = new List<T>();
            int limit = Math.Min(count, list.Count);
            for (int i = 0; i < limit; i++)
            {
                if (i >= 0)
                {
                    result.Add(list[i]);
                }
            }
            return result;
        }
    }

    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int EmployeesCount { get; set; }

        public override string ToString()
        {
            return $"[ID: {Id}] {Name} ({Location}) - {EmployeesCount} працівників";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ LAB13v7 (Generic Methods & Delegates) ===\n");

            Console.WriteLine("--- 1. РОБОТА З ТИПОМ INT ---");
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var squared = FunctionalUtils.Map(numbers, x => x * x);
            Console.WriteLine($"Map (квадрати): {string.Join(", ", squared)}");

            var evenNumbers = FunctionalUtils.Filter(numbers, x => x % 2 == 0);
            Console.WriteLine($"Filter (парні): {string.Join(", ", evenNumbers)}");

            int sum = FunctionalUtils.Reduce(numbers, 0, (acc, x) => acc + x);
            Console.WriteLine($"Reduce (сума): {sum}");

            var skippedNumbers = FunctionalUtils.Skip(numbers, 6);
            Console.WriteLine($"Skip (пропустити 6): {string.Join(", ", skippedNumbers)}");

            var takenNumbers = FunctionalUtils.Take(numbers, 4);
            Console.WriteLine($"Take (взяти 4): {string.Join(", ", takenNumbers)}\n");

            Console.WriteLine("--- 2. РОБОТА З ТИПОМ STRING ---");
            List<string> words = new List<string> { "C#", "Generics", "Delegates", "LINQ", "Lambda", "DotNet" };

            var upperWords = FunctionalUtils.Map(words, w => w.ToUpper());
            Console.WriteLine($"Map (верхній регістр): {string.Join(", ", upperWords)}");

            var longWords = FunctionalUtils.Filter(words, w => w.Length > 5);
            Console.WriteLine($"Filter (довжина > 5): {string.Join(", ", longWords)}");

            string sentence = FunctionalUtils.Reduce(words, "", (acc, w) => acc == "" ? w : acc + " " + w);
            Console.WriteLine($"Reduce (об'єднання): {sentence}");

            var skippedWords = FunctionalUtils.Skip(words, 3);
            Console.WriteLine($"Skip (пропустити 3): {string.Join(", ", skippedWords)}");

            var takenWords = FunctionalUtils.Take(words, 2);
            Console.WriteLine($"Take (взяти 2): {string.Join(", ", takenWords)}\n");

            Console.WriteLine("--- 3. РОБОТА З ВЛАСНИМ КЛАСОМ (Department) ---");
            List<Department> departments = new List<Department>
            {
                new Department { Id = 1, Name = "Software Engineering", Location = "Building A", EmployeesCount = 25 },
                new Department { Id = 2, Name = "Human Resources", Location = "Building B", EmployeesCount = 8 },
                new Department { Id = 3, Name = "Quality Assurance", Location = "Building A", EmployeesCount = 14 },
                new Department { Id = 4, Name = "Cybersecurity", Location = "Building C", EmployeesCount = 18 },
                new Department { Id = 5, Name = "DevOps", Location = "Building C", EmployeesCount = 10 }
            };

            var deptNames = FunctionalUtils.Map(departments, d => d.Name);
            Console.WriteLine($"Map (лише назви): {string.Join(" | ", deptNames)}");

            var largeDepts = FunctionalUtils.Filter(departments, d => d.EmployeesCount >= 15);
            Console.WriteLine("Filter (працівників >= 15):");
            foreach (var d in largeDepts) Console.WriteLine($"  {d}");

            int totalEmployees = FunctionalUtils.Reduce(departments, 0, (acc, d) => acc + d.EmployeesCount);
            Console.WriteLine($"Reduce (загальна кількість працівників): {totalEmployees}");

            var skippedDepts = FunctionalUtils.Skip(departments, 3);
            Console.WriteLine("Skip (пропустити перші 3 відділи):");
            foreach (var d in skippedDepts) Console.WriteLine($"  {d}");

            var takenDepts = FunctionalUtils.Take(departments, 2);
            Console.WriteLine("Take (взяти перші 2 відділи):");
            foreach (var d in takenDepts) Console.WriteLine($"  {d}");
        }
    }
}
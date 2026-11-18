using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Samostijna13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №13: PLINQ: дослідження продуктивності та безпеки ===\n");
            Console.WriteLine($"Кількість логічних ядер процесора: {Environment.ProcessorCount}\n");

            RunPerformanceBenchmarks();
            RunThreadSafetyExperiment();

            Console.WriteLine("=== Самостійну роботу №13 успішно виконано! ===");
        }

        static List<int> GenerateNumbers(int count)
        {
            var random = new Random(42);
            var list = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(random.Next(10_000, 500_000));
            }
            return list;
        }

        static bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            int limit = (int)Math.Sqrt(number);
            for (int i = 3; i <= limit; i += 2)
            {
                if (number % i == 0) return false;
            }
            return true;
        }

        static double HeavyTrigonometry(int number)
        {
            double result = number;
            for (int i = 1; i <= 15; i++)
            {
                result = Math.Sin(result) * Math.Cos(i) + Math.Sqrt(Math.Abs(result) + i);
            }
            return result;
        }

        static void RunPerformanceBenchmarks()
        {
            int[] sizes = { 1_000_000, 5_000_000, 10_000_000 };

            Console.WriteLine("--- Експеримент 1: Перевірка на просте число (CPU-bound) ---");
            Console.WriteLine($"{"Розмір",-12} | {"LINQ (мс)",-12} | {"PLINQ (мс)",-12} | {"Прискорення",-12} | {"Збігів"}");
            Console.WriteLine(new string('-', 66));

            foreach (int size in sizes)
            {
                var data = GenerateNumbers(size);

                var sw = Stopwatch.StartNew();
                int linqCount = data
                    .Where(n => IsPrime(n))
                    .Select(n => (long)n * n)
                    .Count();
                sw.Stop();
                long linqMs = sw.ElapsedMilliseconds;

                sw.Restart();
                int plinqCount = data
                    .AsParallel()
                    .Where(n => IsPrime(n))
                    .Select(n => (long)n * n)
                    .Count();
                sw.Stop();
                long plinqMs = sw.ElapsedMilliseconds;

                double speedup = plinqMs > 0 ? (double)linqMs / plinqMs : 0;
                Console.WriteLine($"{size,-12:N0} | {linqMs,-12} | {plinqMs,-12} | {speedup,-11:F2}x | {linqCount} / {plinqCount}");
            }
            Console.WriteLine();

            Console.WriteLine("--- Експеримент 2: Тригонометричні обчислення (HeavyTrigonometry) ---");
            Console.WriteLine($"{"Розмір",-12} | {"LINQ (мс)",-12} | {"PLINQ (мс)",-12} | {"Прискорення",-12} | {"Сума збігається"}");
            Console.WriteLine(new string('-', 72));

            foreach (int size in sizes)
            {
                var data = GenerateNumbers(size);

                var sw = Stopwatch.StartNew();
                double linqSum = data
                    .Where(n => n % 2 == 1)
                    .Select(n => HeavyTrigonometry(n))
                    .Sum();
                sw.Stop();
                long linqMs = sw.ElapsedMilliseconds;

                sw.Restart();
                double plinqSum = data
                    .AsParallel()
                    .Where(n => n % 2 == 1)
                    .Select(n => HeavyTrigonometry(n))
                    .Sum();
                sw.Stop();
                long plinqMs = sw.ElapsedMilliseconds;

                double speedup = plinqMs > 0 ? (double)linqMs / plinqMs : 0;
                bool isMatch = Math.Abs(linqSum - plinqSum) < 0.01;
                Console.WriteLine($"{size,-12:N0} | {linqMs,-12} | {plinqMs,-12} | {speedup,-11:F2}x | {isMatch}");
            }
            Console.WriteLine();

            Console.WriteLine("--- Експеримент 3: Легка операція (множення на 2) на 1 000 000 елементів ---");
            var smallData = GenerateNumbers(1_000_000);

            var swLight = Stopwatch.StartNew();
            var linqLight = smallData.Where(n => n > 250_000).Select(n => n * 2).ToList();
            swLight.Stop();
            long linqLightMs = swLight.ElapsedMilliseconds;

            swLight.Restart();
            var plinqLight = smallData.AsParallel().Where(n => n > 250_000).Select(n => n * 2).ToList();
            swLight.Stop();
            long plinqLightMs = swLight.ElapsedMilliseconds;

            Console.WriteLine($"LINQ: {linqLightMs} мс | PLINQ: {plinqLightMs} мс (накладні витрати на координацію потоків)\n");
        }

        static void RunThreadSafetyExperiment()
        {
            Console.WriteLine("--- Експеримент 4: Дослідження безпеки та побічних ефектів у PLINQ ---");
            int expectedCount = 1_000_000;
            var numbers = Enumerable.Range(1, expectedCount).ToList();

            Console.WriteLine($"Очікувана кількість елементів: {expectedCount:N0}\n");

            Console.WriteLine("1. Небезпечний PLINQ (модифікація спільної змінної без синхронізації):");
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                int unsafeCounter = 0;
                var unsafeList = new List<int>();

                try
                {
                    numbers.AsParallel().ForAll(n =>
                    {
                        unsafeCounter++;
                        if (n % 100 == 0)
                        {
                            unsafeList.Add(n);
                        }
                    });
                    Console.WriteLine($"   Спроба #{attempt}: unsafeCounter = {unsafeCounter:N0} (втрачено {expectedCount - unsafeCounter:N0}), unsafeList.Count = {unsafeList.Count:N0} (очікувалось 10 000)");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   Спроба #{attempt}: Виняток через стан гонитви (Race Condition) -> {ex.GetType().Name}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("2. Виправлення через Interlocked та ConcurrentBag<T> (потокобезпечно):");
            int safeCounter = 0;
            var safeBag = new ConcurrentBag<int>();

            numbers.AsParallel().ForAll(n =>
            {
                Interlocked.Increment(ref safeCounter);
                if (n % 100 == 0)
                {
                    safeBag.Add(n);
                }
            });

            Console.WriteLine($"   Результат: safeCounter = {safeCounter:N0}, safeBag.Count = {safeBag.Count:N0} (100% точність)\n");

            Console.WriteLine("3. Чистий функціональний підхід PLINQ без побічних ефектів (.Where().Count()):");
            int functionalCount = numbers.AsParallel().Count();
            int filteredCount = numbers.AsParallel().Where(n => n % 100 == 0).Count();
            Console.WriteLine($"   Результат: functionalCount = {functionalCount:N0}, filteredCount = {filteredCount:N0}\n");
        }
    }
}
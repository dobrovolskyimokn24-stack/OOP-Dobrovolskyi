using System;
using System.Collections.Generic;

namespace Samostijna10
{
    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(
            IEnumerable<TSource> collection,
            Func<TSource, TResult> mapper)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (mapper == null) throw new ArgumentNullException(nameof(mapper));

            var result = new List<TResult>();
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        public static T MaxBy<T, TKey>(
            IEnumerable<T> collection,
            Func<T, TKey> keySelector)
            where TKey : IComparable<TKey>
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

            T maxItem = default!;
            TKey maxKey = default!;
            bool first = true;

            foreach (var item in collection)
            {
                TKey key = keySelector(item);
                if (first || key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                    first = false;
                }
            }

            if (first)
            {
                throw new InvalidOperationException("Колекція не містить жодного елемента.");
            }

            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(
            IEnumerable<T> collection,
            Predicate<T> predicate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static void ForEach<T>(
            IEnumerable<T> collection,
            Action<T> action)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in collection)
            {
                action(item);
            }
        }
    }

    public class Product
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №10: Делегати й лямбда-вирази (CollectionUtils) ===\n");

            Console.WriteLine("--- Колекція 1: List<Product> ---");
            var products = new List<Product>
            {
                new Product { Name = "Ноутбук", Price = 28500m },
                new Product { Name = "Мишка", Price = 650m },
                new Product { Name = "Клавіатура", Price = 1800m },
                new Product { Name = "Монітор", Price = 9200m }
            };

            var productNames = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine($"Назви товарів (Map): {string.Join(", ", productNames)}");

            var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine($"Найдорожчий товар (MaxBy): {mostExpensive.Name} ({mostExpensive.Price} грн)");

            var expensiveProducts = CollectionUtils.Filter(products, p => p.Price > 1000m);
            Console.WriteLine("Товари дорожчі за 1000 грн (Filter + ForEach):");
            CollectionUtils.ForEach(expensiveProducts, p => Console.WriteLine($" - {p.Name}: {p.Price} грн"));
            Console.WriteLine();

            Console.WriteLine("--- Колекція 2: List<int> ---");
            var numbers = new List<int> { 12, 5, 27, 8, 42, 19, 3 };

            var evenNumbers = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.WriteLine($"Парні числа (Filter): {string.Join(", ", evenNumbers)}");

            var squaredNumbers = CollectionUtils.Map(evenNumbers, n => n * n);
            Console.WriteLine($"Квадрати парних чисел (Map): {string.Join(", ", squaredNumbers)}");

            var maxNumber = CollectionUtils.MaxBy(numbers, n => n);
            Console.WriteLine($"Максимальне число (MaxBy): {maxNumber}\n");

            Console.WriteLine("--- Колекція 3: List<string> ---");
            var cities = new List<string> { "Рівне", "Київ", "Львів", "Одеса", "Івано-Франківськ" };

            var upperCities = CollectionUtils.Map(cities, c => c.ToUpper());
            Console.WriteLine($"Міста у верхньому регістрі (Map): {string.Join(", ", upperCities)}");

            var longestCity = CollectionUtils.MaxBy(cities, c => c.Length);
            Console.WriteLine($"Найдовша назва міста (MaxBy за довжиною): {longestCity} ({longestCity.Length} симв.)");

            var shortCities = CollectionUtils.Filter(cities, c => c.Length <= 5);
            Console.Write("Короткі назви (<= 5 симв., ForEach): ");
            CollectionUtils.ForEach(shortCities, c => Console.Write($"[{c}] "));
            Console.WriteLine("\n\n=== Самостійну роботу №10 успішно виконано! ===");
        }
    }
}
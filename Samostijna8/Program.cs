using System;
using System.Collections.Generic;

namespace Samostijna8
{
    public class Cache<TKey, TValue> where TKey : notnull
    {
        private readonly int _capacity;
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly Queue<TKey> _keysOrder;

        public Cache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Ємність кешу повинна бути більшою за нуль.", nameof(capacity));
            }

            _capacity = capacity;
            _cache = new Dictionary<TKey, TValue>(capacity);
            _keysOrder = new Queue<TKey>(capacity);
        }

        public int Count => _cache.Count;

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }

            if (_cache.Count >= _capacity)
            {
                TKey oldestKey = _keysOrder.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache.Add(key, value);
            _keysOrder.Enqueue(key);
        }

        public TValue Get(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (_cache.TryGetValue(key, out TValue? value))
            {
                return value;
            }

            throw new KeyNotFoundException($"Ключ '{key}' не знайдено в кеші.");
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
            {
                return false;
            }

            return _cache.ContainsKey(key);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №8: Узагальнений кеш Cache<TKey, TValue> (FIFO) ===\n");

            var cache = new Cache<string, string>(3);

            Console.WriteLine("Додаємо 3 елементи (ємність кешу = 3):");
            cache.Add("user_1", "Олександр");
            cache.Add("user_2", "Марія");
            cache.Add("user_3", "Дмитро");

            Console.WriteLine($"Поточна кількість елементів: {cache.Count}");
            Console.WriteLine($"Отримання 'user_1': {cache.Get("user_1")}\n");

            Console.WriteLine("Додаємо 4-й елемент ('user_4' -> 'Ірина') — має спрацювати FIFO:");
            cache.Add("user_4", "Ірина");

            Console.WriteLine($"Поточна кількість елементів: {cache.Count}");
            Console.WriteLine($"Чи є 'user_1' у кеші? {cache.ContainsKey("user_1")}");
            Console.WriteLine($"Чи є 'user_4' у кеші? {cache.ContainsKey("user_4")}");
            Console.WriteLine($"Отримання 'user_4': {cache.Get("user_4")}\n");

            Console.WriteLine("Спроба отримати видалений ключ 'user_1':");
            try
            {
                cache.Get("user_1");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Очікуваний виняток: {ex.Message}\n");
            }

            Console.WriteLine("=== Самостійну роботу №8 успішно виконано! ===");
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace lab17v7
{
    public class FileProcessor
    {
        private int _attempt = 0;

        public List<string> LoadUsernames(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(path));
            }

            _attempt++;
            if (_attempt <= 3)
            {
                throw new IOException($"Тимчасова помилка доступу до файлу '{path}' (спроба {_attempt}).");
            }

            return new List<string> { "admin_dobrovolskyi", "ivan_koval", "olena_shevchenko", "maksym_bondar" };
        }
    }

    public class NetworkClient
    {
        private int _attempt = 0;

        public List<string> GetUsersFromApi(string url)
        {
            _attempt++;
            if (_attempt <= 2)
            {
                throw new HttpRequestException($"Помилка підключення до API '{url}' (спроба {_attempt}).");
            }

            return new List<string> { "api_user_1", "api_user_2", "api_user_3" };
        }
    }

    public static class RetryHelper
    {
        public static T ExecuteWithRetry<T>(
            Func<T> operation,
            int retryCount = 3,
            TimeSpan initialDelay = default,
            Func<Exception, bool> shouldRetry = null)
        {
            if (initialDelay == default)
            {
                initialDelay = TimeSpan.FromMilliseconds(200);
            }

            int attempt = 0;
            while (true)
            {
                try
                {
                    attempt++;
                    return operation();
                }
                catch (Exception ex)
                {
                    bool canRetry = attempt <= retryCount && (shouldRetry == null || shouldRetry(ex));
                    Console.WriteLine($"[Лог] Спроба #{attempt} невдала: {ex.GetType().Name} — {ex.Message}");

                    if (!canRetry)
                    {
                        Console.WriteLine("[Лог] Повторні спроби припинено.\n");
                        throw;
                    }

                    TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                    Console.WriteLine($"[Лог] Очікування {delay.TotalMilliseconds} мс перед наступною спробою...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №17 | Варіант 7: Отримання списку користувачів ===\n");

            Func<Exception, bool> retryPolicy = ex => ex is IOException || ex is HttpRequestException;

            Console.WriteLine("--- 1. Сценарій FileProcessor.LoadUsernames (3 помилки IOException -> успіх) ---");
            var fileProcessor = new FileProcessor();
            try
            {
                var fileUsers = RetryHelper.ExecuteWithRetry(
                    () => fileProcessor.LoadUsernames("users.txt"),
                    retryCount: 4,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryPolicy
                );
                Console.WriteLine($"Успішно завантажено з файлу: {string.Join(", ", fileUsers)}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критична помилка: {ex.Message}\n");
            }

            Console.WriteLine("--- 2. Сценарій NetworkClient.GetUsersFromApi (2 помилки HttpRequestException -> успіх) ---");
            var networkClient = new NetworkClient();
            try
            {
                var apiUsers = RetryHelper.ExecuteWithRetry(
                    () => networkClient.GetUsersFromApi("https://api.example.com/users"),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryPolicy
                );
                Console.WriteLine($"Успішно отримано з API: {string.Join(", ", apiUsers)}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критична помилка: {ex.Message}\n");
            }

            Console.WriteLine("--- 3. Демонстрація shouldRetry (неповторюваний виняток ArgumentException) ---");
            try
            {
                RetryHelper.ExecuteWithRetry(
                    () => fileProcessor.LoadUsernames(""),
                    retryCount: 3,
                    initialDelay: TimeSpan.FromMilliseconds(200),
                    shouldRetry: retryPolicy
                );
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Перехоплено виняток без повторних спроб: {ex.Message}\n");
            }

            Console.WriteLine("=== Лабораторну роботу №17 успішно виконано! ===");
        }
    }
}

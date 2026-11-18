using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Samostijna15
{
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly TimeSpan _initialDelay;
        private readonly double _jitterFactor;
        private readonly Random _random;

        public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
        {
            if (maxRetries < 0) throw new ArgumentOutOfRangeException(nameof(maxRetries));
            if (initialDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(initialDelay));
            if (jitterFactor < 0 || jitterFactor > 1) throw new ArgumentOutOfRangeException(nameof(jitterFactor));

            _maxRetries = maxRetries;
            _initialDelay = initialDelay;
            _jitterFactor = jitterFactor;
            _random = new Random();
        }

        public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));

            int retryAttempt = 0;

            while (true)
            {
                try
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Виконання спроби #{retryAttempt + 1}...");
                    T result = operation();
                    if (retryAttempt > 0)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Операція успішно завершилася на спробі #{retryAttempt + 1}.");
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    bool canRetry = retryAttempt < _maxRetries && (shouldRetry == null || shouldRetry(ex));

                    if (!canRetry)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Відмова остаточна (спроба #{retryAttempt + 1}). Виняток: {ex.GetType().Name} - {ex.Message}");
                        throw;
                    }

                    double multiplier = Math.Pow(2, retryAttempt) * (1.0 + _random.NextDouble() * _jitterFactor * 2.0 - _jitterFactor);
                    TimeSpan delay = TimeSpan.FromMilliseconds(_initialDelay.TotalMilliseconds * multiplier);

                    retryAttempt++;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Помилка: {ex.Message}. Повтор #{retryAttempt}/{_maxRetries} через {delay.TotalMilliseconds:F0} мс...");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    internal class Program
    {
        private static int _transientAttempts = 0;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №15: Реалізація Exponential Backoff з Jitter ===\n");

            var retryPolicy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(300), jitterFactor: 0.25);

            Console.WriteLine("--- Сценарій 1: Тимчасовий збій мережі (успіх на 3-й спробі) ---");
            try
            {
                string data = retryPolicy.Execute(
                    operation: () => FetchRemoteConfig(),
                    shouldRetry: ex => ex is HttpRequestException || ex is TimeoutException
                );
                Console.WriteLine($"Отриманий результат: {data}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неочікувана помилка: {ex.Message}\n");
            }

            Console.WriteLine("--- Сценарій 2: Постійний збій сервера (вичерпання maxRetries = 3) ---");
            try
            {
                int balance = retryPolicy.Execute<int>(
                    operation: () => AlwaysFailingService(),
                    shouldRetry: ex => ex is IOException
                );
                Console.WriteLine($"Баланс: {balance}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Перехоплено фінальний виняток у Main: [{ex.GetType().Name}] {ex.Message}\n");
            }

            Console.WriteLine("--- Сценарій 3: Невиправний виняток (shouldRetry повертає false) ---");
            try
            {
                string parsed = retryPolicy.Execute<string>(
                    operation: () => throw new ArgumentException("Некоректний формат ідентифікатора клієнта"),
                    shouldRetry: ex => ex is HttpRequestException
                );
                Console.WriteLine(parsed);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Миттєве переривання без повторів: [{ex.GetType().Name}] {ex.Message}\n");
            }

            Console.WriteLine("=== Самостійну роботу №15 успішно виконано! ===");
        }

        static string FetchRemoteConfig()
        {
            _transientAttempts++;
            if (_transientAttempts <= 2)
            {
                throw new HttpRequestException($"Сервер конфігурацій тимчасово недоступний (спроба #{_transientAttempts})");
            }
            return "{ \"theme\": \"dark\", \"version\": \"7.0\" }";
        }

        static int AlwaysFailingService()
        {
            throw new IOException("Втрачено з'єднання зі сховищем даних");
        }
    }
}
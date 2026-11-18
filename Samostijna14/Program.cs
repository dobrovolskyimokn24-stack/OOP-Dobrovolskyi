using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace Samostijna14
{
    internal class Program
    {
        private static int _dbAttempts = 0;
        private static int _gatewayCalls = 0;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №14: Кейси Polly / Retry та відмовостійкість ===\n");

            RunDatabaseRetryScenario();
            RunPaymentCircuitBreakerScenario();
            RunReportTimeoutFallbackScenario();

            Console.WriteLine("=== Самостійну роботу №14 успішно виконано! ===");
        }

        static string QueryDatabase(string sql)
        {
            _dbAttempts++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Спроба підключення до БД #{_dbAttempts}: {sql}");
            if (_dbAttempts <= 2)
            {
                throw new TimeoutException("Тимчасовий таймаут пулу з'єднань SQL Server.");
            }
            return "Отримано 142 записи замовлень";
        }

        static void RunDatabaseRetryScenario()
        {
            Console.WriteLine("--- Сценарій 1: Доступ до БД (Exponential Backoff Retry + Jitter) ---");

            var dbPipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<TimeoutException>(),
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(500),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    OnRetry = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Polly Retry] Повтор #{args.AttemptNumber + 1} через {args.RetryDelay.TotalMilliseconds:F0} мс. Причина: {args.Outcome.Exception?.Message}");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            try
            {
                string result = dbPipeline.Execute(() => QueryDatabase("SELECT * FROM Orders WHERE Status = 'Active'"));
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Результат запиту: {result}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Помилка після всіх спроб: {ex.Message}\n");
            }
        }

        static string ProcessPayment(decimal amount)
        {
            _gatewayCalls++;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Виклик платіжного шлюзу #{_gatewayCalls} на суму {amount} грн...");
            if (_gatewayCalls <= 4)
            {
                throw new IOException("Платіжний шлюз перевантажений (503 Service Unavailable).");
            }
            return $"Платіж на {amount} грн успішно підтверджено";
        }

        static void RunPaymentCircuitBreakerScenario()
        {
            Console.WriteLine("--- Сценарій 2: Платіжний шлюз (Circuit Breaker) ---");

            var cbPipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder().Handle<IOException>(),
                    FailureRatio = 0.5,
                    MinimumThroughput = 3,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    BreakDuration = TimeSpan.FromSeconds(2),
                    OnOpened = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Polly CircuitBreaker] Ланцюг РОЗІМКНУТО (Open) на {args.BreakDuration.TotalSeconds} с через часті збої!");
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Polly CircuitBreaker] Ланцюг ЗАМКНУТО (Closed) — сервіс відновився!");
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Polly CircuitBreaker] Перехід у стан Half-Open — пробний запит...");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            for (int i = 1; i <= 5; i++)
            {
                try
                {
                    string response = cbPipeline.Execute(() => ProcessPayment(1500m));
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Успіх: {response}");
                }
                catch (BrokenCircuitException)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Запит #{i} миттєво відхилено запобіжником (BrokenCircuitException) без навантаження на шлюз.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Запит #{i} завершився збоєм: {ex.Message}");
                }

                if (i == 4)
                {
                    Thread.Sleep(2200);
                }
            }
            Console.WriteLine();
        }

        static string GenerateHeavyAnalyticsReport(CancellationToken token)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Початок генерації важкого аналітичного звіту...");
            for (int step = 1; step <= 5; step++)
            {
                token.ThrowIfCancellationRequested();
                Thread.Sleep(400);
            }
            return "Актуальний звіт сформовано";
        }

        static void RunReportTimeoutFallbackScenario()
        {
            Console.WriteLine("--- Сценарій 3: Генерація звіту (Timeout + Fallback на кеш) ---");

            var reportPipeline = new ResiliencePipelineBuilder<string>()
                .AddFallback(new FallbackStrategyOptions<string>
                {
                    ShouldHandle = new PredicateBuilder<string>().Handle<TimeoutRejectedException>(),
                    FallbackAction = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Polly Fallback] Повертаємо збережену кешовану копію звіту.");
                        return Outcome.FromResultAsValueTask("Кешований звіт за вчорашній день (Fallback)");
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromMilliseconds(900),
                    OnTimeout = args =>
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [Polly Timeout] Перевищено ліміт очікування ({args.Timeout.TotalMilliseconds} мс)!");
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();

            string report = reportPipeline.Execute(token => GenerateHeavyAnalyticsReport(token));
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Отриманий результат: {report}\n");
        }
    }
}
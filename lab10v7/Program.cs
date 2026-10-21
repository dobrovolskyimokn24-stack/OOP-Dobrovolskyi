using System;
using System.Collections.Generic;

namespace lab10v7
{
    public interface INotifier
    {
        void SendNotification(string message);
    }

    public class EmailNotifier : INotifier
    {
        public string EmailAddress { get; set; }

        public EmailNotifier(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public void SendNotification(string message)
        {
            Console.WriteLine($"[Email на {EmailAddress}]: {message}");
        }
    }

    public class SmsNotifier : INotifier
    {
        public string PhoneNumber { get; set; }

        public SmsNotifier(string phoneNumber)
        {
            PhoneNumber = phoneNumber;
        }

        public void SendNotification(string message)
        {
            Console.WriteLine($"[SMS на {PhoneNumber}]: {message}");
        }
    }

    public class PushNotifier : INotifier
    {
        public string DeviceId { get; set; }

        public PushNotifier(string deviceId)
        {
            DeviceId = deviceId;
        }

        public void SendNotification(string message)
        {
            Console.WriteLine($"[Push на пристрій {DeviceId}]: {message}");
        }
    }

    public abstract class Authenticator
    {
        public abstract bool Authenticate(string username, string password);

        public void LogAttempt(string username, bool success)
        {
            string status = success ? "УСПІШНО" : "НЕВДАЛО";
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Console.WriteLine($"[{time}] [АУДИТ] Спроба входу користувача '{username}': {status}");
        }
    }

    public class DatabaseAuthenticator : Authenticator
    {
        public string ConnectionString { get; set; }

        public DatabaseAuthenticator(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public override bool Authenticate(string username, string password)
        {
            Console.WriteLine($"[DB] Перевірка даних користувача '{username}' у БД...");
            bool isValid = username == "admin" && password == "admin123";
            LogAttempt(username, isValid);
            return isValid;
        }
    }

    public class LdapAuthenticator : Authenticator
    {
        public string Domain { get; set; }

        public LdapAuthenticator(string domain)
        {
            Domain = domain;
        }

        public override bool Authenticate(string username, string password)
        {
            Console.WriteLine($"[LDAP] Перевірка даних користувача '{username}' у домені '{Domain}'...");
            bool isValid = username == "john_doe" && password == "securePass";
            LogAttempt(username, isValid);
            return isValid;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("   Лабораторна робота №10 | Варіант 7");
            Console.WriteLine("   Тема: Абстрактні класи та інтерфейси");
            Console.WriteLine("==================================================\n");

            Console.WriteLine("--- 1. Демонстрація робіт з INotifier ---");
            List<INotifier> notifiers = new List<INotifier>
            {
                new EmailNotifier("student7@college.edu.ua"),
                new SmsNotifier("+380971234567"),
                new PushNotifier("DEV-ANDROID-9921")
            };

            string systemMessage = "Увага! Системне оновлення заплановано на 22:00.";
            foreach (var notifier in notifiers)
            {
                notifier.SendNotification(systemMessage);
            }

            Console.WriteLine();

            Console.WriteLine("--- 2. Демонстрація робіт з Authenticator ---");
            List<Authenticator> authenticators = new List<Authenticator>
            {
                new DatabaseAuthenticator("Server=myServerAddress;Database=myDataBase;"),
                new LdapAuthenticator("corp.domain.com")
            };

            Console.WriteLine("\n--> Сценарій 1: Тест авторизації через БД");
            authenticators[0].Authenticate("admin", "admin123");

            Console.WriteLine("\n--> Сценарій 2: Тест авторизації через LDAP");
            authenticators[1].Authenticate("john_doe", "wrong_password");

            Console.WriteLine("\n==================================================");
            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}
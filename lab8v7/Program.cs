using System;
using System.Collections.Generic;

namespace lab8v7
{
    public class Notification
    {
        public string Message { get; set; }

        public Notification(string message)
        {
            Message = message;
        }

        public virtual void Send()
        {
            Console.WriteLine($"Сповіщення: {Message}");
        }
    }

    public class EmailNotification : Notification
    {
        public string RecipientEmail { get; set; }

        public EmailNotification(string message, string recipientEmail) : base(message)
        {
            RecipientEmail = recipientEmail;
        }

        public override void Send()
        {
            Console.WriteLine($"[Email на {RecipientEmail}]: {Message}");
        }
    }

    public class SMSNotification : Notification
    {
        public string PhoneNumber { get; set; }

        public SMSNotification(string message, string phoneNumber) : base(message)
        {
            PhoneNumber = phoneNumber;
        }

        public override void Send()
        {
            Console.WriteLine($"[SMS на {PhoneNumber}]: {Message}");
        }
    }

    public class PushNotification : Notification
    {
        public string DeviceId { get; set; }

        public PushNotification(string message, string deviceId) : base(message)
        {
            DeviceId = deviceId;
        }

        public override void Send()
        {
            Console.WriteLine($"[Push на пристрій {DeviceId}]: {Message}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Notification> notifications = new List<Notification>
            {
                new EmailNotification("Ваш рахунок сформовано", "user@example.com"),
                new SMSNotification("Код підтвердження: 4829", "+380971234567"),
                new PushNotification("Нове повідомлення в чаті", "DEV-9982-X")
            };

            Console.WriteLine("--- ВІДПРАВКА СПОВІЩЕНЬ ---");
            foreach (var notification in notifications)
            {
                notification.Send();
            }

            Console.WriteLine("\n--- АГРЕГАЦІЯ (Підсумок) ---");
            Console.WriteLine($"Загальна кількість відправлених повідомлень: {notifications.Count}");
        }
    }
}
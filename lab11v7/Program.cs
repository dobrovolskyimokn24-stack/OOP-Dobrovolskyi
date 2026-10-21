using System;
using System.Collections.Generic;

namespace lab11v7
{
    public abstract class EmailSender
    {
        public string SenderName { get; set; }
        public string LastStatus { get; protected set; }

        protected EmailSender(string senderName)
        {
            SenderName = senderName;
            LastStatus = "Not Started";
        }

        public void Send(string recipient, string subject, string body)
        {
            Console.WriteLine($"=== Початок відправки листа від {SenderName} ===");
            
            if (!ValidateRecipient(recipient))
            {
                Console.WriteLine("[ERROR] Некоректний отримувач. Відправку скасовано.\n");
                LastStatus = "Failed: Invalid Recipient";
                return;
            }

            ComposeMessage(subject, body);
            AttachFiles();
            SendViaServer(recipient);
            LogResult();

            LastStatus = "Success";
            Console.WriteLine("=== Відправку успішно завершено ===\n");
        }

        protected virtual bool ValidateRecipient(string recipient)
        {
            Console.WriteLine($"[Крок 1: ValidateRecipient] Перевірка адреси '{recipient}'...");
            return !string.IsNullOrWhiteSpace(recipient) && recipient.Contains("@");
        }

        protected virtual void ComposeMessage(string subject, string body)
        {
            Console.WriteLine($"[Крок 2: ComposeMessage] Тема: \"{subject}\", Вміст: \"{body}\"");
        }

        protected abstract void AttachFiles();

        protected abstract void SendViaServer(string recipient);

        protected virtual void LogResult()
        {
            Console.WriteLine($"[Крок 5: LogResult] Запис у лог: Лист від {SenderName} успішно відправлено о {DateTime.Now:HH:mm:ss}");
        }
    }

    public class GMailSender : EmailSender
    {
        public string OAuthToken { get; set; }

        public GMailSender(string senderName, string oauthToken) : base(senderName)
        {
            OAuthToken = oauthToken;
        }

        protected override void AttachFiles()
        {
            Console.WriteLine("[Крок 3: AttachFiles] [GMail] Прикріплення файлів з Google Drive...");
        }

        protected override void SendViaServer(string recipient)
        {
            Console.WriteLine($"[Крок 4: SendViaServer] [GMail API] Авторизація за токеном '{OAuthToken}' та відправка на {recipient}");
        }
    }

    public class OutlookSender : EmailSender
    {
        public bool UseExchangeServer { get; set; }

        public OutlookSender(string senderName, bool useExchangeServer) : base(senderName)
        {
            UseExchangeServer = useExchangeServer;
        }

        protected override void AttachFiles()
        {
            Console.WriteLine("[Крок 3: AttachFiles] [Outlook] Прикріплення вкладень через OneDrive/Exchange...");
        }

        protected override void SendViaServer(string recipient)
        {
            string serverType = UseExchangeServer ? "Microsoft Exchange Server" : "Outlook Cloud";
            Console.WriteLine($"[Крок 4: SendViaServer] [Outlook] Відправка через {serverType} на {recipient}");
        }

        protected override void LogResult()
        {
            Console.WriteLine($"[Крок 5: LogResult] [Outlook Audit] Збережено copy у папку 'Sent Items'. Час: {DateTime.Now:HH:mm:ss}");
        }
    }

    public class CustomSMTPSender : EmailSender
    {
        public string SmtpHost { get; set; }
        public int Port { get; set; }

        public CustomSMTPSender(string senderName, string smtpHost, int port) : base(senderName)
        {
            SmtpHost = smtpHost;
            Port = port;
        }

        protected override void AttachFiles()
        {
            Console.WriteLine("[Крок 3: AttachFiles] [Custom SMTP] Кодування локальних файлів у Base64...");
        }

        protected override void SendViaServer(string recipient)
        {
            Console.WriteLine($"[Крок 4: SendViaServer] [SMTP] З'єднання з {SmtpHost}:{Port} через TLS та надсилання на {recipient}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("==================================================");
            Console.WriteLine("   Лабораторна робота №11 | Варіант 7");
            Console.WriteLine("   Тема: Шаблонний метод (Template Method)");
            Console.WriteLine("==================================================\n");

            List<EmailSender> senders = new List<EmailSender>
            {
                new GMailSender("GMail Client", "ya29.a0ARR43X..."),
                new OutlookSender("Corporate Outlook", true),
                new CustomSMTPSender("Local Mailer", "mail.company.com", 587)
            };

            foreach (var sender in senders)
            {
                sender.Send("student7@college.edu.ua", "Звіт з ООП", "Лабораторна робота №11 виконана.");
            }

            Console.WriteLine("==================================================");
            Console.WriteLine("Натисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }
    }
}
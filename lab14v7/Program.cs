using System;
using System.Collections.Generic;

namespace lab14v7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №14 | Варіант 7: Система підписок ===\n");

            // 1. HASHSET: Унікальні підписники (Email)
            Console.WriteLine("--- 1. Демонстрація HashSet<T> (Унікальні підписники) ---");
            HashSet<string> subscribers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            Console.WriteLine($"Додаємо user1@gmail.com: {subscribers.Add("user1@gmail.com")}");
            Console.WriteLine($"Додаємо user2@outlook.com: {subscribers.Add("user2@outlook.com")}");
            Console.WriteLine($"Додаємо user1@gmail.com (дублікат): {subscribers.Add("user1@gmail.com")}");

            string searchEmail = "user2@outlook.com";
            Console.WriteLine($"Чи є в базі {searchEmail}? {subscribers.Contains(searchEmail)}");

            subscribers.Remove("user2@outlook.com");
            Console.WriteLine("Кількість підписників після видалення: " + subscribers.Count);

            HashSet<string> newSubscribers = new HashSet<string> { "user3@yahoo.com", "user1@gmail.com", "user4@ukr.net" };
            subscribers.UnionWith(newSubscribers);

            Console.WriteLine("\nАктуальний список унікальних підписників:");
            foreach (var email in subscribers)
            {
                Console.WriteLine($" - {email}");
            }
            Console.WriteLine();

            // 2. QUEUE: Черга email для розсилки (FIFO)
            Console.WriteLine("--- 2. Демонстрація Queue<T> (Черга розсилки - FIFO) ---");
            Queue<string> emailQueue = new Queue<string>();

            foreach (var email in subscribers)
            {
                emailQueue.Enqueue(email);
                Console.WriteLine($"Додано в чергу: {email}");
            }

            Console.WriteLine($"\n[Peek] Наступний у черзі: {emailQueue.Peek()}");

            Console.WriteLine("\n[Dequeue] Обробка черги розсилки:");
            while (emailQueue.Count > 0)
            {
                string currentEmail = emailQueue.Dequeue();
                Console.WriteLine($" -> Надіслано на {currentEmail} (Залишилось: {emailQueue.Count})");
            }
            Console.WriteLine();

            // 3. STACK: Історія розсилок (LIFO)
            Console.WriteLine("--- 3. Демонстрація Stack<T> (Історія розсилок - LIFO) ---");
            Stack<string> dispatchHistory = new Stack<string>();

            dispatchHistory.Push("Розсилка #1: Дайджест новин");
            dispatchHistory.Push("Розсилка #2: Акційні пропозиції");
            dispatchHistory.Push("Розсилка #3: Персональні знижки");

            Console.WriteLine($"[Peek] Остання проведена розсилка: {dispatchHistory.Peek()}");

            Console.WriteLine("\n[Pop] Скасування останніх розсилок (Undo):");
            while (dispatchHistory.Count > 0)
            {
                string lastDispatch = dispatchHistory.Pop();
                Console.WriteLine($" <- Вилучено з історії: {lastDispatch}");
            }

            Console.WriteLine("\n=== Виконано! ===");
        }
    }
}
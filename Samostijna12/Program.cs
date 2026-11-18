using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Samostijna12
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CustomerId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №12: LINQ pipelines: ETL та аналітика ===\n");

            var (products, customers, sales) = GenerateTestData(productCount: 60, customerCount: 80, saleCount: 1000);
            Console.WriteLine($"Згенеровано: {products.Count} товарів, {customers.Count} клієнтів, {sales.Count} продажів.\n");

            TopProductsByCategory(products, sales);
            TotalRevenueByCity(products, customers, sales);
            SalesByYearAndMonth(products, sales);
            VipCustomersAnalytics(products, customers, sales);

            Console.WriteLine("=== Самостійну роботу №12 успішно виконано! ===");
        }

        static (List<Product>, List<Customer>, List<Sale>) GenerateTestData(int productCount, int customerCount, int saleCount)
        {
            var random = new Random(42);
            string[] categories = { "Ноутбуки", "Смартфони", "Периферія", "Монітори", "Комплектуючі" };
            string[] cities = { "Рівне", "Київ", "Львів", "Одеса", "Дніпро", "Харків" };

            var products = Enumerable.Range(1, productCount)
                .Select(i => new Product
                {
                    Id = i,
                    Name = $"Товар #{i:D2}",
                    Category = categories[random.Next(categories.Length)],
                    UnitPrice = Math.Round((decimal)(random.Next(15, 450) * 100), 2)
                })
                .ToList();

            var customers = Enumerable.Range(1, customerCount)
                .Select(i => new Customer
                {
                    Id = i,
                    Name = $"Клієнт #{i:D2}",
                    City = cities[random.Next(cities.Length)]
                })
                .ToList();

            var startDate = new DateTime(2025, 1, 1);
            var sales = Enumerable.Range(1, saleCount)
                .Select(i => new Sale
                {
                    Id = i,
                    ProductId = random.Next(1, productCount + 1),
                    CustomerId = random.Next(1, customerCount + 1),
                    Quantity = random.Next(1, 10),
                    SaleDate = startDate.AddDays(random.Next(0, 680))
                })
                .ToList();

            return (products, customers, sales);
        }

        static void TopProductsByCategory(List<Product> products, List<Sale> sales)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(s => s.Quantity)
                })
                .Join(
                    products,
                    stat => stat.ProductId,
                    p => p.Id,
                    (stat, p) => new
                    {
                        p.Name,
                        p.Category,
                        stat.TotalQuantity
                    })
                .GroupBy(x => x.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Top3 = g.OrderByDescending(x => x.TotalQuantity).Take(3).ToList()
                })
                .OrderBy(x => x.Category)
                .ToList();

            sw.Stop();

            Console.WriteLine($"--- Пайплайн 1: Топ-3 найпопулярніші продукти за категоріями (Час: {sw.Elapsed.TotalMilliseconds:F3} мс / {sw.ElapsedTicks} ticks) ---");
            foreach (var group in result)
            {
                Console.WriteLine($"Категорія: {group.Category}");
                foreach (var item in group.Top3)
                {
                    Console.WriteLine($"   - {item.Name}: {item.TotalQuantity} од.");
                }
            }
            Console.WriteLine();
        }

        static void TotalRevenueByCity(List<Product> products, List<Customer> customers, List<Sale> sales)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(
                    products,
                    s => s.ProductId,
                    p => p.Id,
                    (s, p) => new
                    {
                        s.CustomerId,
                        Revenue = s.Quantity * p.UnitPrice
                    })
                .Join(
                    customers,
                    sp => sp.CustomerId,
                    c => c.Id,
                    (sp, c) => new
                    {
                        c.City,
                        sp.Revenue
                    })
                .GroupBy(x => x.City)
                .Select(g => new
                {
                    City = g.Key,
                    TotalRevenue = g.Sum(x => x.Revenue),
                    OrdersCount = g.Count()
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToList();

            sw.Stop();

            Console.WriteLine($"--- Пайплайн 2: Загальний дохід за містами клієнтів (Час: {sw.Elapsed.TotalMilliseconds:F3} мс / {sw.ElapsedTicks} ticks) ---");
            foreach (var item in result)
            {
                Console.WriteLine($"   - {item.City,-10}: {item.TotalRevenue,12:N2} грн (замовлень: {item.OrdersCount})");
            }
            Console.WriteLine();
        }

        static void SalesByYearAndMonth(List<Product> products, List<Sale> sales)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Join(
                    products,
                    s => s.ProductId,
                    p => p.Id,
                    (s, p) => new
                    {
                        s.SaleDate.Year,
                        s.SaleDate.Month,
                        s.Quantity,
                        Revenue = s.Quantity * p.UnitPrice
                    })
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalUnits = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            sw.Stop();

            Console.WriteLine($"--- Пайплайн 3: Продажі за роками та місяцями (Час: {sw.Elapsed.TotalMilliseconds:F3} мс / {sw.ElapsedTicks} ticks) ---");
            foreach (var item in result.Take(10))
            {
                Console.WriteLine($"   - {item.Year}-{item.Month:D2}: продано {item.TotalUnits,4} од. | дохід: {item.TotalRevenue,12:N2} грн");
            }
            if (result.Count > 10)
            {
                Console.WriteLine($"   ... (показано перші 10 із {result.Count} місяців)");
            }
            Console.WriteLine();
        }

        static void VipCustomersAnalytics(List<Product> products, List<Customer> customers, List<Sale> sales)
        {
            var sw = Stopwatch.StartNew();

            var result = sales
                .Where(s => s.SaleDate.Year == 2026)
                .Join(
                    products,
                    s => s.ProductId,
                    p => p.Id,
                    (s, p) => new
                    {
                        s.CustomerId,
                        Amount = s.Quantity * p.UnitPrice,
                        p.Category
                    })
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    CustomerId = g.Key,
                    TotalSpent = g.Sum(x => x.Amount),
                    DistinctCategories = g.Select(x => x.Category).Distinct().Count()
                })
                .Where(x => x.TotalSpent > 250000m)
                .Join(
                    customers,
                    stat => stat.CustomerId,
                    c => c.Id,
                    (stat, c) => new
                    {
                        c.Name,
                        c.City,
                        stat.TotalSpent,
                        stat.DistinctCategories
                    })
                .OrderByDescending(x => x.TotalSpent)
                .Take(5)
                .ToList();

            sw.Stop();

            Console.WriteLine($"--- Пайплайн 4: Топ-5 VIP-клієнтів за 2026 рік з витратами > 250 000 грн (Час: {sw.Elapsed.TotalMilliseconds:F3} мс / {sw.ElapsedTicks} ticks) ---");
            foreach (var vip in result)
            {
                Console.WriteLine($"   - {vip.Name} ({vip.City}): {vip.TotalSpent,12:N2} грн | категорій: {vip.DistinctCategories}");
            }
            Console.WriteLine();
        }
    }
}
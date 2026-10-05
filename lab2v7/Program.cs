using System;

namespace lab2v7
{
    // Клас, що описує мобільний телефон
    public class Phone
    {
        // Приватні поля
        private string _brand;
        private string _model;
        private int _batteryLevel;

        // Властивості з валідацією
        public string Brand
        {
            get => _brand;
            set => _brand = string.IsNullOrWhiteSpace(value) ? "Generic" : value;
        }

        public string Model
        {
            get => _model;
            set => _model = string.IsNullOrWhiteSpace(value) ? "Basic" : value;
        }

        public int BatteryLevel
        {
            get => _batteryLevel;
            set
            {
                // Валідація: рівень батареї від 0 до 100
                if (value < 0)
                    _batteryLevel = 0;
                else if (value > 100)
                    _batteryLevel = 100;
                else
                    _batteryLevel = value;
            }
        }

        // 1. Повний (параметризований) конструктор
        public Phone(string brand, string model, int batteryLevel)
        {
            Brand = brand;
            Model = model;
            BatteryLevel = batteryLevel;
            Console.WriteLine($"[Конструктор] Створено об'єкт Phone: {Brand} {Model} (Заряд: {BatteryLevel}%)");
        }

        // 2. Конструктор за замовчуванням (викликає перший через : this())
        public Phone() : this("Generic", "Basic", 50)
        {
            // Усі значення за замовчуванням передаються у головний конструктор
        }

        // Додатковий зручний конструктор (перевантаження)
        public Phone(string brand, string model) : this(brand, model, 100)
        {
            // Ініціалізує телефон із 100% заряду
        }

        // Метод класу: виклики та зменшення заряду батареї
        public void Call(string number)
        {
            if (BatteryLevel < 5)
            {
                Console.WriteLine($"[{Brand} {Model}] Неможливо здійснити виклик на {number}: занадто низький заряд ({BatteryLevel}%).");
                return;
            }

            BatteryLevel -= 5; // Зменшуємо заряд на 5% при здійсненні виклику
            Console.WriteLine($"[{Brand} {Model}] Дзвінок на номер {number}... Залишок батареї: {BatteryLevel}%");
        }

        // Деструктор (Фіналізатор)
        ~Phone()
        {
            Console.WriteLine($"[Деструктор] Об'єкт Phone ({Brand} {Model}) знищено з пам'яті.");
        }
    }

    internal class Program
    {
        private static void CreateAndUseObjects()
        {
            Console.WriteLine("--- Створення об'єктів ---");

            // 1. Об'єкт через конструктор за замовчуванням
            Phone phone1 = new Phone();
            phone1.Call("+380670000001");

            // 2. Об'єкт через параметризований конструктор
            Phone phone2 = new Phone("Samsung", "Galaxy S23", 85);
            phone2.Call("+380501234567");

            // 3. Об'єкт через додатковий перевантажений конструктор
            Phone phone3 = new Phone("Apple", "iPhone 15");
            phone3.Call("+380939876543");

            Console.WriteLine("--- Завершення роботи з об'єктами ---");
            // Після виходу з методу CreateAndUseObjects посилання phone1, phone2, phone3 зникають із стеку,
            // і об'єкти стають доступними для Garbage Collector.
        }

        static void Main(string[] args)
        {
            Console.WriteLine(" Creating objects ");
            
            // Викликаємо окремий метод, щоб посилання на об'єкти точно вийшли з зони видимості (Scope)
            CreateAndUseObjects();

            Console.WriteLine(" Objects created ");
            Console.WriteLine(" End of Main, preparing for GC ");

            // Примусовий виклик збирача сміття для демонстрації деструктора
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine(" Натисніть к his key для завершення програми...");
        }
    }
}

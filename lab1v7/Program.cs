using System;

class Phone
{
    private string brand;
    private string model;

    public int BatteryLevel { get; set; }

    public Phone(string brand, string model, int batteryLevel)
    {
        this.brand = brand;
        this.model = model;
        BatteryLevel = batteryLevel;
    }

    public void Call(string number)
    {
        if (BatteryLevel > 0)
        {
            Console.WriteLine($"Телефон {brand} {model} телефонує на номер {number}.");
            BatteryLevel -= 10;

            if (BatteryLevel < 0)
                BatteryLevel = 0;

            Console.WriteLine($"Рівень заряду батареї: {BatteryLevel}%");
        }
        else
        {
            Console.WriteLine($"Телефон {brand} {model} не може здійснити дзвінок: батарея розряджена.");
        }
    }

    ~Phone()
    {
        Console.WriteLine($"Об'єкт телефону {brand} {model} знищено.");
    }
}

class Program
{
    static void Main()
    {
        Phone phone1 = new Phone("Samsung", "Galaxy S24", 100);
        Phone phone2 = new Phone("Apple", "iPhone 15", 70);
        Phone phone3 = new Phone("Xiaomi", "Redmi Note 13", 40);

        Console.WriteLine("=== Лабораторна робота №1 ===");
        Console.WriteLine("Варіант 7 — клас Phone");
        Console.WriteLine();

        Console.WriteLine("Телефон 1:");
        Console.WriteLine($"Рівень батареї: {phone1.BatteryLevel}%");
        phone1.Call("+380501234567");
        Console.WriteLine();

        Console.WriteLine("Телефон 2:");
        Console.WriteLine($"Рівень батареї: {phone2.BatteryLevel}%");
        phone2.Call("+380671234567");
        Console.WriteLine();

        Console.WriteLine("Телефон 3:");
        Console.WriteLine($"Рівень батареї: {phone3.BatteryLevel}%");
        phone3.Call("+380931234567");
    }
}
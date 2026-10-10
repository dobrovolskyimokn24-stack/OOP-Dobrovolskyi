using System;
using System.Collections.Generic;

namespace lab7v7
{
    public interface IInfo
    {
        void Show();
    }

    public abstract class Transport : IInfo
    {
        public string Model { get; set; }
        public double Speed { get; set; }

        public Transport(string model, double speed)
        {
            Model = model;
            Speed = speed;
        }

        public abstract double CalculateTime(double distance);

        public virtual void Show()
        {
            Console.WriteLine($"Транспорт: {Model}, Швидкість: {Speed} км/год");
        }
    }

    public class Car : Transport
    {
        public int FuelConsumption { get; set; }

        public Car(string model, double speed, int fuel) : base(model, speed)
        {
            FuelConsumption = fuel;
        }

        public override double CalculateTime(double distance)
        {
            return distance / Speed;
        }

        public override void Show()
        {
            base.Show();
            Console.WriteLine($"Витрата пального: {FuelConsumption} л/100км");
        }
    }

    public class Bicycle : Transport
    {
        public int Gears { get; set; }

        public Bicycle(string model, double speed, int gears) : base(model, speed)
        {
            Gears = gears;
        }

        public override double CalculateTime(double distance)
        {
            return distance / Speed;
        }

        public override void Show()
        {
            base.Show();
            Console.WriteLine($"Кількість передач: {Gears}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Transport> list = new List<Transport>
            {
                new Car("Audi", 120, 8),
                new Bicycle("Trek", 25, 21)
            };

            foreach (var item in list)
            {
                item.Show();
                Console.WriteLine($"Час на 100 км: {item.CalculateTime(100):F1} год.");
                Console.WriteLine();
            }
        }
    }
}

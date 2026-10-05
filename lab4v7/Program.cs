using System;

namespace OOP_Lab4
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ КЛАСУ ComplexNumber ===\n");

            ComplexNumber c1 = new ComplexNumber(3, 4);
            ComplexNumber c2 = new ComplexNumber(1, -2);
            ComplexNumber i = ComplexNumber.I;

            Console.WriteLine($"c1 = {c1}");
            Console.WriteLine($"c2 = {c2}");
            Console.WriteLine($"Уявна одиниця (I) = {i}\n");

            Console.WriteLine("--- Робота з індексатором ---");
            Console.WriteLine($"c1[0] (Real) = {c1[0]}");
            Console.WriteLine($"c1[1] (Imaginary) = {c1[1]}");

            c1[0] = 5;
            Console.WriteLine($"Після c1[0] = 5: c1 = {c1}\n");

            Console.WriteLine("--- Арифметичні операції ---");
            ComplexNumber sum = c1 + c2;
            ComplexNumber product = c1 * c2;

            Console.WriteLine($"({c1}) + ({c2}) = {sum}");
            Console.WriteLine($"({c1}) * ({c2}) = {product}\n");

            Console.WriteLine("--- Порівняння об'єктів ---");
            ComplexNumber c3 = new ComplexNumber(5, 4);

            Console.WriteLine($"c1: {c1}");
            Console.WriteLine($"c3: {c3}");
            Console.WriteLine($"c1 == c3: {c1 == c3}");
            Console.WriteLine($"c1 != c2: {c1 != c2}");
            Console.WriteLine($"c1.Equals(c3): {c1.Equals(c3)}");
        }
    }
}
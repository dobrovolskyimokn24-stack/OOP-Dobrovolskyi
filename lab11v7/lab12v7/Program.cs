using System;
using System.Collections.Generic;
using System.Linq;

namespace lab12v7
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Department : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public interface IRepository<T> where T : class, IEntity
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly List<T> _items = new();

        public T? GetById(int id)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<T> GetAll()
        {
            return _items.ToList();
        }

        public void Add(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_items.Any(x => x.Id == item.Id))
                throw new InvalidOperationException($"Елемент з ID {item.Id} вже існує.");

            _items.Add(item);
        }

        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var index = _items.FindIndex(x => x.Id == item.Id);
            if (index == -1)
                throw new KeyNotFoundException($"Елемент з ID {item.Id} не знайдено.");

            _items[index] = item;
        }

        public void Delete(int id)
        {
            var item = GetById(id);
            if (item == null)
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено.");

            _items.Remove(item);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== ДЕМОНСТРАЦІЯ LAB12v7 (IRepository & Department) ===");

            IRepository<Department> repository = new InMemoryRepository<Department>();

            repository.Add(new Department { Id = 1, Name = "IT", Location = "Building A" });
            repository.Add(new Department { Id = 2, Name = "HR", Location = "Building B" });

            Console.WriteLine("\nСписок усіх відділів:");
            foreach (var dept in repository.GetAll())
            {
                Console.WriteLine($"ID: {dept.Id} | Name: {dept.Name} | Location: {dept.Location}");
            }

            var hr = repository.GetById(2);
            if (hr != null)
            {
                hr.Location = "Building C";
                repository.Update(hr);
            }

            Console.WriteLine("\nПісля оновлення локації HR:");
            foreach (var dept in repository.GetAll())
            {
                Console.WriteLine($"ID: {dept.Id} | Name: {dept.Name} | Location: {dept.Location}");
            }

            repository.Delete(1);
            Console.WriteLine("\nПісля видалення IT відділу (ID 1):");
            foreach (var dept in repository.GetAll())
            {
                Console.WriteLine($"ID: {dept.Id} | Name: {dept.Name} | Location: {dept.Location}");
            }
        }
    }
}
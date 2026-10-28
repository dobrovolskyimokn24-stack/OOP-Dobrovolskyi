using System;
using System.Collections.Generic;
using System.Linq;
using lab12v7;
using Xunit;

namespace lab12v7.Tests
{
    public class InMemoryRepositoryTests
    {
        private readonly InMemoryRepository<Department> _repository;

        public InMemoryRepositoryTests()
        {
            _repository = new InMemoryRepository<Department>();
        }

        [Fact]
        public void Add_ShouldAddItemToRepository()
        {
            var dept = new Department { Id = 1, Name = "IT", Location = "Building A" };

            _repository.Add(dept);

            var result = _repository.GetById(1);
            Assert.NotNull(result);
            Assert.Equal("IT", result.Name);
        }

        [Fact]
        public void Add_DuplicateId_ShouldThrowInvalidOperationException()
        {
            var dept1 = new Department { Id = 1, Name = "IT", Location = "Building A" };
            var dept2 = new Department { Id = 1, Name = "HR", Location = "Building B" };

            _repository.Add(dept1);

            Assert.Throws<InvalidOperationException>(() => _repository.Add(dept2));
        }

        [Fact]
        public void GetById_ExistingId_ShouldReturnItem()
        {
            var dept = new Department { Id = 5, Name = "Finance", Location = "Floor 2" };
            _repository.Add(dept);

            var result = _repository.GetById(5);

            Assert.NotNull(result);
            Assert.Equal("Finance", result.Name);
        }

        [Fact]
        public void GetById_NonExistingId_ShouldReturnNull()
        {
            var result = _repository.GetById(999);

            Assert.Null(result);
        }

        [Fact]
        public void GetAll_ShouldReturnAllItems()
        {
            _repository.Add(new Department { Id = 1, Name = "IT", Location = "A" });
            _repository.Add(new Department { Id = 2, Name = "HR", Location = "B" });

            var items = _repository.GetAll();

            Assert.Equal(2, items.Count());
        }

        [Fact]
        public void Update_ExistingItem_ShouldUpdateData()
        {
            var dept = new Department { Id = 1, Name = "Sales", Location = "Old Location" };
            _repository.Add(dept);

            dept.Location = "New Location";
            _repository.Update(dept);

            var updated = _repository.GetById(1);
            Assert.NotNull(updated);
            Assert.Equal("New Location", updated.Location);
        }

        [Fact]
        public void Update_NonExistingItem_ShouldThrowKeyNotFoundException()
        {
            var dept = new Department { Id = 100, Name = "Unknown", Location = "None" };

            Assert.Throws<KeyNotFoundException>(() => _repository.Update(dept));
        }

        [Fact]
        public void Delete_ExistingItem_ShouldRemoveItem()
        {
            var dept = new Department { Id = 1, Name = "Marketing", Location = "Main Office" };
            _repository.Add(dept);

            _repository.Delete(1);

            Assert.Null(_repository.GetById(1));
        }

        [Fact]
        public void Delete_NonExistingItem_ShouldThrowKeyNotFoundException()
        {
            Assert.Throws<KeyNotFoundException>(() => _repository.Delete(404));
        }
    }
}
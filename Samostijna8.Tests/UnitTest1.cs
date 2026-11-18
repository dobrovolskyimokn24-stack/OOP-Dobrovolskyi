using System;
using System.Collections.Generic;
using Samostijna8;
using Xunit;

namespace Samostijna8.Tests
{
    public class CacheTests
    {
        [Fact]
        public void Add_And_Get_ShouldStoreAndRetrieveItem()
        {
            var cache = new Cache<string, int>(3);
            cache.Add("one", 1);

            Assert.Equal(1, cache.Get("one"));
        }

        [Fact]
        public void Count_ShouldReturnCurrentNumberOfItems()
        {
            var cache = new Cache<int, string>(5);
            Assert.Equal(0, cache.Count);

            cache.Add(1, "A");
            cache.Add(2, "B");

            Assert.Equal(2, cache.Count);
        }

        [Fact]
        public void ContainsKey_ShouldReturnTrueForExistingAndFalseForMissing()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("k1", "v1");

            Assert.True(cache.ContainsKey("k1"));
            Assert.False(cache.ContainsKey("k2"));
        }

        [Fact]
        public void Get_NonExistentKey_ShouldThrowKeyNotFoundException()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("exists", "value");

            Assert.Throws<KeyNotFoundException>(() => cache.Get("missing"));
        }

        [Fact]
        public void Add_ExceedingCapacity_ShouldEvictOldestItemFifo()
        {
            var cache = new Cache<int, string>(2);
            cache.Add(1, "First");
            cache.Add(2, "Second");
            cache.Add(3, "Third");

            Assert.Equal(2, cache.Count);
            Assert.False(cache.ContainsKey(1));
            Assert.True(cache.ContainsKey(2));
            Assert.True(cache.ContainsKey(3));
            Assert.Throws<KeyNotFoundException>(() => cache.Get(1));
            Assert.Equal("Second", cache.Get(2));
            Assert.Equal("Third", cache.Get(3));
        }

        [Fact]
        public void Add_ExistingKey_ShouldUpdateValueWithoutEvicting()
        {
            var cache = new Cache<string, string>(2);
            cache.Add("a", "old");
            cache.Add("b", "second");
            cache.Add("a", "new");

            Assert.Equal(2, cache.Count);
            Assert.Equal("new", cache.Get("a"));
            Assert.True(cache.ContainsKey("b"));
        }

        [Fact]
        public void Constructor_ZeroOrNegativeCapacity_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => new Cache<int, int>(0));
            Assert.Throws<ArgumentException>(() => new Cache<int, int>(-5));
        }
    }
}
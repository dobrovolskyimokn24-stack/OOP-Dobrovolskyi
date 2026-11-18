using System;
using System.Collections.Generic;
using System.Linq;

namespace Samostijna9
{
    public class FileMetadata
    {
        public string FileName { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{FileName} ({Size} байт, змінено: {LastModified:yyyy-MM-dd HH:mm}, SHA: {Hash})";
        }
    }

    public class FileMetadataCache
    {
        private readonly Dictionary<string, FileMetadata> _cache = new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

        public void Add(string filePath, FileMetadata metadata)
        {
            _cache[filePath] = metadata;
        }

        public FileMetadata? Get(string filePath)
        {
            return _cache.TryGetValue(filePath, out var metadata) ? metadata : null;
        }

        public bool Remove(string filePath)
        {
            return _cache.Remove(filePath);
        }

        public bool Contains(string filePath)
        {
            return _cache.ContainsKey(filePath);
        }

        public int Count => _cache.Count;
    }

    public class TagManager
    {
        private readonly Dictionary<string, HashSet<string>> _articleTags = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        public void AddTag(string articleId, string tag)
        {
            if (!_articleTags.ContainsKey(articleId))
            {
                _articleTags[articleId] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            _articleTags[articleId].Add(tag);
        }

        public void RemoveTag(string articleId, string tag)
        {
            if (_articleTags.TryGetValue(articleId, out var tags))
            {
                tags.Remove(tag);
            }
        }

        public HashSet<string> GetAllUniqueTags()
        {
            var allTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var tags in _articleTags.Values)
            {
                allTags.UnionWith(tags);
            }
            return allTags;
        }

        public HashSet<string> FindCommonTags(string articleId1, string articleId2)
        {
            if (!_articleTags.TryGetValue(articleId1, out var tags1) ||
                !_articleTags.TryGetValue(articleId2, out var tags2))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            var common = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
            common.IntersectWith(tags2);
            return common;
        }

        public HashSet<string> FindExclusiveTags(string articleId1, string articleId2)
        {
            if (!_articleTags.TryGetValue(articleId1, out var tags1))
            {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            var exclusive = new HashSet<string>(tags1, StringComparer.OrdinalIgnoreCase);
            if (_articleTags.TryGetValue(articleId2, out var tags2))
            {
                exclusive.ExceptWith(tags2);
            }
            return exclusive;
        }
    }

    public class SearchIndex
    {
        private readonly Dictionary<string, HashSet<int>> _invertedIndex = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

        public void AddDocument(int docId, string content)
        {
            var words = content.Split(new[] { ' ', ',', '.', '!', '?', ';', ':', '-', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (!_invertedIndex.ContainsKey(word))
                {
                    _invertedIndex[word] = new HashSet<int>();
                }
                _invertedIndex[word].Add(docId);
            }
        }

        public HashSet<int> Search(string word)
        {
            if (_invertedIndex.TryGetValue(word, out var docIds))
            {
                return new HashSet<int>(docIds);
            }
            return new HashSet<int>();
        }

        public HashSet<int> SearchMultiple(params string[] words)
        {
            if (words == null || words.Length == 0)
            {
                return new HashSet<int>();
            }

            HashSet<int>? result = null;
            foreach (var word in words)
            {
                if (!_invertedIndex.TryGetValue(word, out var currentDocs))
                {
                    return new HashSet<int>();
                }

                if (result == null)
                {
                    result = new HashSet<int>(currentDocs);
                }
                else
                {
                    result.IntersectWith(currentDocs);
                }
            }

            return result ?? new HashSet<int>();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №9: Dictionary<TKey, TValue> та HashSet<T> ===\n");

            Console.WriteLine("--- Сценарій 1: FileMetadataCache (Dictionary) ---");
            var fileCache = new FileMetadataCache();
            fileCache.Add("C:/docs/report.pdf", new FileMetadata
            {
                FileName = "report.pdf",
                Size = 245760,
                LastModified = new DateTime(2026, 11, 18, 14, 30, 0),
                Hash = "A1B2C3D4"
            });
            fileCache.Add("C:/docs/notes.txt", new FileMetadata
            {
                FileName = "notes.txt",
                Size = 4096,
                LastModified = new DateTime(2026, 11, 18, 15, 10, 0),
                Hash = "E5F6G7H8"
            });

            Console.WriteLine($"Чи є 'C:/DOCS/REPORT.PDF' у кеші? {fileCache.Contains("C:/DOCS/REPORT.PDF")}");
            Console.WriteLine($"Отримано: {fileCache.Get("c:/docs/report.pdf")}");
            fileCache.Remove("C:/docs/notes.txt");
            Console.WriteLine($"Кількість файлів після видалення notes.txt: {fileCache.Count}\n");

            Console.WriteLine("--- Сценарій 2: TagManager (HashSet operations) ---");
            var tagManager = new TagManager();
            tagManager.AddTag("article-1", "CSharp");
            tagManager.AddTag("article-1", "LINQ");
            tagManager.AddTag("article-1", "Collections");
            tagManager.AddTag("article-1", "csharp");

            tagManager.AddTag("article-2", "CSharp");
            tagManager.AddTag("article-2", "Performance");
            tagManager.AddTag("article-2", "Collections");

            Console.WriteLine($"Усі унікальні теги (UnionWith): {string.Join(", ", tagManager.GetAllUniqueTags())}");
            Console.WriteLine($"Спільні теги article-1 та article-2 (IntersectWith): {string.Join(", ", tagManager.FindCommonTags("article-1", "article-2"))}");
            Console.WriteLine($"Унікальні теги тільки для article-1 (ExceptWith): {string.Join(", ", tagManager.FindExclusiveTags("article-1", "article-2"))}\n");

            Console.WriteLine("--- Сценарій 3: SearchIndex (Dictionary + HashSet) ---");
            var searchIndex = new SearchIndex();
            searchIndex.AddDocument(1, "C# та .NET надають швидкі колекції Dictionary і HashSet.");
            searchIndex.AddDocument(2, "Колекція Dictionary використовує хеш-таблицю для пошуку за ключем.");
            searchIndex.AddDocument(3, "HashSet зберігає лише унікальні елементи та підтримує операції множин.");

            Console.WriteLine($"Документи зі словом 'Dictionary': [{string.Join(", ", searchIndex.Search("Dictionary"))}]");
            Console.WriteLine($"Документи зі словом 'HashSet': [{string.Join(", ", searchIndex.Search("HashSet"))}]");
            Console.WriteLine($"Документи зі словами 'Dictionary' І 'HashSet': [{string.Join(", ", searchIndex.SearchMultiple("Dictionary", "HashSet"))}]\n");

            Console.WriteLine("=== Самостійну роботу №9 успішно виконано! ===");
        }
    }
}
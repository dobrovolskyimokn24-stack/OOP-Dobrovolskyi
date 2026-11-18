using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Samostijna11
{
    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public override string ToString() => $"[{Id}] {Title}";
    }

    public static class TextProcessor
    {
        private static readonly HashSet<string> StopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "a", "an", "the", "is", "are", "was", "were",
            "in", "on", "at", "to", "for", "of", "and", "or"
        };

        public static IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return Enumerable.Empty<string>();
            }

            return Regex.Matches(text.ToLowerInvariant(), @"[\w#]+")
                        .Select(m => m.Value);
        }

        public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
        {
            return words.Where(w => !StopWords.Contains(w));
        }
    }

    public class InvertedIndex
    {
        private readonly Dictionary<string, HashSet<int>> _index;
        private readonly Dictionary<int, Document> _documents;

        public InvertedIndex()
        {
            _index = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
            _documents = new Dictionary<int, Document>();
        }

        public int DocumentCount => _documents.Count;
        public int UniqueWords => _index.Keys.Count;

        public void AddDocument(Document doc)
        {
            _documents[doc.Id] = doc;
            var tokens = TextProcessor.RemoveStopWords(TextProcessor.Tokenize(doc.Content));

            foreach (var word in tokens)
            {
                if (!_index.TryGetValue(word, out var docIds))
                {
                    docIds = new HashSet<int>();
                    _index[word] = docIds;
                }
                docIds.Add(doc.Id);
            }
        }

        public List<Document> SearchSingleWord(string word)
        {
            var token = TextProcessor.Tokenize(word).FirstOrDefault();
            if (string.IsNullOrEmpty(token) || !_index.TryGetValue(token, out var docIds))
            {
                return new List<Document>();
            }

            return docIds.Select(id => _documents[id]).ToList();
        }

        public List<Document> SearchMultipleWords(params string[] words)
        {
            var tokens = TextProcessor.RemoveStopWords(
                words.SelectMany(w => TextProcessor.Tokenize(w))
            ).Distinct().ToList();

            if (tokens.Count == 0)
            {
                return new List<Document>();
            }

            HashSet<int>? resultIds = null;
            foreach (var token in tokens)
            {
                if (!_index.TryGetValue(token, out var currentIds))
                {
                    return new List<Document>();
                }

                if (resultIds == null)
                {
                    resultIds = new HashSet<int>(currentIds);
                }
                else
                {
                    resultIds.IntersectWith(currentIds);
                }
            }

            return resultIds?.Select(id => _documents[id]).ToList() ?? new List<Document>();
        }

        public List<Document> SearchAnyWord(params string[] words)
        {
            var tokens = TextProcessor.RemoveStopWords(
                words.SelectMany(w => TextProcessor.Tokenize(w))
            ).Distinct().ToList();

            var resultIds = new HashSet<int>();
            foreach (var token in tokens)
            {
                if (_index.TryGetValue(token, out var currentIds))
                {
                    resultIds.UnionWith(currentIds);
                }
            }

            return resultIds.Select(id => _documents[id]).ToList();
        }
    }

    public static class PerformanceTest
    {
        public static List<Document> GenerateDocuments(int count, List<Document> baseDocuments)
        {
            var result = new List<Document>(count);
            var random = new Random(42);
            string[] extraKeywords = { "cloud", "database", "algorithm", "compiler", "network", "security", "cache", "thread" };

            for (int i = 1; i <= count; i++)
            {
                var template = baseDocuments[(i - 1) % baseDocuments.Count];
                string extra = extraKeywords[random.Next(extraKeywords.Length)];
                result.Add(new Document
                {
                    Id = i,
                    Title = $"{template.Title} #{i}",
                    Content = $"{template.Content} Extra topic: {extra}."
                });
            }

            return result;
        }

        public static void RunBenchmarks(List<Document> baseDocuments)
        {
            int[] sizes = { 10, 100, 1000, 10000 };
            string searchWord = "programming";

            Console.WriteLine($"--- Вимірювання продуктивності пошуку слова '{searchWord}' ---");
            Console.WriteLine($"{"Документів",-12} | {"Linear (ticks)",-16} | {"Index (ticks)",-15} | {"Збігів (Lin/Idx)"}");
            Console.WriteLine(new string('-', 66));

            foreach (int size in sizes)
            {
                var docs = GenerateDocuments(size, baseDocuments);
                var normalizedWord = TextProcessor.Tokenize(searchWord).First();

                var index = new InvertedIndex();
                foreach (var doc in docs)
                {
                    index.AddDocument(doc);
                }

                var sw = Stopwatch.StartNew();
                var linearResults = docs.Where(d =>
                    TextProcessor.RemoveStopWords(TextProcessor.Tokenize(d.Content)).Contains(normalizedWord)).ToList();
                sw.Stop();
                long linearTicks = sw.ElapsedTicks;

                sw.Restart();
                var indexResults = index.SearchSingleWord(searchWord);
                sw.Stop();
                long indexTicks = sw.ElapsedTicks;

                Console.WriteLine($"{size,-12} | {linearTicks,-16} | {indexTicks,-15} | {linearResults.Count} / {indexResults.Count}");
            }
            Console.WriteLine();
        }
    }

    internal class Program
    {
        static List<Document> GetSampleDocuments()
        {
            return new List<Document>
            {
                new Document
                {
                    Id = 1,
                    Title = "Introduction to C#",
                    Content = "C# is a modern object-oriented programming language developed by Microsoft."
                },
                new Document
                {
                    Id = 2,
                    Title = "Java Basics",
                    Content = "Java is a popular programming language known for its portability and performance."
                },
                new Document
                {
                    Id = 3,
                    Title = "Python Overview",
                    Content = "Python is a high-level programming language with simple syntax."
                },
                new Document
                {
                    Id = 4,
                    Title = "C# Advanced Features",
                    Content = "C# supports LINQ, async/await, and many modern programming paradigms."
                },
                new Document
                {
                    Id = 5,
                    Title = "Comparing Languages",
                    Content = "C#, Java, and Python are among the most popular programming languages today."
                }
            };
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Самостійна робота №11: Інвертований індекс і вимірювання продуктивності ===\n");

            var sampleDocs = GetSampleDocuments();
            var index = new InvertedIndex();
            foreach (var doc in sampleDocs)
            {
                index.AddDocument(doc);
            }

            Console.WriteLine($"Завантажено документів: {index.DocumentCount}");
            Console.WriteLine($"Унікальних слів в індексі (без стоп-слів): {index.UniqueWords}\n");

            Console.WriteLine("--- Тест 1: Пошук одного слова ('python') ---");
            foreach (var doc in index.SearchSingleWord("python"))
            {
                Console.WriteLine($" {doc}: {doc.Content}");
            }

            Console.WriteLine("\n--- Тест 2: Пошук усіх слів AND ('c#', 'modern') ---");
            foreach (var doc in index.SearchMultipleWords("c#", "modern"))
            {
                Console.WriteLine($" {doc}: {doc.Content}");
            }

            Console.WriteLine("\n--- Тест 3: Пошук будь-якого слова OR ('linq', 'syntax') ---");
            foreach (var doc in index.SearchAnyWord("linq", "syntax"))
            {
                Console.WriteLine($" {doc}: {doc.Content}");
            }
            Console.WriteLine();

            PerformanceTest.RunBenchmarks(sampleDocs);

            Console.WriteLine("--- Інтерактивний пошук (введіть запит або 'exit' для завершення) ---");
            while (true)
            {
                Console.Write("Введіть пошуковий запит (або 'exit'): ");
                var query = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(query) || query.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                var words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var results = index.SearchMultipleWords(words);

                Console.WriteLine($"Знайдено документів (AND): {results.Count}");
                foreach (var doc in results)
                {
                    Console.WriteLine($" {doc} — {doc.Content}");
                }
                Console.WriteLine();
            }

            Console.WriteLine("\n=== Самостійну роботу №11 успішно виконано! ===");
        }
    }
}
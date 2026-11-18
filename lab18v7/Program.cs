using System;
using System.Collections.Generic;
using System.Linq;

namespace lab18v7
{
    public class ProjectNotFoundException : Exception
    {
        public int ProjectId { get; }

        public ProjectNotFoundException()
            : base("Проект не знайдено.")
        {
        }

        public ProjectNotFoundException(int projectId)
            : base($"Проект із ідентифікатором {projectId} не знайдено.")
        {
            ProjectId = projectId;
        }

        public ProjectNotFoundException(int projectId, string message)
            : base(message)
        {
            ProjectId = projectId;
        }

        public ProjectNotFoundException(int projectId, string message, Exception innerException)
            : base(message, innerException)
        {
            ProjectId = projectId;
        }
    }

    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        private Result(T value, bool isSuccess, string error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(default!, false, error);
        }

        public static Result<T> Failure(Exception ex)
        {
            return new Result<T>(default!, false, ex?.Message ?? "Невідома помилка");
        }
    }

    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<int> AssignedTaskIds { get; set; } = new List<int>();
    }

    public class ProjectService
    {
        private readonly List<Project> _projects = new List<Project>
        {
            new Project { Id = 1, Name = "Портал коледжу", AssignedTaskIds = new List<int> { 101, 102 } },
            new Project { Id = 2, Name = "Система обліку студентів", AssignedTaskIds = new List<int> { 201 } }
        };

        public Project GetProject(int projectId)
        {
            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
            {
                throw new ProjectNotFoundException(projectId);
            }

            return project;
        }

        public Result<bool> AssignTask(int projectId, int taskId)
        {
            var project = _projects.FirstOrDefault(p => p.Id == projectId);
            if (project == null)
            {
                return Result<bool>.Failure($"Неможливо призначити завдання: проект із ID={projectId} не існує.");
            }

            if (taskId <= 0)
            {
                return Result<bool>.Failure("Ідентифікатор завдання повинен бути додатнім числом.");
            }

            if (project.AssignedTaskIds.Contains(taskId))
            {
                return Result<bool>.Failure($"Завдання #{taskId} вже призначено до проекту '{project.Name}'.");
            }

            project.AssignedTaskIds.Add(taskId);
            return Result<bool>.Success(true);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Лабораторна робота №18 | Варіант 7: Управління проектами ===\n");

            var projectService = new ProjectService();

            Console.WriteLine("--- 1. Демонстрація GetProject (власний виняток ProjectNotFoundException) ---");
            try
            {
                var existingProject = projectService.GetProject(1);
                Console.WriteLine($"Знайдено проект: [{existingProject.Id}] {existingProject.Name}");

                Console.WriteLine("Спроба отримати неіснуючий проект (ID = 99)...");
                projectService.GetProject(99);
            }
            catch (ProjectNotFoundException ex)
            {
                Console.WriteLine($"Перехоплено виняток {ex.GetType().Name} (ProjectId = {ex.ProjectId}): {ex.Message}\n");
            }

            Console.WriteLine("--- 2. Демонстрація AssignTask (патерн Result<bool>) ---");
            var assignSuccess = projectService.AssignTask(1, 103);
            if (assignSuccess.IsSuccess)
            {
                Console.WriteLine($"Успіх: завдання #103 призначено (Результат: {assignSuccess.Value}).");
            }
            else
            {
                Console.WriteLine($"Помилка: {assignSuccess.Error}");
            }

            var assignDuplicate = projectService.AssignTask(1, 101);
            if (assignDuplicate.IsSuccess)
            {
                Console.WriteLine($"Успіх: завдання #101 призначено.");
            }
            else
            {
                Console.WriteLine($"Очікувана відмова: {assignDuplicate.Error}");
            }

            var assignMissingProject = projectService.AssignTask(50, 301);
            if (!assignMissingProject.IsSuccess)
            {
                Console.WriteLine($"Очікувана відмова: {assignMissingProject.Error}\n");
            }

            Console.WriteLine("--- 3. Перевірка Result<T>.Failure(Exception) ---");
            try
            {
                projectService.GetProject(777);
            }
            catch (ProjectNotFoundException ex)
            {
                var failedResult = Result<Project>.Failure(ex);
                Console.WriteLine($"IsSuccess: {failedResult.IsSuccess} | Error: {failedResult.Error}\n");
            }

            Console.WriteLine("=== Лабораторну роботу №18 успішно виконано! ===");
        }
    }
}
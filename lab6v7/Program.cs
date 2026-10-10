namespace lab6v7;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("=== 1. Створення об'єктів та виклик власних методів ===");
        Person person = new Person("Олексій", 40);
        Student student = new Student("Іван", 19, "ST12345");
        Professor professor = new Professor("Микола Петрович", 52, "Комп'ютерні науки");

        person.Greet();
        student.Greet();
        student.Study();
        professor.Greet();
        professor.Teach();

        Console.WriteLine("\n=== 2. Демонстрація Поліморфізму (virtual / override) ===");
        List<Person> people = new List<Person> { person, student, professor };
        foreach (var p in people)
        {
            p.Greet();
        }

        Console.WriteLine("\n=== 3. Демонстрація різниці між override та new ===");
        Student directStudent = new Student("Анна", 20, "ST99999");
        Person personRefToStudent = directStudent;

        Console.WriteLine("--- Виклик Greet() (override) ---");
        directStudent.Greet();
        personRefToStudent.Greet();

        Console.WriteLine("--- Виклик GetRole() (new) ---");
        Console.WriteLine($"Через посилання Student: {directStudent.GetRole()}");
        Console.WriteLine($"Через посилання Person:  {personRefToStudent.GetRole()}");
    }
}
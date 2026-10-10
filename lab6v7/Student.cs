namespace lab6v7;

public class Student : Person
{
    private string studentId;

    public string StudentId
    {
        get => studentId;
        set => studentId = value;
    }

    public Student(string name, int age, string studentId) : base(name, age)
    {
        this.studentId = studentId;
    }

    public override void Greet()
    {
        Console.WriteLine($"[Student] Привіт! Я студент {Name}, ID: {StudentId}.");
    }

    public void Study()
    {
        Console.WriteLine($"Студент {Name} навчається.");
    }

    public new string GetRole()
    {
        return "Студент";
    }
}
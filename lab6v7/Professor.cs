namespace lab6v7;

public class Professor : Person
{
    private string department;

    public string Department
    {
        get => department;
        set => department = value;
    }

    public Professor(string name, int age, string department) : base(name, age)
    {
        this.department = department;
    }

    public override void Greet()
    {
        Console.WriteLine($"[Professor] Вітаю! Я викладач {Name} з кафедри {Department}.");
    }

    public void Teach()
    {
        Console.WriteLine($"Викладач {Name} проводить лекцію.");
    }
}
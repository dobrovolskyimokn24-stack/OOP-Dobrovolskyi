namespace lab6v7;

public class Person
{
    private string name;
    private int age;

    public string Name
    {
        get => name;
        set => name = value;
    }

    public int Age
    {
        get => age;
        set => age = value;
    }

    public Person(string name, int age)
    {
        this.name = name;
        this.age = age;
    }

    public virtual void Greet()
    {
        Console.WriteLine($"[Person] Привіт! Мене звати {Name}, мені {Age} років.");
    }

    public string GetRole()
    {
        return "Людина";
    }
}
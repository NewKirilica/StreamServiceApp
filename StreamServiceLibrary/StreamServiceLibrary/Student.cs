public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double AverageGrade { get; set; }

    public Student(int id, string name, double averageGrade)
    {
        Id = id;
        Name = name;
        AverageGrade = averageGrade;
    }

    public override string ToString() => $"{Id} - {Name} - {AverageGrade:F2}";
}

namespace WebApplication1.Core
{
    public class Employee
    {
        public Employee(int id, string name, string position, double salary)
        {
            Id = id;
            Name = name;
            Position = position;
            Salary = salary;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Position { get; private set; }
        public double  Salary { get; private set; }

        public void Update(string name, string position, double salary)
        {
            Name = name;
            Position = position;
            Salary = salary;
        }
    }
}

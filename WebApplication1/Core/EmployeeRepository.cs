namespace WebApplication1.Core
{
    public static class EmployeeRepository

    {
        public static IEnumerable<Employee> Employees = new List<Employee>
            {
                new Employee(1, "Alice", "Developer", 70000),
                new Employee(2, "Bob", "Designer", 65000),
                new Employee(3, "Charlie", "Manager", 90000)
            };

        public static IEnumerable<Employee> GetEmployees() => Employees;

        public static void AddEmployee(Employee employee)
        {
            Employees = Employees.Append(employee);
        }
        public static void Uppdate(Employee employee)
        {
            var emp = Employees.FirstOrDefault(e => e.Id == employee.Id) ?? throw new Exception("Employee not found");

            emp.Update(employee.Name, employee.Position, employee.Salary);
        }

    }
}

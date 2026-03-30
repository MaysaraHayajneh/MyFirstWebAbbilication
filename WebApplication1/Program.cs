using System.Text.Json;
using WebApplication1.Core;
using HttpMethod = Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpMethod;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// THIS IS A MIDDLEWARE COMPONENT  
app.Run(async (HttpContext context) =>
{
    // I AM APPLYING LIKE ROUTING MECHANISM
    if (string.Equals(context.Request.Method, HttpMethod.Get.ToString(), StringComparison.OrdinalIgnoreCase))
    {
        if (context.Request.Path.StartsWithSegments("/")) // I AM APPLYING LIKE ROUTING MECHANISM BY CHECKING WHERE TEH REQUEST GO 
        // AND INSIDE EACH IF THE LOGIC IS LIKE THE CONTROLLER ACTIONS END POINT (HANDLER)
        {
            await context.Response.WriteAsync($"The method is  {context.Request.Method}\n" +
               $"The Url is  {context.Request.Path} /base :{context.Request.PathBase}\n");

            foreach (var key in context.Request.Headers.Keys)
            {
                await context.Response.WriteAsync($"{key}:{context.Request.Headers[key]}");
            }
        }
        else if (context.Request.Path.StartsWithSegments("/employee"))  // I AM APPLYING LIKE ROUTING MECHANISM BY CHECKING WHERE TEH REQUEST GO 
        // AND INSIDE EACH IF THE LOGIC IS LIKE THE CONTROLLER ACTIONS END POINT (HANDLER)
        {
            var employees = EmployeeRepository.GetEmployees();
            foreach (var employee in employees)
            {
                await context.Response.WriteAsync($"Id: {employee.Id}, Name: {employee.Name}, Position: {employee.Position}, Salary: {employee.Salary}\n");
            }
            await context.Response.WriteAsync("List of employee");
        }
    }
    else if (string.Equals(context.Request.Method, HttpMethod.Post.ToString(), StringComparison.OrdinalIgnoreCase))
    {
        if (context.Request.Path.StartsWithSegments("/employee"))
        {
            Stream bodyStream = context.Request.Body;

            using StreamReader reader = new StreamReader(bodyStream);

            string body = await reader.ReadToEndAsync();

            Employee? employee = JsonSerializer.Deserialize<Employee>(body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (employee is null || employee.Id == default) throw new Exception("Invalid employee data");

            EmployeeRepository.AddEmployee(employee);
        }
    }
    else if (string.Equals(context.Request.Method, HttpMethod.Put.ToString(), StringComparison.OrdinalIgnoreCase))
    {
        if (context.Request.Path.StartsWithSegments("/employee"))
        {
            Stream bodyStream = context.Request.Body;

            using StreamReader reader = new StreamReader(bodyStream);

            string body = await reader.ReadToEndAsync();

            Employee? employee = JsonSerializer.Deserialize<Employee>(body, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (employee is null || employee.Id == default) throw new Exception("Invalid employee data");

            EmployeeRepository.Uppdate(employee);
        }
    }
});
app.Run();

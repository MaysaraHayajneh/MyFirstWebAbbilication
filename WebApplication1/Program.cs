using System.Net;
using System.Text.Json;
using WebApplication1.Core;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// THIS IS A MIDDLEWARE COMPONENT  
app.Run(async (HttpContext context) =>
{
	var path = context.Request.Path;
	var method = context.Request.Method;

	// ROOT "/"
	if (path.StartsWithSegments("/"))
	{
		if (HttpMethods.IsGet(method))
		{
			await context.Response.WriteAsync($"The method is {method}\n" +
				$"The Url is {path} /base :{context.Request.PathBase}\n");

			foreach (var key in context.Request.Headers.Keys)
			{
				await context.Response.WriteAsync($"{key}:{context.Request.Headers[key]}\n");
			}
		}
	}

	// EMPLOYEE "/employee"
	else if (path.StartsWithSegments("/employee"))
	{
		if (HttpMethods.IsGet(method))
		{
			var employees = EmployeeRepository.GetEmployees();

			foreach (var employee in employees)
			{
				await context.Response.WriteAsync(
					$"Id: {employee.Id}, Name: {employee.Name}, Position: {employee.Position}, Salary: {employee.Salary}\n");
			}

			context.Response.StatusCode = (int)HttpStatusCode.OK;
		}
		else if (HttpMethods.IsPost(method))
		{
			var employee = await ReadEmployeeFromBody(context);

			EmployeeRepository.AddEmployee(employee);
			context.Response.StatusCode = (int)HttpStatusCode.Created;
			await context.Response.WriteAsync("Employee added");

		}
		else if (HttpMethods.IsPut(method))
		{
			var employee = await ReadEmployeeFromBody(context);

			EmployeeRepository.Uppdate(employee);
			context.Response.StatusCode = (int)HttpStatusCode.NoContent;
		}
		else if (HttpMethods.IsDelete(method))
		{
			if (context.Request.Headers["Authorization"] != "maysara")
			{
				context.Response.StatusCode = 403;
				await context.Response.WriteAsync("Forbidden");
				return;
			}

			var idValue = context.Request.Query["id"];

			if (!int.TryParse(idValue, out int id))
				throw new Exception("Invalid id");

			EmployeeRepository.Delete(id);
			await context.Response.WriteAsync("Employee deleted");
		}
	}
});

#region Helper 
static async Task<Employee> ReadEmployeeFromBody(HttpContext context)
{
	using StreamReader reader = new StreamReader(context.Request.Body);
	string body = await reader.ReadToEndAsync();

	var employee = JsonSerializer.Deserialize<Employee>(
		body,
		new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

	if (employee is null || employee.Id == default)
		throw new Exception("Invalid employee data");

	return employee;
}
#endregion
app.Run();

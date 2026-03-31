using System.Linq.Expressions;
using System.Net;
using System.Text.Json;
using WebApplication1.Core;
using WebApplication1.CustomExceptions;

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
            context.Response.Headers["Content-Type"] = "text/html"; // now the browser understand teh the content in the reponse will be handles as html 

            await context.Response.WriteAsync($"{context.Response.Headers["Content-Type"]}  The method is {method} <br/>" +
                $"The Url is {path} /base :{context.Request.PathBase}<br/>");

            foreach (var key in context.Request.Headers.Keys)
            {
                await context.Response.WriteAsync($"<b>{key}</b>:{context.Request.Headers[key]}<br/>");
            }
        }
    }

    // EMPLOYEE "/employee"
    else if (path.StartsWithSegments("/employee"))
    {
        try
        {


            if (HttpMethods.IsGet(method))
            {
                var employees = EmployeeRepository.GetEmployees();
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                foreach (var employee in employees)
                {
        

                    await context.Response.WriteAsync(
                        $"Id: {employee.Id}, Name: {employee.Name}, Position: {employee.Position}, Salary: {employee.Salary}\n");
                }

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
        catch (Exception ex)
        {
            if (ex is BadRequest)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(ex.Message);

            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("An error occurred: " + ex.Message);
            }
        }
    }

    else if (path.StartsWithSegments("/redirect"))
    {
        context.Response.Redirect("/employee");
    }

    else
    {
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
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
        throw new BadRequest("Invalid employee data");


    return employee;
}
#endregion
app.Run();

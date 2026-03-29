var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// THIS IS A MIDDLEWARE COMPONENT  
app.Run(async (HttpContext context) =>
{
	await context.Response.WriteAsync($"The method is  {context.Request.Method}\n" +
   $"The Url is  {context.Request.Path} /base :{context.Request.PathBase}\n");


	foreach (var key in context.Request.Headers.Keys)
	{
		await context.Response.WriteAsync($"{key}:{context.Request.Headers[key]}");
	}

});
app.Run();

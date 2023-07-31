using HoneyRaesAPI.Models;

var builder = WebApplication.CreateBuilder(args);

List<Customer> Customers = new List<Customer>
{
    new Customer()
    {
        Id = 1,
        Name = "Customer 1",
        Address = "456 cusomter way"
    },
    new Customer()
    {
        Id = 2,
        Name = "Customer 2",
        Address = "872 cusomter way"
    },
    new Customer()
    {
        Id = 3,
        Name = "Customer 3",
        Address = "453 cusomter way"
    }

};
List<Employee> Employees = new List<Employee>()
{
    new Employee
    {
        Id = 1,
        Name = "Employee 1",
        Specialty = "React"
    },
    new Employee
    {
        Id = 2,
        Name = "Employee 2",
        Specialty = "HTML"
    },
    new Employee
    {
        Id = 3,
        Name = "Employee 3",
        Specialty = "C#"
    },
};
List<ServiceTicket> ServiceTickets = new List<ServiceTicket>()
{
    new ServiceTicket
    {
        Id=1,
        CustomerId=1,
        EmployeeId=1,
        Description="Ticket 1",
        Emergency=false,
        DateCompleted=new DateTime()
    },
    new ServiceTicket
    {
        Id=2,
        CustomerId=2,
        EmployeeId=2,
        Description="Ticket 2",
        Emergency=false,
        DateCompleted=new DateTime()
    },
    new ServiceTicket
    {
        Id=3,
        CustomerId=3,
        EmployeeId=3,
        Description="Ticket 3",
        Emergency=true,
        DateCompleted=new DateTime()
    },
    new ServiceTicket
    {
        Id=4,
        CustomerId=4,
        EmployeeId=4,
        Description="Ticket 4",
        Emergency=false,
        DateCompleted=new DateTime(2023,05,31)
    },
    new ServiceTicket
    {
        Id=5,
        CustomerId=5,
        EmployeeId=5,
        Description="Ticket 5",
        Emergency=true,
        DateCompleted=new DateTime(2023,02,14)
    }
};

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/hello", () =>
{
    return "hello";
});
//dewq
app.MapGet("/servicetickets", () =>
{
    return ServiceTickets;
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    foreach (var t in ServiceTickets)
    {
        t.Employee = null;
    }
    foreach (var e in Employees)
    {
        e.ServiceTickets = null;
    }
    foreach (var c in Customers)
    {
        c.ServiceTickets = null;
    }
    ServiceTicket serviceTicket = ServiceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = Employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    serviceTicket.Customer = Customers.FirstOrDefault(e => e.Id == serviceTicket.CustomerId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/servicetickets/{id}", (int id) =>
{
    ServiceTicket serviceTicket = ServiceTickets.FirstOrDefault(st => st.Id == id);
    if (serviceTicket == null)
    {
        return Results.NotFound();
    }
    serviceTicket.Employee = Employees.FirstOrDefault(e => e.Id == serviceTicket.EmployeeId);
    return Results.Ok(serviceTicket);
});

app.MapGet("/employees", () =>
{
    return Employees;
});

app.MapGet("/employees/{id}", (int id) =>
{
    Employee employee = Employees.FirstOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.ServiceTickets = ServiceTickets.Where(st => st.EmployeeId == id).ToList();
    return Results.Ok(employee);
});

app.MapGet("/customers", () =>
{
    return Customers;
});

app.MapGet("/customers/{id}", (int id) =>
{
    return Customers.FirstOrDefault(st => st.Id == id);
});

app.MapPost("/servicetickets", (ServiceTicket serviceTicket) =>
{
    // creates a new id (When we get to it later, our SQL database will do this for us like JSON Server did!)
    serviceTicket.Id = ServiceTickets.Max(st => st.Id) + 1;
    ServiceTickets.Add(serviceTicket);
    return serviceTicket;
});

app.Run();
//as
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

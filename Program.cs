using KeyRemappingService.Data;
using KeyRemappingService.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=app.db"));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    db.Database.EnsureCreated();

    if (!db.Keyboards.Any())
    {
        db.Keyboards.Add(new Keyboard
        {
            Name = "Apex Pro Gen 3"
        });

        db.Keyboards.Add(new Keyboard
        {
            Name = "Apex Pro Mini Gen 3"
        });

        db.SaveChanges();
    }
}

app.Run();

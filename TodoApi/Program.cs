using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(b =>
{
    b.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Version = "v1" });
});
//Register DbContext as a service
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), ServerVersion.Parse("8.0.36-mysql")),
    ServiceLifetime.Singleton);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
    builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
    
}
app.UseCors("AllowAll");
// שליפת כל המשימות
app.MapGet("/todos", async (ToDoDbContext context) =>
{
    var items = await context.Items.ToListAsync();
    return JsonSerializer.Serialize(items);
});
//הוספת משימה חדשה
app.MapPost("/todos", async (ToDoDbContext context, Item newItem) =>
{
    context.Items.Add(newItem);
    await context.SaveChangesAsync();
    return newItem;
});

//עדכון משימה
app.MapPut("/todos/{id}", async (ToDoDbContext context, int id, Item updatedItem) =>
{
    var existingItem = await context.Items.FindAsync(id);
    if (existingItem == null)
    {
        return Results.NotFound();
    }

    if(updatedItem.Name != null)
    {
        existingItem.Name = updatedItem.Name;
    }

    existingItem.IsComplete = updatedItem.IsComplete;

    await context.SaveChangesAsync();
    return Results.NoContent();
});


//מחיקת משימה
app.MapDelete("/todos/{id}", async (ToDoDbContext context, int id) =>
{
    var existingItem = await context.Items.FindAsync(id);
    if (existingItem == null)
    {
        return Results.NotFound();
    }

    context.Items.Remove(existingItem);
    await context.SaveChangesAsync();
    return Results.NoContent();
});



app.Run();

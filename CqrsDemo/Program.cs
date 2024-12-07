using Microsoft.EntityFrameworkCore;
using CqrsDemo.Infrastructure.Persistence;
using CqrsDemo.Application;
using CqrsDemo.Domain.Entities;
using CqrsDemo.Application.Mapper;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("OrderDb"));
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register Automapper
builder.Services.AddAutoMapper(typeof(OrderMapping));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.Run();

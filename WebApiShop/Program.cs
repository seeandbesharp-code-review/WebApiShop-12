using Microsoft.EntityFrameworkCore;
using Repository;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IPasswordsService, PasswordsService>();
builder.Services.AddDbContext<WebApiShopContext>(option => option.UseSqlServer
("Data Source=srv2\\pupils;Initial Catalog=WebApiShop;Integrated Security=True;Trust Server Certificate=True; Pooling = False"));

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

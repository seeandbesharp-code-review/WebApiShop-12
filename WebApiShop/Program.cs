using Microsoft.EntityFrameworkCore;
using NLog.Web;
using PresidentsApp.Middlewares;
using Repository;
using Services;
using WebApiShop.MiddleWare;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IPasswordsService, PasswordsService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Host.UseNLog();
builder.Services.AddDbContext<WebApiShopContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("Home")));

var redisCs = builder.Configuration.GetSection("Redis:ConnectionString").Value
    ?? throw new InvalidOperationException("Redis:ConnectionString is not configured. Add it to appsettings or User Secrets.");
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisCs));

// TODO: Add JWT authentication — AddAuthentication not configured.
// Add the following before AddControllers():
//   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//       .AddJwtBearer(options => { ... });
// and app.UseAuthentication() before app.UseAuthorization() in the pipeline.
// Rule B3 — Critical: without this, [Authorize] attributes are not enforced.
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

app.UseRatingMiddleware();

app.UseErrorHandlingMiddleware();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

using CalendarApp.Data;
using CalendarApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Подключаем EF Core к SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Разрешаем CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAngular",
//        policy =>
//        {
//            policy.WithOrigins("http://localhost:4200") // наш Angular
//                  .AllowAnyMethod()
//                  .AllowAnyHeader();
//        });
//});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClients",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200",// Angular
                    //"http://10.0.2.2:5000",       // Android эмулятор (доступ к хост-машине)
                    //"http://192.168.1.17:7105"   // IP твоего ПК в локальной сети (для телефона)
                    //"http://194.164.33.248:7105"
                    "http://localhost:5029"   // Android через USB (через adb reverse)
                    //"http://localhost:7105"
                )
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});







// Add services to the container.

//builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        //options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.ReferenceHandler = null;
    });


// Регистрация PasswordHasher для админов
builder.Services.AddScoped<IPasswordHasher<Admin>, PasswordHasher<Admin>>();





// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Показывает реальные ошибки
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseCors("AllowAngular");
app.UseCors("AllowClients");




app.UseAuthorization();


app.MapControllers();

app.Run();

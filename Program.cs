using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Runtime.Intrinsics.X86;
using WhereToEat_BE.Data;
using WhereToEat_BE.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql("Host=aws-1-ap-northeast-2.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.objdwqjfnynzltwlexpe;Password=TzoFYET4VDRVrbtO;SSL Mode=Require;Trust Server Certificate=true;Pooling=false"));
builder.Services.AddScoped<AIService>();
builder.Services.AddScoped<PlacesService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();

using IPWhistleblower;
using IPWhistleblower.Services;
using IPWhistleblower.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//builder.Services.AddHttpClient<IIPInformationService, IPInformationService>("Ip2cClient", client =>
//{
//    client.BaseAddress = new Uri("https://api.ip2c.org/"); // Use the correct API endpoint here
//});

builder.Services.AddHttpClient("DefaultClient");
builder.Services.AddTransient<IIPInformationService, IPInformationService>();
builder.Services.AddTransient<IIPAddressService, IPAddressService>();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, CacheService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment()) 
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
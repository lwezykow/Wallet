using Scalar.AspNetCore;
using Serilog;
using Wallet.Data;
using Wallet.Domain;
using Wallet.External.Nbp;
using Wallet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddNbpCurrencyRates();
builder.Services.AddDomain();
builder.Services.AddWalletData();
builder.Services.AddRatesPooler();  

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.Title = "Wallet Demo Application";
        opt.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.Http11);
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
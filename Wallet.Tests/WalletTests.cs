using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Data;
using Wallet.Domain.Entities;
using Wallet.Host.Console.Models;

namespace Wallet.Tests;

public class WalletTests
{
    private CustomWebApplicationFactory<Program> _factory;

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    
    [Test]
    public async Task Should_be_able_to_create_and_delete_wallet()
    {
        string expectedWalletName = $"New wallet name {DateTime.UtcNow:s}";
        
        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = expectedWalletName });
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response.Headers.Location, Is.Not.Null);
        
        response = await client.GetAsync(response.Headers.Location);
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response, Is.Not.Null);
        
        var actual = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(actual,  Is.Not.Null);
        Assert.That(actual.Name, Is.EqualTo(expectedWalletName));
        
        response = await client.DeleteAsync("api/Wallets/1");
        Assert.That(response.IsSuccessStatusCode);
        
        response = await client.GetAsync("api/Wallets/1");
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test] public async Task Should_be_able_to_list_all_wallets()
    {
        string[] walletNames = ["wallet1", "wallet2", "wallet3"];
        
        var client = _factory.CreateClient();

        HttpResponseMessage response;
        
        foreach (var name in walletNames)
        {
            response = await client.PostAsJsonAsync("api/Wallets",  new { Name = name });
            Assert.That(response.IsSuccessStatusCode);
        }
        
        response = await client.GetAsync("api/Wallets");
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response, Is.Not.Null);
        
        var actual = await JsonSerializer.DeserializeAsync<WalletModel[]>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(actual,  Is.Not.Null);
        Assert.That(actual, Has.Length.EqualTo(walletNames.Length));
        
        foreach (var wallet in walletNames)
        {
            Assert.That(actual.Any(w => w.Name == wallet));
        }
    }
    
    [Test]
    [TestCase("")]
    [TestCase(null)]
    public async Task Should_return_400_bad_request_when_wallet_name_is_empty_or_missing(string? walletName)
    {
        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = walletName });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    [Test]
    public async Task Should_be_able_to_deposit_funds()
    {
        MoneyModel[] deposits =
        [
            new() { Amount = 100, Currency = "EUR" },
            new() { Amount = 200, Currency = "EUR" },
            new() { Amount = 123, Currency = "PLN" },
            new() { Amount = 456, Currency = "USD" }
        ];

        MoneyModel[] expected =
        [
            new() { Amount = 300, Currency = "EUR" },
            new() { Amount = 123, Currency = "PLN" },
            new() { Amount = 456, Currency = "USD" }
        ];
        
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = "Deposit Test Wallet" });
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response.Headers.Location, Is.Not.Null);
        
        var walletQueryUri = response.Headers.Location;
        
        response = await client.GetAsync(walletQueryUri);
        
        var wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(wallet, Is.Not.Null);
        
        foreach (var deposit in deposits)
        {
            response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Deposit", deposit);
            Assert.That(response.IsSuccessStatusCode);
        }
        
        response = await client.GetAsync(walletQueryUri);
        Assert.That(response.IsSuccessStatusCode);
        
        wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Has.Length.EqualTo(expected.Length));
        Assert.That(expected, Is.EquivalentTo(wallet.Funds));
    }
    
    [Test]
    public async Task Should_return_400_if_deposit_currency_is_not_supported()
    {
        MoneyModel deposit = new() { Amount = 100, Currency = "XAG" };

        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = "Deposit Test Wallet" });
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response.Headers.Location, Is.Not.Null);
        
        var walletQueryUri = response.Headers.Location;
        
        response = await client.GetAsync(walletQueryUri);
        
        var wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Empty);
        
        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Deposit", deposit);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    public async Task Should_be_able_to_deposit_and_withdraw_partial_amounts()
    {
        MoneyModel deposit = new() { Amount = 100, Currency = "EUR" };
        MoneyModel withdraw = new() { Amount = 50, Currency = "EUR" };

        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = "Deposit Test Wallet" });
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response.Headers.Location, Is.Not.Null);
        
        var walletQueryUri = response.Headers.Location;
        
        response = await client.GetAsync(walletQueryUri);
        
        var wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Empty);
        
        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Deposit", deposit);
        
        Assert.That(response.IsSuccessStatusCode);

        for (int i = 0; i < 2; i++)
        {
            response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Withdraw", withdraw);
            Assert.That(response.IsSuccessStatusCode);    
        }
        
        wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Empty);
    }
    
    [Test]
    [TestCase("USD")]
    [TestCase("XAG")]
    public async Task Should_return_400_when_trying_to_withdraw_currency_that_is_not_in_the_wallet_or_unsupported(string currency)
    {
        MoneyModel deposit = new() { Amount = 100, Currency = "EUR" };
        MoneyModel withdraw = new() { Amount = 100, Currency = currency };

        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = "Deposit Test Wallet" });
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response.Headers.Location, Is.Not.Null);
        
        var walletQueryUri = response.Headers.Location;
        
        response = await client.GetAsync(walletQueryUri);
        
        var wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Empty);
        
        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Deposit", deposit);
        
        Assert.That(response.IsSuccessStatusCode);

        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Withdraw", withdraw);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
        response = await client.GetAsync(walletQueryUri);
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response, Is.Not.Null);
        
        wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.EquivalentTo([deposit]));
    }
    
    [Test]
    public async Task Should_return_400_when_when_withdraw_exceeds_available_amount()
    {
        MoneyModel deposit = new() { Amount = 50, Currency = "EUR" };
        MoneyModel withdraw = new() { Amount = 100, Currency = "EUR" };

        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = "Deposit Test Wallet" });
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response.Headers.Location, Is.Not.Null);
        
        var walletQueryUri = response.Headers.Location;
        
        response = await client.GetAsync(walletQueryUri);
        
        var wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Empty);
        
        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Deposit", deposit);
        Assert.That(response.IsSuccessStatusCode);
        
        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Withdraw", withdraw);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        
        response = await client.GetAsync(walletQueryUri);
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response, Is.Not.Null);
        
        wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.EquivalentTo([deposit]));
    }
    
    [Test]
    public async Task Should_be_able_to_sell_currency()
    {
        MoneyModel initialDeposit = new() { Amount = 100, Currency = "EUR" };
        SellCurrencyRequestModel usdRequest = new()
        {
            Source = 
                new MoneyModel
                {
                    Amount = 30, 
                    Currency = "EUR"
                }, 
            TargetCurrency = "USD"
        };
        
        SellCurrencyRequestModel plnRequest = new()
        {
            Source = 
                new MoneyModel
                {
                    Amount = 10, 
                    Currency = "EUR"
                }, 
            TargetCurrency = "PLN"
        };

        SellCurrencyRequestModel[] requests = [usdRequest, plnRequest];

        MoneyModel[] expected = 
        [
            new() { Amount = 60, Currency = "EUR" },
            new() { Amount = 40, Currency = "PLN" },
            new() { Amount = 40, Currency = "USD" }
        ];
        
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = "Deposit Test Wallet" });
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response.Headers.Location, Is.Not.Null);
        
        var walletQueryUri = response.Headers.Location;
        
        response = await client.GetAsync(walletQueryUri);
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response, Is.Not.Null);
        
        var wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(wallet,  Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Empty);

        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Deposit", initialDeposit);
        Assert.That(response.IsSuccessStatusCode);
        
        foreach (var request in requests)
        {
            response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/SellCurrency", request);
        
            Assert.That(response.IsSuccessStatusCode);    
        }
        
        wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        
        Assert.That(wallet, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Has.Length.EqualTo(3));
        Assert.That(wallet.Funds, Is.EquivalentTo(expected));
    }
    
    [Test]
    [TestCase("XAU", "USD")]
    [TestCase("EUR", "XAU")]
    public async Task Should_return_400_when_trying_to_use_sell_with_unsupported_currency(string source, string target)
    {
        MoneyModel initialDeposit = new() { Amount = 100, Currency = "EUR" };
        SellCurrencyRequestModel request = new()
        {
            Source = 
                new MoneyModel
                {
                    Amount = 30, 
                    Currency = source
                }, 
            TargetCurrency = target
        };

        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = "Deposit Test Wallet" });
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response.Headers.Location, Is.Not.Null);
        
        var walletQueryUri = response.Headers.Location;
        
        response = await client.GetAsync(walletQueryUri);
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response, Is.Not.Null);
        
        var wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(wallet,  Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Empty);

        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Deposit", initialDeposit);
        Assert.That(response.IsSuccessStatusCode);
        
        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/SellCurrency", request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
    
    [Test]
    [TestCase("XAU", "USD")]
    [TestCase("EUR", "XAU")]
    public async Task Should_return_400_when_trying_to_use_buy_with_unsupported_currency(string source, string target)
    {
        MoneyModel initialDeposit = new() { Amount = 100, Currency = "EUR" };
        BuyCurrencyRequestModel request = new()
        {
            Target = 
                new MoneyModel
                {
                    Amount = 40, 
                    Currency = target
                }, 
            SourceCurrency = source
        };
        
        var client = _factory.CreateClient();
        
        var response = await client.PostAsJsonAsync("api/Wallets",  new { Name = "Deposit Test Wallet" });
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response.Headers.Location, Is.Not.Null);
        
        var walletQueryUri = response.Headers.Location;
        
        response = await client.GetAsync(walletQueryUri);
        Assert.That(response.IsSuccessStatusCode);
        Assert.That(response, Is.Not.Null);
        
        var wallet = await JsonSerializer.DeserializeAsync<WalletModel>(
            await response.Content.ReadAsStreamAsync(), JsonOptions);
        Assert.That(wallet,  Is.Not.Null);
        Assert.That(wallet.Funds, Is.Not.Null);
        Assert.That(wallet.Funds, Is.Empty);

        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/Deposit", initialDeposit);
        Assert.That(response.IsSuccessStatusCode);
        
        response = await client.PostAsJsonAsync($"/api/Wallets/{wallet.Id}/BuyCurrency", request);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));    
    }
    
    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        
        var context = _factory.Services.GetRequiredService<WalletDbContext>();
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        context.CurrencyRates.Add(new CurrencyRate { Name = "Euro", IsBase = false, Rate = 4.0, Symbol = "EUR" });
        context.CurrencyRates.Add(new CurrencyRate { Name = "Dolar Amerykański", IsBase = false, Rate = 3.0, Symbol = "USD" });
        context.CurrencyRates.Add(new CurrencyRate { Name = "Polski Nowy Złoty", IsBase = true, Rate = 1.0, Symbol = "PLN" });
        
        context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _factory.Dispose();
    }
}
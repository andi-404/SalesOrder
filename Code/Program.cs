using Microsoft.AspNetCore.Localization;
using SalesOrder.Services.Order;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
builder.Services.AddScoped<IOrder, OrderDAL>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var ci = new CultureInfo("en-US");
ci.NumberFormat.NumberDecimalSeparator = ".";
ci.NumberFormat.CurrencyDecimalSeparator = ".";

var requestOpt = new RequestLocalizationOptions();
requestOpt.SupportedCultures = new List<CultureInfo> { ci };
requestOpt.SupportedUICultures = new List<CultureInfo> { ci };
requestOpt.RequestCultureProviders.Clear();
requestOpt.RequestCultureProviders.Add(new SingleCultureProvider());
app.UseRequestLocalization(requestOpt);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Order}/{action=Index}/{id?}");

app.Run();


public class SingleCultureProvider : IRequestCultureProvider
{
    public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
    {
        return Task.Run(() => new ProviderCultureResult("en-US", "en-US"));
    }
}

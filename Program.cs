using Hangfire;
using HangfireBasicAuthenticationFilter;
using MarketScreener.Contracts;
using MarketScreener.Impl;
using MarketScreener.Jobs;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();



//Configure Hangfire
var connectionString = builder.Configuration.GetConnectionString("HangfireConnection");


builder.Services.AddTransient<IMarketData, MarketData>();
builder.Services.AddTransient<IMagnetStrategy, MagnetStrategy>();


builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new Hangfire.SqlServer.SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));
// Add the processing server as IhostedService
builder.Services.AddHangfireServer();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "MagnetScreener",
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = builder.Configuration.GetSection("HangfireSettings:UserName").Value,
            Pass = builder.Configuration.GetSection("HangfireSettings:Password").Value
        }
    }
});

app.UseEndpoints(endpoint =>
{
    endpoint.MapControllers();
    endpoint.MapHangfireDashboard();
});

//builder.Services
//var marketService = serviceProvider.GetService<IMagnetStrategy>();
//RecurringJob.AddOrUpdate("myrecurringjob", () => marketService.SayHello(), Cron.Minutely);


var jobid = BackgroundJob.Enqueue(() => Console.WriteLine("fire-and-forget"));



app.Run();



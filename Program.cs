using CpuScheduler.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// Register scheduler service
builder.Services.AddScoped<ISchedulerService, SchedulerService>();

// Configure session (in-memory, 30 min timeout)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// Default route → Scheduler/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Scheduler}/{action=Index}/{id?}");

app.Run();

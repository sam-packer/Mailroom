using Mailroom;
using Mailroom.Pages.Mail;
using Mailroom.Utils;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContextPool<MailroomDbContext>((provider, options) =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var interceptor = provider.GetRequiredService<DynamicLeaderRecoveryInterceptor>();

    var connStr = config.GetConnectionString("MailroomDB");
    if (!string.IsNullOrWhiteSpace(connStr))
    {
        options.UseNpgsql(connStr);
    }
    else
    {
        var dbPass = config["db_password"];
        var dbUser = config["db_user"] ?? "postgres";
        var dbName = config["db_name"] ?? "mailroom";

        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = "pg-leader.internal", // the leader is determined by the servers using scripts and internal DNS
            Port = 5432,
            Username = dbUser,
            Password = dbPass,
            Database = dbName,
            Pooling = true
        }.ConnectionString;

        options.UseNpgsql(connectionString).AddInterceptors(interceptor);
    }
});

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(3);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Errors/{0}");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();
using GuestService.Data;
using GuestService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiGatewayPolicy", builder =>
    {
        builder.WithOrigins("http://nginx", "https://nginx")    
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddScoped<IGuestService, GuestService.Services.GuestService>();

builder.WebHost.ConfigureKestrel((context, options) =>
{
    var env = context.HostingEnvironment;

    if (env.IsDevelopment())
    {
        options.ListenAnyIP(8082); 
        options.ListenAnyIP(8442, listenOptions =>
        {
            listenOptions.UseHttps(); 
        });
    }
    else
    {
        var certPath = context.Configuration["GUESTSERVICE_CERT_PATH"];
        var certPassword = context.Configuration["GUESTSERVICE_CERT_PASSWORD"];

        if (string.IsNullOrEmpty(certPath) || string.IsNullOrEmpty(certPassword))
        {
            throw new InvalidOperationException(
                $"Certificate path or password is not configured. Path: {certPath}, Password: {(string.IsNullOrEmpty(certPassword) ? "Not Provided" : "Provided")}");
        }

        options.ListenAnyIP(8082); 
        options.ListenAnyIP(8442, listenOptions =>
        {
            listenOptions.UseHttps(certPath, certPassword);
        });
    }
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetValue<string>("DefaultConnection") ??
                       $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";

builder.Services.AddDbContext<GuestDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GuestDbContext>();
    dbContext.Database.Migrate();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ApiGatewayPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
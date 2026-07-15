var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

const string allowedOrigins = "_allowedOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(allowedOrigins, policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors(allowedOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/health", () => new
{
    status = "ok",
    message = "Server is running"
});

app.MapGet("/api/dashboard", () => new
{
    healthMessage = "ASP.NET Core API connected",
    period = "2026. 05. 20. - 2026. 06. 20.",
    metrics = new[]
    {
        new
        {
            title = "Lejárt eszközök",
            value = 12,
            subtitle = "Összesen 248 eszközből",
            tone = "danger",
            trend = new[] { 12, 11, 13, 12, 15, 14, 16, 16, 18, 17, 19 }
        },
        new
        {
            title = "Közelgő lejáratok (30 napon belül)",
            value = 36,
            subtitle = "Figyelmet igénylő eszköz",
            tone = "warning",
            trend = new[] { 34, 33, 35, 32, 31, 34, 36, 33, 32, 35, 36 }
        },
        new
        {
            title = "Rendben lévő eszközök",
            value = 200,
            subtitle = "Az összes eszköz 80,6%-a",
            tone = "success",
            trend = new[] { 190, 194, 201, 199, 193, 194, 200, 196, 202, 198, 200 }
        },
        new
        {
            title = "Hibás eszközök",
            value = 8,
            subtitle = "Javítást igényel",
            tone = "info",
            trend = new[] { 6, 7, 8, 8, 9, 8, 6, 6, 7, 6, 7 }
        }
    },
    statusSummary = new[]
    {
        new { label = "Rendben", value = 200, percentage = 80.6, color = "green" },
        new { label = "Figyelmeztetés", value = 36, percentage = 14.5, color = "orange" },
        new { label = "Lejárt", value = 12, percentage = 4.8, color = "red" },
        new { label = "Hibás", value = 8, percentage = 3.1, color = "violet" }
    },
    expiringEquipment = new[]
    {
        new { id = "PO-2023-0012", location = "Irodaépület - 1. emelet", daysLeft = 14 },
        new { id = "TC-2022-0005", location = "Raktár csarnok", daysLeft = 14 },
        new { id = "PO-2023-0018", location = "Üzlethelyiség - Raktár", daysLeft = 7 },
        new { id = "CO2-2022-0003", location = "Adatközpont", daysLeft = 1 }
    },
    expiredEquipment = new[]
    {
        new { id = "PO-2023-0001", type = "Porral oltó", site = "Irodaépület - 2. emelet", dueDate = "2026. 05. 18.", status = "Lejárt" },
        new { id = "TC-2022-0002", type = "Tűzcsap", site = "Raktár csarnok", dueDate = "2026. 05. 17.", status = "Lejárt" },
        new { id = "CO2-2021-0004", type = "CO2 oltó", site = "Adatközpont", dueDate = "2026. 05. 16.", status = "Lejárt" },
        new { id = "PO-2022-0011", type = "Porral oltó", site = "Üzlethelyiség", dueDate = "2026. 05. 15.", status = "Lejárt" }
    },
    activities = new[]
    {
        new { title = "Karbantartás rögzítve", target = "PO-2023-0007", time = "ma 09:42", tone = "success" },
        new { title = "Hibabejelentés létrehozva", target = "TC-2021-0002", time = "ma 08:15", tone = "danger" },
        new { title = "Eszköz frissítve", target = "CO2-2022-0004", time = "tegnap 16:21", tone = "info" },
        new { title = "Munkalap letöltve", target = "PO-2023-0003", time = "tegnap 14:03", tone = "violet" }
    }
});

app.Run();

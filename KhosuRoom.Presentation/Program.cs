using KhosuRoom.Business.ServiceRegistrations;
using KhosuRoom.DataAccess.Abstractions;
using KhosuRoom.DataAccess.ServiceRegistartions;
using KhosuRoom.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddBusinessServices();
var app = builder.Build();

var scope = app.Services.CreateScope();
var contextInitalizer = scope.ServiceProvider.GetRequiredService<IContextInitalizer>();

await contextInitalizer.InitDatabaseAsync();

app.UseMiddleware<GlobalExceptionnHandler>();
app.UseCors("MyPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

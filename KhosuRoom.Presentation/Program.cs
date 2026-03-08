using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

using KhosuRoom.Business.Hubs;
using KhosuRoom.Business.ServiceRegistrations;
using KhosuRoom.DataAccess.Abstractions;
using KhosuRoom.DataAccess.ServiceRegistartions;
using KhosuRoom.Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Ensure code pages provider and UTF-8 console output (helps when running locally)
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = Encoding.UTF8;

// Add services to the container.

// Configure controllers and JSON encoder to avoid escaping non-ASCII characters
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // Allow full unicode characters in JSON responses (prevents \uXXXX escaping)
        o.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your token}"
    });

    opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddSignalR();

builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddBusinessServices(builder.Configuration);
var app = builder.Build();

var scope = app.Services.CreateScope();
var contextInitalizer = scope.ServiceProvider.GetRequiredService<IContextInitalizer>();

await contextInitalizer.InitDatabaseAsync();

if(!app.Environment.IsDevelopment())
    app.UseMiddleware<GlobalExceptionnHandler>();

app.UseCors("MyPolicy");

// Add middleware to ensure response Content-Type carries utf-8 charset when appropriate.
// This appends '; charset=utf-8' to the Content-Type if the header is present and no charset is specified.
app.Use(async (context, next) =>
{
    await next();

    var ct = context.Response.ContentType;
    if (!string.IsNullOrEmpty(ct) && !ct.Contains("charset", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.ContentType = ct + "; charset=utf-8";
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<GroupChatHub>("/hubs/groupchat");
app.MapControllers();

await app.RunAsync();

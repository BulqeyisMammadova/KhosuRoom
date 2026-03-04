’using KhosuRoom.Business.Hubs;
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
’"(04193ce157f6a2c891ad4ecb2dee43f046ac19442Mfile:///c:/Users/Balqeyis/Desktop/KhosuRoom/KhosuRoom.Presentation/Program.cs:+file:///c:/Users/Balqeyis/Desktop/KhosuRoom
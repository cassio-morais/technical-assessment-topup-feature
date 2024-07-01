using Backend.TopUp.Application.Services;
using Backend.TopUp.Core.Application.Services;
using Backend.TopUp.Core.Contracts;
using Backend.TopUp.Core.Infrastruture.Repository;
using Backend.TopUp.Core.Infrastruture.WebServices;
using Backend.TopUp.Infrastructure.Configuration.Db;
using Backend.TopUp.Infrastructure.Repository;
using Backend.TopUp.Infrastructure.WebServices;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// todo: later remove especific app configurations from this file
builder.Services.AddDbContext<DatabaseContext>();
builder.Services.AddScoped<IDatabaseContext, DatabaseContext>();
builder.Services.AddScoped<ITopUpService, TopUpService>();
builder.Services.AddScoped<ITopUpRepository, TopUpRepository>();
builder.Services.AddScoped<IUserWebService, UserWebService>();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });

    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //option.IncludeXmlComments(xmlPath);
});

builder.Services.AddApiVersioning(setup =>
{
   setup.DefaultApiVersion = new ApiVersion(1, 0);
   setup.AssumeDefaultVersionWhenUnspecified = true;
   setup.ReportApiVersions = true;
   setup.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                     new HeaderApiVersionReader("x-api-version"),
                                                     new MediaTypeApiVersionReader("x-api-version"));
}); ;

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


 public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions(
        IApiVersionDescriptionProvider provider)
    {
        this.provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        // add swagger document for every API version discovered
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description));
        }
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private OpenApiInfo CreateVersionInfo(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "TopUp Api",
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}

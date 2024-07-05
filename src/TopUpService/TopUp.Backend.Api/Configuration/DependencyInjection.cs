using Backend.TopUp.Application.Services;
using Backend.TopUp.Core.Application.Services;
using Backend.TopUp.Core.Extensions;
using Backend.TopUp.Core.Infrastruture.Configuration;
using Backend.TopUp.Core.Infrastruture.Repositories;
using Backend.TopUp.Core.Infrastruture.WebServices;
using Backend.TopUp.Core.Messages;
using Backend.TopUp.Infrastructure.Configuration.Db;
using Backend.TopUp.Infrastructure.Repositories;
using Backend.TopUp.Infrastructure.Repository;
using Backend.TopUp.Infrastructure.WebServices;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Refit;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Backend.TopUp.Api.Configuration
{
    public static class DependencyInjection
    {
        // todo: split all this configuration in separeted D.I. extension classes like this
        public static IServiceCollection AddCustomDependencyInjection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseNpgsql(GetFromEnvOrConfig("CONNECTION_STRING", "ConnectionStrings:postgres", configuration)));

            services.AddRouting(opt => opt.LowercaseUrls = true);

            services.AddScoped<IDatabaseContext, DatabaseContext>();

            services.AddScoped<ITopUpService, TopUpService>();

            services.AddScoped<ITopUpBeneficiaryRepository, TopUpBeneficiaryRepository>();
            services.AddScoped<ITopUpOptionRepository, TopUpOptionRepository>();
            services.AddScoped<ITopUpTransactionRepository, TopUpTransactionRepository>();

            services.AddScoped<IUserWebService, UserWebService>();
            services.AddScoped<IBankAccountWebService, BankAccountWebService>();

            services.AddScoped<IDateTimeExtensions, DateTimeExtensions>();

            services.AddRefitClient<IBankAccountApi>().ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(
                    GetFromEnvOrConfig("ACCOUNT_API_BASE_ADDRESS", "accountApi:baseAddress", configuration) + ":" + GetFromEnvOrConfig("ACCOUNT_API_PORT", "accountApi:port", configuration));

            });

            services.ConfigureOptions<ConfigureSwaggerOptions>();

            services.AddSwaggerGen(option =>
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

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                option.IncludeXmlComments(xmlPath);
            });

            services.AddApiVersioning(setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
                setup.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                                  new HeaderApiVersionReader("x-api-version"),
                                                                  new MediaTypeApiVersionReader("x-api-version"));
            }); ;

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            });

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(GetFromEnvOrConfig("RABBIT_MQ_HOST", "rabbitmq:host", configuration), "/",
                        h =>
                        {
                            h.Username(GetFromEnvOrConfig("RABBIT_MQ_USER", "rabbitmq:user", configuration));
                            h.Password(GetFromEnvOrConfig("RABBIT_MQ_PASS", "rabbitmq:pass", configuration));
                        });

                    cfg.Message<TopUpTransactionFailed>(x => x.SetEntityName("topup:transaction:failed"));

                    cfg.ConfigureEndpoints(context);
                });
            });

            return services;
        }

        public static string GetFromEnvOrConfig(string envVar, string appSettingsPath, IConfiguration configuration)
            => Environment.GetEnvironmentVariable(envVar)
                        ?? configuration[appSettingsPath]
                        ?? throw new InvalidOperationException("It was not possible to retrieve some configuration");

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
    }
}

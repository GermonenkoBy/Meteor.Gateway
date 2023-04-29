using System.Text;
using Grpc.Core;
using Mapster;
using MapsterMapper;
using Meteor.Common.Messaging.DependencyInjection.Extensions;
using Meteor.Gateway.Api.Middleware;
using Meteor.Gateway.Core.Contracts;
using Meteor.Gateway.Core.Dtos;
using Meteor.Gateway.Core.Services;
using Meteor.Gateway.Core.Services.Abstractions;
using Meteor.Gateway.Infrastructure.Contracts;
using Meteor.Gateway.Infrastructure.Grpc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

var azureAppConfigurationConnectionString = builder.Configuration
    .GetValue<string>("ConnectionStrings:AzureAppConfiguration");

if (!string.IsNullOrEmpty(azureAppConfigurationConnectionString))
{
    builder.Configuration.AddAzureAppConfiguration(options =>
        options
            .Connect(azureAppConfigurationConnectionString)
            .UseFeatureFlags()
            .Select(KeyFilter.Any)
            .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
            .Select(KeyFilter.Any, $"{builder.Environment.EnvironmentName}-Gateway")
    );
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var serviceBusConnectionString = builder.Configuration.GetConnectionString("AzureServiceBus") ?? string.Empty;
builder.Services.AddAzureClients(azureBuilder =>
{
    azureBuilder.AddServiceBusClient(serviceBusConnectionString);
});

builder.Services.AddServiceBusPublisher<MigrationsTriggerDto>(options =>
{
    options.SenderName = "Gateway";
    options.TopicName = "migrations";
});

var jwtSection = builder.Configuration.GetSection("Security:Jwt");
var jwtSecretKey = jwtSection.GetValue<string>("SecretKey");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = jwtSection.GetValue<bool>("ValidateIssuer"),
            ValidateAudience = jwtSection.GetValue<bool>("ValidateAudience"),
            ValidateLifetime = jwtSection.GetValue<bool>("ValidateLifetime"),
            RequireExpirationTime = jwtSection.GetValue<bool>("RequireExpirationTime"),
            ClockSkew = TimeSpan.FromSeconds(5),
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey ?? "")
            ),
        };

        if (options.TokenValidationParameters.ValidateIssuer)
        {
            options.TokenValidationParameters.ValidIssuer = jwtSection.GetValue<string>("Issuer");
        }

        if (options.TokenValidationParameters.ValidateAudience)
        {
            options.TokenValidationParameters.ValidAudience = jwtSection.GetValue<string>("Audience");
        }
    });

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new()
    {
        Title = "Meteor Gateway",
        Description = "The meteor gateway microservice REST-like API.",
        Version = "0.1.0",
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put ",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddGrpcClient<SessionsService.SessionsServiceClient>(options =>
{
    var url = builder.Configuration.GetValue<string>("Routing:SessionsServiceUrl") ?? string.Empty;
    options.Address = new Uri(url);
    if (options.Address.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
    {
        options.ChannelOptionsActions.Add(opt => opt.Credentials = ChannelCredentials.Insecure);
    }
});

var mapperConfig = new TypeAdapterConfig();
mapperConfig.Apply(new Meteor.Gateway.Infrastructure.Mapping.MappingRegister());
builder.Services.AddSingleton<IMapper>(new Mapper(mapperConfig));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
builder.Services.AddScoped<ISessionsClient, GrpcSessionsClient>();
builder.Services.AddScoped<IMigrationsTriggerService, MigrationsTriggerService>();

builder.Services.AddScoped<ExceptionsMiddleware>();
builder.Services.AddSingleton<RequestBufferingMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocExpansion(DocExpansion.None);
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = "swagger";
});

app.UseMiddleware<RequestBufferingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseMiddleware<ExceptionsMiddleware>();
app.MapControllers();
app.Run();
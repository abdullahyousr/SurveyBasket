using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SurveyBasket.Api.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using SurveyBasket.Api.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using Hangfire;
using SurveyBasket.Api.Health;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace SurveyBasket.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();

        services.AddDistributedMemoryCache();
        
        var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
            options.AddDefaultPolicy(builder => 
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            )
        );

        services.AddAuthconfig(configuration);

        services
            .AddSwaggerServices()
            .AddMapsterConfig()
            .AddFluentValidationConfig();

        services.AddDatabaseConfig(configuration);

        services.AddScoped<IPollService, PollService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IQuestionService, QuestionService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IVoteService, VoteService>();
        services.AddScoped<IResultService, ResultService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<ICasheService, CasheService>();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        services.AddProblemDetails();

        services.AddHttpContextAccessor();

        services.AddBackgroundJobsConfig(configuration);

        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        services.AddHealthChecks()
            .AddSqlServer(name: "database", connectionString: configuration.GetConnectionString("DefaultConnection")!)
            .AddHangfire(options => { options.MinimumAvailableServers = 1; })
            .AddCheck<MailProviderHealthCheck>(name:"mail service");
        services.AddRateLimitingServices();

        return services;
    }

    private static IServiceCollection AddRateLimitingServices(this IServiceCollection services)
    {
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            // ip address limit
            rateLimiterOptions.AddPolicy(RateLimiters.IpLimit, httpCcontext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpCcontext.Connection.RemoteIpAddress?.ToString(),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(20),
                    }
                    )
            );

            // user limit
            rateLimiterOptions.AddPolicy(RateLimiters.UserLimiter, httpCcontext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpCcontext.User.GetUserId(),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 2,
                        Window = TimeSpan.FromSeconds(20),
                    }
                    )
            );
            // Concurrent Limit 
            rateLimiterOptions.AddConcurrencyLimiter(RateLimiters.Concurrency, options =>
            {
                options.PermitLimit = 10;
                options.QueueLimit = 5;
                options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });

            // Token Bucket Limit
            //rateLimiterOptions.AddTokenBucketLimiter("token", Options =>
            //{
            //    Options.TokenLimit = 2;
            //    Options.QueueLimit = 1;
            //    Options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //    Options.ReplenishmentPeriod = TimeSpan.FromSeconds(30);
            //    Options.TokensPerPeriod = 2;
            //    Options.AutoReplenishment = true;
            //});

            // Fixed Window Limit
            //rateLimiterOptions.AddFixedWindowLimiter("fixed", Options =>
            //{
            //    Options.PermitLimit = 5;
            //    Options.QueueLimit = 1;
            //    Options.Window = TimeSpan.FromSeconds(20);
            //    Options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});

            // Sliding Window Limit
            //rateLimiterOptions.AddSlidingWindowLimiter("sliding", Options =>
            //{
            //    Options.PermitLimit = 2;
            //    Options.QueueLimit = 1;
            //    Options.SegmentsPerWindow = 2;
            //    Options.Window = TimeSpan.FromSeconds(20);
            //    Options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            //});
            // IP Address Limiter
        }
        );

        return services;
    }

    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton<IMapper>(new Mapper(mappingConfig));

        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection AddDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection String 'DefaultConnection' not found.");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        return services;
    }

    private static IServiceCollection AddAuthconfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                ValidIssuer = jwtSettings?.Issure,
                ValidAudience = jwtSettings?.Audience,
            };
        });

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 6;
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });

        return services;
    }

    private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("HangFireConnection")));

        services.AddHangfireServer();

        return services;
    }
}

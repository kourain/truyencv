using TruyenCV.Helpers;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Pgvector.EntityFrameworkCore;
using TruyenCV.Services;
namespace TruyenCV
{
    public class Program
    {
        protected static (string?, bool) GetConnectionString(WebApplicationBuilder builder)
        {
            string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            var enableValue = builder.Configuration.GetSection("ConnectionStrings")["Enable"] ?? "false";
            bool isEnabled = enableValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            return (connectionString, isEnabled);
        }
        protected static (string, string) GetRedisConnectionString(WebApplicationBuilder builder)
        {
            var connection = builder.Configuration.GetSection("Redis:RedisConnection").Value ?? "localhost:6379";
            var instanceName = builder.Configuration.GetSection("Redis:RedisInstanceName").Value ?? "TruyenCV";
            return (connection, instanceName);
        }
        protected static void AddDefaultCorsPolicy(WebApplicationBuilder builder)
        {
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]?>();

            if (allowedOrigins is null || allowedOrigins.Length == 0)
            {
                allowedOrigins = [
                    "*"
                ];
            }

            builder.Services.AddCors(option =>
            {
                option.AddDefaultPolicy(policy =>
                    policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }
        protected static void AddCorsPolicy(WebApplicationBuilder builder)
        {
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]?>();

            if (allowedOrigins is null || allowedOrigins.Length == 0)
            {
                allowedOrigins = builder.Configuration.GetSection("AllowedHosts")
                    .Get<string[]?>() ?? [
                        "http://localhost:3000"
                    ];
            }

            builder.Services.AddCors(option =>
            {
                option.AddDefaultPolicy(policy =>
                    policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }
        protected static bool TestRedisConnection(string connectionString)
        {
            bool isConnected = false;
            System.Net.Sockets.TcpClient client = new();
            var parts = connectionString.Split(':');
            string host = parts[0];
            int port = parts.Length > 1 && int.TryParse(parts[1], out int p) ? p : 6379;
            if (IPAddress.TryParse(host, out IPAddress address))
                try
                {
                    Log.Information($"INFO: [REDIS] Testing Redis connection to {connectionString}");
                    client.Connect(address, port);
                    isConnected = true;
                }
                catch { }
                finally
                {
                    client.Close();
                }
            return isConnected;
        }
        protected static void AddJwtBearerAuthentication(WebApplicationBuilder builder)
        {
            // Cấu hình xác thực JWT
            var jwtSettings = builder.Configuration.GetSection("JWT");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["ValidIssuer"];
            var audience = jwtSettings["ValidAudience"];
            var accessTokenExpiry = int.Parse(jwtSettings["AccessTokenExpiryMinutes"]);
            var refreshTokenExpiry = int.Parse(jwtSettings["RefreshTokenExpiryDays"]);
            JwtHelper.Init(secretKey, issuer, audience, accessTokenExpiry, refreshTokenExpiry);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey!)),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            SnowflakeIdGenerator.Init(uint.Parse(builder.Configuration.GetSection("Snowflake:MachineId").Value));
            Directory.CreateDirectory("logs"); // Tạo thư mục logs nếu chưa tồn tại
            var errorLogPath = builder.Environment.IsProduction()
                ? $"logs/{DateTime.Now:yyyyMMdd_HHmmss}_errors.log"
                : "errors.log";
            var warnLogPath = builder.Environment.IsProduction()
                ? $"logs/{DateTime.Now:yyyyMMdd_HHmmss}_warnings.log"
                : "warn.log";
            if(builder.Environment.IsDevelopment())
            {
                File.WriteAllText(errorLogPath, string.Empty); // Xóa nội dung file log cũ khi chạy ở môi trường Development
                File.WriteAllText(warnLogPath, string.Empty); // Xóa nội dung file log cũ khi chạy ở môi trường Development
            }
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
                                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                            .WriteTo.File(errorLogPath, restrictedToMinimumLevel: LogEventLevel.Error)
                            .WriteTo.Logger(lc =>
                                lc.Filter.ByIncludingOnly(evt => evt.Level == LogEventLevel.Warning)
                                    .WriteTo.File(warnLogPath))
                            .CreateLogger();
            builder.Host.UseSerilog();
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            // Cấu hình xác thực JWT
            AddJwtBearerAuthentication(builder);
            if (GetConnectionString(builder) is (string connectionString, bool enable) && enable)
                builder.Services.AddDbContext<Models.AppDataContext>(optionsBuilder =>
                {
                    optionsBuilder.UseNpgsql(connectionString, options =>
                    {
                        options.EnableRetryOnFailure(50);
                        options.UseVector();
                    }).ConfigureWarnings(warnings =>
                    {
                        warnings.Ignore(RelationalEventId.PendingModelChangesWarning);
                    })
                    .EnableDetailedErrors();
                });

            var (redisConnectionString, redisInstanceName) = GetRedisConnectionString(builder);
            // Kiểm tra xem chuỗi kết nối Redis có hợp lệ không
            if (!string.IsNullOrEmpty(redisConnectionString) && TestRedisConnection(redisConnectionString))
            {
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                    options.InstanceName = $"{redisInstanceName}:"; // Nên đặt tên instance khác để phân biệt với các cache khác
                    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions
                    {
                        AbortOnConnectFail = false,
                        ConnectTimeout = 30, // 30 seconds
                        EndPoints = { redisConnectionString }
                    };
                });
                Log.Information($"INFO: [REDIS] Using Redis Cache with connection: {redisConnectionString}");
            }
            else
            {
                Log.Error("ERROR: [REDIS error] Now Use MEMORYCACHE");
                // Fallback to in-memory cache for development or when Redis is not available
                builder.Services.AddDistributedMemoryCache();
                Log.Information("INFO: [CACHE] Using Memory Cache instead of Redis");
            }
            builder.Services.AddHttpClient();
            // Đăng ký Background Services
            builder.Services.AddBackgroundServices();
            // Đăng ký Repositories
            builder.Services.AddRepositories();
            // Đăng ký Services
            builder.Services.Configure<EmbeddingOptions>(builder.Configuration.GetSection("Search:ComicVector"));
            builder.Services.AddServices();

            if (builder.Environment.IsDevelopment())
            {
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                AddDefaultCorsPolicy(builder);
            }
            else
            {
                AddCorsPolicy(builder);
            }
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors();
            // app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            // Add Middleware
            app.AddMiddlewares();
            app.MapControllers();
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
            );
            app.Run();
        }
    }
}

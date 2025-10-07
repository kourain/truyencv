using TruyenCV.Helpers;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Net;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Diagnostics;
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
        protected static void AddCorsPolicy(WebApplicationBuilder builder)
        {
            builder.Services.AddCors(option =>
            {
                option.AddDefaultPolicy(policy =>
                policy.WithOrigins(
                    "https://admin-truyencv.cdms.io.vn",
                    "https://truyencv.cdms.io.vn"
                )
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
                catch (System.Net.Sockets.SocketException _)
                { }
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

            Directory.CreateDirectory("logs"); // Tạo thư mục logs nếu chưa tồn tại
            Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
                                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                            .WriteTo.File($"logs/{DateTime.Now:yyyyMMdd_HHmmss}_errors.log", Serilog.Events.LogEventLevel.Error)
                            .CreateLogger();
            builder.Host.UseSerilog();
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            // Cấu hình xác thực JWT
            AddJwtBearerAuthentication(builder);
            if (GetConnectionString(builder) is (string connectionString, bool enable) && enable)
                builder.Services.AddDbContext<Models.DataContext>(optionsBuilder =>
                {
                    optionsBuilder.UseNpgsql(connectionString, options =>
                    {
                        options.EnableRetryOnFailure(50);
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
            SnowflakeIdGenerator.Init();
            // Đăng ký Background Services
            builder.Services.AddBackgroundServices();
            // Đăng ký Repositories
            builder.Services.AddRepositories();
            // Đăng ký Services
            builder.Services.AddServices();

            if (builder.Environment.IsDevelopment())
            {
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
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

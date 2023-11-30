using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApiTemplate.Data;
using WebApiTemplate.Data.Models.Entities;
using StackExchange.Redis;
using WebApiTemplate.Services.Infrastructure.Cache;
using MassTransit;
using System;
using Microsoft.Extensions.Configuration;
using WebApiTemplate.Web.Common.Models.Config;
using System.Linq;
using WebApiTemplate.Services.Infrastructure.MessageQueue;
using WebApiTemplate.Services.Models.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApiTemplate.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			//Configure DbContext
			builder.Services.AddDbContext<ApplicationDbContext>(opt =>
			opt.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

			//Identity
			builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Default Lockout settings.
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;
			});

			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Default Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 8;
				options.Password.RequiredUniqueChars = 1;
			});

			var tokenSection = builder.Configuration.GetSection(nameof(AuthTokenConfig));
			var tokenConfig = tokenSection.Get<AuthTokenConfig>();
			builder.Services.Configure<AuthTokenConfig>(tokenSection);

			builder.Services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).AddJwtBearer(o =>
			{
				var key = Encoding.UTF8.GetBytes(tokenConfig.Key);
				o.SaveToken = true;
				o.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = tokenConfig.Issuer,
					ValidAudience = tokenConfig.Audience,
					IssuerSigningKey = new SymmetricSecurityKey(key)
				};
			});

			builder.Services.AddCors(c =>
			{
				c.AddPolicy(name: "AllowOrigins",
				 options =>
				 {
					 options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
				 });

			});


			//Redis
			var redisConnectionString = builder.Configuration.GetSection("RedisCache").Value;

			var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
			builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
			builder.Services.AddSingleton<ICacheService, RedisCacheService>();

			builder.Services.AddStackExchangeRedisCache(opt =>
			{
				opt.Configuration = redisConnectionString;
				opt.InstanceName = nameof(WebApiTemplate);
			});

			//MQ

			builder.Services.AddTransient<IMessageService, MessageService>();
			var mqModel = builder.Configuration.GetSection(nameof(RabbitMqConfig)).Get<RabbitMqConfig>();

			var consumers = typeof(Program).Assembly
				.GetExportedTypes()
				.Where(type => typeof(IConsumer).IsAssignableFrom(type))
				.ToList();

			builder.Services.AddMassTransit(mt =>
			{
				consumers.ForEach(consumer => mt.AddConsumer(consumer).Endpoint(endpointConfig =>
				{
					endpointConfig.Name = consumer.Name;
				}));

				mt.UsingRabbitMq((context, cfg) =>
				{
					cfg.Host(mqModel.Host, mqModel.VirtualHost, h =>
					{
						h.Username(mqModel.User);
						h.Password(mqModel.Password);
					});

					consumers.ForEach(c => cfg.ReceiveEndpoint(c.FullName!, endpoint =>
					{
						endpoint.ConfigureConsumer(context, c);
					}));
				});
			});


			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			using (var serviceScope = app.Services.CreateScope())
			{
				var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
				dbContext.Database.Migrate();
			}

			app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
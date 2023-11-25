using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiTemplate.Data;
using WebApiTemplate.Data.Models.Entities;

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

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using WebApiTemplate.Data;
using WebApiTemplate.Data.Models.Entities;
using WebApiTemplate.Services.Infrastructure.Cache;

namespace WebApiTemplate.Web.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private readonly ApplicationDbContext dbContext;
		private static readonly string[] Summaries = new[]
		{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};
		private readonly ICacheService cache;
		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext db, ICacheService cache)
		{
			_logger = logger;
			this.dbContext = db;
			this.cache = cache;
		}

		[HttpGet(Name = "GetWeatherForecast")]
		public IEnumerable<WeatherForecast> Get()
		{
			return Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays(index),
				TemperatureC = Random.Shared.Next(-20, 55),
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray();
		}

		[HttpGet]
		[Route("getAll")]
		public async Task<IActionResult> GetAll()
		{
			return this.Ok(this.dbContext.Users.ToList());
		}

		[HttpGet]
		[Route("testRedisCache")]
		public async Task<IActionResult> TestRedisCache()
		{
			var result = await this.cache.GetOrSetAsync<ApplicationUser>("User",() =>
			{
				 return this.dbContext.Users.FirstOrDefaultAsync();
			}, 1000);

			return this.Ok(result);
		}
	}
}
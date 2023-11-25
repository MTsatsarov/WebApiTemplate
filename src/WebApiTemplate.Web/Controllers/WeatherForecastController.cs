using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using WebApiTemplate.Data;
using WebApiTemplate.Data.Models.Entities;

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

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger, ApplicationDbContext db)
		{
			_logger = logger;
			this.dbContext = db;
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
	}
}
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using WebApiTemplate.Data;

namespace WebApiTemplate.Tests.Common
{
	[SetUpFixture]
	public class DbHelper
	{
		private readonly string connectionString = 
			"Server=.\\SQLEXPRESS;Database=WebApiTemplateTestContext;Trusted_Connection=True;MultipleActiveResultSets=true";

		private static ApplicationDbContext db;

		[OneTimeSetUp]
		public void GlobalSetup()
		{
			db = CreateDbContext();
		}

		[OneTimeTearDown]
		public void GlobalTeardown()
		{
			db.Database.EnsureDeleted();
			db.Dispose();
		}

		private ApplicationDbContext CreateDbContext()
		{
			var dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseSqlServer(connectionString)
				.Options;

			return new ApplicationDbContext(dbContextOptions);
		}

		public static ApplicationDbContext GetDbContext() => db;
	}
}

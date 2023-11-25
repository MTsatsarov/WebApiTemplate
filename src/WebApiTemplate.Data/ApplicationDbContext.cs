using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApiTemplate.Data.Common.Interfaces;
using WebApiTemplate.Data.Models.Entities;

namespace WebApiTemplate.Data
{

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
	{
		private static readonly MethodInfo SetIsDeletedQueryFilterMethod =
		typeof(ApplicationDbContext).GetMethod(
		   nameof(SetIsDeletedQueryFilter),
		   BindingFlags.NonPublic | BindingFlags.Static);

		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}

		public override Task<int> SaveChangesAsync(
		 bool acceptAllChangesOnSuccess,
		 CancellationToken cancellationToken = default)
		{
			this.ApplyIModifier();
			return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);


			var entityTypes = builder.Model.GetEntityTypes().ToList();

			// Set global query filter for not deleted entities only
			var deletableEntityTypes = entityTypes
				.Where(et => et.ClrType != null && typeof(IDeletableEntity).IsAssignableFrom(et.ClrType));
			foreach (var deletableEntityType in deletableEntityTypes)
			{
				var method = SetIsDeletedQueryFilterMethod.MakeGenericMethod(deletableEntityType.ClrType);
				method.Invoke(null, new object[] { builder });
			}

			// Disable cascade delete
			var foreignKeys = entityTypes
				.SelectMany(e => e.GetForeignKeys().Where(f => f.DeleteBehavior == DeleteBehavior.Cascade));
			foreach (var foreignKey in foreignKeys)
			{
				foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
			}
		}

		private static void SetIsDeletedQueryFilter<T>(ModelBuilder builder)
		   where T : class, IDeletableEntity
		{
			builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted);
		}


		private void ApplyIModifier()
		{
			var changedEntries = this.ChangeTracker
				.Entries()
				.Where(e =>
					e.Entity is IModifier &&
					(e.State == EntityState.Added || e.State == EntityState.Modified));

			foreach (var entry in changedEntries)
			{
				var entity = (IModifier)entry.Entity;
				if (entry.State == EntityState.Added && entity.CreatedOn == default)
				{
					entity.CreatedOn = DateTime.UtcNow;
				}
				else
				{
					entity.ModifiedOn = DateTime.UtcNow;
				}
			}
		}

	}
}

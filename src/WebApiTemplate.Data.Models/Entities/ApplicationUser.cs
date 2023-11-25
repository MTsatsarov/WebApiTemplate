using Microsoft.AspNetCore.Identity;
using WebApiTemplate.Data.Common.Interfaces;

namespace WebApiTemplate.Data.Models.Entities
{
	public class ApplicationUser : IdentityUser<string>, IDeletableEntity, IModifier
	{
		public DateTime CreatedOn { get; set; }

		public DateTime ModifiedOn { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? DeletedOn { get; set; }
	}
}

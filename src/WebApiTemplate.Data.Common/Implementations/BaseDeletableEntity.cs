using System.ComponentModel.DataAnnotations;
using WebApiTemplate.Data.Common.Interfaces;

namespace WebApiTemplate.Data.Common.Implementations
{
	public class BaseDeletableEntity<TId> : IDeletableEntity, IModifier
	{

		[Key]
		public TId Id { get; set; }

		public DateTime CreatedOn { get; set; }

		public DateTime ModifiedOn { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? DeletedOn { get; set; }
	}
}

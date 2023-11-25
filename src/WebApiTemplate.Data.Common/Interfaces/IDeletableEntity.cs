namespace WebApiTemplate.Data.Common.Interfaces
{
	public interface IDeletableEntity
	{
		bool IsDeleted { get; set; }

		DateTime? DeletedOn { get; set; }
	}
}

namespace WebApiTemplate.Data.Common.Interfaces
{
	public interface IModifier
	{
		DateTime CreatedOn { get; set; }

		DateTime ModifiedOn { get; set; }
	}
}

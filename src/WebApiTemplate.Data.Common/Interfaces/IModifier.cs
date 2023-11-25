namespace WebApiTemplate.Data.Common.Interfaces
{
	public interface IModifier
	{
		DateTime CreateOn { get; set; }

		DateTime ModifiedOn { get; set; }
	}
}

using System.Text;

namespace WebApiTemplate.Common.Helpers
{
	public static class StringHelper
	{
		public static byte[] ToByteArray(this string sourceString)
		{
			var encoding = new UTF8Encoding();
			return encoding.GetBytes(sourceString);
		}
	}
}

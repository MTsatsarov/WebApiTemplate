using AutoMapper;
using System.Reflection;
using WebApiTemplate.Services.Mappings.Interfaces;
using WebApiTemplate.Services.Mappings.Models;

namespace WebApiTemplate.Services.Mappings
{
	public static class WebApiTemplateProfile
	{
		public static IMapper mapper;

		public static void RegisterMappings(Assembly assembly )
		{
			var types = assembly.GetExportedTypes().ToList();

			var config = new MapperConfigurationExpression();
			config.CreateProfile("WebApiTemplateProfile",
				configuration =>
				{


				});
		}

		private static IEnumerable<MapTypes> GetFromMaps(IEnumerable<Type> types)
		{
			var fromMaps = from t in types
						   from i in t.GetTypeInfo().GetInterfaces()
						   where i.GetTypeInfo().IsGenericType &&
								 i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
								 !t.GetTypeInfo().IsAbstract &&
								 !t.GetTypeInfo().IsInterface
						   select new MapTypes
						   {
							   From = i.GetTypeInfo().GetGenericArguments()[0],
							   To = t,
						   };

			return fromMaps;
		}

		private static IEnumerable<MapTypes> GetToMaps(IEnumerable<Type> types)
		{
			var toMaps = from t in types
						 from i in t.GetTypeInfo().GetInterfaces()
						 where i.GetTypeInfo().IsGenericType &&
							   i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IMapTo<>) &&
							   !t.GetTypeInfo().IsAbstract &&
							   !t.GetTypeInfo().IsInterface
						 select new MapTypes
						 {
							 From = t,
							 To = i.GetTypeInfo().GetGenericArguments()[0],
						 };

			return toMaps;
		}
	}
}

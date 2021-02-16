using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Sisus.Compatibility;

namespace Sisus
{
	public static class Attribute<TAttribute> where TAttribute : Attribute
	{
		#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
		private static readonly System.Collections.Concurrent.ConcurrentDictionary<MemberInfo, IEnumerable<TAttribute>> cache = new System.Collections.Concurrent.ConcurrentDictionary<MemberInfo, IEnumerable<TAttribute>>(4, 128);
		private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, IEnumerable<TAttribute>> cacheForClasses = new System.Collections.Concurrent.ConcurrentDictionary<Type, IEnumerable<TAttribute>>(4, 128);
		#endif

		public static bool ExistsOn([NotNull]MemberInfo member)
		{
			IEnumerable<TAttribute> attributes;

			#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
			if(!cache.TryGetValue(member, out attributes))
			#endif
			{
				#if CSHARP_7_3_OR_NEWER
				attributes = member.GetCustomAttributes<TAttribute>(false);
				#else
				attributes = member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
				#endif

				AddAliases(member, ref attributes);

				#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
				cache[member] = attributes;
				#endif
			}

			return attributes.Any();
		}

		public static bool ExistsOn([NotNull]Type classType)
		{
			IEnumerable<TAttribute> attributes;

			#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
			if(!cacheForClasses.TryGetValue(classType, out attributes))
			#endif
			{
				#if CSHARP_7_3_OR_NEWER
				attributes = classType.GetCustomAttributes<TAttribute>(false);
				#else
				attributes = classType.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
				#endif

				AddAliases(classType, ref attributes);

				#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
				cacheForClasses[classType] = attributes;
				#endif
			}

			if(attributes.Any())
			{
				return true;
			}

			return false;
		}

		[CanBeNull]
		public static TAttribute Get([NotNull]MemberInfo member)
		{
			IEnumerable<TAttribute> attributes;

			#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
			if(!cache.TryGetValue(member, out attributes))
			#endif
			{
				#if CSHARP_7_3_OR_NEWER
				attributes = member.GetCustomAttributes<TAttribute>(false);
				#else
				attributes = member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
				#endif

				AddAliases(member, ref attributes);

				#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
				cache[member] = attributes;
				#endif
			}

			return attributes.FirstOrDefault();
		}

		public static bool TryGet([NotNull]MemberInfo member, [CanBeNull]out TAttribute result)
		{
			IEnumerable<TAttribute> attributes;

			#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
			if(!cache.TryGetValue(member, out attributes))
			#endif
			{
				#if CSHARP_7_3_OR_NEWER
				attributes = member.GetCustomAttributes<TAttribute>(false);
				#else
				attributes = member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
				#endif

				AddAliases(member, ref attributes);

				#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
				cache[member] = attributes;
				#endif
			}

			result = attributes.FirstOrDefault();
			return result != null;
		}

		public static void ForEach([NotNull]MemberInfo member, [NotNull]Action<TAttribute> action)
		{
			IEnumerable<TAttribute> attributes;
			
			#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
			if(!cache.TryGetValue(member, out attributes))
			#endif
			{
				#if CSHARP_7_3_OR_NEWER
				attributes = member.GetCustomAttributes<TAttribute>(false);
				#else
				attributes = member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
				#endif

				AddAliases(member, ref attributes);

				#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
				cache[member] = attributes;
				#endif
			}

			foreach(var attribute in attributes)
			{
				action(attribute);
			}
		}

		public static bool TryGetAll([NotNull]MemberInfo member, [NotNull]out IEnumerable<TAttribute> result)
		{
			IEnumerable<TAttribute> attributes;

			#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
			if(!cache.TryGetValue(member, out attributes))
			#endif
			{
				#if CSHARP_7_3_OR_NEWER
				attributes = member.GetCustomAttributes<TAttribute>(false);
				#else
				attributes = member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
				#endif

				AddAliases(member, ref attributes);

				#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
				cache[member] = attributes;
				#endif
			}

			result = attributes;
			return result.Any();
		}

		public static bool TryGetAll([NotNull]Type classType, [NotNull]out IEnumerable<TAttribute> result)
		{
			IEnumerable<TAttribute> attributes;

			#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
			if(!cacheForClasses.TryGetValue(classType, out attributes))
			#endif
			{
				//#if CSHARP_7_3_OR_NEWER
				//attributes = classType.GetCustomAttributes<TAttribute>(false); // UPDATE: This doesn't seem to be thread safe!
				//#else 
				attributes = classType.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
				//#endif

				AddAliases(classType, ref attributes);

				#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
				cacheForClasses[classType] = attributes;
				#endif
			}

			result = attributes;
			return result.Any();
		}

		[NotNull]
		public static IEnumerable<TAttribute> GetAll([NotNull]MemberInfo member)
		{
			IEnumerable<TAttribute> attributes;

			#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
			if(!cache.TryGetValue(member, out attributes))
			#endif
			{
				#if CSHARP_7_3_OR_NEWER
				attributes = member.GetCustomAttributes<TAttribute>(false);
				#else
				attributes = member.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
				#endif

				AddAliases(member, ref attributes);

				#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
				cache[member] = attributes;
				#endif
			}

			return attributes;
		}

		private static void AddAliases([NotNull]MemberInfo member, [NotNull]ref IEnumerable<TAttribute> attributes)
		{
			var aliasAttributeTypes = PluginAttributeConverterProvider.GetAliases(typeof(TAttribute));
			for(int n = aliasAttributeTypes.Length - 1; n >= 0; n--)
			{
				var aliasAttributeInstances = member.GetCustomAttributes(aliasAttributeTypes[n], false);
				for(int a = aliasAttributeInstances.Length - 1; a >= 0; a--)
				{
					TAttribute converted;
					if(PluginAttributeConverterProvider.TryConvert(aliasAttributeInstances[a], out converted))
					{
						#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
						attributes = attributes.Append(converted);
						#else
						attributes = Append(attributes, converted);
						#endif
					}
				}
			}
		}

		private static void AddAliases([NotNull]Type classType, [NotNull]ref IEnumerable<TAttribute> attributes)
		{
			var aliasAttributeTypes = PluginAttributeConverterProvider.GetAliases(typeof(TAttribute));
			for(int n = aliasAttributeTypes.Length - 1; n >= 0; n--)
			{
				var aliasAttributeInstances = classType.GetCustomAttributes(aliasAttributeTypes[n], false);
				for(int a = aliasAttributeInstances.Length - 1; a >= 0; a--)
				{
					TAttribute converted;
					if(PluginAttributeConverterProvider.TryConvert(aliasAttributeInstances[a], out converted))
					{
						#if !NET_2_0 && !NET_2_0_SUBSET && !NET_STANDARD_2_0
						attributes = attributes.Append(converted);
						#else
						attributes = Append(attributes, converted);
						#endif
					}
				}
			}
		}

		#if NET_2_0 || NET_2_0_SUBSET || NET_STANDARD_2_0
		private static IEnumerable<TAttribute> Append(IEnumerable<TAttribute> ienumerable, TAttribute attribute)
		{
			foreach(var existing in ienumerable)
			{
				yield return existing;
			}
			yield return attribute;
		}
		#endif
	}
}
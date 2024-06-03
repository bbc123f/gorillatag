using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BuildSafe
{
	public static class Reflection
	{
		public static Assembly[] AllAssemblies
		{
			get
			{
				return Reflection.PreFetchAllAssemblies();
			}
		}

		public static Type[] AllTypes
		{
			get
			{
				return Reflection.PreFetchAllTypes();
			}
		}

		static Reflection()
		{
			Reflection.PreFetchAllAssemblies();
			Reflection.PreFetchAllTypes();
		}

		private static Assembly[] PreFetchAllAssemblies()
		{
			if (Reflection.gAssemblyCache != null)
			{
				return Reflection.gAssemblyCache;
			}
			return Reflection.gAssemblyCache = (from a in AppDomain.CurrentDomain.GetAssemblies()
			where a != null
			select a).ToArray<Assembly>();
		}

		private static Type[] PreFetchAllTypes()
		{
			if (Reflection.gTypeCache != null)
			{
				return Reflection.gTypeCache;
			}
			return Reflection.gTypeCache = (from t in Reflection.PreFetchAllAssemblies().SelectMany((Assembly a) => a.GetTypes())
			where t != null
			select t).ToArray<Type>();
		}

		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return (from m in Reflection.AllTypes.SelectMany((Type t) => t.GetRuntimeMethods())
			where m.GetCustomAttributes(typeof(T), false).Length != 0
			select m).ToArray<MethodInfo>();
		}

		private static Assembly[] gAssemblyCache;

		private static Type[] gTypeCache;

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c
		{
			// Note: this type is marked as 'beforefieldinit'.
			static <>c()
			{
			}

			public <>c()
			{
			}

			internal bool <PreFetchAllAssemblies>b__7_0(Assembly a)
			{
				return a != null;
			}

			internal IEnumerable<Type> <PreFetchAllTypes>b__8_0(Assembly a)
			{
				return a.GetTypes();
			}

			internal bool <PreFetchAllTypes>b__8_1(Type t)
			{
				return t != null;
			}

			public static readonly Reflection.<>c <>9 = new Reflection.<>c();

			public static Func<Assembly, bool> <>9__7_0;

			public static Func<Assembly, IEnumerable<Type>> <>9__8_0;

			public static Func<Type, bool> <>9__8_1;
		}

		[CompilerGenerated]
		[Serializable]
		private sealed class <>c__9<T> where T : Attribute
		{
			// Note: this type is marked as 'beforefieldinit'.
			static <>c__9()
			{
			}

			public <>c__9()
			{
			}

			internal IEnumerable<MethodInfo> <GetMethodsWithAttribute>b__9_0(Type t)
			{
				return t.GetRuntimeMethods();
			}

			internal bool <GetMethodsWithAttribute>b__9_1(MethodInfo m)
			{
				return m.GetCustomAttributes(typeof(T), false).Length != 0;
			}

			public static readonly Reflection.<>c__9<T> <>9 = new Reflection.<>c__9<T>();

			public static Func<Type, IEnumerable<MethodInfo>> <>9__9_0;

			public static Func<MethodInfo, bool> <>9__9_1;
		}
	}
}

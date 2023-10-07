using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x020002C2 RID: 706
	public static class Reflection
	{
		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06001311 RID: 4881 RVA: 0x0006E6F0 File Offset: 0x0006C8F0
		public static Assembly[] AllAssemblies
		{
			get
			{
				return Reflection.PreFetchAllAssemblies();
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06001312 RID: 4882 RVA: 0x0006E6F7 File Offset: 0x0006C8F7
		public static Type[] AllTypes
		{
			get
			{
				return Reflection.PreFetchAllTypes();
			}
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x0006E6FE File Offset: 0x0006C8FE
		static Reflection()
		{
			Reflection.PreFetchAllAssemblies();
			Reflection.PreFetchAllTypes();
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x0006E70C File Offset: 0x0006C90C
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

		// Token: 0x06001315 RID: 4885 RVA: 0x0006E760 File Offset: 0x0006C960
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

		// Token: 0x06001316 RID: 4886 RVA: 0x0006E7D4 File Offset: 0x0006C9D4
		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return (from m in Reflection.AllTypes.SelectMany((Type t) => t.GetRuntimeMethods())
			where m.GetCustomAttributes(typeof(T), false).Length != 0
			select m).ToArray<MethodInfo>();
		}

		// Token: 0x040015FC RID: 5628
		private static Assembly[] gAssemblyCache;

		// Token: 0x040015FD RID: 5629
		private static Type[] gTypeCache;
	}
}

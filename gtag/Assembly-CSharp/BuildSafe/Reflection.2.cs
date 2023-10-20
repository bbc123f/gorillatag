using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x020002C4 RID: 708
	public static class Reflection
	{
		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06001318 RID: 4888 RVA: 0x0006EBBC File Offset: 0x0006CDBC
		public static Assembly[] AllAssemblies
		{
			get
			{
				return Reflection.PreFetchAllAssemblies();
			}
		}

		// Token: 0x17000124 RID: 292
		// (get) Token: 0x06001319 RID: 4889 RVA: 0x0006EBC3 File Offset: 0x0006CDC3
		public static Type[] AllTypes
		{
			get
			{
				return Reflection.PreFetchAllTypes();
			}
		}

		// Token: 0x0600131A RID: 4890 RVA: 0x0006EBCA File Offset: 0x0006CDCA
		static Reflection()
		{
			Reflection.PreFetchAllAssemblies();
			Reflection.PreFetchAllTypes();
		}

		// Token: 0x0600131B RID: 4891 RVA: 0x0006EBD8 File Offset: 0x0006CDD8
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

		// Token: 0x0600131C RID: 4892 RVA: 0x0006EC2C File Offset: 0x0006CE2C
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

		// Token: 0x0600131D RID: 4893 RVA: 0x0006ECA0 File Offset: 0x0006CEA0
		public static MethodInfo[] GetMethodsWithAttribute<T>() where T : Attribute
		{
			return (from m in Reflection.AllTypes.SelectMany((Type t) => t.GetRuntimeMethods())
			where m.GetCustomAttributes(typeof(T), false).Length != 0
			select m).ToArray<MethodInfo>();
		}

		// Token: 0x04001609 RID: 5641
		private static Assembly[] gAssemblyCache;

		// Token: 0x0400160A RID: 5642
		private static Type[] gTypeCache;
	}
}

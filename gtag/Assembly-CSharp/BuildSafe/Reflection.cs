using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x020002C1 RID: 705
	public static class Reflection<T>
	{
		// Token: 0x1700011C RID: 284
		// (get) Token: 0x06001307 RID: 4871 RVA: 0x0006E62C File Offset: 0x0006C82C
		public static Type Type { get; } = typeof(T);

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x06001308 RID: 4872 RVA: 0x0006E633 File Offset: 0x0006C833
		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06001309 RID: 4873 RVA: 0x0006E63A File Offset: 0x0006C83A
		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x0600130A RID: 4874 RVA: 0x0006E641 File Offset: 0x0006C841
		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600130B RID: 4875 RVA: 0x0006E648 File Offset: 0x0006C848
		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		// Token: 0x0600130C RID: 4876 RVA: 0x0006E64F File Offset: 0x0006C84F
		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Reflection<T>.Type.GetRuntimeEvents().ToArray<EventInfo>();
		}

		// Token: 0x0600130D RID: 4877 RVA: 0x0006E673 File Offset: 0x0006C873
		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Reflection<T>.Type.GetRuntimeProperties().ToArray<PropertyInfo>();
		}

		// Token: 0x0600130E RID: 4878 RVA: 0x0006E697 File Offset: 0x0006C897
		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Reflection<T>.Type.GetRuntimeMethods().ToArray<MethodInfo>();
		}

		// Token: 0x0600130F RID: 4879 RVA: 0x0006E6BB File Offset: 0x0006C8BB
		private static FieldInfo[] PreFetchFields()
		{
			if (Reflection<T>.gFieldsCache != null)
			{
				return Reflection<T>.gFieldsCache;
			}
			return Reflection<T>.gFieldsCache = Reflection<T>.Type.GetRuntimeFields().ToArray<FieldInfo>();
		}

		// Token: 0x040015F6 RID: 5622
		private static Type gCachedType;

		// Token: 0x040015F7 RID: 5623
		private static MethodInfo[] gMethodsCache;

		// Token: 0x040015F8 RID: 5624
		private static FieldInfo[] gFieldsCache;

		// Token: 0x040015F9 RID: 5625
		private static PropertyInfo[] gPropertiesCache;

		// Token: 0x040015FA RID: 5626
		private static EventInfo[] gEventsCache;
	}
}

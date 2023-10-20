using System;
using System.Linq;
using System.Reflection;

namespace BuildSafe
{
	// Token: 0x020002C3 RID: 707
	public static class Reflection<T>
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x0600130E RID: 4878 RVA: 0x0006EAF8 File Offset: 0x0006CCF8
		public static Type Type { get; } = typeof(T);

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x0600130F RID: 4879 RVA: 0x0006EAFF File Offset: 0x0006CCFF
		public static EventInfo[] Events
		{
			get
			{
				return Reflection<T>.PreFetchEvents();
			}
		}

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x06001310 RID: 4880 RVA: 0x0006EB06 File Offset: 0x0006CD06
		public static MethodInfo[] Methods
		{
			get
			{
				return Reflection<T>.PreFetchMethods();
			}
		}

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x06001311 RID: 4881 RVA: 0x0006EB0D File Offset: 0x0006CD0D
		public static FieldInfo[] Fields
		{
			get
			{
				return Reflection<T>.PreFetchFields();
			}
		}

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x06001312 RID: 4882 RVA: 0x0006EB14 File Offset: 0x0006CD14
		public static PropertyInfo[] Properties
		{
			get
			{
				return Reflection<T>.PreFetchProperties();
			}
		}

		// Token: 0x06001313 RID: 4883 RVA: 0x0006EB1B File Offset: 0x0006CD1B
		private static EventInfo[] PreFetchEvents()
		{
			if (Reflection<T>.gEventsCache != null)
			{
				return Reflection<T>.gEventsCache;
			}
			return Reflection<T>.gEventsCache = Reflection<T>.Type.GetRuntimeEvents().ToArray<EventInfo>();
		}

		// Token: 0x06001314 RID: 4884 RVA: 0x0006EB3F File Offset: 0x0006CD3F
		private static PropertyInfo[] PreFetchProperties()
		{
			if (Reflection<T>.gPropertiesCache != null)
			{
				return Reflection<T>.gPropertiesCache;
			}
			return Reflection<T>.gPropertiesCache = Reflection<T>.Type.GetRuntimeProperties().ToArray<PropertyInfo>();
		}

		// Token: 0x06001315 RID: 4885 RVA: 0x0006EB63 File Offset: 0x0006CD63
		private static MethodInfo[] PreFetchMethods()
		{
			if (Reflection<T>.gMethodsCache != null)
			{
				return Reflection<T>.gMethodsCache;
			}
			return Reflection<T>.gMethodsCache = Reflection<T>.Type.GetRuntimeMethods().ToArray<MethodInfo>();
		}

		// Token: 0x06001316 RID: 4886 RVA: 0x0006EB87 File Offset: 0x0006CD87
		private static FieldInfo[] PreFetchFields()
		{
			if (Reflection<T>.gFieldsCache != null)
			{
				return Reflection<T>.gFieldsCache;
			}
			return Reflection<T>.gFieldsCache = Reflection<T>.Type.GetRuntimeFields().ToArray<FieldInfo>();
		}

		// Token: 0x04001603 RID: 5635
		private static Type gCachedType;

		// Token: 0x04001604 RID: 5636
		private static MethodInfo[] gMethodsCache;

		// Token: 0x04001605 RID: 5637
		private static FieldInfo[] gFieldsCache;

		// Token: 0x04001606 RID: 5638
		private static PropertyInfo[] gPropertiesCache;

		// Token: 0x04001607 RID: 5639
		private static EventInfo[] gEventsCache;
	}
}

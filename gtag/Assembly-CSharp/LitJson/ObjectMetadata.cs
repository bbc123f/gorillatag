using System;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000276 RID: 630
	internal struct ObjectMetadata
	{
		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06001004 RID: 4100 RVA: 0x0005505C File Offset: 0x0005325C
		// (set) Token: 0x06001005 RID: 4101 RVA: 0x0005507D File Offset: 0x0005327D
		public Type ElementType
		{
			get
			{
				if (this.element_type == null)
				{
					return typeof(JsonData);
				}
				return this.element_type;
			}
			set
			{
				this.element_type = value;
			}
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x06001006 RID: 4102 RVA: 0x00055086 File Offset: 0x00053286
		// (set) Token: 0x06001007 RID: 4103 RVA: 0x0005508E File Offset: 0x0005328E
		public bool IsDictionary
		{
			get
			{
				return this.is_dictionary;
			}
			set
			{
				this.is_dictionary = value;
			}
		}

		// Token: 0x170000DD RID: 221
		// (get) Token: 0x06001008 RID: 4104 RVA: 0x00055097 File Offset: 0x00053297
		// (set) Token: 0x06001009 RID: 4105 RVA: 0x0005509F File Offset: 0x0005329F
		public IDictionary<string, PropertyMetadata> Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		// Token: 0x040011EA RID: 4586
		private Type element_type;

		// Token: 0x040011EB RID: 4587
		private bool is_dictionary;

		// Token: 0x040011EC RID: 4588
		private IDictionary<string, PropertyMetadata> properties;
	}
}

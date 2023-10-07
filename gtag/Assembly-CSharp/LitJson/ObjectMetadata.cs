using System;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000275 RID: 629
	internal struct ObjectMetadata
	{
		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06000FFD RID: 4093 RVA: 0x00054C80 File Offset: 0x00052E80
		// (set) Token: 0x06000FFE RID: 4094 RVA: 0x00054CA1 File Offset: 0x00052EA1
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

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06000FFF RID: 4095 RVA: 0x00054CAA File Offset: 0x00052EAA
		// (set) Token: 0x06001000 RID: 4096 RVA: 0x00054CB2 File Offset: 0x00052EB2
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

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x06001001 RID: 4097 RVA: 0x00054CBB File Offset: 0x00052EBB
		// (set) Token: 0x06001002 RID: 4098 RVA: 0x00054CC3 File Offset: 0x00052EC3
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

		// Token: 0x040011E4 RID: 4580
		private Type element_type;

		// Token: 0x040011E5 RID: 4581
		private bool is_dictionary;

		// Token: 0x040011E6 RID: 4582
		private IDictionary<string, PropertyMetadata> properties;
	}
}

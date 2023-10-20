using System;

namespace LitJson
{
	// Token: 0x02000275 RID: 629
	internal struct ArrayMetadata
	{
		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000FFE RID: 4094 RVA: 0x00055010 File Offset: 0x00053210
		// (set) Token: 0x06000FFF RID: 4095 RVA: 0x00055031 File Offset: 0x00053231
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

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x06001000 RID: 4096 RVA: 0x0005503A File Offset: 0x0005323A
		// (set) Token: 0x06001001 RID: 4097 RVA: 0x00055042 File Offset: 0x00053242
		public bool IsArray
		{
			get
			{
				return this.is_array;
			}
			set
			{
				this.is_array = value;
			}
		}

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x06001002 RID: 4098 RVA: 0x0005504B File Offset: 0x0005324B
		// (set) Token: 0x06001003 RID: 4099 RVA: 0x00055053 File Offset: 0x00053253
		public bool IsList
		{
			get
			{
				return this.is_list;
			}
			set
			{
				this.is_list = value;
			}
		}

		// Token: 0x040011E7 RID: 4583
		private Type element_type;

		// Token: 0x040011E8 RID: 4584
		private bool is_array;

		// Token: 0x040011E9 RID: 4585
		private bool is_list;
	}
}

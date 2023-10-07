using System;

namespace LitJson
{
	// Token: 0x02000274 RID: 628
	internal struct ArrayMetadata
	{
		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000FF7 RID: 4087 RVA: 0x00054C34 File Offset: 0x00052E34
		// (set) Token: 0x06000FF8 RID: 4088 RVA: 0x00054C55 File Offset: 0x00052E55
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

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x00054C5E File Offset: 0x00052E5E
		// (set) Token: 0x06000FFA RID: 4090 RVA: 0x00054C66 File Offset: 0x00052E66
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

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x06000FFB RID: 4091 RVA: 0x00054C6F File Offset: 0x00052E6F
		// (set) Token: 0x06000FFC RID: 4092 RVA: 0x00054C77 File Offset: 0x00052E77
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

		// Token: 0x040011E1 RID: 4577
		private Type element_type;

		// Token: 0x040011E2 RID: 4578
		private bool is_array;

		// Token: 0x040011E3 RID: 4579
		private bool is_list;
	}
}

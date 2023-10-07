using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000271 RID: 625
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x06000FE9 RID: 4073 RVA: 0x00054B12 File Offset: 0x00052D12
		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x00054B20 File Offset: 0x00052D20
		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000FEB RID: 4075 RVA: 0x00054B4C File Offset: 0x00052D4C
		public object Key
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Key;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000FEC RID: 4076 RVA: 0x00054B6C File Offset: 0x00052D6C
		public object Value
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Value;
			}
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x00054B8C File Offset: 0x00052D8C
		public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			this.list_enumerator = enumerator;
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x00054B9B File Offset: 0x00052D9B
		public bool MoveNext()
		{
			return this.list_enumerator.MoveNext();
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x00054BA8 File Offset: 0x00052DA8
		public void Reset()
		{
			this.list_enumerator.Reset();
		}

		// Token: 0x040011DD RID: 4573
		private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;
	}
}

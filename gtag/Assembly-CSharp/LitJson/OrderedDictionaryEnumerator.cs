using System;
using System.Collections;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000272 RID: 626
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator, IEnumerator
	{
		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x00054EEE File Offset: 0x000530EE
		public object Current
		{
			get
			{
				return this.Entry;
			}
		}

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x06000FF1 RID: 4081 RVA: 0x00054EFC File Offset: 0x000530FC
		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return new DictionaryEntry(keyValuePair.Key, keyValuePair.Value);
			}
		}

		// Token: 0x170000D6 RID: 214
		// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x00054F28 File Offset: 0x00053128
		public object Key
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Key;
			}
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x06000FF3 RID: 4083 RVA: 0x00054F48 File Offset: 0x00053148
		public object Value
		{
			get
			{
				KeyValuePair<string, JsonData> keyValuePair = this.list_enumerator.Current;
				return keyValuePair.Value;
			}
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x00054F68 File Offset: 0x00053168
		public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			this.list_enumerator = enumerator;
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x00054F77 File Offset: 0x00053177
		public bool MoveNext()
		{
			return this.list_enumerator.MoveNext();
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x00054F84 File Offset: 0x00053184
		public void Reset()
		{
			this.list_enumerator.Reset();
		}

		// Token: 0x040011E3 RID: 4579
		private IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;
	}
}

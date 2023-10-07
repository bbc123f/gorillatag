using System;
using System.Collections;
using System.Collections.Specialized;

namespace LitJson
{
	// Token: 0x0200026F RID: 623
	public interface IJsonWrapper : IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary
	{
		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000F77 RID: 3959
		bool IsArray { get; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x06000F78 RID: 3960
		bool IsBoolean { get; }

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000F79 RID: 3961
		bool IsDouble { get; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000F7A RID: 3962
		bool IsInt { get; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000F7B RID: 3963
		bool IsLong { get; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000F7C RID: 3964
		bool IsObject { get; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000F7D RID: 3965
		bool IsString { get; }

		// Token: 0x06000F7E RID: 3966
		bool GetBoolean();

		// Token: 0x06000F7F RID: 3967
		double GetDouble();

		// Token: 0x06000F80 RID: 3968
		int GetInt();

		// Token: 0x06000F81 RID: 3969
		JsonType GetJsonType();

		// Token: 0x06000F82 RID: 3970
		long GetLong();

		// Token: 0x06000F83 RID: 3971
		string GetString();

		// Token: 0x06000F84 RID: 3972
		void SetBoolean(bool val);

		// Token: 0x06000F85 RID: 3973
		void SetDouble(double val);

		// Token: 0x06000F86 RID: 3974
		void SetInt(int val);

		// Token: 0x06000F87 RID: 3975
		void SetJsonType(JsonType type);

		// Token: 0x06000F88 RID: 3976
		void SetLong(long val);

		// Token: 0x06000F89 RID: 3977
		void SetString(string val);

		// Token: 0x06000F8A RID: 3978
		string ToJson();

		// Token: 0x06000F8B RID: 3979
		void ToJson(JsonWriter writer);
	}
}

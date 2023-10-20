using System;
using System.Collections;
using System.Collections.Specialized;

namespace LitJson
{
	// Token: 0x02000270 RID: 624
	public interface IJsonWrapper : IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary
	{
		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000F7E RID: 3966
		bool IsArray { get; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000F7F RID: 3967
		bool IsBoolean { get; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000F80 RID: 3968
		bool IsDouble { get; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x06000F81 RID: 3969
		bool IsInt { get; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000F82 RID: 3970
		bool IsLong { get; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000F83 RID: 3971
		bool IsObject { get; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000F84 RID: 3972
		bool IsString { get; }

		// Token: 0x06000F85 RID: 3973
		bool GetBoolean();

		// Token: 0x06000F86 RID: 3974
		double GetDouble();

		// Token: 0x06000F87 RID: 3975
		int GetInt();

		// Token: 0x06000F88 RID: 3976
		JsonType GetJsonType();

		// Token: 0x06000F89 RID: 3977
		long GetLong();

		// Token: 0x06000F8A RID: 3978
		string GetString();

		// Token: 0x06000F8B RID: 3979
		void SetBoolean(bool val);

		// Token: 0x06000F8C RID: 3980
		void SetDouble(double val);

		// Token: 0x06000F8D RID: 3981
		void SetInt(int val);

		// Token: 0x06000F8E RID: 3982
		void SetJsonType(JsonType type);

		// Token: 0x06000F8F RID: 3983
		void SetLong(long val);

		// Token: 0x06000F90 RID: 3984
		void SetString(string val);

		// Token: 0x06000F91 RID: 3985
		string ToJson();

		// Token: 0x06000F92 RID: 3986
		void ToJson(JsonWriter writer);
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace LitJson
{
	// Token: 0x02000270 RID: 624
	public class JsonData : IJsonWrapper, IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary, IEquatable<JsonData>
	{
		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000F8C RID: 3980 RVA: 0x00053D0A File Offset: 0x00051F0A
		public int Count
		{
			get
			{
				return this.EnsureCollection().Count;
			}
		}

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000F8D RID: 3981 RVA: 0x00053D17 File Offset: 0x00051F17
		public bool IsArray
		{
			get
			{
				return this.type == JsonType.Array;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000F8E RID: 3982 RVA: 0x00053D22 File Offset: 0x00051F22
		public bool IsBoolean
		{
			get
			{
				return this.type == JsonType.Boolean;
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000F8F RID: 3983 RVA: 0x00053D2D File Offset: 0x00051F2D
		public bool IsDouble
		{
			get
			{
				return this.type == JsonType.Double;
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000F90 RID: 3984 RVA: 0x00053D38 File Offset: 0x00051F38
		public bool IsInt
		{
			get
			{
				return this.type == JsonType.Int;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000F91 RID: 3985 RVA: 0x00053D43 File Offset: 0x00051F43
		public bool IsLong
		{
			get
			{
				return this.type == JsonType.Long;
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000F92 RID: 3986 RVA: 0x00053D4E File Offset: 0x00051F4E
		public bool IsObject
		{
			get
			{
				return this.type == JsonType.Object;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000F93 RID: 3987 RVA: 0x00053D59 File Offset: 0x00051F59
		public bool IsString
		{
			get
			{
				return this.type == JsonType.String;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000F94 RID: 3988 RVA: 0x00053D64 File Offset: 0x00051F64
		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000F95 RID: 3989 RVA: 0x00053D6C File Offset: 0x00051F6C
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.EnsureCollection().IsSynchronized;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000F96 RID: 3990 RVA: 0x00053D79 File Offset: 0x00051F79
		object ICollection.SyncRoot
		{
			get
			{
				return this.EnsureCollection().SyncRoot;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000F97 RID: 3991 RVA: 0x00053D86 File Offset: 0x00051F86
		bool IDictionary.IsFixedSize
		{
			get
			{
				return this.EnsureDictionary().IsFixedSize;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000F98 RID: 3992 RVA: 0x00053D93 File Offset: 0x00051F93
		bool IDictionary.IsReadOnly
		{
			get
			{
				return this.EnsureDictionary().IsReadOnly;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000F99 RID: 3993 RVA: 0x00053DA0 File Offset: 0x00051FA0
		ICollection IDictionary.Keys
		{
			get
			{
				this.EnsureDictionary();
				IList<string> list = new List<string>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Key);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000F9A RID: 3994 RVA: 0x00053E08 File Offset: 0x00052008
		ICollection IDictionary.Values
		{
			get
			{
				this.EnsureDictionary();
				IList<JsonData> list = new List<JsonData>();
				foreach (KeyValuePair<string, JsonData> keyValuePair in this.object_list)
				{
					list.Add(keyValuePair.Value);
				}
				return (ICollection)list;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000F9B RID: 3995 RVA: 0x00053E70 File Offset: 0x00052070
		bool IJsonWrapper.IsArray
		{
			get
			{
				return this.IsArray;
			}
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000F9C RID: 3996 RVA: 0x00053E78 File Offset: 0x00052078
		bool IJsonWrapper.IsBoolean
		{
			get
			{
				return this.IsBoolean;
			}
		}

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000F9D RID: 3997 RVA: 0x00053E80 File Offset: 0x00052080
		bool IJsonWrapper.IsDouble
		{
			get
			{
				return this.IsDouble;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000F9E RID: 3998 RVA: 0x00053E88 File Offset: 0x00052088
		bool IJsonWrapper.IsInt
		{
			get
			{
				return this.IsInt;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000F9F RID: 3999 RVA: 0x00053E90 File Offset: 0x00052090
		bool IJsonWrapper.IsLong
		{
			get
			{
				return this.IsLong;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x00053E98 File Offset: 0x00052098
		bool IJsonWrapper.IsObject
		{
			get
			{
				return this.IsObject;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x00053EA0 File Offset: 0x000520A0
		bool IJsonWrapper.IsString
		{
			get
			{
				return this.IsString;
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000FA2 RID: 4002 RVA: 0x00053EA8 File Offset: 0x000520A8
		bool IList.IsFixedSize
		{
			get
			{
				return this.EnsureList().IsFixedSize;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000FA3 RID: 4003 RVA: 0x00053EB5 File Offset: 0x000520B5
		bool IList.IsReadOnly
		{
			get
			{
				return this.EnsureList().IsReadOnly;
			}
		}

		// Token: 0x170000CD RID: 205
		object IDictionary.this[object key]
		{
			get
			{
				return this.EnsureDictionary()[key];
			}
			set
			{
				if (!(key is string))
				{
					throw new ArgumentException("The key has to be a string");
				}
				JsonData value2 = this.ToJsonData(value);
				this[(string)key] = value2;
			}
		}

		// Token: 0x170000CE RID: 206
		object IOrderedDictionary.this[int idx]
		{
			get
			{
				this.EnsureDictionary();
				return this.object_list[idx].Value;
			}
			set
			{
				this.EnsureDictionary();
				JsonData value2 = this.ToJsonData(value);
				KeyValuePair<string, JsonData> keyValuePair = this.object_list[idx];
				this.inst_object[keyValuePair.Key] = value2;
				KeyValuePair<string, JsonData> value3 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value2);
				this.object_list[idx] = value3;
			}
		}

		// Token: 0x170000CF RID: 207
		object IList.this[int index]
		{
			get
			{
				return this.EnsureList()[index];
			}
			set
			{
				this.EnsureList();
				JsonData value2 = this.ToJsonData(value);
				this[index] = value2;
			}
		}

		// Token: 0x170000D0 RID: 208
		public JsonData this[string prop_name]
		{
			get
			{
				this.EnsureDictionary();
				return this.inst_object[prop_name];
			}
			set
			{
				this.EnsureDictionary();
				KeyValuePair<string, JsonData> keyValuePair = new KeyValuePair<string, JsonData>(prop_name, value);
				if (this.inst_object.ContainsKey(prop_name))
				{
					for (int i = 0; i < this.object_list.Count; i++)
					{
						if (this.object_list[i].Key == prop_name)
						{
							this.object_list[i] = keyValuePair;
							break;
						}
					}
				}
				else
				{
					this.object_list.Add(keyValuePair);
				}
				this.inst_object[prop_name] = value;
				this.json = null;
			}
		}

		// Token: 0x170000D1 RID: 209
		public JsonData this[int index]
		{
			get
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					return this.inst_array[index];
				}
				return this.object_list[index].Value;
			}
			set
			{
				this.EnsureCollection();
				if (this.type == JsonType.Array)
				{
					this.inst_array[index] = value;
				}
				else
				{
					KeyValuePair<string, JsonData> keyValuePair = this.object_list[index];
					KeyValuePair<string, JsonData> value2 = new KeyValuePair<string, JsonData>(keyValuePair.Key, value);
					this.object_list[index] = value2;
					this.inst_object[keyValuePair.Key] = value;
				}
				this.json = null;
			}
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x00054113 File Offset: 0x00052313
		public JsonData()
		{
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x0005411B File Offset: 0x0005231B
		public JsonData(bool boolean)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = boolean;
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x00054131 File Offset: 0x00052331
		public JsonData(double number)
		{
			this.type = JsonType.Double;
			this.inst_double = number;
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x00054147 File Offset: 0x00052347
		public JsonData(int number)
		{
			this.type = JsonType.Int;
			this.inst_int = number;
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x0005415D File Offset: 0x0005235D
		public JsonData(long number)
		{
			this.type = JsonType.Long;
			this.inst_long = number;
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x00054174 File Offset: 0x00052374
		public JsonData(object obj)
		{
			if (obj is bool)
			{
				this.type = JsonType.Boolean;
				this.inst_boolean = (bool)obj;
				return;
			}
			if (obj is double)
			{
				this.type = JsonType.Double;
				this.inst_double = (double)obj;
				return;
			}
			if (obj is int)
			{
				this.type = JsonType.Int;
				this.inst_int = (int)obj;
				return;
			}
			if (obj is long)
			{
				this.type = JsonType.Long;
				this.inst_long = (long)obj;
				return;
			}
			if (obj is string)
			{
				this.type = JsonType.String;
				this.inst_string = (string)obj;
				return;
			}
			throw new ArgumentException("Unable to wrap the given object with JsonData");
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x0005421D File Offset: 0x0005241D
		public JsonData(string str)
		{
			this.type = JsonType.String;
			this.inst_string = str;
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x00054233 File Offset: 0x00052433
		public static implicit operator JsonData(bool data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x0005423B File Offset: 0x0005243B
		public static implicit operator JsonData(double data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x00054243 File Offset: 0x00052443
		public static implicit operator JsonData(int data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x0005424B File Offset: 0x0005244B
		public static implicit operator JsonData(long data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x00054253 File Offset: 0x00052453
		public static implicit operator JsonData(string data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x0005425B File Offset: 0x0005245B
		public static explicit operator bool(JsonData data)
		{
			if (data.type != JsonType.Boolean)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_boolean;
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x00054277 File Offset: 0x00052477
		public static explicit operator double(JsonData data)
		{
			if (data.type != JsonType.Double)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_double;
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x00054293 File Offset: 0x00052493
		public static explicit operator int(JsonData data)
		{
			if (data.type != JsonType.Int)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_int;
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x000542AF File Offset: 0x000524AF
		public static explicit operator long(JsonData data)
		{
			if (data.type != JsonType.Long)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_long;
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x000542CB File Offset: 0x000524CB
		public static explicit operator string(JsonData data)
		{
			if (data.type != JsonType.String)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a string");
			}
			return data.inst_string;
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x000542E7 File Offset: 0x000524E7
		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureCollection().CopyTo(array, index);
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x000542F8 File Offset: 0x000524F8
		void IDictionary.Add(object key, object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.EnsureDictionary().Add(key, value2);
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>((string)key, value2);
			this.object_list.Add(item);
			this.json = null;
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x0005433B File Offset: 0x0005253B
		void IDictionary.Clear()
		{
			this.EnsureDictionary().Clear();
			this.object_list.Clear();
			this.json = null;
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x0005435A File Offset: 0x0005255A
		bool IDictionary.Contains(object key)
		{
			return this.EnsureDictionary().Contains(key);
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x00054368 File Offset: 0x00052568
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IOrderedDictionary)this).GetEnumerator();
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x00054370 File Offset: 0x00052570
		void IDictionary.Remove(object key)
		{
			this.EnsureDictionary().Remove(key);
			for (int i = 0; i < this.object_list.Count; i++)
			{
				if (this.object_list[i].Key == (string)key)
				{
					this.object_list.RemoveAt(i);
					break;
				}
			}
			this.json = null;
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x000543D5 File Offset: 0x000525D5
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnsureCollection().GetEnumerator();
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x000543E2 File Offset: 0x000525E2
		bool IJsonWrapper.GetBoolean()
		{
			if (this.type != JsonType.Boolean)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
			}
			return this.inst_boolean;
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x000543FE File Offset: 0x000525FE
		double IJsonWrapper.GetDouble()
		{
			if (this.type != JsonType.Double)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a double");
			}
			return this.inst_double;
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x0005441A File Offset: 0x0005261A
		int IJsonWrapper.GetInt()
		{
			if (this.type != JsonType.Int)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold an int");
			}
			return this.inst_int;
		}

		// Token: 0x06000FC9 RID: 4041 RVA: 0x00054436 File Offset: 0x00052636
		long IJsonWrapper.GetLong()
		{
			if (this.type != JsonType.Long)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a long");
			}
			return this.inst_long;
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x00054452 File Offset: 0x00052652
		string IJsonWrapper.GetString()
		{
			if (this.type != JsonType.String)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a string");
			}
			return this.inst_string;
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0005446E File Offset: 0x0005266E
		void IJsonWrapper.SetBoolean(bool val)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = val;
			this.json = null;
		}

		// Token: 0x06000FCC RID: 4044 RVA: 0x00054485 File Offset: 0x00052685
		void IJsonWrapper.SetDouble(double val)
		{
			this.type = JsonType.Double;
			this.inst_double = val;
			this.json = null;
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x0005449C File Offset: 0x0005269C
		void IJsonWrapper.SetInt(int val)
		{
			this.type = JsonType.Int;
			this.inst_int = val;
			this.json = null;
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x000544B3 File Offset: 0x000526B3
		void IJsonWrapper.SetLong(long val)
		{
			this.type = JsonType.Long;
			this.inst_long = val;
			this.json = null;
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x000544CA File Offset: 0x000526CA
		void IJsonWrapper.SetString(string val)
		{
			this.type = JsonType.String;
			this.inst_string = val;
			this.json = null;
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x000544E1 File Offset: 0x000526E1
		string IJsonWrapper.ToJson()
		{
			return this.ToJson();
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x000544E9 File Offset: 0x000526E9
		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			this.ToJson(writer);
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x000544F2 File Offset: 0x000526F2
		int IList.Add(object value)
		{
			return this.Add(value);
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x000544FB File Offset: 0x000526FB
		void IList.Clear()
		{
			this.EnsureList().Clear();
			this.json = null;
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x0005450F File Offset: 0x0005270F
		bool IList.Contains(object value)
		{
			return this.EnsureList().Contains(value);
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x0005451D File Offset: 0x0005271D
		int IList.IndexOf(object value)
		{
			return this.EnsureList().IndexOf(value);
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x0005452B File Offset: 0x0005272B
		void IList.Insert(int index, object value)
		{
			this.EnsureList().Insert(index, value);
			this.json = null;
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x00054541 File Offset: 0x00052741
		void IList.Remove(object value)
		{
			this.EnsureList().Remove(value);
			this.json = null;
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x00054556 File Offset: 0x00052756
		void IList.RemoveAt(int index)
		{
			this.EnsureList().RemoveAt(index);
			this.json = null;
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x0005456B File Offset: 0x0005276B
		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			this.EnsureDictionary();
			return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x00054584 File Offset: 0x00052784
		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string text = (string)key;
			JsonData value2 = this.ToJsonData(value);
			this[text] = value2;
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(text, value2);
			this.object_list.Insert(idx, item);
		}

		// Token: 0x06000FDB RID: 4059 RVA: 0x000545C0 File Offset: 0x000527C0
		void IOrderedDictionary.RemoveAt(int idx)
		{
			this.EnsureDictionary();
			this.inst_object.Remove(this.object_list[idx].Key);
			this.object_list.RemoveAt(idx);
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x00054600 File Offset: 0x00052800
		private ICollection EnsureCollection()
		{
			if (this.type == JsonType.Array)
			{
				return (ICollection)this.inst_array;
			}
			if (this.type == JsonType.Object)
			{
				return (ICollection)this.inst_object;
			}
			throw new InvalidOperationException("The JsonData instance has to be initialized first");
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x00054638 File Offset: 0x00052838
		private IDictionary EnsureDictionary()
		{
			if (this.type == JsonType.Object)
			{
				return (IDictionary)this.inst_object;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a dictionary");
			}
			this.type = JsonType.Object;
			this.inst_object = new Dictionary<string, JsonData>();
			this.object_list = new List<KeyValuePair<string, JsonData>>();
			return (IDictionary)this.inst_object;
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x00054698 File Offset: 0x00052898
		private IList EnsureList()
		{
			if (this.type == JsonType.Array)
			{
				return (IList)this.inst_array;
			}
			if (this.type != JsonType.None)
			{
				throw new InvalidOperationException("Instance of JsonData is not a list");
			}
			this.type = JsonType.Array;
			this.inst_array = new List<JsonData>();
			return (IList)this.inst_array;
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x000546EA File Offset: 0x000528EA
		private JsonData ToJsonData(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is JsonData)
			{
				return (JsonData)obj;
			}
			return new JsonData(obj);
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x00054708 File Offset: 0x00052908
		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj.IsString)
			{
				writer.Write(obj.GetString());
				return;
			}
			if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
				return;
			}
			if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
				return;
			}
			if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
				return;
			}
			if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
				return;
			}
			if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object obj2 in obj)
				{
					JsonData.WriteJson((JsonData)obj2, writer);
				}
				writer.WriteArrayEnd();
				return;
			}
			if (obj.IsObject)
			{
				writer.WriteObjectStart();
				foreach (object obj3 in obj)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj3;
					writer.WritePropertyName((string)dictionaryEntry.Key);
					JsonData.WriteJson((JsonData)dictionaryEntry.Value, writer);
				}
				writer.WriteObjectEnd();
				return;
			}
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x00054850 File Offset: 0x00052A50
		public int Add(object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.json = null;
			return this.EnsureList().Add(value2);
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x00054878 File Offset: 0x00052A78
		public void Clear()
		{
			if (this.IsObject)
			{
				((IDictionary)this).Clear();
				return;
			}
			if (this.IsArray)
			{
				((IList)this).Clear();
				return;
			}
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x00054898 File Offset: 0x00052A98
		public bool Equals(JsonData x)
		{
			if (x == null)
			{
				return false;
			}
			if (x.type != this.type)
			{
				return false;
			}
			switch (this.type)
			{
			case JsonType.None:
				return true;
			case JsonType.Object:
				return this.inst_object.Equals(x.inst_object);
			case JsonType.Array:
				return this.inst_array.Equals(x.inst_array);
			case JsonType.String:
				return this.inst_string.Equals(x.inst_string);
			case JsonType.Int:
				return this.inst_int.Equals(x.inst_int);
			case JsonType.Long:
				return this.inst_long.Equals(x.inst_long);
			case JsonType.Double:
				return this.inst_double.Equals(x.inst_double);
			case JsonType.Boolean:
				return this.inst_boolean.Equals(x.inst_boolean);
			default:
				return false;
			}
		}

		// Token: 0x06000FE4 RID: 4068 RVA: 0x0005496D File Offset: 0x00052B6D
		public JsonType GetJsonType()
		{
			return this.type;
		}

		// Token: 0x06000FE5 RID: 4069 RVA: 0x00054978 File Offset: 0x00052B78
		public void SetJsonType(JsonType type)
		{
			if (this.type == type)
			{
				return;
			}
			switch (type)
			{
			case JsonType.Object:
				this.inst_object = new Dictionary<string, JsonData>();
				this.object_list = new List<KeyValuePair<string, JsonData>>();
				break;
			case JsonType.Array:
				this.inst_array = new List<JsonData>();
				break;
			case JsonType.String:
				this.inst_string = null;
				break;
			case JsonType.Int:
				this.inst_int = 0;
				break;
			case JsonType.Long:
				this.inst_long = 0L;
				break;
			case JsonType.Double:
				this.inst_double = 0.0;
				break;
			case JsonType.Boolean:
				this.inst_boolean = false;
				break;
			}
			this.type = type;
		}

		// Token: 0x06000FE6 RID: 4070 RVA: 0x00054A18 File Offset: 0x00052C18
		public string ToJson()
		{
			if (this.json != null)
			{
				return this.json;
			}
			StringWriter stringWriter = new StringWriter();
			JsonData.WriteJson(this, new JsonWriter(stringWriter)
			{
				Validate = false
			});
			this.json = stringWriter.ToString();
			return this.json;
		}

		// Token: 0x06000FE7 RID: 4071 RVA: 0x00054A64 File Offset: 0x00052C64
		public void ToJson(JsonWriter writer)
		{
			bool validate = writer.Validate;
			writer.Validate = false;
			JsonData.WriteJson(this, writer);
			writer.Validate = validate;
		}

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00054A90 File Offset: 0x00052C90
		public override string ToString()
		{
			switch (this.type)
			{
			case JsonType.Object:
				return "JsonData object";
			case JsonType.Array:
				return "JsonData array";
			case JsonType.String:
				return this.inst_string;
			case JsonType.Int:
				return this.inst_int.ToString();
			case JsonType.Long:
				return this.inst_long.ToString();
			case JsonType.Double:
				return this.inst_double.ToString();
			case JsonType.Boolean:
				return this.inst_boolean.ToString();
			default:
				return "Uninitialized JsonData";
			}
		}

		// Token: 0x040011D3 RID: 4563
		private IList<JsonData> inst_array;

		// Token: 0x040011D4 RID: 4564
		private bool inst_boolean;

		// Token: 0x040011D5 RID: 4565
		private double inst_double;

		// Token: 0x040011D6 RID: 4566
		private int inst_int;

		// Token: 0x040011D7 RID: 4567
		private long inst_long;

		// Token: 0x040011D8 RID: 4568
		private IDictionary<string, JsonData> inst_object;

		// Token: 0x040011D9 RID: 4569
		private string inst_string;

		// Token: 0x040011DA RID: 4570
		private string json;

		// Token: 0x040011DB RID: 4571
		private JsonType type;

		// Token: 0x040011DC RID: 4572
		private IList<KeyValuePair<string, JsonData>> object_list;
	}
}

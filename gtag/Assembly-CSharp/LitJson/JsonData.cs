using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace LitJson
{
	// Token: 0x02000271 RID: 625
	public class JsonData : IJsonWrapper, IList, ICollection, IEnumerable, IOrderedDictionary, IDictionary, IEquatable<JsonData>
	{
		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000F93 RID: 3987 RVA: 0x000540E6 File Offset: 0x000522E6
		public int Count
		{
			get
			{
				return this.EnsureCollection().Count;
			}
		}

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000F94 RID: 3988 RVA: 0x000540F3 File Offset: 0x000522F3
		public bool IsArray
		{
			get
			{
				return this.type == JsonType.Array;
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000F95 RID: 3989 RVA: 0x000540FE File Offset: 0x000522FE
		public bool IsBoolean
		{
			get
			{
				return this.type == JsonType.Boolean;
			}
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000F96 RID: 3990 RVA: 0x00054109 File Offset: 0x00052309
		public bool IsDouble
		{
			get
			{
				return this.type == JsonType.Double;
			}
		}

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000F97 RID: 3991 RVA: 0x00054114 File Offset: 0x00052314
		public bool IsInt
		{
			get
			{
				return this.type == JsonType.Int;
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000F98 RID: 3992 RVA: 0x0005411F File Offset: 0x0005231F
		public bool IsLong
		{
			get
			{
				return this.type == JsonType.Long;
			}
		}

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000F99 RID: 3993 RVA: 0x0005412A File Offset: 0x0005232A
		public bool IsObject
		{
			get
			{
				return this.type == JsonType.Object;
			}
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000F9A RID: 3994 RVA: 0x00054135 File Offset: 0x00052335
		public bool IsString
		{
			get
			{
				return this.type == JsonType.String;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000F9B RID: 3995 RVA: 0x00054140 File Offset: 0x00052340
		int ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000F9C RID: 3996 RVA: 0x00054148 File Offset: 0x00052348
		bool ICollection.IsSynchronized
		{
			get
			{
				return this.EnsureCollection().IsSynchronized;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000F9D RID: 3997 RVA: 0x00054155 File Offset: 0x00052355
		object ICollection.SyncRoot
		{
			get
			{
				return this.EnsureCollection().SyncRoot;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000F9E RID: 3998 RVA: 0x00054162 File Offset: 0x00052362
		bool IDictionary.IsFixedSize
		{
			get
			{
				return this.EnsureDictionary().IsFixedSize;
			}
		}

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x06000F9F RID: 3999 RVA: 0x0005416F File Offset: 0x0005236F
		bool IDictionary.IsReadOnly
		{
			get
			{
				return this.EnsureDictionary().IsReadOnly;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x0005417C File Offset: 0x0005237C
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

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x06000FA1 RID: 4001 RVA: 0x000541E4 File Offset: 0x000523E4
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

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x06000FA2 RID: 4002 RVA: 0x0005424C File Offset: 0x0005244C
		bool IJsonWrapper.IsArray
		{
			get
			{
				return this.IsArray;
			}
		}

		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000FA3 RID: 4003 RVA: 0x00054254 File Offset: 0x00052454
		bool IJsonWrapper.IsBoolean
		{
			get
			{
				return this.IsBoolean;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000FA4 RID: 4004 RVA: 0x0005425C File Offset: 0x0005245C
		bool IJsonWrapper.IsDouble
		{
			get
			{
				return this.IsDouble;
			}
		}

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000FA5 RID: 4005 RVA: 0x00054264 File Offset: 0x00052464
		bool IJsonWrapper.IsInt
		{
			get
			{
				return this.IsInt;
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000FA6 RID: 4006 RVA: 0x0005426C File Offset: 0x0005246C
		bool IJsonWrapper.IsLong
		{
			get
			{
				return this.IsLong;
			}
		}

		// Token: 0x170000CB RID: 203
		// (get) Token: 0x06000FA7 RID: 4007 RVA: 0x00054274 File Offset: 0x00052474
		bool IJsonWrapper.IsObject
		{
			get
			{
				return this.IsObject;
			}
		}

		// Token: 0x170000CC RID: 204
		// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x0005427C File Offset: 0x0005247C
		bool IJsonWrapper.IsString
		{
			get
			{
				return this.IsString;
			}
		}

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x06000FA9 RID: 4009 RVA: 0x00054284 File Offset: 0x00052484
		bool IList.IsFixedSize
		{
			get
			{
				return this.EnsureList().IsFixedSize;
			}
		}

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x06000FAA RID: 4010 RVA: 0x00054291 File Offset: 0x00052491
		bool IList.IsReadOnly
		{
			get
			{
				return this.EnsureList().IsReadOnly;
			}
		}

		// Token: 0x170000CF RID: 207
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

		// Token: 0x170000D0 RID: 208
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

		// Token: 0x170000D1 RID: 209
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

		// Token: 0x170000D2 RID: 210
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

		// Token: 0x170000D3 RID: 211
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

		// Token: 0x06000FB5 RID: 4021 RVA: 0x000544EF File Offset: 0x000526EF
		public JsonData()
		{
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x000544F7 File Offset: 0x000526F7
		public JsonData(bool boolean)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = boolean;
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x0005450D File Offset: 0x0005270D
		public JsonData(double number)
		{
			this.type = JsonType.Double;
			this.inst_double = number;
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x00054523 File Offset: 0x00052723
		public JsonData(int number)
		{
			this.type = JsonType.Int;
			this.inst_int = number;
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x00054539 File Offset: 0x00052739
		public JsonData(long number)
		{
			this.type = JsonType.Long;
			this.inst_long = number;
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x00054550 File Offset: 0x00052750
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

		// Token: 0x06000FBB RID: 4027 RVA: 0x000545F9 File Offset: 0x000527F9
		public JsonData(string str)
		{
			this.type = JsonType.String;
			this.inst_string = str;
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x0005460F File Offset: 0x0005280F
		public static implicit operator JsonData(bool data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x00054617 File Offset: 0x00052817
		public static implicit operator JsonData(double data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x0005461F File Offset: 0x0005281F
		public static implicit operator JsonData(int data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x00054627 File Offset: 0x00052827
		public static implicit operator JsonData(long data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x0005462F File Offset: 0x0005282F
		public static implicit operator JsonData(string data)
		{
			return new JsonData(data);
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x00054637 File Offset: 0x00052837
		public static explicit operator bool(JsonData data)
		{
			if (data.type != JsonType.Boolean)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_boolean;
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x00054653 File Offset: 0x00052853
		public static explicit operator double(JsonData data)
		{
			if (data.type != JsonType.Double)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a double");
			}
			return data.inst_double;
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x0005466F File Offset: 0x0005286F
		public static explicit operator int(JsonData data)
		{
			if (data.type != JsonType.Int)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_int;
		}

		// Token: 0x06000FC4 RID: 4036 RVA: 0x0005468B File Offset: 0x0005288B
		public static explicit operator long(JsonData data)
		{
			if (data.type != JsonType.Long)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold an int");
			}
			return data.inst_long;
		}

		// Token: 0x06000FC5 RID: 4037 RVA: 0x000546A7 File Offset: 0x000528A7
		public static explicit operator string(JsonData data)
		{
			if (data.type != JsonType.String)
			{
				throw new InvalidCastException("Instance of JsonData doesn't hold a string");
			}
			return data.inst_string;
		}

		// Token: 0x06000FC6 RID: 4038 RVA: 0x000546C3 File Offset: 0x000528C3
		void ICollection.CopyTo(Array array, int index)
		{
			this.EnsureCollection().CopyTo(array, index);
		}

		// Token: 0x06000FC7 RID: 4039 RVA: 0x000546D4 File Offset: 0x000528D4
		void IDictionary.Add(object key, object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.EnsureDictionary().Add(key, value2);
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>((string)key, value2);
			this.object_list.Add(item);
			this.json = null;
		}

		// Token: 0x06000FC8 RID: 4040 RVA: 0x00054717 File Offset: 0x00052917
		void IDictionary.Clear()
		{
			this.EnsureDictionary().Clear();
			this.object_list.Clear();
			this.json = null;
		}

		// Token: 0x06000FC9 RID: 4041 RVA: 0x00054736 File Offset: 0x00052936
		bool IDictionary.Contains(object key)
		{
			return this.EnsureDictionary().Contains(key);
		}

		// Token: 0x06000FCA RID: 4042 RVA: 0x00054744 File Offset: 0x00052944
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IOrderedDictionary)this).GetEnumerator();
		}

		// Token: 0x06000FCB RID: 4043 RVA: 0x0005474C File Offset: 0x0005294C
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

		// Token: 0x06000FCC RID: 4044 RVA: 0x000547B1 File Offset: 0x000529B1
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.EnsureCollection().GetEnumerator();
		}

		// Token: 0x06000FCD RID: 4045 RVA: 0x000547BE File Offset: 0x000529BE
		bool IJsonWrapper.GetBoolean()
		{
			if (this.type != JsonType.Boolean)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a boolean");
			}
			return this.inst_boolean;
		}

		// Token: 0x06000FCE RID: 4046 RVA: 0x000547DA File Offset: 0x000529DA
		double IJsonWrapper.GetDouble()
		{
			if (this.type != JsonType.Double)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a double");
			}
			return this.inst_double;
		}

		// Token: 0x06000FCF RID: 4047 RVA: 0x000547F6 File Offset: 0x000529F6
		int IJsonWrapper.GetInt()
		{
			if (this.type != JsonType.Int)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold an int");
			}
			return this.inst_int;
		}

		// Token: 0x06000FD0 RID: 4048 RVA: 0x00054812 File Offset: 0x00052A12
		long IJsonWrapper.GetLong()
		{
			if (this.type != JsonType.Long)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a long");
			}
			return this.inst_long;
		}

		// Token: 0x06000FD1 RID: 4049 RVA: 0x0005482E File Offset: 0x00052A2E
		string IJsonWrapper.GetString()
		{
			if (this.type != JsonType.String)
			{
				throw new InvalidOperationException("JsonData instance doesn't hold a string");
			}
			return this.inst_string;
		}

		// Token: 0x06000FD2 RID: 4050 RVA: 0x0005484A File Offset: 0x00052A4A
		void IJsonWrapper.SetBoolean(bool val)
		{
			this.type = JsonType.Boolean;
			this.inst_boolean = val;
			this.json = null;
		}

		// Token: 0x06000FD3 RID: 4051 RVA: 0x00054861 File Offset: 0x00052A61
		void IJsonWrapper.SetDouble(double val)
		{
			this.type = JsonType.Double;
			this.inst_double = val;
			this.json = null;
		}

		// Token: 0x06000FD4 RID: 4052 RVA: 0x00054878 File Offset: 0x00052A78
		void IJsonWrapper.SetInt(int val)
		{
			this.type = JsonType.Int;
			this.inst_int = val;
			this.json = null;
		}

		// Token: 0x06000FD5 RID: 4053 RVA: 0x0005488F File Offset: 0x00052A8F
		void IJsonWrapper.SetLong(long val)
		{
			this.type = JsonType.Long;
			this.inst_long = val;
			this.json = null;
		}

		// Token: 0x06000FD6 RID: 4054 RVA: 0x000548A6 File Offset: 0x00052AA6
		void IJsonWrapper.SetString(string val)
		{
			this.type = JsonType.String;
			this.inst_string = val;
			this.json = null;
		}

		// Token: 0x06000FD7 RID: 4055 RVA: 0x000548BD File Offset: 0x00052ABD
		string IJsonWrapper.ToJson()
		{
			return this.ToJson();
		}

		// Token: 0x06000FD8 RID: 4056 RVA: 0x000548C5 File Offset: 0x00052AC5
		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			this.ToJson(writer);
		}

		// Token: 0x06000FD9 RID: 4057 RVA: 0x000548CE File Offset: 0x00052ACE
		int IList.Add(object value)
		{
			return this.Add(value);
		}

		// Token: 0x06000FDA RID: 4058 RVA: 0x000548D7 File Offset: 0x00052AD7
		void IList.Clear()
		{
			this.EnsureList().Clear();
			this.json = null;
		}

		// Token: 0x06000FDB RID: 4059 RVA: 0x000548EB File Offset: 0x00052AEB
		bool IList.Contains(object value)
		{
			return this.EnsureList().Contains(value);
		}

		// Token: 0x06000FDC RID: 4060 RVA: 0x000548F9 File Offset: 0x00052AF9
		int IList.IndexOf(object value)
		{
			return this.EnsureList().IndexOf(value);
		}

		// Token: 0x06000FDD RID: 4061 RVA: 0x00054907 File Offset: 0x00052B07
		void IList.Insert(int index, object value)
		{
			this.EnsureList().Insert(index, value);
			this.json = null;
		}

		// Token: 0x06000FDE RID: 4062 RVA: 0x0005491D File Offset: 0x00052B1D
		void IList.Remove(object value)
		{
			this.EnsureList().Remove(value);
			this.json = null;
		}

		// Token: 0x06000FDF RID: 4063 RVA: 0x00054932 File Offset: 0x00052B32
		void IList.RemoveAt(int index)
		{
			this.EnsureList().RemoveAt(index);
			this.json = null;
		}

		// Token: 0x06000FE0 RID: 4064 RVA: 0x00054947 File Offset: 0x00052B47
		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			this.EnsureDictionary();
			return new OrderedDictionaryEnumerator(this.object_list.GetEnumerator());
		}

		// Token: 0x06000FE1 RID: 4065 RVA: 0x00054960 File Offset: 0x00052B60
		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string text = (string)key;
			JsonData value2 = this.ToJsonData(value);
			this[text] = value2;
			KeyValuePair<string, JsonData> item = new KeyValuePair<string, JsonData>(text, value2);
			this.object_list.Insert(idx, item);
		}

		// Token: 0x06000FE2 RID: 4066 RVA: 0x0005499C File Offset: 0x00052B9C
		void IOrderedDictionary.RemoveAt(int idx)
		{
			this.EnsureDictionary();
			this.inst_object.Remove(this.object_list[idx].Key);
			this.object_list.RemoveAt(idx);
		}

		// Token: 0x06000FE3 RID: 4067 RVA: 0x000549DC File Offset: 0x00052BDC
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

		// Token: 0x06000FE4 RID: 4068 RVA: 0x00054A14 File Offset: 0x00052C14
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

		// Token: 0x06000FE5 RID: 4069 RVA: 0x00054A74 File Offset: 0x00052C74
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

		// Token: 0x06000FE6 RID: 4070 RVA: 0x00054AC6 File Offset: 0x00052CC6
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

		// Token: 0x06000FE7 RID: 4071 RVA: 0x00054AE4 File Offset: 0x00052CE4
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

		// Token: 0x06000FE8 RID: 4072 RVA: 0x00054C2C File Offset: 0x00052E2C
		public int Add(object value)
		{
			JsonData value2 = this.ToJsonData(value);
			this.json = null;
			return this.EnsureList().Add(value2);
		}

		// Token: 0x06000FE9 RID: 4073 RVA: 0x00054C54 File Offset: 0x00052E54
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

		// Token: 0x06000FEA RID: 4074 RVA: 0x00054C74 File Offset: 0x00052E74
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

		// Token: 0x06000FEB RID: 4075 RVA: 0x00054D49 File Offset: 0x00052F49
		public JsonType GetJsonType()
		{
			return this.type;
		}

		// Token: 0x06000FEC RID: 4076 RVA: 0x00054D54 File Offset: 0x00052F54
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

		// Token: 0x06000FED RID: 4077 RVA: 0x00054DF4 File Offset: 0x00052FF4
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

		// Token: 0x06000FEE RID: 4078 RVA: 0x00054E40 File Offset: 0x00053040
		public void ToJson(JsonWriter writer)
		{
			bool validate = writer.Validate;
			writer.Validate = false;
			JsonData.WriteJson(this, writer);
			writer.Validate = validate;
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x00054E6C File Offset: 0x0005306C
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

		// Token: 0x040011D9 RID: 4569
		private IList<JsonData> inst_array;

		// Token: 0x040011DA RID: 4570
		private bool inst_boolean;

		// Token: 0x040011DB RID: 4571
		private double inst_double;

		// Token: 0x040011DC RID: 4572
		private int inst_int;

		// Token: 0x040011DD RID: 4573
		private long inst_long;

		// Token: 0x040011DE RID: 4574
		private IDictionary<string, JsonData> inst_object;

		// Token: 0x040011DF RID: 4575
		private string inst_string;

		// Token: 0x040011E0 RID: 4576
		private string json;

		// Token: 0x040011E1 RID: 4577
		private JsonType type;

		// Token: 0x040011E2 RID: 4578
		private IList<KeyValuePair<string, JsonData>> object_list;
	}
}

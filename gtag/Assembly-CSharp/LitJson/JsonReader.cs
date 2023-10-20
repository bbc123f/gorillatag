﻿using System;
using System.Collections.Generic;
using System.IO;

namespace LitJson
{
	// Token: 0x0200027E RID: 638
	public class JsonReader
	{
		// Token: 0x170000DE RID: 222
		// (get) Token: 0x06001038 RID: 4152 RVA: 0x00056560 File Offset: 0x00054760
		// (set) Token: 0x06001039 RID: 4153 RVA: 0x0005656D File Offset: 0x0005476D
		public bool AllowComments
		{
			get
			{
				return this.lexer.AllowComments;
			}
			set
			{
				this.lexer.AllowComments = value;
			}
		}

		// Token: 0x170000DF RID: 223
		// (get) Token: 0x0600103A RID: 4154 RVA: 0x0005657B File Offset: 0x0005477B
		// (set) Token: 0x0600103B RID: 4155 RVA: 0x00056588 File Offset: 0x00054788
		public bool AllowSingleQuotedStrings
		{
			get
			{
				return this.lexer.AllowSingleQuotedStrings;
			}
			set
			{
				this.lexer.AllowSingleQuotedStrings = value;
			}
		}

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x0600103C RID: 4156 RVA: 0x00056596 File Offset: 0x00054796
		public bool EndOfInput
		{
			get
			{
				return this.end_of_input;
			}
		}

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x0600103D RID: 4157 RVA: 0x0005659E File Offset: 0x0005479E
		public bool EndOfJson
		{
			get
			{
				return this.end_of_json;
			}
		}

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x0600103E RID: 4158 RVA: 0x000565A6 File Offset: 0x000547A6
		public JsonToken Token
		{
			get
			{
				return this.token;
			}
		}

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x0600103F RID: 4159 RVA: 0x000565AE File Offset: 0x000547AE
		public object Value
		{
			get
			{
				return this.token_value;
			}
		}

		// Token: 0x06001040 RID: 4160 RVA: 0x000565B6 File Offset: 0x000547B6
		static JsonReader()
		{
			JsonReader.PopulateParseTable();
		}

		// Token: 0x06001041 RID: 4161 RVA: 0x000565BD File Offset: 0x000547BD
		public JsonReader(string json_text) : this(new StringReader(json_text), true)
		{
		}

		// Token: 0x06001042 RID: 4162 RVA: 0x000565CC File Offset: 0x000547CC
		public JsonReader(TextReader reader) : this(reader, false)
		{
		}

		// Token: 0x06001043 RID: 4163 RVA: 0x000565D8 File Offset: 0x000547D8
		private JsonReader(TextReader reader, bool owned)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			this.parser_in_string = false;
			this.parser_return = false;
			this.read_started = false;
			this.automaton_stack = new Stack<int>();
			this.automaton_stack.Push(65553);
			this.automaton_stack.Push(65543);
			this.lexer = new Lexer(reader);
			this.end_of_input = false;
			this.end_of_json = false;
			this.reader = reader;
			this.reader_is_owned = owned;
		}

		// Token: 0x06001044 RID: 4164 RVA: 0x00056664 File Offset: 0x00054864
		private static void PopulateParseTable()
		{
			JsonReader.parse_table = new Dictionary<int, IDictionary<int, int[]>>();
			JsonReader.TableAddRow(ParserToken.Array);
			JsonReader.TableAddCol(ParserToken.Array, 91, new int[]
			{
				91,
				65549
			});
			JsonReader.TableAddRow(ParserToken.ArrayPrime);
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 34, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 91, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 93, new int[]
			{
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 123, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65537, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65538, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65539, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddCol(ParserToken.ArrayPrime, 65540, new int[]
			{
				65550,
				65551,
				93
			});
			JsonReader.TableAddRow(ParserToken.Object);
			JsonReader.TableAddCol(ParserToken.Object, 123, new int[]
			{
				123,
				65545
			});
			JsonReader.TableAddRow(ParserToken.ObjectPrime);
			JsonReader.TableAddCol(ParserToken.ObjectPrime, 34, new int[]
			{
				65546,
				65547,
				125
			});
			JsonReader.TableAddCol(ParserToken.ObjectPrime, 125, new int[]
			{
				125
			});
			JsonReader.TableAddRow(ParserToken.Pair);
			JsonReader.TableAddCol(ParserToken.Pair, 34, new int[]
			{
				65552,
				58,
				65550
			});
			JsonReader.TableAddRow(ParserToken.PairRest);
			JsonReader.TableAddCol(ParserToken.PairRest, 44, new int[]
			{
				44,
				65546,
				65547
			});
			JsonReader.TableAddCol(ParserToken.PairRest, 125, new int[]
			{
				65554
			});
			JsonReader.TableAddRow(ParserToken.String);
			JsonReader.TableAddCol(ParserToken.String, 34, new int[]
			{
				34,
				65541,
				34
			});
			JsonReader.TableAddRow(ParserToken.Text);
			JsonReader.TableAddCol(ParserToken.Text, 91, new int[]
			{
				65548
			});
			JsonReader.TableAddCol(ParserToken.Text, 123, new int[]
			{
				65544
			});
			JsonReader.TableAddRow(ParserToken.Value);
			JsonReader.TableAddCol(ParserToken.Value, 34, new int[]
			{
				65552
			});
			JsonReader.TableAddCol(ParserToken.Value, 91, new int[]
			{
				65548
			});
			JsonReader.TableAddCol(ParserToken.Value, 123, new int[]
			{
				65544
			});
			JsonReader.TableAddCol(ParserToken.Value, 65537, new int[]
			{
				65537
			});
			JsonReader.TableAddCol(ParserToken.Value, 65538, new int[]
			{
				65538
			});
			JsonReader.TableAddCol(ParserToken.Value, 65539, new int[]
			{
				65539
			});
			JsonReader.TableAddCol(ParserToken.Value, 65540, new int[]
			{
				65540
			});
			JsonReader.TableAddRow(ParserToken.ValueRest);
			JsonReader.TableAddCol(ParserToken.ValueRest, 44, new int[]
			{
				44,
				65550,
				65551
			});
			JsonReader.TableAddCol(ParserToken.ValueRest, 93, new int[]
			{
				65554
			});
		}

		// Token: 0x06001045 RID: 4165 RVA: 0x000569DD File Offset: 0x00054BDD
		private static void TableAddCol(ParserToken row, int col, params int[] symbols)
		{
			JsonReader.parse_table[(int)row].Add(col, symbols);
		}

		// Token: 0x06001046 RID: 4166 RVA: 0x000569F1 File Offset: 0x00054BF1
		private static void TableAddRow(ParserToken rule)
		{
			JsonReader.parse_table.Add((int)rule, new Dictionary<int, int[]>());
		}

		// Token: 0x06001047 RID: 4167 RVA: 0x00056A04 File Offset: 0x00054C04
		private void ProcessNumber(string number)
		{
			double num;
			if ((number.IndexOf('.') != -1 || number.IndexOf('e') != -1 || number.IndexOf('E') != -1) && double.TryParse(number, out num))
			{
				this.token = JsonToken.Double;
				this.token_value = num;
				return;
			}
			int num2;
			if (int.TryParse(number, out num2))
			{
				this.token = JsonToken.Int;
				this.token_value = num2;
				return;
			}
			long num3;
			if (long.TryParse(number, out num3))
			{
				this.token = JsonToken.Long;
				this.token_value = num3;
				return;
			}
			this.token = JsonToken.Int;
			this.token_value = 0;
		}

		// Token: 0x06001048 RID: 4168 RVA: 0x00056AA0 File Offset: 0x00054CA0
		private void ProcessSymbol()
		{
			if (this.current_symbol == 91)
			{
				this.token = JsonToken.ArrayStart;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 93)
			{
				this.token = JsonToken.ArrayEnd;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 123)
			{
				this.token = JsonToken.ObjectStart;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 125)
			{
				this.token = JsonToken.ObjectEnd;
				this.parser_return = true;
				return;
			}
			if (this.current_symbol == 34)
			{
				if (this.parser_in_string)
				{
					this.parser_in_string = false;
					this.parser_return = true;
					return;
				}
				if (this.token == JsonToken.None)
				{
					this.token = JsonToken.String;
				}
				this.parser_in_string = true;
				return;
			}
			else
			{
				if (this.current_symbol == 65541)
				{
					this.token_value = this.lexer.StringValue;
					return;
				}
				if (this.current_symbol == 65539)
				{
					this.token = JsonToken.Boolean;
					this.token_value = false;
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65540)
				{
					this.token = JsonToken.Null;
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65537)
				{
					this.ProcessNumber(this.lexer.StringValue);
					this.parser_return = true;
					return;
				}
				if (this.current_symbol == 65546)
				{
					this.token = JsonToken.PropertyName;
					return;
				}
				if (this.current_symbol == 65538)
				{
					this.token = JsonToken.Boolean;
					this.token_value = true;
					this.parser_return = true;
				}
				return;
			}
		}

		// Token: 0x06001049 RID: 4169 RVA: 0x00056C12 File Offset: 0x00054E12
		private bool ReadToken()
		{
			if (this.end_of_input)
			{
				return false;
			}
			this.lexer.NextToken();
			if (this.lexer.EndOfInput)
			{
				this.Close();
				return false;
			}
			this.current_input = this.lexer.Token;
			return true;
		}

		// Token: 0x0600104A RID: 4170 RVA: 0x00056C51 File Offset: 0x00054E51
		public void Close()
		{
			if (this.end_of_input)
			{
				return;
			}
			this.end_of_input = true;
			this.end_of_json = true;
			if (this.reader_is_owned)
			{
				this.reader.Close();
			}
			this.reader = null;
		}

		// Token: 0x0600104B RID: 4171 RVA: 0x00056C84 File Offset: 0x00054E84
		public bool Read()
		{
			if (this.end_of_input)
			{
				return false;
			}
			if (this.end_of_json)
			{
				this.end_of_json = false;
				this.automaton_stack.Clear();
				this.automaton_stack.Push(65553);
				this.automaton_stack.Push(65543);
			}
			this.parser_in_string = false;
			this.parser_return = false;
			this.token = JsonToken.None;
			this.token_value = null;
			if (!this.read_started)
			{
				this.read_started = true;
				if (!this.ReadToken())
				{
					return false;
				}
			}
			while (!this.parser_return)
			{
				this.current_symbol = this.automaton_stack.Pop();
				this.ProcessSymbol();
				if (this.current_symbol == this.current_input)
				{
					if (!this.ReadToken())
					{
						if (this.automaton_stack.Peek() != 65553)
						{
							throw new JsonException("Input doesn't evaluate to proper JSON text");
						}
						return this.parser_return;
					}
				}
				else
				{
					int[] array;
					try
					{
						array = JsonReader.parse_table[this.current_symbol][this.current_input];
					}
					catch (KeyNotFoundException inner_exception)
					{
						throw new JsonException((ParserToken)this.current_input, inner_exception);
					}
					if (array[0] != 65554)
					{
						for (int i = array.Length - 1; i >= 0; i--)
						{
							this.automaton_stack.Push(array[i]);
						}
					}
				}
			}
			if (this.automaton_stack.Peek() == 65553)
			{
				this.end_of_json = true;
			}
			return true;
		}

		// Token: 0x0400120A RID: 4618
		private static IDictionary<int, IDictionary<int, int[]>> parse_table;

		// Token: 0x0400120B RID: 4619
		private Stack<int> automaton_stack;

		// Token: 0x0400120C RID: 4620
		private int current_input;

		// Token: 0x0400120D RID: 4621
		private int current_symbol;

		// Token: 0x0400120E RID: 4622
		private bool end_of_json;

		// Token: 0x0400120F RID: 4623
		private bool end_of_input;

		// Token: 0x04001210 RID: 4624
		private Lexer lexer;

		// Token: 0x04001211 RID: 4625
		private bool parser_in_string;

		// Token: 0x04001212 RID: 4626
		private bool parser_return;

		// Token: 0x04001213 RID: 4627
		private bool read_started;

		// Token: 0x04001214 RID: 4628
		private TextReader reader;

		// Token: 0x04001215 RID: 4629
		private bool reader_is_owned;

		// Token: 0x04001216 RID: 4630
		private object token_value;

		// Token: 0x04001217 RID: 4631
		private JsonToken token;
	}
}

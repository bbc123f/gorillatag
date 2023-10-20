using System;

namespace LitJson
{
	// Token: 0x02000273 RID: 627
	public class JsonException : ApplicationException
	{
		// Token: 0x06000FF7 RID: 4087 RVA: 0x00054F91 File Offset: 0x00053191
		public JsonException()
		{
		}

		// Token: 0x06000FF8 RID: 4088 RVA: 0x00054F99 File Offset: 0x00053199
		internal JsonException(ParserToken token) : base(string.Format("Invalid token '{0}' in input string", token))
		{
		}

		// Token: 0x06000FF9 RID: 4089 RVA: 0x00054FB1 File Offset: 0x000531B1
		internal JsonException(ParserToken token, Exception inner_exception) : base(string.Format("Invalid token '{0}' in input string", token), inner_exception)
		{
		}

		// Token: 0x06000FFA RID: 4090 RVA: 0x00054FCA File Offset: 0x000531CA
		internal JsonException(int c) : base(string.Format("Invalid character '{0}' in input string", (char)c))
		{
		}

		// Token: 0x06000FFB RID: 4091 RVA: 0x00054FE3 File Offset: 0x000531E3
		internal JsonException(int c, Exception inner_exception) : base(string.Format("Invalid character '{0}' in input string", (char)c), inner_exception)
		{
		}

		// Token: 0x06000FFC RID: 4092 RVA: 0x00054FFD File Offset: 0x000531FD
		public JsonException(string message) : base(message)
		{
		}

		// Token: 0x06000FFD RID: 4093 RVA: 0x00055006 File Offset: 0x00053206
		public JsonException(string message, Exception inner_exception) : base(message, inner_exception)
		{
		}
	}
}

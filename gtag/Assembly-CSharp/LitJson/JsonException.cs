using System;

namespace LitJson
{
	// Token: 0x02000272 RID: 626
	public class JsonException : ApplicationException
	{
		// Token: 0x06000FF0 RID: 4080 RVA: 0x00054BB5 File Offset: 0x00052DB5
		public JsonException()
		{
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x00054BBD File Offset: 0x00052DBD
		internal JsonException(ParserToken token) : base(string.Format("Invalid token '{0}' in input string", token))
		{
		}

		// Token: 0x06000FF2 RID: 4082 RVA: 0x00054BD5 File Offset: 0x00052DD5
		internal JsonException(ParserToken token, Exception inner_exception) : base(string.Format("Invalid token '{0}' in input string", token), inner_exception)
		{
		}

		// Token: 0x06000FF3 RID: 4083 RVA: 0x00054BEE File Offset: 0x00052DEE
		internal JsonException(int c) : base(string.Format("Invalid character '{0}' in input string", (char)c))
		{
		}

		// Token: 0x06000FF4 RID: 4084 RVA: 0x00054C07 File Offset: 0x00052E07
		internal JsonException(int c, Exception inner_exception) : base(string.Format("Invalid character '{0}' in input string", (char)c), inner_exception)
		{
		}

		// Token: 0x06000FF5 RID: 4085 RVA: 0x00054C21 File Offset: 0x00052E21
		public JsonException(string message) : base(message)
		{
		}

		// Token: 0x06000FF6 RID: 4086 RVA: 0x00054C2A File Offset: 0x00052E2A
		public JsonException(string message, Exception inner_exception) : base(message, inner_exception)
		{
		}
	}
}

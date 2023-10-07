using System;

// Token: 0x02000052 RID: 82
public class ComponentMember
{
	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000189 RID: 393 RVA: 0x0000C119 File Offset: 0x0000A319
	public string Name { get; }

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600018A RID: 394 RVA: 0x0000C121 File Offset: 0x0000A321
	public string Value
	{
		get
		{
			return this.getValue();
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x0600018B RID: 395 RVA: 0x0000C12E File Offset: 0x0000A32E
	public bool IsStarred { get; }

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x0600018C RID: 396 RVA: 0x0000C136 File Offset: 0x0000A336
	public string Color { get; }

	// Token: 0x0600018D RID: 397 RVA: 0x0000C13E File Offset: 0x0000A33E
	public ComponentMember(string name, Func<string> getValue, bool isStarred, string color)
	{
		this.Name = name;
		this.getValue = getValue;
		this.IsStarred = isStarred;
		this.Color = color;
	}

	// Token: 0x04000266 RID: 614
	private Func<string> getValue;

	// Token: 0x04000267 RID: 615
	public string computedPrefix;

	// Token: 0x04000268 RID: 616
	public string computedSuffix;
}

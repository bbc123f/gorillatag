using System;

// Token: 0x0200004D RID: 77
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DevInspectorColor : Attribute
{
	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000183 RID: 387 RVA: 0x0000C120 File Offset: 0x0000A320
	public string Color { get; }

	// Token: 0x06000184 RID: 388 RVA: 0x0000C128 File Offset: 0x0000A328
	public DevInspectorColor(string color)
	{
		this.Color = color;
	}
}

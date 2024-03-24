using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class <>f__AnonymousType0<<KeyToFollow>j__TPar, <RoomToJoin>j__TPar, <Set>j__TPar>
{
	public <KeyToFollow>j__TPar KeyToFollow
	{
		get
		{
			return this.<KeyToFollow>i__Field;
		}
	}

	public <RoomToJoin>j__TPar RoomToJoin
	{
		get
		{
			return this.<RoomToJoin>i__Field;
		}
	}

	public <Set>j__TPar Set
	{
		get
		{
			return this.<Set>i__Field;
		}
	}

	[DebuggerHidden]
	public <>f__AnonymousType0(<KeyToFollow>j__TPar KeyToFollow, <RoomToJoin>j__TPar RoomToJoin, <Set>j__TPar Set)
	{
		this.<KeyToFollow>i__Field = KeyToFollow;
		this.<RoomToJoin>i__Field = RoomToJoin;
		this.<Set>i__Field = Set;
	}

	[DebuggerHidden]
	public override bool Equals(object value)
	{
		var <>f__AnonymousType = value as <>f__AnonymousType0<<KeyToFollow>j__TPar, <RoomToJoin>j__TPar, <Set>j__TPar>;
		return this == <>f__AnonymousType || (<>f__AnonymousType != null && EqualityComparer<<KeyToFollow>j__TPar>.Default.Equals(this.<KeyToFollow>i__Field, <>f__AnonymousType.<KeyToFollow>i__Field) && EqualityComparer<<RoomToJoin>j__TPar>.Default.Equals(this.<RoomToJoin>i__Field, <>f__AnonymousType.<RoomToJoin>i__Field) && EqualityComparer<<Set>j__TPar>.Default.Equals(this.<Set>i__Field, <>f__AnonymousType.<Set>i__Field));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return ((-1752229285 * -1521134295 + EqualityComparer<<KeyToFollow>j__TPar>.Default.GetHashCode(this.<KeyToFollow>i__Field)) * -1521134295 + EqualityComparer<<RoomToJoin>j__TPar>.Default.GetHashCode(this.<RoomToJoin>i__Field)) * -1521134295 + EqualityComparer<<Set>j__TPar>.Default.GetHashCode(this.<Set>i__Field);
	}

	[DebuggerHidden]
	[return: Nullable(1)]
	public override string ToString()
	{
		IFormatProvider formatProvider = null;
		string text = "{{ KeyToFollow = {0}, RoomToJoin = {1}, Set = {2} }}";
		object[] array = new object[3];
		int num = 0;
		<KeyToFollow>j__TPar <KeyToFollow>j__TPar = this.<KeyToFollow>i__Field;
		array[num] = ((<KeyToFollow>j__TPar != null) ? <KeyToFollow>j__TPar.ToString() : null);
		int num2 = 1;
		<RoomToJoin>j__TPar <RoomToJoin>j__TPar = this.<RoomToJoin>i__Field;
		array[num2] = ((<RoomToJoin>j__TPar != null) ? <RoomToJoin>j__TPar.ToString() : null);
		int num3 = 2;
		<Set>j__TPar <Set>j__TPar = this.<Set>i__Field;
		array[num3] = ((<Set>j__TPar != null) ? <Set>j__TPar.ToString() : null);
		return string.Format(formatProvider, text, array);
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly <KeyToFollow>j__TPar <KeyToFollow>i__Field;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly <RoomToJoin>j__TPar <RoomToJoin>i__Field;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly <Set>j__TPar <Set>i__Field;
}

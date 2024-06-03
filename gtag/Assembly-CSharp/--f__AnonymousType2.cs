using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[CompilerGenerated]
internal sealed class <>f__AnonymousType2<<CurrentVersion>j__TPar, <UpdatedSynchTest>j__TPar>
{
	public <CurrentVersion>j__TPar CurrentVersion
	{
		get
		{
			return this.<CurrentVersion>i__Field;
		}
	}

	public <UpdatedSynchTest>j__TPar UpdatedSynchTest
	{
		get
		{
			return this.<UpdatedSynchTest>i__Field;
		}
	}

	[DebuggerHidden]
	public <>f__AnonymousType2(<CurrentVersion>j__TPar CurrentVersion, <UpdatedSynchTest>j__TPar UpdatedSynchTest)
	{
		this.<CurrentVersion>i__Field = CurrentVersion;
		this.<UpdatedSynchTest>i__Field = UpdatedSynchTest;
	}

	[DebuggerHidden]
	public override bool Equals(object value)
	{
		var <>f__AnonymousType = value as <>f__AnonymousType2<<CurrentVersion>j__TPar, <UpdatedSynchTest>j__TPar>;
		return this == <>f__AnonymousType || (<>f__AnonymousType != null && EqualityComparer<<CurrentVersion>j__TPar>.Default.Equals(this.<CurrentVersion>i__Field, <>f__AnonymousType.<CurrentVersion>i__Field) && EqualityComparer<<UpdatedSynchTest>j__TPar>.Default.Equals(this.<UpdatedSynchTest>i__Field, <>f__AnonymousType.<UpdatedSynchTest>i__Field));
	}

	[DebuggerHidden]
	public override int GetHashCode()
	{
		return (1950661697 * -1521134295 + EqualityComparer<<CurrentVersion>j__TPar>.Default.GetHashCode(this.<CurrentVersion>i__Field)) * -1521134295 + EqualityComparer<<UpdatedSynchTest>j__TPar>.Default.GetHashCode(this.<UpdatedSynchTest>i__Field);
	}

	[DebuggerHidden]
	[return: Nullable(1)]
	public override string ToString()
	{
		IFormatProvider provider = null;
		string format = "{{ CurrentVersion = {0}, UpdatedSynchTest = {1} }}";
		object[] array = new object[2];
		int num = 0;
		<CurrentVersion>j__TPar <CurrentVersion>j__TPar = this.<CurrentVersion>i__Field;
		array[num] = ((<CurrentVersion>j__TPar != null) ? <CurrentVersion>j__TPar.ToString() : null);
		int num2 = 1;
		<UpdatedSynchTest>j__TPar <UpdatedSynchTest>j__TPar = this.<UpdatedSynchTest>i__Field;
		array[num2] = ((<UpdatedSynchTest>j__TPar != null) ? <UpdatedSynchTest>j__TPar.ToString() : null);
		return string.Format(provider, format, array);
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly <CurrentVersion>j__TPar <CurrentVersion>i__Field;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	private readonly <UpdatedSynchTest>j__TPar <UpdatedSynchTest>i__Field;
}

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class BasePageHandler : MonoBehaviour
{
	private protected int selectedIndex
	{
		[CompilerGenerated]
		protected get
		{
			return this.<selectedIndex>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<selectedIndex>k__BackingField = value;
		}
	}

	private protected int currentPage
	{
		[CompilerGenerated]
		protected get
		{
			return this.<currentPage>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<currentPage>k__BackingField = value;
		}
	}

	private protected int pages
	{
		[CompilerGenerated]
		protected get
		{
			return this.<pages>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<pages>k__BackingField = value;
		}
	}

	private protected int maxEntires
	{
		[CompilerGenerated]
		protected get
		{
			return this.<maxEntires>k__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			this.<maxEntires>k__BackingField = value;
		}
	}

	protected abstract int pageSize { get; }

	protected abstract int entriesCount { get; }

	protected virtual void Start()
	{
		Debug.Log("base page handler " + this.entriesCount.ToString() + " " + this.pageSize.ToString());
		this.pages = this.entriesCount / this.pageSize + 1;
		this.maxEntires = this.pages * this.pageSize;
	}

	public void SelectEntryOnPage(int entryIndex)
	{
		int num = entryIndex + this.pageSize * this.currentPage;
		if (num > this.entriesCount)
		{
			return;
		}
		this.selectedIndex = num;
		this.PageEntrySelected(entryIndex, this.selectedIndex);
	}

	public void SelectEntryFromIndex(int index)
	{
		this.selectedIndex = index;
		this.currentPage = this.selectedIndex / this.pageSize;
		int num = index - this.pageSize * this.currentPage;
		this.PageEntrySelected(num, index);
		this.SetPage(this.currentPage);
	}

	public void ChangePage(bool left)
	{
		int num = (left ? (-1) : 1);
		this.SetPage(Mathf.Abs((this.currentPage + num) % this.pages));
	}

	public void SetPage(int page)
	{
		if (page > this.pages)
		{
			return;
		}
		this.currentPage = page;
		int num = this.pageSize * page;
		this.ShowPage(this.currentPage, num, Mathf.Min(num + this.pageSize, this.entriesCount));
	}

	protected abstract void ShowPage(int selectedPage, int startIndex, int endIndex);

	protected abstract void PageEntrySelected(int pageEntry, int selectionIndex);

	protected BasePageHandler()
	{
	}

	[CompilerGenerated]
	private int <selectedIndex>k__BackingField;

	[CompilerGenerated]
	private int <currentPage>k__BackingField;

	[CompilerGenerated]
	private int <pages>k__BackingField;

	[CompilerGenerated]
	private int <maxEntires>k__BackingField;
}

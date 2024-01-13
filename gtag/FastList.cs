using System;
using System.Collections.Generic;
using UnityEngine;

public class FastList<T>
{
	public delegate int CompareFunc(T left, T right);

	public T[] array;

	public int size;

	public int Count
	{
		get
		{
			return size;
		}
		set
		{
		}
	}

	public T this[int i]
	{
		get
		{
			return array[i];
		}
		set
		{
			array[i] = value;
		}
	}

	public FastList()
	{
	}

	public FastList(int size)
	{
		if (size > 0)
		{
			this.size = 0;
			array = new T[size];
		}
		else
		{
			this.size = 0;
		}
	}

	public void Add(T item)
	{
		if (array == null || size == array.Length)
		{
			Allocate();
		}
		array[size] = item;
		size++;
	}

	public void AddUnique(T item)
	{
		if (array == null || size == array.Length)
		{
			Allocate();
		}
		if (!Contains(item))
		{
			array[size] = item;
			size++;
		}
	}

	public void AddRange(IEnumerable<T> items)
	{
		foreach (T item in items)
		{
			Add(item);
		}
	}

	public void Insert(int index, T item)
	{
		if (array == null || size == array.Length)
		{
			Allocate();
		}
		if (index < size)
		{
			for (int num = size; num > index; num--)
			{
				array[num] = array[num - 1];
			}
			array[index] = item;
			size++;
		}
		else
		{
			Add(item);
		}
	}

	public bool Remove(T item)
	{
		if (array != null)
		{
			for (int i = 0; i < size; i++)
			{
				if (item.Equals(array[i]))
				{
					size--;
					for (int j = i; j < size; j++)
					{
						array[j] = array[j + 1];
					}
					array[size] = default(T);
					return true;
				}
			}
		}
		return false;
	}

	public void RemoveAt(int index)
	{
		if (array != null && size > 0 && index < size)
		{
			size--;
			for (int i = index; i < size; i++)
			{
				array[i] = array[i + 1];
			}
			array[size] = default(T);
		}
	}

	public bool RemoveFast(T item)
	{
		if (array != null)
		{
			for (int i = 0; i < size; i++)
			{
				if (item.Equals(array[i]))
				{
					if (i < size - 1)
					{
						T val = array[size - 1];
						array[size - 1] = default(T);
						array[i] = val;
					}
					else
					{
						array[i] = default(T);
					}
					size--;
					return true;
				}
			}
		}
		return false;
	}

	public void RemoveAtFast(int index)
	{
		if (array != null && index < size && index >= 0)
		{
			if (index == size - 1)
			{
				array[index] = default(T);
			}
			else
			{
				T val = array[size - 1];
				array[index] = val;
				array[size - 1] = default(T);
			}
			size--;
		}
	}

	public bool Contains(T item)
	{
		if (array == null || size <= 0)
		{
			return false;
		}
		for (int i = 0; i < size; i++)
		{
			if (array[i].Equals(item))
			{
				return true;
			}
		}
		return false;
	}

	public int IndexOf(T item)
	{
		if (size <= 0 || array == null)
		{
			return -1;
		}
		for (int i = 0; i < size; i++)
		{
			if (item.Equals(array[i]))
			{
				return i;
			}
		}
		return -1;
	}

	public T Pop()
	{
		if (array != null && size > 0)
		{
			T result = array[size - 1];
			array[size - 1] = default(T);
			size--;
			return result;
		}
		return default(T);
	}

	public T[] ToArray()
	{
		Trim();
		return array;
	}

	public void Sort(CompareFunc comparer)
	{
		int num = 0;
		int num2 = size - 1;
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = num; i < num2; i++)
			{
				if (comparer(array[i], array[i + 1]) > 0)
				{
					T val = array[i];
					array[i] = array[i + 1];
					array[i + 1] = val;
					flag = true;
				}
				else if (!flag)
				{
					num = ((i != 0) ? (i - 1) : 0);
				}
			}
		}
	}

	public void InsertionSort(CompareFunc comparer)
	{
		for (int i = 1; i < size; i++)
		{
			T val = array[i];
			int num = i;
			while (num > 0 && comparer(array[num - 1], val) > 0)
			{
				array[num] = array[num - 1];
				num--;
			}
			array[num] = val;
		}
	}

	public IEnumerator<T> GetEnumerator()
	{
		if (array != null)
		{
			for (int i = 0; i < size; i++)
			{
				yield return array[i];
			}
		}
	}

	public T Find(Predicate<T> match)
	{
		if (match != null && array != null)
		{
			for (int i = 0; i < size; i++)
			{
				if (match(array[i]))
				{
					return array[i];
				}
			}
		}
		return default(T);
	}

	private void Allocate()
	{
		T[] array = ((this.array != null) ? new T[Mathf.Max(this.array.Length << 1, 32)] : new T[32]);
		if (this.array != null && size > 0)
		{
			this.array.CopyTo(array, 0);
		}
		this.array = array;
	}

	private void Trim()
	{
		if (size > 0)
		{
			T[] array = new T[size];
			for (int i = 0; i < size; i++)
			{
				array[i] = this.array[i];
			}
			this.array = array;
		}
		else
		{
			this.array = null;
		}
	}

	public void Clear()
	{
		size = 0;
	}

	public void Release()
	{
		Clear();
		array = null;
	}
}

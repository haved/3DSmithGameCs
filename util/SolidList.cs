using System;

namespace DSmithGameCs
{
	public class SolidList<T>
	{
		readonly int capacity;
		int count = 0;

		readonly T[] items;

		public int Capacity {
			get {
				return capacity;
			}
		}
		public int Count {
			get {
				return count;
			}
		}

		public SolidList (int capacity)
		{
			this.capacity = capacity;
			items = new T[capacity];
		}

		public void Add(T item)
		{
			for (int i = count; i < capacity; i++)
				if (items [i] == null) {
					items [i] = item;
					break;
				}
			count++;
		}

		public void RemoveAt(int index)
		{
			items [index] = default(T);
		}

		public T this[int i]{
			get{ return items [i]; }
			set{ items [i] = value;}
		}
	}
}


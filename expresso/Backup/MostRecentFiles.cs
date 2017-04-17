using System;
using System.Collections;

namespace Expresso
{
	/// <summary>
	/// This implements an array of recently used file names, with a specified limited capacity. 
	/// Elements are added to the beginning of the list and then fall out the bottom if too many
	/// are added. If a FileName is added that is already in the list, it is bumped to the front of
	/// the list, but appears only once.
	/// </summary>
	public class MostRecentFiles
	{
		private ArrayList MRUList;
		private int capacity=10;

		/// <summary>
		///  Here is the indexer that returns the desired string if the index is within range
		/// </summary>
		public string this [int index]
		{
			get 
			{
				if(index>=0 && index<MRUList.Count)return (string)MRUList[index];
				else return null;
			}
		}

		/// <summary>
		/// Get the number of elements in the list
		/// </summary>
		public int Count
		{
			get{return MRUList.Count;}
		}

		// This specifies the maximum number of filenames to store in the list
		public int Capacity
		{
			get{return capacity;}
			set
			{  // If the capacity is set less than the number in the array, remove items from the end
				capacity=value;
				if(MRUList.Count>capacity)MRUList.RemoveRange(capacity,MRUList.Count-capacity);
			}
		}

		public MostRecentFiles()
		{
			MRUList = new ArrayList();
		}

		/// <summary>
		/// Add a file name to the MRU List
		/// </summary>
		/// <param name="FileName"></param>
		public void Add(string FileName)
		{
			if(MRUList.Contains(FileName))
			{
				// If it is already in the list, move it to the top
				MRUList.Remove(FileName);
				MRUList.Insert(0,FileName);
			}
			else 
			{
				MRUList.Insert(0,FileName);
				if(MRUList.Count>Capacity)
				{
					MRUList.RemoveAt(MRUList.Count-1);
				}
			}
		}

		/// <summary>
		/// Add an array of file names to the MRU List
		/// </summary>
		/// <param name="files"></param>
		public void AddRange(string[] files)
		{
			foreach(string s in files)
			{
				Add(s);
			}
		}

		/// <summary>
		/// Remove all items from the MRU List
		/// </summary>
		public void Clear()
		{
			MRUList.Clear();
		}


		public void Remove(string FileName)
		{
			if(MRUList.Contains(FileName))MRUList.Remove(FileName);
		}

		/// <summary>
		/// Return an array of strings containting the MRU file names
		/// </summary>
		/// <returns></returns>
		public string[] GetFileNames()
		{
			string[] files = new string[MRUList.Count];
			for(int i=0;i<MRUList.Count;i++)
			{
				files[i]=(string)MRUList[i];
			}
			return files;
		}
	}
}

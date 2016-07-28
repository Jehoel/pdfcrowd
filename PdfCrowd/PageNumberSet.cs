using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PdfCrowd
{
	/// <summary>Represents a set of page numbers in sorted-order (in 0, 1, 2, 3, 4, -5, -4, -3, -2, -1 order).</summary>
	public class PageNumberSet : IEnumerable<Int32>
	{
		private class PageNumberComparer : IComparer<Int32>
		{
			public Int32 Compare(Int32 x, Int32 y)
			{
				// Normal ordering, except reversed if negative.
				// 0, 1, 2, 3, 4, -5, -4, -3, -2, -1

				// There are 4 classes of input:
				if( x == y ) return 0;

				if     ( x < 0 && y < 0 ) // "x: -4, y: -3" or "x: -4, y: -10"
				{
					return x.CompareTo( y );
				}
				else if( x < 0 && y >= 0 ) // "x: -4, y: 3"
				{
					return 1;
				}
				else if( x >= 0 && y < 0 ) // "x: 4, y: -3"
				{
					return -1;
				}
				else if( x >= 0 && y >= 0 ) // "x: 4, y: 3" or "x: 4, y: 7"
				{
					return x.CompareTo( y );
				}
				else
				{
					throw new InvalidProgramException("This should never happen.");
				}
			}
		}

		private static readonly PageNumberComparer _comparer = new PageNumberComparer();

		/// <summary>Returns an enumerator that iterates through the sorted page numbers.</summary>
		public IEnumerator<Int32> GetEnumerator()
		{
			return this.GetSortedPageNumbers().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Returns a comma-separated list (without spaces) of page numbers, in the intended order (see Summary of PageNumberSet class).</summary>
		public override String ToString()
		{
			IEnumerable<Int32> sortedPageNumbers = this.GetSortedPageNumbers();

			StringBuilder sb = new StringBuilder();

			Boolean first = true;
			foreach(Int32 pageNumber in sortedPageNumbers)
			{
				if( !first ) sb.Append(",");
				first = false;
				sb.Append( pageNumber.ToString( CultureInfo.InvariantCulture ) );
			}

			if( first ) return null;

			return sb.ToString();
		}

		/// <summary>Adds all of the specified numbers into this instance.</summary>
		/// <param name="numbers">If null, this method returns safely.</param>
		public void AddRange(IEnumerable<Int32> numbers)
		{
			if( numbers == null ) return;

			foreach(Int32 n in numbers) this.Add( n );
		}

#if DOTNET45

		private readonly SortedSet<Int32> set = new SortedSet<Int32>( _comparer );

		/// <summary>Adds the specified page number to this instance. If a number already exists it won't be added again (and returns false, otherwise returns true if added successfully).</summary>
		public void Add(Int32 pageNumber)
		{
			if( !this.set.Contains( pageNumber ) )
			{
				this.set.Add( pageNumber );
			}
		}

		/// <summary>Indicates if the specified number exists in this set.</summary>
		public Boolean Contains(Int32 pageNumber)
		{
			return this.set.Contains( pageNumber );
		}

		/// <summary>Removes the specified number from this set. Returns true if the removal was successful because the <paramref name="pageNumber" /> exists in this set.</summary>
		public void Remove(Int32 pageNumber)
		{
			this.set.Remove( pageNumber );
		}

		private IEnumerable<Int32> GetSortedPageNumbers()
		{
			return this.set;
		}

#elif DOTNET35
		
		private readonly HashSet<Int32> set = new HashSet<Int32>();

		/// <summary>Adds the specified page number to this instance. If a number already exists it won't be added again (and returns false, otherwise returns true if added successfully).</summary>
		public void Add(Int32 pageNumber)
		{
			if( !this.set.Contains( pageNumber ) )
			{
				this.set.Add( pageNumber );
			}
		}

		/// <summary>Indicates if the specified number exists in this set.</summary>
		public Boolean Contains(Int32 pageNumber)
		{
			return this.set.Contains( pageNumber );
		}

		/// <summary>Removes the specified number from this set. Returns true if the removal was successful because the <paramref name="pageNumber" /> exists in this set.</summary>
		public void Remove(Int32 pageNumber)
		{
			this.set.Remove( pageNumber );
		}

		private IEnumerable<Int32> GetSortedPageNumbers()
		{
			Int32[] pageNumbers = new Int32[ this.set.Count ];
			this.set.CopyTo( pageNumbers, 0 );

			Array.Sort( pageNumbers, _comparer );
			
			return pageNumbers;
		}

#elif DOTNET20
		
		private readonly Dictionary<Int32,Byte> dict = new Dictionary<Int32,Byte>();

		/// <summary>Adds the specified page number to this instance. If a number already exists it won't be added again (and returns false, otherwise returns true if added successfully).</summary>
		public Boolean Add(Int32 pageNumber)
		{
			if( !this.dict.ContainsKey( pageNumber ) )
			{
				this.dict.Add( pageNumber, 1 );
				return true;
			}
			return false;
		}

		/// <summary>Indicates if the specified number exists in this set.</summary>
		public Boolean Contains(Int32 pageNumber)
		{
			return this.dict.ContainsKey( pageNumber );
		}

		/// <summary>Removes the specified number from this set. Returns true if the removal was successful because the <paramref name="pageNumber" /> exists in this set.</summary>
		public Boolean Remove(Int32 pageNumber)
		{
			return this.dict.Remove( pageNumber );
		}

		private IEnumerable<Int32> GetSortedPageNumbers()
		{
			Int32[] pageNumbers = new Int32[ this.dict.Keys.Count ];
			this.dict.Keys.CopyTo( pageNumbers, 0 );

			Array.Sort( pageNumbers, _comparer );

			return pageNumbers;
		}

#else

#error No DOTNETxx symbol defined

#endif
	}
}

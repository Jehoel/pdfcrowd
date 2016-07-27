using System;
using System.ComponentModel;
using System.Globalization;

namespace PdfCrowd
{
	/// <summary>Represents a mutable single-dimension length value for PDF documents, with an associated unit.</summary>
	public class Length : IComparable<Length>, IEquatable<Length>
	{
		/// <summary>Constructs a new Length instance with <c>Unspecified</c> units.</summary>
		public Length(Decimal value) : this(value, LengthUnit.Unspecified)
		{
		}

		/// <summary>Constructs a new Length instance with the specified units.</summary>
		public Length(Decimal value, LengthUnit unit)
		{
			this.Value = value;
			this.Unit  = unit;
		}

		/// <summary>Constructs a new Length instance as a copy of the specified instance.</summary>
		/// <param name="copyFrom"></param>
		public Length(Length copyFrom)
		{
			if( copyFrom == null ) throw new ArgumentNullException( nameof(copyFrom) );

			this.Value = copyFrom.Value;
			this.Unit  = copyFrom.Unit;
		}

		private Decimal length;

		/// <summary>The length value. Negative values (except -1) are not permitted.</summary>
		/// <exception cref="ArgumentOutOfRangeException">The value was below <c>0</c> but was not <c>-1</c>.</exception>
		public Decimal Value
		{
			get { return this.length; }
			set
			{
				if( value < 0 && value != -1 ) throw new ArgumentOutOfRangeException( nameof( value ) );
				this.length = value;
			}
		}

		private LengthUnit unit;

		/// <summary>The unit associated with this length.</summary>
		/// <exception cref="InvalidEnumArgumentException">The value was not a defined <c>LengthUnit</c> enumeration.</exception>
		public LengthUnit Unit
		{
			get { return this.unit; }
			set
			{
				if( !Enum.IsDefined( typeof(LengthUnit), value ) ) throw new InvalidEnumArgumentException( nameof(value), (Int32)value, typeof(LengthUnit) );
				this.unit = value;
			}
		}

		/// <summary>Returns a string equal to <c>Value</c> concatenated with the current unit's symbol, if not <c>Unspecified</c>. For example "123" or "17in".</summary>
		public override String ToString()
		{
			return this.Value.ToString( CultureInfo.InvariantCulture ) + GetUnitSymbol( this.Unit );
		}

		#region Unit Utility

		/// <summary>Returns a string that is the abbreviated name (or symbol) of the unit. For example, "cm" for Centimeters, "in" for Inches, etc.</summary>
		/// <exception cref="InvalidEnumArgumentException">The value was not a defined <c>LengthUnit</c> enumeration.</exception>
		public static String GetUnitSymbol(LengthUnit unit)
		{
			switch(unit)
			{
				case LengthUnit.Unspecified:
					return "";
				case LengthUnit.Points:
					return "pt";
				case LengthUnit.Inches:
					return "in";
				case LengthUnit.Centimeters:
					return "cm";
				case LengthUnit.Millimeters:
					return "mm";
				default:
					throw new InvalidEnumArgumentException( nameof(unit), (Int32)unit, typeof(LengthUnit) );
			}
		}

		/// <summary>Gets a conversion factor (to millimeters) for the given <c>LengthUnit</c>.</summary>
		/// <exception cref="InvalidEnumArgumentException">The value was not a defined <c>LengthUnit</c> enumeration.</exception>
		public static Decimal GetMillimeterFactor(LengthUnit unit)
		{
			switch(unit)
			{
				case LengthUnit.Unspecified:
				case LengthUnit.Points:
					return 0.352778M;
				case LengthUnit.Inches:
					return 25.4M;
				case LengthUnit.Centimeters:
					return 10M;
				case LengthUnit.Millimeters:
					return 1M;
				default:
					throw new InvalidEnumArgumentException( nameof(unit), (Int32)unit, typeof(LengthUnit) );
			}
		}

		#endregion

		/// <summary>Converts the current length to millimeters. Points are assumed to be "PostScript Points" which are 1/72 of an inch.</summary>
		public Decimal ToMillimeters()
		{
			Decimal factor = GetMillimeterFactor( this.Unit );
			return this.Value * factor;
		}

		#region IComparable + IEquatable

		/// <summary>Compares a Length value to another. The comparison is based on the Millimeter length of both Length values (that is, both instances are converted to Millimeters, then compared).</summary>
		public static Int32 CompareTo(Length first, Length second)
		{
			if( Equals( first, second ) ) return 0;

			if( Object.ReferenceEquals( first,  null ) ) return -1;
			if( Object.ReferenceEquals( second, null ) ) return  1;

			Decimal xmm = first.ToMillimeters();
			Decimal ymm = second.ToMillimeters();

			return xmm.CompareTo( ymm );
		}

		/// <summary>Compares this Length value to another. The comparison is based on the Millimeter length of both Length values (that is, both instances are converted to Millimeters, then compared).</summary>
		public Int32 CompareTo(Length other)
		{
			return CompareTo( this, other );
		}

		/// <summary>Indicates if both Length values are exactly identical (memberwise) to another. So 10cm is not considered equal to 100mm.</summary>
		public static Boolean Equals(Length first, Length second)
		{
			if( Object.ReferenceEquals( first, second  ) ) return true;

			if( Object.ReferenceEquals( first,  null ) ) return false;
			if( Object.ReferenceEquals( second, null ) ) return false;

			return
				first.Value == second.Value &&
				first.Unit  == second.Unit;
		}

		/// <summary>Indicates if this Length value is exactly identical (memberwise) to an other. So 10cm is not considered equal to 100mm.</summary>
		public Boolean Equals(Length other)
		{
			if( other == null ) return false;

			return Equals( this, other );
		}

		/// <summary>Calls through to Equals(Length). Returns false if <paramref name="obj" /> is null or not a Length instance.</summary>
		public override Boolean Equals(Object obj)
		{
			return this.Equals( obj as Length );
		}

		/// <summary>Gets a hash-code for this instance.</summary>
		public override Int32 GetHashCode()
		{
			Int32 hash = 17;
			hash = hash * 23 + this.Value.GetHashCode();
			hash = hash * 23 + this.Unit.GetHashCode();
			return hash;
		}

		/// <summary>Calls into <c>Length.Equals</c>.</summary>
		public static Boolean operator==(Length first, Length second)
		{
			return Equals( first, second );
		}

		/// <summary>Calls into <c>Length.Equals</c>.</summary>
		public static Boolean operator!=(Length first, Length second)
		{
			return !Equals( first, second );
		}

		/// <summary>Calls into <c>Length.CompareTo</c>.</summary>
		public static Boolean operator<(Length first, Length second)
		{
			return CompareTo( first, second ) == -1;
		}

		/// <summary>Calls into <c>Length.CompareTo</c>.</summary>
		public static Boolean operator>(Length first, Length second)
		{
			return CompareTo( first, second ) == 1;
		}

		#endregion
	}
}

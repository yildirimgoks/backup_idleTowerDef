using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Assets.Scripts
{
    [DataContract]
    public class BigIntWithUnit : IComparable, ICloneable
    {
        private static readonly string[] Units =
        {
            "",
            "Thousand",
            "Million",
            "Billion",
            "Trillion",
            "Quadrillion",
            "Quintillion",
            "Sextillion",
            "Septillion",
            "Octillion",
            "Nonillion",
            "Decillion",
            "Undecillion",
            "Duodecillion",
            "Tredecillion",
            "Quattuordecillion",
            "Quinquadecillion",
            "Sedecillion",
            "Septendecillion",
            "Octodecillion",
            "Novendecillion",
            "Vigintillion",
            "Unvigintillion",
            "Duovigintillion",
            "Tresvigintillion",
            "Quattuorvigintillion",
            "Quinquavigintillion",
            "Sesvigintillion",
            "Septemvigintillion",
            "Octovigintillion",
            "Novemvigintillion",
            "Trigintillion",
            "Untrigintillion",
            "Duotrigintillion",
            "Trestrigintillion",
            "Quattuortrigintillion",
            "Quinquatrigintillion",
            "Sestrigintillion",
            "Septentrigintillion",
            "Octotrigintillion",
            "Noventrigintillion",
            "Quadragintillion",
            "Quinquagintillion",
            "Sexagintillion",
            "Septuagintillion",
            "Octogintillion",
            "Nonagintillion",
            "Centillion",
            "Uncentillion",
            "Duocentillion",
            "Trescentillion",
            "Decicentillion",
            "Undecicentillion",
            "Viginticentillion",
            "Unviginticentillion",
            "Trigintacentillion",
            "Quadragintacentillion",
            "Quinquagintacentillion",
            "Sexagintacentillion",
            "Septuagintacentillion",
            "Octogintacentillion",
            "Nonagintacentillion",
            "Ducentillion",
            "Trecentillion",
            "Quadringentillion",
            "Quingentillion",
            "Sescentillion",
            "Septingentillion",
            "Octingentillion",
            "Nongentillion",
            "Millinillion"
        };

        [DataMember]
        private List<ushort> _intArray { get; set; }

        public BigIntWithUnit()
        {
            _intArray = new List<ushort> { 0 };
        }

        public static implicit operator BigIntWithUnit(int v)
        {
            return new BigIntWithUnit(v.ToString());
        }

        public BigIntWithUnit(string numberAsString)
        {
            _intArray = new List<ushort>();
            numberAsString = numberAsString.PadLeft(numberAsString.Length + (3 - (numberAsString.Length % 3)), '0');
            for (var i = 0; i < numberAsString.Length; i = i + 3)
            {
                _intArray.Add(ushort.Parse(numberAsString.Substring(i, 3)));
            }
            _intArray.Reverse();
            Trim();
        }

        public static bool operator <(BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            return elem1.CompareTo(elem2) < 0;
        }

        public static bool operator <=(BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            return !(elem1 > elem2);
        }

        public static bool operator >(BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            if (ReferenceEquals(null, elem1)) return false;
            return elem1.CompareTo(elem2) > 0;
        }

        public static bool operator >=(BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            return !(elem1 < elem2);
        }

        public static bool operator ==(BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            if (ReferenceEquals(null, elem1)) return false;
            return elem1.CompareTo(elem2) == 0;
        }

        public static bool operator !=(BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            if (ReferenceEquals(null, elem1)) return false;
            return elem1.CompareTo(elem2) != 0;
        }

        public static BigIntWithUnit operator +(BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            BigIntWithUnit result = new BigIntWithUnit();
            result.Add(elem1);
            result.Add(elem2);
            return result;
        }

        public static BigIntWithUnit operator -(BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            BigIntWithUnit result = new BigIntWithUnit();
            result.Add(elem1);
            result.Sub(elem2);
            return result;
        }

		public static BigIntWithUnit operator ++(BigIntWithUnit elem1)
		{
			BigIntWithUnit result = new BigIntWithUnit();
			result = elem1 + 1;
			return result;
		}

		public static BigIntWithUnit operator --(BigIntWithUnit elem1)
		{
			BigIntWithUnit result = new BigIntWithUnit();
			result = elem1 - 1;
			return result;
		}

		public static BigIntWithUnit operator *(BigIntWithUnit elem1, BigIntWithUnit elem2)
		{
			BigIntWithUnit result = 0;
			for (BigIntWithUnit i = 0; i < elem1; i++) {
				result = result + elem2;
			}
			return result;
		}

		public static BigIntWithUnit operator *(BigIntWithUnit elem1, int elem2)
		{
			BigIntWithUnit result = new BigIntWithUnit();
			result.Add(elem1);
			elem1.Multiply(elem2);
			return result;
		}

		public static BigIntWithUnit operator *(BigIntWithUnit elem1, double elem2)
		{
			BigIntWithUnit result = new BigIntWithUnit();
			result.Add(elem1);
			elem1.Multiply((float)elem2, 2);
			return result;
		}

		public static BigIntWithUnit operator *(BigIntWithUnit elem1, float elem2)
		{
			BigIntWithUnit result = new BigIntWithUnit();
			result.Add(elem1);
			elem1.Multiply(elem2, 2);
			return result;
		}

		public static float operator / (BigIntWithUnit elem1, BigIntWithUnit elem2)
		{
            return elem1.Divide(elem2);
		}

        /// <summary>
        /// Calculates elem1 * elem2/100
        /// </summary>
        /// <param name="elem1"></param>
        /// <param name="elem2"></param>
        /// <returns></returns>
        public static BigIntWithUnit MultiplyPercent(BigIntWithUnit elem1, double elem2)
        {
            BigIntWithUnit result = new BigIntWithUnit();
            if (elem2 < 100)
            {
                return result;
            }
            result.Add(elem1);
            result.IncreasePercent((int)elem2 - 100);
            return result;
        }

        public int Length()
        {
            var result = (_intArray.Count - 1) * 3;
            result += _intArray.Last().ToString().Length;
            return result;
        }

        public ushort SafeGetPart(int i)
        {
            if (i >= 0 && _intArray.Count > i)
            {
                return _intArray[i];
            }
            return 0;
        }

        public void Trim()
        {
            while (_intArray.Count > 1 && _intArray.Last() == 0)
            {
                _intArray.RemoveAt(_intArray.Count - 1);
            }
        }

        /// <summary>
        /// Pads the internal array to prevent possible null pointer exceptions
        /// </summary>
        /// <param name="resultingSize">resultingSize given in 3 digits, 2 results in Xxxx, 3 results in Xxxxxxx </param>
        public void Pad(int resultingSize)
        {
            while (_intArray.Count <= resultingSize)
            {
                _intArray.Add(0);
            }
        }

        /// <summary>
        /// Sets the value in place for 10^i
        /// </summary>
        /// <param name="i"></param>
        /// <param name="value"></param>
        public void SafeSetPart(int i, ushort value)
        {
            ushort shortValue = (ushort)(value % 1000);
            if (shortValue >= 1000)
            {
                throw new ArgumentException("Value cannot be bigger than 1000");
            }

            Pad(i);
            _intArray[i] = shortValue;

            value = (ushort)(value / 1000);
            if (value > 0)
            {
                SafeSetPart(i + 1, value);
            }
        }

        public void Add(BigIntWithUnit other)
        {
            // +1 for overflow
            Pad((other.Length() / 3) + 1);
            ushort overflow = 0;
            for (int i = 0; i < _intArray.Count; i++)
            {
                _intArray[i] += (ushort)(other.SafeGetPart(i) + overflow);
                overflow = (ushort)(_intArray[i] / 1000);
                _intArray[i] = (ushort)(_intArray[i] % 1000);
            }
            Trim();
        }

        public void Sub(BigIntWithUnit other)
        {
            if (this <= other)
            {
                _intArray = new List<ushort> { 0 };
                return;
            }

            for (int i = 0; i < _intArray.Count; i++)
            {
                if (_intArray[i] < other.SafeGetPart(i))
                {
                    _intArray[i + 1] -= 1;
                    _intArray[i] += 1000;
                }
                _intArray[i] -= other.SafeGetPart(i);
            }
        }

        /// <summary>
        /// Calculates number + (number * percent / 100)
        /// </summary>
        /// <param name="percent"></param>
        public void IncreasePercent(int percent)
        {
            var thousand = percent * 10;
            var toAdd = new BigIntWithUnit();

            ushort overflow = (ushort)(_intArray[0] * thousand / 1000);

            for (int i = 1; i <= _intArray.Count + 1; i++)
            {
                var result = SafeGetPart(i) * thousand;
                result += overflow;
                toAdd.SafeSetPart(i - 1, (ushort)(result % 1000));
                overflow = (ushort)(result / 1000);
            }

            Add(toAdd);
        }

		public void Multiply(float elem2, int precision)
		{
			Multiply((int) (elem2 * Math.Pow(10, precision)));
		}

		public void Multiply(int elem2)
		{
			ushort overflow = 0;
			int i = 0;
			do {
				var result = SafeGetPart (i) * elem2;
				result += overflow;
				SafeSetPart (i, (ushort)(result % 1000));
				overflow = (ushort)(result / 1000);
				i++;
			} while(!(_intArray.Count < i && overflow == 0));
		}

        /// <summary>
        /// Divides this to elem2 with presision of two digits after comma
        /// </summary>
        public float Divide(BigIntWithUnit elem2)
        {
            if (elem2 == 0 || this == 0)
            {
                return 0;
            }
            Trim();
            elem2.Trim();
            if (_intArray.Count - elem2._intArray.Count > 1)
            {
                BigIntWithUnit recursionTemp = (BigIntWithUnit) Clone();
                recursionTemp.ShiftRight(3);
                return 1000.0f + recursionTemp.Divide(elem2);
            }
            if (elem2._intArray.Count - _intArray.Count > 1)
            {
                return 0.00f;
            }

            //Actual division by substraction
            //Only need the first two parts because of accuracy
            BigIntWithUnit tempElem1 = 0;
            BigIntWithUnit tempElem2 = 0;
            if (_intArray.Count > 1)
            {
                tempElem1.SafeSetPart(0, SafeGetPart(_intArray.Count - 2));
                tempElem1.SafeSetPart(1, SafeGetPart(_intArray.Count - 1));
            }
            else
            {
                tempElem1.SafeSetPart(0, SafeGetPart(_intArray.Count - 1));
            }
            if (elem2._intArray.Count > 1)
            {
                tempElem2.SafeSetPart(0, elem2.SafeGetPart(elem2._intArray.Count - 2));
                tempElem2.SafeSetPart(1, elem2.SafeGetPart(elem2._intArray.Count - 1));
            }
            else
            {
                tempElem2.SafeSetPart(0, elem2.SafeGetPart(elem2._intArray.Count - 1));
            }

            float result = 0;
            for (var i = 0; i > -3; i--)
            {
                while (tempElem1 >= tempElem2 && tempElem2 != 0)
                {
                    tempElem1.Sub(tempElem2);
                    result += (float)Math.Pow(10, i);
                }
                var toAdd = (tempElem2.SafeGetPart(1)*100)%1000;
                tempElem2.SafeSetPart(1, (ushort)(tempElem2.SafeGetPart(1) / 10));
                tempElem2.SafeSetPart(0, (ushort)(tempElem2.SafeGetPart(0) / 10 + toAdd));
            }

            return result;
        }

        public void ShiftLeft(int i)
        {
            if (i >= 3)
            {
                for (int j = _intArray.Count-1; j > -1; j--)
                {
                    SafeSetPart(j+1, SafeGetPart(j));
                }
                SafeSetPart(0, 0);
                //Recursion
                ShiftLeft(i-3);
            } else if (i>0)
            {
                int overflow = 0;
                for (int j = 0; j < _intArray.Count; j++)
                {
                    var value = SafeGetPart(j) + overflow;
                    SafeSetPart(j, (ushort)((value % 100) * 10));
                    overflow = value/100;
                }
                //Recursion
                ShiftLeft(i-1);
            }
        }

        public void ShiftRight(int i)
        {
            if (i >= 3)
            {
                for (int j = _intArray.Count - 1; j > -1; j--)
                {
                    SafeSetPart(j, SafeGetPart(j+1));
                }
                //Recursion
                ShiftRight(i - 3);
            }
            else if (i > 0)
            {
                int overflow = 0;
                for (int j = _intArray.Count - 1; j > -1; j--)
                {
                    var value = SafeGetPart(j) + overflow * 100;
                    overflow = value % 10;
                    SafeSetPart(j, (ushort)(value / 10));
                    
                }
                //Recursion
                ShiftRight(i - 1);
            }
        }


        protected bool Equals(BigIntWithUnit other)
        {
            return Equals(_intArray, other._intArray);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BigIntWithUnit)obj);
        }

        public override int GetHashCode()
        {
            return (_intArray != null ? _intArray.GetHashCode() : 0);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;

            var other = obj as BigIntWithUnit;
            if (other == null) throw new ArgumentException("Wrong argument type for comparison");
            Trim();
            other.Trim();
            if (other._intArray.Count > _intArray.Count) return -1;
            if (other._intArray.Count < _intArray.Count) return 1;

            for (var i = _intArray.Count - 1; i >= 0; i--)
            {
                if (SafeGetPart(i) != other.SafeGetPart(i))
                {
                    return SafeGetPart(i).CompareTo(other.SafeGetPart(i));
                }
            }
            return 0;
        }

        public override string ToString()
        {
            if (_intArray.Count == Units.Length)
            {
                return "Cok Oynadin Sen Sanki";
            }

            if (_intArray.Count == 2)
            {
                return _intArray[1] + "" + _intArray[0].ToString().PadLeft(3, '0');
            }
            var unitString = Units[_intArray.Count - 1];
            var result = _intArray.Last().ToString();

            if (result.Length < 3 && _intArray.Count > 1)
            {
                result = result + "," + _intArray[_intArray.Count - 2].ToString().PadLeft(3, '0').Substring(0, 3 - result.Length);
            }

            //ToDo: Abbreviate UnitString properly
            result += " " + unitString;

            return result;
        }

        public object Clone()
        {
            var clone = new BigIntWithUnit();
            for (var i = 0; i<_intArray.Count; i++) { 
                clone.SafeSetPart(i,_intArray[i]);
            }
            return clone;
        }
    }
}
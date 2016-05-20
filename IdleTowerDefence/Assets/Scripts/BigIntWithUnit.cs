using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions.Comparers;

namespace Assets.Scripts
{
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

        private List<ushort> _intArray { get; set; }

        public BigIntWithUnit()
        {
            _intArray = new List<ushort>{ 0 };
        }

        public static implicit operator BigIntWithUnit(int v)
        {
            return new BigIntWithUnit(v.ToString());
        }

        public BigIntWithUnit(string numberAsString)
        {
            _intArray = new List<ushort>();

            while (numberAsString.Length % 3 != 0)
            {
                numberAsString = numberAsString.PadLeft(1, '0');
            }

            for (var i = 0; i < numberAsString.Length; i=i+3)
            {
                _intArray.Add(ushort.Parse(numberAsString.Substring(i, 3)));
            }
        }

        public static bool operator < (BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            return elem1.CompareTo(elem2) < 0;
        }

        public static bool operator > (BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            return elem1.CompareTo(elem2) > 0;
        }

        public static bool operator == (BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            return elem1 != null && elem1.CompareTo(elem2) == 0;
        }

        public static bool operator != (BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            return elem1 != null && elem1.CompareTo(elem2) != 0;
        }

        public static BigIntWithUnit operator + (BigIntWithUnit elem1, BigIntWithUnit elem2)
        {
            BigIntWithUnit result = new BigIntWithUnit();
            result.Add(elem1);
            result.Add(elem2);
            return result;
        }

        public int Length()
        {
            var result = (_intArray.Count - 1)*3;
            result += _intArray.Last().ToString().Length;
            return result;
        }

        public ushort SafeGetPart(int i)
        {
            if (_intArray.Count > i)
            {
                return _intArray[i];
            }
            return 0;
        }

        public void Trim()
        {
            while (_intArray.Last().Equals(0))
            {
                _intArray.RemoveAt(_intArray.Count-1);
            }
        }
        
        /// <summary>
        /// Pads the internal array to prevent possible null pointer exceptions
        /// </summary>
        /// <param name="resultingSize">resultingSize given in 3 digits, 2 results in Xxxx, 3 results in Xxxxxxx </param>
        public void Pad(int resultingSize)
        {
            while (_intArray.Count < resultingSize)
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
            if (value >= 1000)
            {
                throw new ArgumentException("Value cannot be bigger than 1000");
            }

            Pad(i);
            _intArray[i] = value;
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
        }

        /// <summary>
        /// Calculates number + (number * percent / 100)
        /// </summary>
        /// <param name="percent"></param>
        public void IncreasePercent(int percent)
        {
            var thousand = percent*10;
            var toAdd = new BigIntWithUnit();

            ushort overflow = 0;
            
            for (int i = 1; i < _intArray.Count; i++)
            {
                var result = _intArray[i]*thousand;
                result += overflow;
                toAdd.SafeSetPart(i - 1, (ushort)(result % 1000));
                overflow = (ushort)(result / 1000);
            }

            Add(toAdd);
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var other = obj as BigIntWithUnit;
            if (other == null) throw new ArgumentException("Wrong argument type for comparison");

            for (var i = _intArray.Count-1; i > 0; i--)
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
            var unitString = Units[_intArray.Count - 1];
            var result = _intArray.Last().ToString();

            if (result.Length < 3)
            {
                result = result + "," + _intArray[_intArray.Count - 2].ToString().Substring(0, 3 - result.Length);
            }

            //ToDo: Abbreviate UnitString properly
            result += " " + unitString;

            return result;
        }

        public object Clone()
        {
            BigIntWithUnit clone = new BigIntWithUnit();
            clone._intArray.AddRange(_intArray);
            return clone;
        }
    }
}

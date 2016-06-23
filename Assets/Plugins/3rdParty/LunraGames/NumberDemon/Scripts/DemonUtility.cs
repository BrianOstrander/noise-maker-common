using System;
using UnityRandom = UnityEngine.Random;

namespace LunraGames.NumberDemon
{
	public static class DemonUtility
	{
		static Random _Generator;
		static Random Generator { get { return _Generator ?? (_Generator = new Random()); } }

		const uint UintHalfValue = (uint.MaxValue / 2u) + 1u;
		const ulong UlongHalfValue = (ulong.MaxValue / 2uL) + 1uL;

		public static int IntSeed { get { return UnityRandom.Range(int.MinValue, int.MaxValue); } }

		public static long LongSeed 
		{ 
			get 
			{ 
				var bytes = new byte[8];
				Generator.NextBytes(bytes);
				return BitConverter.ToInt64(bytes, 0);
			}
		}

		public static int CantorPair(int value0, int value1)
		{
			return Convert.ToInt32(CantorPair(Convert.ToUInt32(value0), Convert.ToUInt32(value1)));
		}

		public static uint CantorPair(uint value0, uint value1)
		{
			return (((value0 + value1) * (value0 + value1 + 1u)) / 2u) + value1;
		}

		public static long CantorPair(long value0, long value1)
		{
			return Convert.ToInt64(CantorPair(Convert.ToUInt64(value0), Convert.ToUInt64(value1)));
		}

		public static ulong CantorPair(ulong value0, ulong value1)
		{
			return (((value0 + value1) * (value0 + value1 + 1uL)) / 2uL) + value1;
		}

		public static uint ToUint(int value)
		{
			if (value < 0) return UintHalfValue - ((uint)Math.Abs((long)value));
			else return UintHalfValue + (uint)value;
		}

		public static int ToInt(uint value)
		{
			if (value < UintHalfValue) return 0 - (int)(UintHalfValue - value);
			else return (int)(value - UintHalfValue);
		}

		public static ulong ToUlong(long value)
		{
			if (value < 0L) 
			{
				if (value == long.MinValue) return 0uL;
				else return UlongHalfValue - ((ulong)Math.Abs(value));
			}
			else return UlongHalfValue + (ulong)value;
		}

		public static long ToLong(ulong value)
		{
			if (value < UlongHalfValue) return 0L - (long)(UlongHalfValue - value);
			else return (long)(value - UlongHalfValue);
		}
	}
}
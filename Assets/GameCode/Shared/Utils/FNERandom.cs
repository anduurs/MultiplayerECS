using Unity.Mathematics;

namespace FNZ.Shared.Utils
{
	public static class FNERandom
	{
		private static System.Random random = null;

		static FNERandom()
		{
			random = new System.Random(1);
		}

		public static void InitSeed(int seed)
		{
			random = new System.Random(seed);
		}

		public static int GetRandomIntInRange(int start, int end)
		{
			if (end - start == 0)
				return start;

			return start + (random.Next() % (end - start));
		}

		public static float GetRandomFloatInRange(float start, float end)
		{
			//double mantissa = (random.NextDouble() * 2.0) - 1.0;
			//double exponent = Mathf.Pow(2.0f, random.Next(-126, 128));
			//float ret = Mathf.Abs((float)(mantissa * exponent));
			//return start + (ret % (end - start));
			return (float)(random.NextDouble() * (end - start) + start);
		}

		public static float2 GetRandomVector2(float minRadius, float maxRadius)
		{
			float2 vec = math.normalize(new float2(GetRandomIntInRange(-1000, 1000),
				GetRandomIntInRange(-1000, 1000))) * GetRandomFloatInRange(minRadius, maxRadius);
			return vec;
		}

		public static bool Roll(float oddsOfTrue)
		{
			return oddsOfTrue > GetRandomFloatInRange(0, 1);
		}
	}
}

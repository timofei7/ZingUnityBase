using System.Collections;
using System.Collections.Generic;
using System;

public static class Extensions  {

	public static void Shuffle<T>(this IList<T> list)
	{
		Random rand = new Random();
		for (int i = list.Count-1; i > 0; i--)
		{
			int j = rand.Next(i+1);
			T val = list[j];
			list[j] = list[i];
			list[i] = val;
		}
	}

}

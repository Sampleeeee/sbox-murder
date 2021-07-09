using System;
using System.Collections.Generic;
using System.Linq;

namespace Murder
{
	public static class Extensions
	{
		public static T Random<T>( this IEnumerable<T> enumerable )
		{
			T[] array = enumerable.ToArray();
			return array[new Random().Next( array.Length )];
		}
	}
}

using System;

namespace Murder
{
	public static class Extensions
	{
		public static T Random<T>( this T[] array )
		{
			return array[new Random().Next( array.Length )];
		}
	}
}

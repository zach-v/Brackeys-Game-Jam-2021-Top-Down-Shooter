using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Components
{
	public static class Extensions
	{
		/// <summary>
		/// Adds all items given a collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="items"></param>
		public static void AddAll<T>(this IList<T> collection, params T[] items)
		{
			try
			{
				foreach (T item in items)
					collection.Add(item);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error with AddAll { e.Message }\n{ e.StackTrace }");
			}
		}
		/// <summary>
		/// Removes all items given a collection.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection"></param>
		/// <param name="items"></param>
		public static void RemoveAll<T>(this IList<T> collection, params T[] items)
		{
			try
			{
				foreach (T item in items)
					collection.Remove(item);
			}
			catch (Exception e)
			{
				Console.WriteLine($"Error with RemoveAll { e.Message }\n{ e.StackTrace }");
			}
		}
	}
}

using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace KaraokeWebApp3
{
	public static class DateTimeExtensions
	{
		public static string GetMonthInRus(this DateTime dt)
		{
			return dt.ToString("MMM");
		}
	}
}

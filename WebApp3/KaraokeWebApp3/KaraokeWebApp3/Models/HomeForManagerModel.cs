using KaraokeWebApp3.Dto;

namespace KaraokeWebApp3.Models
{
	public class HomeForManagerModel
	{
		public List<BookItemDto> Bookings { get; set; }
		public List<User> Users { get; set; }

		/*public string GetMonthName(int i)
		{
			if (i == 5)
				return "Май";
			else
				return "-";

		}*/
	}
}

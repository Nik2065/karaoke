namespace KaraokeWebApp3.Models
{
	public class HomeForManagerModel
	{
		public List<Booking> Bookings { get; set; }
		public List<User> Users { get; set; }

		public string GetMonthName(int i)
		{
			if (i == 5)
				return "Май";
			else
				return "-";

		}
	}
}

using KaraokeWebApp3.Dto;

namespace KaraokeWebApp3.Models
{
	public class HomeForClientModel
    {
		public List<BookItemDto> FutureBookings { get; set; }
		public List<BookItemDto> PastBookings { get; set; }
		//public List<User> Users { get; set; }
	}
}

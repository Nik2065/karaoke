namespace KaraokeWebApp3.Models
{
	public class EditBookForManagerModel
	{
		public List<User> Clients { get; set; }

		public List<Space> Spaces { get; set; }

		public Booking Booking { get; set; }
	}
}

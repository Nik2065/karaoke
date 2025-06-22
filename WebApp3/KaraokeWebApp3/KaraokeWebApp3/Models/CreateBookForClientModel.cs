using KaraokeWebApp3.Dto;

namespace KaraokeWebApp3.Models
{
	public class CreateBookForClientModel
	{
		public List<Space> Spaces { get; set; }
		public List<BookItemDto> BookItems { get; set; }
	}
}

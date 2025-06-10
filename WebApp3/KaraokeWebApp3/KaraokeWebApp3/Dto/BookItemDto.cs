namespace KaraokeWebApp3.Dto
{
	public class BookItemDto
	{
		public int Id { get; set; }
		public int ClientId { get; set; }
		public int AuthorId { get; set; }
		public DateTime DtBegin { get; set; }
		public DateTime DtEnd { get; set; }
		public DateTime Created { get; set; }

		public string SpaceName { get; set; }//название зала
		public int SpaceId { get; set; }

	}



}

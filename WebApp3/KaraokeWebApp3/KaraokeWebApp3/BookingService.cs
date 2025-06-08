namespace KaraokeWebApp3
{
	public class BookingService
	{
		public BookingService(AppDbContext db) 
		{
			_db = db;
		}

		private readonly AppDbContext _db;

		//получаем список бронирований 
		public List<Booking> GetBookings(SearchOptions options)
		{
			var list = _db.Bookings.AsQueryable();
			var dtNow = DateTime.Now;
			if (options.UserId != null)
			{
				list = list.Where(x => x.ClientId == options.UserId);

				if(options.TimeType == TimeType.Future)
				{
					list = list.Where(x => x.DtBegin >= dtNow);
				}
				else if(options.TimeType == TimeType.Past)
				{
					list = list.Where(x => x.DtBegin < dtNow);
				}
			}

			return list.ToList();
		}






		public List<User> GetUsers()
		{
			var list = _db.Users.ToList();


			return list;
		}

		public List<User> GetClients()
		{
			var list = _db.Users.Where(x => x.UserType == (int)UserTypeEnum.Client).ToList();


			return list;
		}

		public List<Space> GetSpaces()
		{
			var list = _db.Spaces.ToList();


			return list;
		}

		public void BookClient(int clientId, DateTime begin, DateTime end, int zalId)
		{
			var zal = _db.Spaces.FirstOrDefault(x => x.Id == zalId);

			var one = new Booking();
			one.ClientId = clientId;
			one.AuthorId = clientId;
			one.DtBegin = begin; one.DtEnd = end;
			one.Created = DateTime.Now;
			one.SpaceName = zal?.SpaceName ?? "-";


			_db.Bookings.Add(one);
			_db.SaveChanges();
		}

		public void BookClientByManager(int clientId, int managerId, DateTime begin, DateTime end, int zalId)
		{
			var zal = _db.Spaces.FirstOrDefault(x => x.Id == zalId);

			var one = new Booking();
			one.ClientId = clientId;
			one.AuthorId = managerId;
			one.DtBegin = begin; one.DtEnd = end;
			one.Created = DateTime.Now;
			one.SpaceName = zal?.SpaceName ?? "-";


			_db.Bookings.Add(one);
			_db.SaveChanges();
		}

		public void DeleteBookItem(int id)
		{
			var one = _db.Bookings.FirstOrDefault(x => x.Id == id);
			if (one != null)
			{
				_db.Bookings.Remove(one);
				_db.SaveChanges();
			}
		}
	}

	public class SearchOptions
	{
		public SearchOptions()
		{
			UserId = null;
		}

		public int Year { get; set; }
		public int Month { get; set; }

		public int? UserId { get; set; }

		public TimeType TimeType { get; set; }
	}

	public enum TimeType
	{
		All = 0,
		Future = 1,
		Past = 2
	}
}

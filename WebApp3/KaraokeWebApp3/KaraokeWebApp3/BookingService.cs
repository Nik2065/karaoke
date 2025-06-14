using KaraokeWebApp3.Dto;

namespace KaraokeWebApp3
{
	public class BookingService
	{
		public BookingService(AppDbContext db) 
		{
			_db = db;
		}

		private readonly AppDbContext _db;

		/// <summary>
		/// получаем список всех бронирований по фильтру
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public List<BookItemDto> GetBookings(SearchOptions options)
		{
			var list = from b in _db.Bookings
					   join s in _db.Spaces on b.SpaceId equals s.Id
					   select new BookItemDto
					   {
						   Id = b.Id,
						   SpaceId = b.SpaceId,
						   AuthorId = b.AuthorId,
						   ClientId = b.ClientId,
						   Created = b.Created,
						   DtBegin = b.DtBegin,
						   DtEnd = b.DtEnd,
						   SpaceName = s.SpaceName
					   };


			var dtNow = DateTime.Now;
			if (options.UserId != null)
			{

				list = list.Where(x => x.ClientId == options.UserId);
			}

			if (options.TimeType == TimeType.Future)
			{
				list = list.Where(x => x.DtBegin >= dtNow);
			}
			else if (options.TimeType == TimeType.Past)
			{
				list = list.Where(x => x.DtBegin < dtNow);
			}

			//по залу
			if(options.SpaceId != null)
			{
				list = list.Where(x => x.SpaceId == options.SpaceId);
			}

			//var resultList = new List<BookItemDto>();
			//var spaces = _db.Spaces.ToList();

			/*foreach (var item in list) 
			{ 
			
			}*/

			return list.ToList();
		}

		//максимальный период бронирования
		private decimal MaxDurationInHours = 5;

		public void CheckBookParams(DateTime begin, DateTime end, int spaceId)
		{
			//bool result = false;
			
			
			
			//проверка длительности
			var duration = (end - begin).Hours;
			if (duration > MaxDurationInHours)
				throw new CheckException($"Выбрана слишком большая (более {MaxDurationInHours} часов) длительность бронирования. Уменьшите период");

			

			var list = _db.Bookings.Where(x => x.SpaceId == spaceId);

			foreach (var item in list)
			{
				if (
					(begin < item.DtBegin && end <= item.DtBegin)
					|| (begin >= item.DtEnd && end > item.DtEnd))
				{
					//не пересекается с бронями 
				}
				else
				{
					//пересекается. выходим
					throw new CheckException("На это время уже существует бронирование. Выберите другие пераметры бронивароя");
				}

			}


			//return result;
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


		public void CreateBookByClient(int clientId, DateTime begin, DateTime end, int zalId)
		{
			var zal = _db.Spaces.FirstOrDefault(x => x.Id == zalId);

			var one = new Booking();
			one.ClientId = clientId;
			one.AuthorId = clientId;
			one.DtBegin = begin; one.DtEnd = end;
			one.Created = DateTime.Now;
			//one.SpaceName = zal?.SpaceName ?? "-";
			one.SpaceId = zalId;
			

			_db.Bookings.Add(one);
			_db.SaveChanges();
		}

		/// <summary>
		/// Создание бронирования менеджером
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="managerId"></param>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		/// <param name="zalId"></param>
		public void CreateBookByManager(int clientId, int managerId, DateTime begin, DateTime end, int zalId)
		{
			var zal = _db.Spaces.FirstOrDefault(x => x.Id == zalId);

			var one = new Booking();
			one.ClientId = clientId;
			one.AuthorId = managerId;
			one.DtBegin = begin; one.DtEnd = end;
			one.Created = DateTime.Now;
			//one.SpaceName = zal?.SpaceName ?? "-";
			one.SpaceId = zalId;

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

		//id зала
		public int? SpaceId { get; set; }


	}

	public enum TimeType
	{
		All = 0,
		Future = 1,
		Past = 2
	}

	public class CheckCheckBookParamsResult
	{
		public string Msg = "";
		public bool Success = false;
	}

	public class CheckException : Exception
	{
		public CheckException(string msg): base(msg)
		{

		}
	}
}

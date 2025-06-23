using KaraokeWebApp3.Dto;

namespace KaraokeWebApp3
{
	public class BookingService
	{
		public BookingService(AppDbContext db)
		{
			_db = db;
			//_futureBookingPeriod = futureBookingPeriod;
			//_maxDurationInHours = maxDurationInHours;
		}

		public BookingService(AppDbContext db, int futureBookingPeriod, int maxDurationInHours)
		{
			_db = db;
			_futureBookingPeriod = futureBookingPeriod;
			_maxDurationInHours = maxDurationInHours;
		}

		private readonly AppDbContext _db;
		public int _futureBookingPeriod;
		public int _maxDurationInHours;

		/// <summary>
		/// получаем список всех бронирований по фильтру
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public List<BookItemDto> GetBookings(SearchOptions options)
		{
			//ограничиваем период поиска до X дней вперед
			var future = DateTime.Now.Date.AddDays(_futureBookingPeriod);


			var list = from b in _db.Bookings.Where(x => x.DtBegin <= future)
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


			//var dtNow = DateTime.Now;
			if (options.UserId != null)
			{

				list = list.Where(x => x.ClientId == options.UserId);
			}

			if (options.BeginPeriod != null)
			{
				list = list.Where(x => x.DtBegin >= options.BeginPeriod);
			}
			
			if (options.EndPeriod != null)
			{
				list = list.Where(x => x.DtBegin < options.EndPeriod);
			}

			//по залу
			if(options.SpaceId != null)
			{
				list = list.Where(x => x.SpaceId == options.SpaceId);
			}

			//var resultList = new List<BookItemDto>();
			//var spaces = _db.Spaces.ToList();

			var result = list.OrderBy(x => x.DtBegin).ToList();
			return result;
		}


		//незабронированное время на весь следующий период
		//начало работы караоке 10.00
		//конец работы караоке 00.00
		public List<NotBookItemDto> GetNotBookings(List<BookItemDto> bookItems, int spaceId)
		{
			var notBookings = new List<NotBookItemDto>();
			var spaces = _db.Spaces.ToList();

			var duration = 14;//часов в день
			

			for (int d = 0; d < _futureBookingPeriod; d++)
			{
				//стартовый час
				var currentWorkHour = DateTime.Now.Date.AddDays(d).AddHours(10);
				for (int i = 0; i < duration; i++)
				{

					var n = new NotBookItemDto {
						DtBegin = currentWorkHour.AddHours(i), 
						DtEnd = currentWorkHour.AddHours(i + 1),
						SpaceId  = spaceId,
						SpaceName = spaces.FirstOrDefault(y=>y.Id==spaceId)?.SpaceName ?? ""
					};
					//если уже забронировано - не добавляем в список
					if (!AlreadyTaken(n.DtBegin, n.DtEnd, bookItems, spaceId))
						notBookings.Add(n);
				}
			}

			return notBookings;
		}

		/// <summary>
		/// возвращаем сущность бронирования из базы по id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Booking? GetBooking(int id)
		{
			return _db.Bookings.FirstOrDefault(x => x.Id == id);
		}

		public void SaveBooking(Booking booking)
		{
			var one =  _db.Bookings.FirstOrDefault(x => x.Id == booking.Id);
			if (one != null)
			{
				one = booking;
				_db.SaveChanges();
			}
		}

		//Проверка:Пересекается с уже забрнированным временем
		//periodBegin, periodEnd - проверяемый период
		public bool AlreadyTaken(DateTime periodBegin, DateTime periodEnd, List<BookItemDto> bookings, int spaceId)
		{
			var result = false;


			//var list = _db.Bookings.Where(x => x.SpaceId == spaceId);

			foreach (var item in bookings)
			{

				//if (periodBegin >= item.DtBegin && periodBegin >= item.DtEnd && 
				//	periodEnd<=item.DtBegin && periodEnd <= item.DtEnd)

				if(periodBegin>item.DtBegin && periodBegin<item.DtEnd
					|| periodBegin>=item.DtBegin && periodEnd<=item.DtEnd
					|| periodEnd<item.DtBegin && periodEnd>item.DtEnd
					)
				{
					result = true;
					break;
				}

				/*if (
					(periodBegin < item.DtBegin && periodEnd <= item.DtBegin)
					|| (periodBegin >= item.DtEnd && periodEnd > item.DtEnd))
				{
					//не пересекается с бронями 
				}
				else
				{
					//пересекается. выходим
					throw new CheckException("На это время уже существует бронирование. Выберите другие пераметры бронивароя");
				}*/

			}


			return result;
		}

		//период уже забронирован
		public bool AlreadyTaken(DateTime periodBegin, DateTime periodEnd, int spaceId, int? bookToExclude = null)
		{
			var result = false;
			var d = DateTime.Now.Date;

			var bookings = _db.Bookings.Where(x => x.SpaceId == spaceId && x.DtBegin>d).ToList();

			if (bookToExclude != null)
				bookings = bookings.Where(x => x.Id != bookToExclude).ToList();

			foreach (var item in bookings)
			{
				//if (periodBegin >= item.DtBegin && periodBegin >= item.DtEnd && 
				//	periodEnd<=item.DtBegin && periodEnd <= item.DtEnd)

				if (periodBegin > item.DtBegin && periodBegin < item.DtEnd
					|| periodBegin >= item.DtBegin && periodEnd <= item.DtEnd
					|| periodEnd < item.DtBegin && periodEnd > item.DtEnd
					)
				{
					result = true;
					break;
				}
			}

			return result;
		}



		//максимальный период бронирования
		//private decimal MaxDurationInHours = 5;
		//todo
		//bookToExclude - если редактируем бронирование то исключаем его из проверки
		public bool CheckBookParams(int userId, DateTime begin, DateTime end, int spaceId, int? bookToExclude = null)
		{
			bool result = false;
			//var boo
			
			if(end<=begin)
				throw new CheckException($"Конец периода бронирования должен быть позже чем начало");


			//проверка длительности
			var duration = (end - begin).Hours;
			if (duration > _maxDurationInHours)
				throw new CheckException($"Выбрана слишком большая (более {_maxDurationInHours} часов) длительность бронирования. Уменьшите период");

			if (duration == 0)
				throw new CheckException($"Длительность бронирования не может быть нулевой. Увеличьте период");


			if (AlreadyTaken(begin, end, spaceId, bookToExclude))
					throw new CheckException("На это время уже существует бронирование. Выберите другие пераметры");

			return result;
		}

		//создать бронирование
		/*public void CreateBookItem(DateTime begin, DateTime end, int spaceId)
		{

		}*/


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


		/// <summary>
		/// Создение бронирования клиентом
		/// </summary>
		/// <param name="clientId"></param>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		/// <param name="zalId"></param>
		public void CreateBookByClient(int clientId, DateTime begin, DateTime end, int spaceId)
		{
			var zal = _db.Spaces.FirstOrDefault(x => x.Id == spaceId);

			var one = new Booking();
			one.ClientId = clientId;
			one.AuthorId = clientId;
			one.DtBegin = begin; 
			one.DtEnd = end;
			one.Created = DateTime.Now;
			one.SpaceId = spaceId;

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
		/// <param name="spaceId"></param>
		public void CreateBookByManager(int clientId, int managerId, DateTime begin, DateTime end, int spaceId)
		{
			var zal = _db.Spaces.FirstOrDefault(x => x.Id == spaceId);

			var one = new Booking();
			one.ClientId = clientId;
			one.AuthorId = managerId;
			one.DtBegin = begin; 
			one.DtEnd = end;
			one.Created = DateTime.Now;
			one.SpaceId = spaceId;

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

		//public int Year { get; set; }
		//public int Month { get; set; }
		public DateTime? BeginPeriod { get; set; }
		public DateTime? EndPeriod { get; set; }

		public int? UserId { get; set; }

		//public TimeType TimeType { get; set; }

		//id зала
		public int? SpaceId { get; set; }


	}

	/*public enum TimeType
	{
		All = 0,
		Future = 1,
		Past = 2
	}*/

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

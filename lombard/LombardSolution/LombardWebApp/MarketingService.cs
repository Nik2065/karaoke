namespace LombardWebApp
{
    public class MarketingService
    {

        public MarketingService(AppDbContext db) 
        {
            _db = db;
        }

        private readonly AppDbContext _db;

        //Добавить опции поиска
        public List<Lead> GetLeads()
        {
            var list = _db.Leads.ToList();

            return list;
        }

        public void AddLead(string name, string phone)
        {
            var a = new Lead { Username = name, Phone = phone, Created = DateTime.Now };
            _db.Leads.Add(a);
            _db.SaveChanges();
        }

        //рассчитываем займ
        public decimal CalcDeposit(DepositTypeEnum type, decimal sum)
        {
            decimal rate = 0;


            if (type == DepositTypeEnum.Auto)
                rate = 0.96m;
            else if (type == DepositTypeEnum.Jewelry)
                rate = 1.08m;
            else if (type == DepositTypeEnum.Other)
                rate = 0.9m;
            else
                throw new Exception("Не найдена ставка для вычисления");

            decimal result = (decimal)sum * (1 + rate);
            result = Math.Round(result / 12, 2);

            return result;
		}

    }

	public enum DepositTypeEnum
	{
		NotInList = -1,
		Auto = 1,
		Jewelry = 2,
		Other = 3,
	}
}

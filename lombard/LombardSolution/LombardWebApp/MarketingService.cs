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

    }
}

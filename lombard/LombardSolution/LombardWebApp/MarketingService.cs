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

    }
}

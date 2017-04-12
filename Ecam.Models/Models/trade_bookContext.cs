using System.Data.Entity;
using System.Data.Entity.Infrastructure;
namespace Ecam.Models{
	public partial class EcamContext : DbContext {
		static EcamContext() {
            //Database.SetInitializer<trade_bookContext>(null);
			Database.SetInitializer<EcamContext>(null);
        }
		public EcamContext(): base("Name=EcamContext") {
		}
        public DbSet<tra_category> tra_category { get; set; }
        public DbSet<tra_company> tra_company { get; set; }
        public DbSet<tra_company_category> tra_company_category { get; set; }
        public DbSet<tra_market> tra_market { get; set; }
		public DbSet<aspnetuserroles> aspnetuserroles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new tra_categoryMap());
            modelBuilder.Configurations.Add(new tra_companyMap());
            modelBuilder.Configurations.Add(new tra_company_categoryMap());
            modelBuilder.Configurations.Add(new tra_marketMap());
			modelBuilder.Configurations.Add(new aspnetuserrolesMap());
             
        }
    }
}

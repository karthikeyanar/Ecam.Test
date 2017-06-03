using System.Data.Entity;
using System.Data.Entity.Infrastructure;
namespace Ecam.Models{
	public partial class EcamContext : DbContext {
		static EcamContext() {
            //Database.SetInitializer<ecamtest_backupContext>(null);
			Database.SetInitializer<EcamContext>(null);
        }
		public EcamContext(): base("Name=EcamContext") {
		}
        public DbSet<tra_category> tra_category { get; set; }
        public DbSet<tra_company> tra_company { get; set; }
        public DbSet<tra_company_category> tra_company_category { get; set; }
        public DbSet<tra_intra_day_profit> tra_intra_day_profit { get; set; }
        public DbSet<tra_market> tra_market { get; set; }
        public DbSet<tra_market_intra_day> tra_market_intra_day { get; set; }
        public DbSet<tra_mutual_fund> tra_mutual_fund { get; set; }
        public DbSet<tra_mutual_fund_pf> tra_mutual_fund_pf { get; set; }
		public DbSet<aspnetuserroles> aspnetuserroles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new tra_categoryMap());
            modelBuilder.Configurations.Add(new tra_companyMap());
            modelBuilder.Configurations.Add(new tra_company_categoryMap());
            modelBuilder.Configurations.Add(new tra_intra_day_profitMap());
            modelBuilder.Configurations.Add(new tra_marketMap());
            modelBuilder.Configurations.Add(new tra_market_intra_dayMap());
            modelBuilder.Configurations.Add(new tra_mutual_fundMap());
            modelBuilder.Configurations.Add(new tra_mutual_fund_pfMap());
			modelBuilder.Configurations.Add(new aspnetuserrolesMap());
        }
    }
}

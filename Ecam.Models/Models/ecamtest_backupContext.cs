using System.Data.Entity;
using System.Data.Entity.Infrastructure;
namespace Ecam.Models {
    public partial class EcamContext:DbContext {
        static EcamContext() {
            //Database.SetInitializer<ecamtest_backupContext>(null);
            Database.SetInitializer<EcamContext>(null);
        }
        public EcamContext() : base("Name=EcamContext") {
        }
        public DbSet<tra_category> tra_category { get; set; }
        public DbSet<tra_category_profit> tra_category_profit { get; set; }
        public DbSet<tra_company> tra_company { get; set; }
        public DbSet<tra_company_category> tra_company_category { get; set; }
        public DbSet<tra_holding> tra_holding { get; set; }
        public DbSet<tra_market> tra_market { get; set; }
        public DbSet<tra_market_avg> tra_market_avg { get; set; }
        public DbSet<tra_market_intra_day> tra_market_intra_day { get; set; }
        public DbSet<tra_mutual_fund> tra_mutual_fund { get; set; }
        public DbSet<tra_mutual_fund_pf> tra_mutual_fund_pf { get; set; }
        public DbSet<tra_split> tra_split { get; set; }
        public DbSet<tra_daily_log> tra_daily_log { get; set; }
        public DbSet<aspnetuserroles> aspnetuserroles { get; set; }
        public DbSet<tra_year_log> tra_year_log { get; set; }
        public DbSet<tra_financial> tra_financial { get; set; }
        public DbSet<tra_financial_category> tra_financial_category { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Configurations.Add(new tra_categoryMap());
            modelBuilder.Configurations.Add(new tra_category_profitMap());
            modelBuilder.Configurations.Add(new tra_companyMap());
            modelBuilder.Configurations.Add(new tra_company_categoryMap());
            modelBuilder.Configurations.Add(new tra_holdingMap());
            modelBuilder.Configurations.Add(new tra_marketMap());
            modelBuilder.Configurations.Add(new tra_market_avgMap());
            modelBuilder.Configurations.Add(new tra_market_intra_dayMap());
            modelBuilder.Configurations.Add(new tra_mutual_fundMap());
            modelBuilder.Configurations.Add(new tra_mutual_fund_pfMap());
            modelBuilder.Configurations.Add(new tra_splitMap());
            modelBuilder.Configurations.Add(new aspnetuserrolesMap());
            modelBuilder.Configurations.Add(new tra_daily_logMap());
            modelBuilder.Configurations.Add(new tra_year_logMap());
            modelBuilder.Configurations.Add(new tra_financial_categoryMap());
            modelBuilder.Configurations.Add(new tra_financialMap());
        }
    }
}

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
        public DbSet<tra_intra_day_profit_calc> tra_intra_day_profit_calc { get; set; }
        public DbSet<tra_market> tra_market { get; set; }
        public DbSet<tra_market_intra_day> tra_market_intra_day { get; set; }
        public DbSet<tra_mutual_fund> tra_mutual_fund { get; set; }
        public DbSet<tra_mutual_fund_pf> tra_mutual_fund_pf { get; set; }
        public DbSet<tra_pre_calc_item> tra_pre_calc_item { get; set; }
        public DbSet<tra_prev_calc> tra_prev_calc { get; set; }
        public DbSet<tra_rsi_profit> tra_rsi_profit { get; set; }
		public DbSet<aspnetuserroles> aspnetuserroles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new tra_categoryMap());
            modelBuilder.Configurations.Add(new tra_companyMap());
            modelBuilder.Configurations.Add(new tra_company_categoryMap());
            modelBuilder.Configurations.Add(new tra_intra_day_profitMap());
            modelBuilder.Configurations.Add(new tra_intra_day_profit_calcMap());
            modelBuilder.Configurations.Add(new tra_marketMap());
            modelBuilder.Configurations.Add(new tra_market_intra_dayMap());
            modelBuilder.Configurations.Add(new tra_mutual_fundMap());
            modelBuilder.Configurations.Add(new tra_mutual_fund_pfMap());
            modelBuilder.Configurations.Add(new tra_pre_calc_itemMap());
            modelBuilder.Configurations.Add(new tra_prev_calcMap());
            modelBuilder.Configurations.Add(new tra_rsi_profitMap());
			modelBuilder.Configurations.Add(new aspnetuserrolesMap());

			/* SQL Page Helper  */
						List<tra_category> tra_category_List = context.tra_category.Take(1).ToList();
						List<tra_company> tra_company_List = context.tra_company.Take(1).ToList();
						List<tra_company_category> tra_company_category_List = context.tra_company_category.Take(1).ToList();
						List<tra_intra_day_profit> tra_intra_day_profit_List = context.tra_intra_day_profit.Take(1).ToList();
						List<tra_intra_day_profit_calc> tra_intra_day_profit_calc_List = context.tra_intra_day_profit_calc.Take(1).ToList();
						List<tra_market> tra_market_List = context.tra_market.Take(1).ToList();
						List<tra_market_intra_day> tra_market_intra_day_List = context.tra_market_intra_day.Take(1).ToList();
						List<tra_mutual_fund> tra_mutual_fund_List = context.tra_mutual_fund.Take(1).ToList();
						List<tra_mutual_fund_pf> tra_mutual_fund_pf_List = context.tra_mutual_fund_pf.Take(1).ToList();
						List<tra_pre_calc_item> tra_pre_calc_item_List = context.tra_pre_calc_item.Take(1).ToList();
						List<tra_prev_calc> tra_prev_calc_List = context.tra_prev_calc.Take(1).ToList();
						List<tra_rsi_profit> tra_rsi_profit_List = context.tra_rsi_profit.Take(1).ToList();
					/* End SQL Page Helper */

        }
    }
}

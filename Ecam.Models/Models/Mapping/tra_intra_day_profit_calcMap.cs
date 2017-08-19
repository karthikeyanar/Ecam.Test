
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_intra_day_profit_calcMap : EntityTypeConfiguration<tra_intra_day_profit_calc> {
        public tra_intra_day_profit_calcMap() {
		            // Primary Key
		            this.HasKey(t => t.id);
		
            // Properties
            this.Property(t => t.profit)
                .HasPrecision(13,4);

            this.Property(t => t.loss)
                .HasPrecision(13,4);

            // Table & Column Mappings
			this.ToTable("tra_intra_day_profit_calc");
            this.Property(t => t.id).HasColumnName("profit_calc_id");
            this.Property(t => t.trade_date).HasColumnName("trade_date");
            this.Property(t => t.profit).HasColumnName("profit");
            this.Property(t => t.loss).HasColumnName("loss");
			       Ignore(t=>t.created_date);
						       Ignore(t=>t.created_by);
						       Ignore(t=>t.last_updated_date);
						       Ignore(t=>t.last_updated_by);
			        }
    }
}

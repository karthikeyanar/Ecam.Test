
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_daily_logMap : EntityTypeConfiguration<tra_daily_log> {
        public tra_daily_logMap() {
		            // Primary Key
		            this.HasKey(t => t.id);

            // Table & Column Mappings
			this.ToTable("tra_daily_log");
            this.Property(t => t.id).HasColumnName("log_id");
            this.Property(t => t.trade_date).HasColumnName("trade_date");
            this.Property(t => t.negative).HasColumnName("negative");
            this.Property(t => t.positive).HasColumnName("positive");
            this.Property(t => t.is_book_mark).HasColumnName("is_book_mark");
            Ignore(t=>t.created_date);
						       Ignore(t=>t.created_by);
            Ignore(t=>t.last_updated_date);
            Ignore(t=>t.last_updated_by);
			        }
    }
}

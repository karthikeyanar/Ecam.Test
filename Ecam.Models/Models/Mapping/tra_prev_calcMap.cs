
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_prev_calcMap : EntityTypeConfiguration<tra_prev_calc> {
        public tra_prev_calcMap() {
		            // Primary Key
		            this.HasKey(t => t.id);
		
            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.open_profit)
                .HasPrecision(13,4);

            this.Property(t => t.high_profit)
                .HasPrecision(13,4);

            // Table & Column Mappings
			this.ToTable("tra_prev_calc");
            this.Property(t => t.id).HasColumnName("prev_calc_id");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.positive_count).HasColumnName("positive_count");
            this.Property(t => t.negative_count).HasColumnName("negative_count");
            this.Property(t => t.open_profit).HasColumnName("open_profit");
            this.Property(t => t.high_profit).HasColumnName("high_profit");
            this.Property(t => t.success_count).HasColumnName("success_count");
            this.Property(t => t.fail_count).HasColumnName("fail_count");
			       Ignore(t=>t.created_date);
						       Ignore(t=>t.created_by);
						       Ignore(t=>t.last_updated_date);
						       Ignore(t=>t.last_updated_by);
			        }
    }
}

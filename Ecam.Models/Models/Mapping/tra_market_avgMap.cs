
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_market_avgMap : EntityTypeConfiguration<tra_market_avg> {
        public tra_market_avgMap() {
		            // Primary Key
		            this.HasKey(t => t.id);
		
            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.avg_type)
                .IsRequired()
                .HasMaxLength(1);

            this.Property(t => t.percentage)
                .HasPrecision(13,4);

            // Table & Column Mappings
			this.ToTable("tra_market_avg");
            this.Property(t => t.id).HasColumnName("market_avg_id");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.avg_type).HasColumnName("avg_type");
            this.Property(t => t.avg_date).HasColumnName("avg_date");
            this.Property(t => t.percentage).HasColumnName("percentage");
			       Ignore(t=>t.created_date);
						       Ignore(t=>t.created_by);
						       Ignore(t=>t.last_updated_date);
						       Ignore(t=>t.last_updated_by);
			        }
    }
}

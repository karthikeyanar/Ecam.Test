
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_holdingMap : EntityTypeConfiguration<tra_holding> {
        public tra_holdingMap() {
		            // Primary Key
		            this.HasKey(t => t.id);
		
            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.avg_price)
                .HasPrecision(13,4);

            // Table & Column Mappings
			this.ToTable("tra_holding");
            this.Property(t => t.id).HasColumnName("holding_id");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.trade_date).HasColumnName("trade_date");
            this.Property(t => t.quantity).HasColumnName("quantity");
            this.Property(t => t.avg_price).HasColumnName("avg_price");
			       Ignore(t=>t.created_date);
						       Ignore(t=>t.created_by);
						       Ignore(t=>t.last_updated_date);
						       Ignore(t=>t.last_updated_by);
			        }
    }
}

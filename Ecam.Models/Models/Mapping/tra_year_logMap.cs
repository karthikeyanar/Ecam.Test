
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_year_logMap:EntityTypeConfiguration<tra_year_log> {
        public tra_year_logMap() {
            // Primary Key
            this.HasKey(t => new { t.symbol,t.year });

            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.open_price)
                .HasPrecision(13,4);

            this.Property(t => t.close_price)
                .HasPrecision(13,4);

            this.Property(t => t.percentage)
                .HasPrecision(13,4);

            // Table & Column Mappings
            this.ToTable("tra_year_log");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.year).HasColumnName("year");
            this.Property(t => t.open_price).HasColumnName("open_price");
            this.Property(t => t.close_price).HasColumnName("close_price");
            this.Property(t => t.percentage).HasColumnName("percentage");
        }
    }
}

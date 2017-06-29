
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_intra_day_profitMap : EntityTypeConfiguration<tra_intra_day_profit> {
        public tra_intra_day_profitMap() {
		            // Primary Key
            this.HasKey(t => new { t.symbol, t.trade_date });

            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.first_percentage)
                .HasPrecision(13,4);

            this.Property(t => t.last_percentage)
                .HasPrecision(13,4);

            this.Property(t => t.profit_percentage)
                .HasPrecision(13,4);

            this.Property(t => t.reverse_percentage)
                .HasPrecision(13,4);

            this.Property(t => t.final_percentage)
                .HasPrecision(13,4);

            this.Property(t => t.rsi)
    .HasPrecision(13, 4);

            this.Property(t => t.prev_rsi)
    .HasPrecision(13, 4);

            this.Property(t => t.diff_rsi)
.HasPrecision(13, 4);

            // Table & Column Mappings
            this.ToTable("tra_intra_day_profit");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.trade_date).HasColumnName("trade_date");
            this.Property(t => t.first_percentage).HasColumnName("first_percentage");
            this.Property(t => t.last_percentage).HasColumnName("last_percentage");
            this.Property(t => t.profit_percentage).HasColumnName("profit_percentage");
            this.Property(t => t.reverse_percentage).HasColumnName("reverse_percentage");
            this.Property(t => t.final_percentage).HasColumnName("final_percentage");
            this.Property(t => t.high_count).HasColumnName("high_count");
            this.Property(t => t.low_count).HasColumnName("low_count");
            this.Property(t => t.rsi).HasColumnName("rsi");
            this.Property(t => t.prev_rsi).HasColumnName("prev_rsi");
            this.Property(t => t.diff_rsi).HasColumnName("diff_rsi");
        }
    }
}

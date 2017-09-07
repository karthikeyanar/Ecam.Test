
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_marketMap : EntityTypeConfiguration<tra_market> {
        public tra_marketMap() {
		            // Primary Key
            this.HasKey(t => new { t.symbol, t.trade_date });

            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.open_price)
                .HasPrecision(13,4);

            this.Property(t => t.high_price)
                .HasPrecision(13,4);

            this.Property(t => t.low_price)
                .HasPrecision(13,4);

            this.Property(t => t.ltp_price)
                .HasPrecision(13,4);

            this.Property(t => t.close_price)
                .HasPrecision(13,4);

            this.Property(t => t.prev_price)
                .HasPrecision(13,4);

            this.Property(t => t.rsi)
                .HasPrecision(13,4);

            this.Property(t => t.prev_rsi)
                .HasPrecision(13,4);

            this.Property(t => t.upward)
                .HasPrecision(13,4);

            this.Property(t => t.downward)
                .HasPrecision(13,4);

            this.Property(t => t.avg_upward)
                .HasPrecision(13,4);

            this.Property(t => t.avg_downward)
                .HasPrecision(13,4);

            this.Property(t => t.rs)
                .HasPrecision(13,4);

            this.Property(t => t.prev_ltp_price)
                .HasPrecision(13,4);

            // Table & Column Mappings
			this.ToTable("tra_market");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.trade_date).HasColumnName("trade_date");
            this.Property(t => t.open_price).HasColumnName("open_price");
            this.Property(t => t.high_price).HasColumnName("high_price");
            this.Property(t => t.low_price).HasColumnName("low_price");
            this.Property(t => t.ltp_price).HasColumnName("ltp_price");
            this.Property(t => t.close_price).HasColumnName("close_price");
            this.Property(t => t.prev_price).HasColumnName("prev_price");
            this.Property(t => t.rsi).HasColumnName("rsi");
            this.Property(t => t.prev_rsi).HasColumnName("prev_rsi");
            this.Property(t => t.upward).HasColumnName("upward");
            this.Property(t => t.downward).HasColumnName("downward");
            this.Property(t => t.avg_upward).HasColumnName("avg_upward");
            this.Property(t => t.avg_downward).HasColumnName("avg_downward");
            this.Property(t => t.rs).HasColumnName("rs");
            this.Property(t => t.prev_ltp_price).HasColumnName("prev_ltp_price");
												        }
    }
}


using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models
{
    public partial class tra_market_intra_dayMap : EntityTypeConfiguration<tra_market_intra_day>
    {
        public tra_market_intra_dayMap()
        {
            // Primary Key
            this.HasKey(t => new { t.symbol, t.trade_date });

            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ltp_price)
                .HasPrecision(13, 4);

            this.Property(t => t.rsi)
    .HasPrecision(13, 4);

            // Table & Column Mappings
            this.ToTable("tra_market_intra_day");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.trade_date).HasColumnName("trade_date");
            this.Property(t => t.ltp_price).HasColumnName("ltp_price");
            this.Property(t => t.rsi).HasColumnName("rsi");
        }
    }
}

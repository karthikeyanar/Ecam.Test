
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_rsi_profitMap : EntityTypeConfiguration<tra_rsi_profit> {
        public tra_rsi_profitMap() {
		            // Primary Key
		            this.HasKey(t => t.id);
		
            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.buy_rsi)
                .HasPrecision(13,4);

            this.Property(t => t.sell_rsi)
                .HasPrecision(13,4);

            this.Property(t => t.buy_price)
                .HasPrecision(13,4);

            this.Property(t => t.sell_price)
                .HasPrecision(13,4);

            this.Property(t => t.profit)
                .HasPrecision(13,4);

            // Table & Column Mappings
			this.ToTable("tra_rsi_profit");
            this.Property(t => t.id).HasColumnName("rsi_profit_id");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.buy_rsi).HasColumnName("buy_rsi");
            this.Property(t => t.sell_rsi).HasColumnName("sell_rsi");
            this.Property(t => t.buy_price).HasColumnName("buy_price");
            this.Property(t => t.sell_price).HasColumnName("sell_price");
            this.Property(t => t.buy_date).HasColumnName("buy_date");
            this.Property(t => t.sell_date).HasColumnName("sell_date");
            this.Property(t => t.profit).HasColumnName("profit");
			       Ignore(t=>t.created_date);
						       Ignore(t=>t.created_by);
						       Ignore(t=>t.last_updated_date);
						       Ignore(t=>t.last_updated_by);
			        }
    }
}

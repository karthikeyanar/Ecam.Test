
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models
{
    public partial class tra_financialMap : EntityTypeConfiguration<tra_financial>
    {
        public tra_financialMap()
        {
            // Primary Key
            this.HasKey(t => new { t.symbol, t.financial_category_id, t.financial_date });

            // Properties
            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.value)
                .HasPrecision(13, 4);

            this.Property(t => t.prev_value)
                .HasPrecision(13,4);

            // Table & Column Mappings
            this.ToTable("tra_financial");
            this.Property(t => t.symbol).HasColumnName("symbol");
            this.Property(t => t.financial_category_id).HasColumnName("financial_category_id");
            this.Property(t => t.financial_date).HasColumnName("financial_date");
            this.Property(t => t.value).HasColumnName("value");
            this.Property(t => t.prev_value).HasColumnName("prev_value");
        }
    }
}


using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models {
    public partial class tra_company_categoryMap : EntityTypeConfiguration<tra_company_category> {
        public tra_company_categoryMap() {
		            // Primary Key
            this.HasKey(t => new { t.category_name, t.symbol });

            // Properties
            this.Property(t => t.category_name)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.symbol)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
			this.ToTable("tra_company_category");
            this.Property(t => t.category_name).HasColumnName("category_name");
            this.Property(t => t.symbol).HasColumnName("symbol");
												        }
    }
}


using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models
{
    public partial class tra_financial_categoryMap : EntityTypeConfiguration<tra_financial_category>
    {
        public tra_financial_categoryMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.category_name)
                .IsRequired()
                .HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("tra_financial_category");
            this.Property(t => t.id).HasColumnName("financial_category_id");
            this.Property(t => t.category_name).HasColumnName("category_name");
            Ignore(t => t.created_date);
            Ignore(t => t.created_by);
            Ignore(t => t.last_updated_date);
            Ignore(t => t.last_updated_by);
        }
    }
}

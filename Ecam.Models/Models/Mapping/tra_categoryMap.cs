
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
// namespace CodeFirstClassGenerate.Models.Mapping
namespace Ecam.Models
{
    public partial class tra_categoryMap : EntityTypeConfiguration<tra_category>
    {
        public tra_categoryMap()
        {
            // Primary Key
            this.HasKey(t => t.id);

            // Properties
            this.Property(t => t.category_name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("tra_category");
            this.Property(t => t.id).HasColumnName("category_id");
            this.Property(t => t.category_name).HasColumnName("category_name");
            this.Property(t => t.is_archive).HasColumnName("is_archive");
            this.Property(t => t.is_book_mark).HasColumnName("is_book_mark");
            Ignore(t => t.created_date);
            Ignore(t => t.created_by);
            Ignore(t => t.last_updated_date);
            Ignore(t => t.last_updated_by);
        }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Ecam.Models
{
    public class aspnetuserrolesMap : EntityTypeConfiguration<aspnetuserroles>
    {
		public aspnetuserrolesMap()
        {
            // Primary Key
            this.HasKey(t => new { t.UserId, t.RoleId });

            // Properties
            this.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.RoleId)
                .IsRequired()
				.HasMaxLength(128);
			 
            // Table & Column Mappings
			this.ToTable("aspnetuserroles");
			this.Property(t => t.UserId).HasColumnName("UserId");
			this.Property(t => t.RoleId).HasColumnName("RoleId");
        }
    }
}

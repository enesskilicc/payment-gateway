using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.INFRASTRUCTURE.Persistence.Configurations
{
    public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
    {
        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.ToTable("api_keys");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.KeyHash)
                .HasMaxLength(64)
                .IsRequired();

            builder.HasIndex(x => x.KeyHash)
                .IsUnique();

            builder.Property(x => x.Prefix)
                .HasMaxLength(12)
                .IsRequired();

            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

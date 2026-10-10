using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.INFRASTRUCTURE.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("payments");
            builder.HasKey(x => x.Id);

            builder.OwnsOne(x => x.Amount, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("amount");

                money.Property(x => x.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.OwnsOne(x => x.CapturedAmount, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("captured_amount");

                money.Property(x => x.Currency)
                    .HasColumnName("captured_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.OwnsOne(x => x.RefundedAmount, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("refunded_amount");

                money.Property(x => x.Currency)
                    .HasColumnName("refunded_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();


            builder.HasOne<Merchant>()
                .WithMany()
                .HasForeignKey(x => x.MerchantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

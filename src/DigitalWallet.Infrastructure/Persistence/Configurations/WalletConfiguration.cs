using DigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Infrastructure.Persistence.Configurations
{
    public class WalletConfiguration :IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.Balance).HasPrecision(18, 2).IsRequired().HasDefaultValue(0);
            builder.Property(w => w.Currency).IsRequired().HasMaxLength(3);
            builder.Property(w => w.CreatedAt).HasColumnType("timestamptz");
            builder.Property(w => w.UpdatedAt).HasColumnType("timestamptz");


            builder.HasOne(w => w.User)
                .WithOne(u => u.Wallet)
                .HasForeignKey<Wallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(w => w.UserId).IsUnique();

                

        }
    }
}

using DigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount).IsRequired().HasPrecision(18, 2);

            builder.Property(t => t.Type).HasConversion<string>().IsRequired();
            builder.Property(t => t.Status).HasConversion<string>().IsRequired();
            builder.Property(t => t.CreatedAt).HasColumnType("timestamptz");


            builder.HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Request)
                .WithMany()
                .HasForeignKey(t => t.ReferenceId).IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}

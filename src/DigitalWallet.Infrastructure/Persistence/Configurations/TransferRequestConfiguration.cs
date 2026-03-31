using DigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Infrastructure.Persistence.Configurations
{
    public class TransferRequestConfiguration : IEntityTypeConfiguration<TransferRequest>
    {
        public void Configure(EntityTypeBuilder<TransferRequest> builder)
        {
            builder.ToTable("TransferRequests");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Amount).IsRequired().HasPrecision(18, 2);
            builder.Property(r => r.Status).IsRequired().HasConversion<string>();
            
            builder.Property(r => r.IdempotencyKey).IsRequired();
            builder.HasIndex(r => r.IdempotencyKey).IsUnique();

            builder.Property(r => r.CreatedAt).HasColumnType("timestamptz");
            builder.Property(r => r.CompletedAt).IsRequired(false).HasColumnType("timestamptz");


            builder.HasOne(r => r.SenderWallet)
                .WithMany(w => w.SentTransfers)
                .HasForeignKey(r => r.SenderWalletId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(r => r.ReceiverWallet)
                .WithMany(w => w.ReceivedTransfers)
                .HasForeignKey(r => r.ReceiverWalletId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

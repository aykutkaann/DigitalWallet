using DigitalWallet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration :IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("Audit_Logs");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.EventType).IsRequired().HasMaxLength(255);
            builder.Property(a => a.Payload).IsRequired();

            builder.Property(a => a.CreatedAt).HasColumnType("timestamptz");

            builder.HasIndex(a => a.EntityId);

        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace testZugether1.Models;

public partial class ZugetherContext : DbContext
{
    public ZugetherContext(DbContextOptions<ZugetherContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Album { get; set; }

    public virtual DbSet<Device_List> Device_List { get; set; }

    public virtual DbSet<Favor_List> Favor_List { get; set; }

    public virtual DbSet<Favorites> Favorites { get; set; }

    public virtual DbSet<Landlord> Landlord { get; set; }

    public virtual DbSet<Member> Member { get; set; }

    public virtual DbSet<Message> Message { get; set; }

    public virtual DbSet<Message_Board> Message_Board { get; set; }

    public virtual DbSet<Photo> Photo { get; set; }

    public virtual DbSet<Room> Room { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.album_id).HasName("PK__Album__B0E1DDB2331E977A");
        });

        modelBuilder.Entity<Device_List>(entity =>
        {
            entity.HasKey(e => e.device_list_id).HasName("PK__Device_L__D19D9FC18052BCBE");
        });

        modelBuilder.Entity<Favor_List>(entity =>
        {
            entity.HasKey(e => e.favor_list_id).HasName("PK__Favor_Li__8141CFAAA05927BA");

            entity.HasOne(d => d.member).WithMany(p => p.Favor_List)
                .HasForeignKey(d => d.member_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Favor_List_FK_member_id");
        });

        modelBuilder.Entity<Favorites>(entity =>
        {
            entity.HasKey(e => e.favorites_id).HasName("PK__Favorite__605162DBF586B521");

            entity.HasOne(d => d.favor_list).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.favor_list_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorites_FK_favor_list_id");

            entity.HasOne(d => d.room).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.room_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Favorites_FK_room_id");
        });

        modelBuilder.Entity<Landlord>(entity =>
        {
            entity.HasKey(e => e.landlord_id).HasName("PK__Landlord__0AA6026EB915628F");

            entity.Property(e => e.gender).HasMaxLength(2);
            entity.Property(e => e.name).HasMaxLength(10);
            entity.Property(e => e.phone)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.member_id).HasName("PK__Member__B29B85340D37070F");

            entity.Property(e => e.email)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.gender).HasMaxLength(2);
            entity.Property(e => e.introduce).HasMaxLength(200);
            entity.Property(e => e.job).HasMaxLength(10);
            entity.Property(e => e.jobtime).HasMaxLength(10);
            entity.Property(e => e.name).HasMaxLength(10);
            entity.Property(e => e.nickname).HasMaxLength(10);
            entity.Property(e => e.password)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.phone)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.message_id).HasName("PK__Message__0BBF6EE65701AAF8");

            entity.ToTable(tb => tb.HasTrigger("SetMessageBasement"));

            entity.Property(e => e.message_content).HasMaxLength(200);
            entity.Property(e => e.post_time)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.reply_member_content).HasMaxLength(50);

            entity.HasOne(d => d.member).WithMany(p => p.Messagemember)
                .HasForeignKey(d => d.member_id)
                .HasConstraintName("Message_FK_member_id");

            entity.HasOne(d => d.message_board).WithMany(p => p.Message)
                .HasForeignKey(d => d.message_board_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Message_FK_message_board_id");

            entity.HasOne(d => d.reply_member).WithMany(p => p.Messagereply_member)
                .HasForeignKey(d => d.reply_member_id)
                .HasConstraintName("Message_FK_reply_member_id");
        });

        modelBuilder.Entity<Message_Board>(entity =>
        {
            entity.HasKey(e => e.message_board_id).HasName("PK__Message___341A5D2743197A43");

            entity.HasOne(d => d.room).WithMany(p => p.Message_Board)
                .HasForeignKey(d => d.room_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Message_Board_FK_room_id");
        });

        modelBuilder.Entity<Photo>(entity =>
        {
            entity.HasKey(e => e.photo_id).HasName("PK__Photo__CB48C83D811EFDC7");

            entity.Property(e => e.photo_type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.album).WithMany(p => p.Photo)
                .HasForeignKey(d => d.album_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Photo_FK_album_id");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.room_id).HasName("PK__Room__19675A8A420383C2");

            entity.Property(e => e.address).HasMaxLength(100);
            entity.Property(e => e.bed_type).HasMaxLength(20);
            entity.Property(e => e.lease_type).HasMaxLength(20);
            entity.Property(e => e.pay_type).HasMaxLength(20);
            entity.Property(e => e.prefer_jobtime).HasMaxLength(20);
            entity.Property(e => e.room_title).HasMaxLength(50);
            entity.Property(e => e.room_type).HasMaxLength(20);
            entity.Property(e => e.roommate_description).HasMaxLength(200);

            entity.HasOne(d => d.device_list).WithMany(p => p.Room)
                .HasForeignKey(d => d.device_list_id)
                .HasConstraintName("ROOM_FK_device_list_id");

            entity.HasOne(d => d.landlord).WithMany(p => p.Room)
                .HasForeignKey(d => d.landlord_id)
                .HasConstraintName("Rom_FK_landlord_id");

            entity.HasOne(d => d.member).WithMany(p => p.Room)
                .HasForeignKey(d => d.member_id)
                .HasConstraintName("Room_FK_member_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

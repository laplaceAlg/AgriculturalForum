using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AgriculturalForum.Web.Models;

public partial class KltnDbContext : DbContext
{
    public KltnDbContext()
    {
    }

    public KltnDbContext(DbContextOptions<KltnDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CategoryPost> CategoryPosts { get; set; }

    public virtual DbSet<CategoryProduct> CategoryProducts { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostReply> PostReplies { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-USTNBI6\\MSSQL;Database=KLTN_DB;User Id=sa;Password=123456;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CategoryPost>(entity =>
        {
            entity.ToTable("CategoryPost");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<CategoryProduct>(entity =>
        {
            entity.ToTable("CategoryProduct");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.ToTable("Post");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.CategoryPost).WithMany(p => p.Posts)
                .HasForeignKey(d => d.CategoryPostId)
                .HasConstraintName("FK_Post_CategoryPost");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Post_Users");
        });

        modelBuilder.Entity<PostReply>(entity =>
        {
            entity.ToTable("PostReply");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.PostReplies)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_PostReply_Post1");

            entity.HasOne(d => d.User).WithMany(p => p.PostReplies)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PostReply_Users");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Image).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.CategoryProduct).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryProductId)
                .HasConstraintName("FK_Product_CategoryProduct");

            entity.HasOne(d => d.User).WithMany(p => p.Products)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Product_Users");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.Property(e => e.Image).HasMaxLength(255);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductImages_Product");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.ProvinceName);

            entity.Property(e => e.ProvinceName).HasMaxLength(255);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_User");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.Birthday).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.MemberSince).HasColumnType("datetime");
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.ProfileImage).HasMaxLength(255);
            entity.Property(e => e.Province).HasMaxLength(255);

            entity.HasOne(d => d.ProvinceNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Province)
                .HasConstraintName("FK_Users_Provinces");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

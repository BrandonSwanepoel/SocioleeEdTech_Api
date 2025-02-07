//using System;
//using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;

//namespace SocioleeMarkingApi.Models.Database;

//public partial class DatabaseContext : DbContext
//{
//    public DatabaseContext()
//    {
//    }

//    public DatabaseContext(DbContextOptions<DatabaseContext> options)
//        : base(options)
//    {
//    }

//    public virtual DbSet<ContentAsset> ContentAssets { get; set; }

//    public virtual DbSet<ContentForm> ContentForms { get; set; }

//    public virtual DbSet<ContentFormExamplePhoto> ContentFormExamplePhotos { get; set; }

//    public virtual DbSet<ContentFormPrimaryColor> ContentFormPrimaryColors { get; set; }

//    public virtual DbSet<ContentNote> ContentNotes { get; set; }

//    public virtual DbSet<EmailTracking> EmailTrackings { get; set; }

//    public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }

//	public virtual DbSet<StepType> StepType { get; set; }

//	public virtual DbSet<User> Users { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=SocioleeMarking.database.windows.net,1433;Database=SocioleeMarking;User=SocioleeMarking;Password=Govettie27;TrustServerCertificate=True");

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<ContentAsset>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("PK__ContentA__3214EC0702E0AB6E");

//            entity.HasIndex(e => e.UserId, "idx_content_asset_user_id");

//            entity.Property(e => e.Id).ValueGeneratedNever();
//            entity.Property(e => e.Type).HasMaxLength(100);

//            //entity.HasOne(d => d.ContentForm).WithMany(p => p.ContentAssets)
//            //    .HasForeignKey(d => d.ContentFormId)
//            //    .OnDelete(DeleteBehavior.ClientSetNull)
//            //    .HasConstraintName("FK_ContentAsset_ContentForm");

//            entity.HasOne(d => d.User).WithMany(p => p.ContentAssets)
//                .HasForeignKey(d => d.UserId)
//                .OnDelete(DeleteBehavior.ClientSetNull)
//                .HasConstraintName("FK_ContentAsset_User");
//        });

//        modelBuilder.Entity<ContentForm>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("PK__ContentF__3214EC07AE75913E");

//            entity.HasIndex(e => e.UserId, "idx_content_forms_user_id");

//            entity.Property(e => e.Id).ValueGeneratedNever();
//            entity.Property(e => e.ContentType).HasMaxLength(100);
//            entity.Property(e => e.Created).HasColumnType("datetime");
//            entity.Property(e => e.Email).HasMaxLength(100);
//            entity.Property(e => e.PhoneNumber).HasMaxLength(20);

//            entity.HasOne(d => d.User).WithMany(p => p.ContentForms)
//                .HasForeignKey(d => d.UserId)
//                .OnDelete(DeleteBehavior.ClientSetNull)
//                .HasConstraintName("FK_ContentForm_User");
//        });

//        modelBuilder.Entity<ContentFormExamplePhoto>(entity =>
//        {
//            entity.HasKey(e => new { e.ContentFormId, e.ExamplePhotoId }).HasName("PK__ContentF__0BBF7445066C56A2");

//            entity.HasIndex(e => e.ContentFormId, "idx_example_photo_ids_content_form_id");

//            //entity.HasOne(d => d.ContentForm).WithMany(p => p.ContentFormExamplePhotos)
//            //    .HasForeignKey(d => d.ContentFormId)
//            //    .OnDelete(DeleteBehavior.ClientSetNull)
//            //    .HasConstraintName("FK_ContentFormExamplePhotos_ContentForm");
//        });

//        modelBuilder.Entity<ContentFormPrimaryColor>(entity =>
//        {
//            entity.HasKey(e => new { e.ContentFormId, e.PrimaryColor }).HasName("PK__ContentF__BB6513A708277E09");

//            entity.HasIndex(e => e.ContentFormId, "idx_content_form_primary_colors_content_form_id");

//            entity.Property(e => e.PrimaryColor).HasMaxLength(20);

//            //entity.HasOne(d => d.ContentForm).WithMany(p => p.ContentFormPrimaryColors)
//            //    .HasForeignKey(d => d.ContentFormId)
//            //    .OnDelete(DeleteBehavior.ClientSetNull)
//            //    .HasConstraintName("FK_ContentFormPrimaryColors_ContentForm");
//        });

//        modelBuilder.Entity<ContentNote>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("PK__ContentN__3214EC072959632A");

//            entity.HasIndex(e => e.ContentFormId, "idx_content_notes_content_form_id");

//            entity.Property(e => e.Id).ValueGeneratedNever();
//            entity.Property(e => e.Created).HasColumnType("datetime");

//            //entity.HasOne(d => d.ContentForm).WithMany(p => p.ContentNotes)
//            //    .HasForeignKey(d => d.ContentFormId)
//            //    .OnDelete(DeleteBehavior.ClientSetNull)
//            //    .HasConstraintName("FK_ContentNotes_ContentForm");

//            entity.HasOne(d => d.User).WithMany(p => p.ContentNotes)
//                .HasForeignKey(d => d.UserId)
//                .OnDelete(DeleteBehavior.ClientSetNull)
//                .HasConstraintName("FK_ContentNotes_User");
//        });

//        modelBuilder.Entity<EmailTracking>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("PK__EmailTra__3214EC07DC3C7A39");

//            entity.Property(e => e.Id).ValueGeneratedNever();
//        });

//        modelBuilder.Entity<PaymentDetail>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("PK__PaymentD__3214EC07B1097670");

//            entity.HasIndex(e => e.UserId, "IX_PaymentDetails_UserId");

//            entity.Property(e => e.Id).ValueGeneratedNever();
//            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

//            entity.HasOne(d => d.User).WithMany(p => p.PaymentDetails).HasForeignKey(d => d.UserId);
//        });

//        modelBuilder.Entity<User>(entity =>
//        {
//            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07C3EC83A0");

//            entity.Property(e => e.Id).ValueGeneratedNever();
//        });

//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}

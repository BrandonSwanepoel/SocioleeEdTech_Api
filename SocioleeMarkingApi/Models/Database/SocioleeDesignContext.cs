using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SocioleeMarkingApi.Models.Database;

public partial class SocioleeDesignContext : DbContext
{
    public SocioleeDesignContext()
    {
    }

    public SocioleeDesignContext(DbContextOptions<SocioleeDesignContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssetAspectRatio> AssetAspectRatios { get; set; }

    public virtual DbSet<ContentAsset> ContentAssets { get; set; }

    public virtual DbSet<ContentForm> ContentForms { get; set; }

    public virtual DbSet<ContentFormExamplePhoto> ContentFormExamplePhotos { get; set; }

    public virtual DbSet<ContentFormPrimaryColor> ContentFormPrimaryColors { get; set; }

    public virtual DbSet<ContentStep> ContentSteps { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<DesignChangeRequest> DesignChangeRequests { get; set; }

    public virtual DbSet<EmailTracking> EmailTrackings { get; set; }

    public virtual DbSet<Institution> Institutions { get; set; }

    public virtual DbSet<InstitutionCourse> InstitutionCourses { get; set; }

    public virtual DbSet<InstitutionLecturer> InstitutionLecturers { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageAsset> MessageAssets { get; set; }

    public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }

    public virtual DbSet<PaymentPackage> PaymentPackages { get; set; }

    public virtual DbSet<PaymentRequest> PaymentRequests { get; set; }

    public virtual DbSet<ProjectRubric> ProjectRubrics { get; set; }

    public virtual DbSet<ProjectRubricStudentMark> ProjectRubricStudentMarks { get; set; }

    public virtual DbSet<ProjectRubricsOfStudent> ProjectRubricsOfStudents { get; set; }

    public virtual DbSet<RoleType> RoleTypes { get; set; }

    public virtual DbSet<StepType> StepTypes { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentCourse> StudentCourses { get; set; }

    public virtual DbSet<StudentProject> StudentProjects { get; set; }

    public virtual DbSet<StudentProjectAssignment> StudentProjectAssignments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=socioleedesign.database.windows.net,1433;Database=SocioleeDesign;User=socioleedesign;Password=Govettie27;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssetAspectRatio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AssetAsp__3214EC07F69408C1");

            entity.ToTable("AssetAspectRatio");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AspectRatio).HasMaxLength(10);
            entity.Property(e => e.Usage).HasMaxLength(20);
        });

        modelBuilder.Entity<ContentAsset>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentA__3214EC07743DCB36");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContentPath).HasMaxLength(150);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasOne(d => d.AssetAspectRatio).WithMany(p => p.ContentAssets)
                .HasForeignKey(d => d.AssetAspectRatioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ContentAssets_assetAspectRatio");

            entity.HasOne(d => d.ContentForm).WithMany(p => p.ContentAssets)
                .HasForeignKey(d => d.ContentFormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContentAs__Conte__498EEC8D");

            entity.HasOne(d => d.Note).WithMany(p => p.ContentAssets)
                .HasForeignKey(d => d.NoteId)
                .HasConstraintName("FK__ContentAs__NoteI__489AC854");

            entity.HasOne(d => d.User).WithMany(p => p.ContentAssets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("User_Content_Asset");
        });

        modelBuilder.Entity<ContentForm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentF__3214EC07AE75913E");

            entity.HasIndex(e => e.UserId, "idx_content_forms_user_id");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContentHeadline).HasMaxLength(200);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(30);

            entity.HasOne(d => d.AssetAspectRatio).WithMany(p => p.ContentForms)
                .HasForeignKey(d => d.AssetAspectRatioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ContentForms_assetAspectRatio");

            entity.HasOne(d => d.User).WithMany(p => p.ContentForms)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentForm_User");
        });

        modelBuilder.Entity<ContentFormExamplePhoto>(entity =>
        {
            entity.HasKey(e => new { e.ContentFormId, e.ExamplePhotoId }).HasName("PK__ContentF__0BBF7445066C56A2");

            entity.HasIndex(e => e.ContentFormId, "idx_example_photo_ids_content_form_id");

            entity.HasOne(d => d.ContentForm).WithMany(p => p.ContentFormExamplePhotos)
                .HasForeignKey(d => d.ContentFormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentFormExamplePhotos_ContentForm");
        });

        modelBuilder.Entity<ContentFormPrimaryColor>(entity =>
        {
            entity.HasKey(e => new { e.ContentFormId, e.PrimaryColor }).HasName("PK__ContentF__BB6513A708277E09");

            entity.HasIndex(e => e.ContentFormId, "idx_content_form_primary_colors_content_form_id");

            entity.Property(e => e.PrimaryColor).HasMaxLength(20);

            entity.HasOne(d => d.ContentForm).WithMany(p => p.ContentFormPrimaryColors)
                .HasForeignKey(d => d.ContentFormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentFormPrimaryColors_ContentForm");
        });

        modelBuilder.Entity<ContentStep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentS__3214EC072A5B6CC3");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Completed).HasDefaultValueSql("((0))");

            entity.HasOne(d => d.ContentForm).WithMany(p => p.ContentSteps)
                .HasForeignKey(d => d.ContentFormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContentSt__Conte__2DE6D218");

            entity.HasOne(d => d.StepType).WithMany(p => p.ContentSteps)
                .HasForeignKey(d => d.StepTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ContentSt__StepT__2CF2ADDF");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Coupons__3214EC07C06FF1C4");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.Created).HasColumnType("datetime");

            entity.HasOne(d => d.User).WithMany(p => p.Coupons)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Coupons__UserId__7755B73D");
        });

        modelBuilder.Entity<DesignChangeRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContentN__3214EC072959632A");

            entity.HasIndex(e => e.ContentFormId, "idx_content_notes_content_form_id");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Created).HasColumnType("datetime");

            entity.HasOne(d => d.ContentAsset).WithMany(p => p.DesignChangeRequests)
                .HasForeignKey(d => d.ContentAssetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentNotes_MessageAssetId");

            entity.HasOne(d => d.ContentForm).WithMany(p => p.DesignChangeRequests)
                .HasForeignKey(d => d.ContentFormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentNotes_ContentForm");

            entity.HasOne(d => d.User).WithMany(p => p.DesignChangeRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ContentNotes_User");
        });

        modelBuilder.Entity<EmailTracking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailTra__3214EC07DC3C7A39");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Institution>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Institut__3214EC07EA7260E8");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<InstitutionCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Institut__3214EC07081A4E31");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Course)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Institution).WithMany(p => p.InstitutionCourses)
                .HasForeignKey(d => d.InstitutionId)
                .HasConstraintName("FK__Instituti__Insti__29AC2CE0");
        });

        modelBuilder.Entity<InstitutionLecturer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Institut__3214EC07503C148F");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Institution).WithMany(p => p.InstitutionLecturers)
                .HasForeignKey(d => d.InstitutionId)
                .HasConstraintName("FK__Instituti__Insti__1699586C");

            entity.HasOne(d => d.User).WithMany(p => p.InstitutionLecturers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Instituti__UserI__178D7CA5");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.HasIndex(e => e.RequestId, "IX_Messages_RequestId").IsClustered();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Created).HasColumnType("datetime");

            entity.HasOne(d => d.Asset).WithMany(p => p.Messages)
                .HasForeignKey(d => d.AssetId)
                .HasConstraintName("FK_Messages_AssetId");

            entity.HasOne(d => d.Request).WithMany(p => p.Messages)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_ContentForm");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Users");
        });

        modelBuilder.Entity<MessageAsset>(entity =>
        {
            entity.HasKey(e => e.Id).IsClustered(false);

            entity.HasIndex(e => e.MessageId, "IX_MessageAssets_MessageId").IsClustered();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ContentPath).HasMaxLength(150);
            entity.Property(e => e.Reviewed).HasColumnName("reviewed");
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasOne(d => d.Message).WithMany(p => p.MessageAssets)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageAssets_Id");

            entity.HasOne(d => d.Note).WithMany(p => p.MessageAssets)
                .HasForeignKey(d => d.NoteId)
                .HasConstraintName("FK_MessageAssets_NoteId");
        });

        modelBuilder.Entity<PaymentDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentD__3214EC07B1097670");

            entity.HasIndex(e => e.UserId, "IX_PaymentDetails_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.PaymentPackage).WithMany(p => p.PaymentDetails)
                .HasForeignKey(d => d.PaymentPackageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PaymentPackagesId");

            entity.HasOne(d => d.User).WithMany(p => p.PaymentDetails).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<PaymentPackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentP__3214EC0748517709");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(15);
        });

        modelBuilder.Entity<PaymentRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentR__3214EC0737BF17E6");

            entity.ToTable("PaymentRequest");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Created).HasColumnType("datetime");
            entity.Property(e => e.PaymentReference).HasMaxLength(20);

            entity.HasOne(d => d.User).WithMany(p => p.PaymentRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PaymentRe__UserI__11158940");
        });

        modelBuilder.Entity<ProjectRubric>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectR__3214EC07497F092D");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectRubrics)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__ProjectRu__Proje__038683F8");
        });

        modelBuilder.Entity<ProjectRubricStudentMark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectR__3214EC07D1A72D60");

            entity.ToTable("ProjectRubricStudentMark");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectRubricStudentMarks)
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__ProjectRu__Proje__0E04126B");

            entity.HasOne(d => d.Student).WithMany(p => p.ProjectRubricStudentMarks)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__ProjectRu__Stude__0EF836A4");
        });

        modelBuilder.Entity<ProjectRubricsOfStudent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProjectR__3214EC07160407B3");

            entity.ToTable("ProjectRubricsOfStudent");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Rubric).WithMany(p => p.ProjectRubricsOfStudents)
                .HasForeignKey(d => d.RubricId)
                .HasConstraintName("FK__ProjectRu__Rubri__084B3915");

            entity.HasOne(d => d.Student).WithMany(p => p.ProjectRubricsOfStudents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__ProjectRu__Stude__093F5D4E");
        });

        modelBuilder.Entity<RoleType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RoleType__3214EC07EE04267F");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<StepType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StepType__3214EC0787BA32F8");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(255);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Students__3214EC0769E10E29");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Institution).WithMany(p => p.Students)
                .HasForeignKey(d => d.InstitutionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_student_institution");

            entity.HasOne(d => d.User).WithMany(p => p.Students)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_student_userId");
        });

        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StudentC__3214EC07FC1949BB");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.InstitutionCourses).WithMany(p => p.StudentCourses)
                .HasForeignKey(d => d.InstitutionCoursesId)
                .HasConstraintName("FK__StudentCo__Insti__3335971A");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentCourses)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__StudentCo__Stude__3429BB53");
        });

        modelBuilder.Entity<StudentProject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StudentP__3214EC078E5B5E6A");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDateTime).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.StartDateTime).HasColumnType("datetime");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.StudentProjects)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentProjects_Users");

            entity.HasOne(d => d.InstitutionCourse).WithMany(p => p.StudentProjects)
                .HasForeignKey(d => d.InstitutionCourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_institution_courseId");
        });

        modelBuilder.Entity<StudentProjectAssignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__StudentP__3214EC07A52FED28");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.AssignedByNavigation).WithMany(p => p.StudentProjectAssignments)
                .HasForeignKey(d => d.AssignedBy)
                .HasConstraintName("FK_StudentProjectAssignments_AssignedBy");

            entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.StudentProjectAssignments)
                .HasForeignKey(d => d.AssignedTo)
                .HasConstraintName("FK_StudentProjectAssignments_AssignedTo");

            entity.HasOne(d => d.StudentProject).WithMany(p => p.StudentProjectAssignments)
                .HasForeignKey(d => d.StudentProjectId)
                .HasConstraintName("FK_StudentProjectAssignments_StudentProjects");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07C3EC83A0");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.RoleId).HasDefaultValueSql("('11b3ef99-3974-416e-b72a-7565a59f8615')");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Users_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

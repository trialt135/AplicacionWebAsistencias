  using System;
  using System.Collections.Generic;
  using Microsoft.EntityFrameworkCore;

  namespace AspnetCoreMvcFull.Models;

  public partial class ManagmentRfidDbContext : DbContext
  {
      public ManagmentRfidDbContext()
      {
      }

      public ManagmentRfidDbContext(DbContextOptions<ManagmentRfidDbContext> options)
          : base(options)
      {
      }

      public virtual DbSet<Administrador> Administradors { get; set; }

      public virtual DbSet<Alumno> Alumnos { get; set; }

      public virtual DbSet<Asistencium> Asistencia { get; set; }

      public virtual DbSet<Grupo> Grupos { get; set; }

      public virtual DbSet<Materium> Materia { get; set; }

      public virtual DbSet<Profesor> Profesors { get; set; }

      public virtual DbSet<Registro> Registros { get; set; }

      public virtual DbSet<Rfid> Rfids { get; set; }

      public virtual DbSet<Rol> Rols { get; set; }

      public virtual DbSet<Usuario> Usuarios { get; set; }

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
          => optionsBuilder.UseSqlServer("Server=ALEX\\SQLEXPRESS;Database=managment_Rfid_DB;Trusted_Connection=True;TrustServerCertificate=True;");

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
          modelBuilder.Entity<Administrador>(entity =>
          {
              entity.HasKey(e => e.IdAdministrador).HasName("PK__Administ__2B3E34A8F6D2610D");

              entity.ToTable("Administrador");

              entity.Property(e => e.IdAdministrador).ValueGeneratedNever();
              entity.Property(e => e.AreaTrabajo).HasMaxLength(100);

              entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Administradors)
                  .HasForeignKey(d => d.IdUsuario)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__Administr__IdUsu__5629CD9C");
          });

          modelBuilder.Entity<Alumno>(entity =>
          {
              entity.HasKey(e => e.IdAlumno).HasName("PK__Alumno__460B474052CAE741");

              entity.ToTable("Alumno");

              entity.Property(e => e.IdAlumno).ValueGeneratedNever();

              entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Alumnos)
                  .HasForeignKey(d => d.IdUsuario)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__Alumno__IdUsuari__534D60F1");
          });

          modelBuilder.Entity<Asistencium>(entity =>
          {
              entity.HasKey(e => e.IdAsistencia).HasName("PK__Asistenc__3956DEE6FF26E3B5");

              entity.HasOne(d => d.IdAlumnoNavigation).WithMany(p => p.Asistencia)
                  .HasForeignKey(d => d.IdAlumno)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__Asistenci__IdAlu__6754599E");

              entity.HasOne(d => d.IdRegistroNavigation).WithMany(p => p.Asistencia)
                  .HasForeignKey(d => d.IdRegistro)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__Asistenci__IdReg__66603565");
          });

          modelBuilder.Entity<Grupo>(entity =>
          {
              entity.HasKey(e => e.IdGrupo).HasName("PK__Grupo__303F6FD992EB38A6");

              entity.ToTable("Grupo");

              entity.Property(e => e.NombreGrupo).HasMaxLength(100);

              entity.HasOne(d => d.IdMateriaNavigation).WithMany(p => p.Grupos)
                  .HasForeignKey(d => d.IdMateria)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__Grupo__IdMateria__5FB337D6");

              entity.HasMany(d => d.IdAlumnos).WithMany(p => p.IdGrupos)
                  .UsingEntity<Dictionary<string, object>>(
                      "GrupoAlumno",
                      r => r.HasOne<Alumno>().WithMany()
                          .HasForeignKey("IdAlumno")
                          .OnDelete(DeleteBehavior.ClientSetNull)
                          .HasConstraintName("FK__Grupo_Alu__IdAlu__6B24EA82"),
                      l => l.HasOne<Grupo>().WithMany()
                          .HasForeignKey("IdGrupo")
                          .OnDelete(DeleteBehavior.ClientSetNull)
                          .HasConstraintName("FK__Grupo_Alu__IdGru__6A30C649"),
                      j =>
                      {
                          j.HasKey("IdGrupo", "IdAlumno").HasName("PK__Grupo_Al__245FDBADDC592434");
                          j.ToTable("Grupo_Alumno");
                      });
          });

          modelBuilder.Entity<Materium>(entity =>
          {
              entity.HasKey(e => e.IdMateria).HasName("PK__Materia__EC174670A96156B2");

              entity.Property(e => e.NombreMateria).HasMaxLength(100);

              entity.HasOne(d => d.IdProfesorNavigation).WithMany(p => p.Materia)
                  .HasForeignKey(d => d.IdProfesor)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__Materia__IdProfe__5CD6CB2B");
          });

          modelBuilder.Entity<Profesor>(entity =>
          {
              entity.HasKey(e => e.IdProfesor).HasName("PK__Profesor__C377C3A103C2895A");

              entity.ToTable("Profesor");

              entity.Property(e => e.IdProfesor).ValueGeneratedNever();
              entity.Property(e => e.Departamento).HasMaxLength(100);

              entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Profesors)
                  .HasForeignKey(d => d.IdUsuario)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__Profesor__IdUsua__5070F446");
          });

          modelBuilder.Entity<Registro>(entity =>
          {
              entity.HasKey(e => e.IdRegistro).HasName("PK__Registro__FFA45A99602B90CC");

              entity.ToTable("Registro");

              entity.HasOne(d => d.IdGrupoNavigation).WithMany(p => p.Registros)
                  .HasForeignKey(d => d.IdGrupo)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__Registro__IdGrup__628FA481");
          });

          modelBuilder.Entity<Rfid>(entity =>
          {
              entity.HasKey(e => e.IdRfid).HasName("PK__RFID__B50B632619B56212");

              entity.ToTable("RFID");

              entity.HasIndex(e => e.CodigoRfid, "UQ__RFID__117C4055AD60DE98").IsUnique();

              entity.Property(e => e.IdRfid).HasColumnName("IdRFID");
              entity.Property(e => e.CodigoRfid)
                  .HasMaxLength(50)
                  .HasColumnName("CodigoRFID");

              entity.HasOne(d => d.IdAlumnoNavigation).WithMany(p => p.Rfids)
                  .HasForeignKey(d => d.IdAlumno)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__RFID__IdAlumno__59FA5E80");
          });

          modelBuilder.Entity<Rol>(entity =>
          {
              entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584C47B6BA5F");

              entity.ToTable("Rol");

              entity.Property(e => e.NombreRol).HasMaxLength(50);
          });

          modelBuilder.Entity<Usuario>(entity =>
          {
              entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF97B97743B9");

              entity.ToTable("Usuario");

              entity.HasIndex(e => e.Correo, "UQ__Usuario__60695A19159A751F").IsUnique();

              entity.Property(e => e.Contraseña).HasMaxLength(100);
              entity.Property(e => e.Correo).HasMaxLength(100);
              entity.Property(e => e.Nombre).HasMaxLength(100);

              entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                  .HasForeignKey(d => d.IdRol)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK__Usuario__IdRol__4D94879B");
          });

          OnModelCreatingPartial(modelBuilder);
      }

      partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }

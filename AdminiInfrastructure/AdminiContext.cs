using AdminiDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminiInfrastructure
{
  /// <summary>
  /// DbContext for AdminiDomain entities.
  /// </summary>
  public class AdminiContext : DbContext
  {
    public AdminiContext(DbContextOptions<AdminiContext> options)
        : base(options)
    {
    }

    public DbSet<Note> Notes => Set<Note>();
    public DbSet<NoteContent> NotesContent => Set<NoteContent>();
    public DbSet<NoteFile> NoteFiles => Set<NoteFile>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<Note>()
        .HasMany<NoteFile>()
        .WithOne()
        .HasForeignKey(file => file.NoteId)
        .IsRequired();

      modelBuilder.Entity<Note>()
        .HasOne<NoteContent>()
        .WithOne()
        .HasForeignKey<NoteContent>(noteContent => noteContent.NoteId)
        .IsRequired();

      modelBuilder.Entity<User>()
        .HasMany<Note>()
        .WithOne()
        .HasForeignKey(note => note.UserId)
        .IsRequired();

      modelBuilder.Entity<User>()
        .HasMany<Tag>()
        .WithOne()
        .HasForeignKey(tag => tag.UserId)
        .IsRequired();
    }
  }
}

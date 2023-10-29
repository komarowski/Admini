namespace AdminiDomain.Entities
{
  /// <summary>
  /// Note markdown content for database.
  /// </summary>
  public class NoteContent : IEntity
  {
    /// <summary>Note content Id.</summary>
    public int Id { get; set; }

    /// <summary>Note Id.</summary>
    public int NoteId { get; set; }

    /// <summary>Markdown content.</summary>
    public string Content { get; set; } = string.Empty;
  }
}

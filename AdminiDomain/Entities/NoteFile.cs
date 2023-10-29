namespace AdminiDomain.Entities
{
  /// <summary>
  /// Note file for database.
  /// </summary>
  public class NoteFile : IEntity
  {
    /// <summary>Note file Id.</summary>
    public int Id { get; set; }

    /// <summary>Note Id.</summary>
    public int NoteId { get; set; }

    /// <summary>File name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Note file folder.</summary>
    public string Folder { get; set; } = string.Empty;
  }
}

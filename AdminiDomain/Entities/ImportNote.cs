namespace AdminiDomain.Entities
{
  /// <summary>
  /// Entity for import/export note data.
  /// </summary>
  public class ImportNote
  {
    public Note Note { get; set; }
    public NoteContent Content { get; set; }
    public List<NoteFile> Files { get; set; }

    public ImportNote(Note note, NoteContent content)
    {
      Note = note;
      Content = content;
      Files = new List<NoteFile>();
    }

    public ImportNote(Note note, NoteContent content, List<NoteFile> files)
    {
      Note = note;
      Content = content;
      Files = files;
    }
  }
}

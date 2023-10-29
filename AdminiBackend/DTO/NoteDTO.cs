using AdminiDomain.Entities;

namespace AdminiBackend.DTO
{
  /// <summary>
  /// DTO for <see cref="Note"/>.
  /// </summary>
  public class NoteDTO
  {
    public string Code { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string TagsString { get; set; }

    public bool IsMark { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public DateTime? LastUpdate { get; set; }

    public NoteDTO(Note note, string? tagString = null)
    {
      this.Code = note.Code;
      this.Title = note.Title;
      this.Description = note.Description is null ? string.Empty : note.Description;
      this.TagsString = tagString is null ? string.Empty : tagString;
      this.IsMark = note.IsMark;
      this.Latitude = note.Latitude;
      this.Longitude = note.Longitude;
      this.LastUpdate = note.LastUpdate;
    }
  }
}

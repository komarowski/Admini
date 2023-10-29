using AdminiDomain.Entities;

namespace AdminiDomain.Services
{
  /// <summary>
  /// Service for <see cref="NoteContent"/> entity management. 
  /// </summary>
  public class NoteContentService : GenericService<NoteContent>
  {
    public NoteContentService(IRepository repository) 
      : base(repository)
    {
    }
  }
}

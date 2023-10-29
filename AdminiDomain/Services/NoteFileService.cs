using AdminiDomain.Entities;

namespace AdminiDomain.Services
{
  /// <summary>
  /// Service for <see cref="NoteFile"/> entity management.
  /// </summary>
  public class NoteFileService : GenericService<NoteFile>
  {
    public NoteFileService(IRepository repository) 
      : base(repository)
    {
    }
  }
}

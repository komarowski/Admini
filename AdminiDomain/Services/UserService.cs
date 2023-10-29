using AdminiDomain.Entities;

namespace AdminiDomain.Services
{
  /// <summary>
  /// Service for <see cref="User"/> entity management.
  /// </summary>
  public class UserService : GenericService<User>
  {
    public UserService(IRepository repository) 
      : base(repository)
    {
    }
  }
}

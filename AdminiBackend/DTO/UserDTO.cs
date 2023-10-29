using AdminiDomain.Entities;

namespace AdminiBackend.DTO
{
  /// <summary>
  /// DTO for <see cref="User"/>.
  /// </summary>
  public class UserDTO
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public UserDTO(User user) 
    {
      this.Id = user.Id;
      this.Name = user.Name;
    }
  }
}

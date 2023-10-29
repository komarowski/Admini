using System.ComponentModel.DataAnnotations;

namespace AdminiDomain.Entities
{
  /// <summary>
  /// User roles.
  /// </summary>
  public enum UserRoles
  {
    /// <summary>User role.</summary>
    User,
    /// <summary>Admin role.</summary>
    Admin
  }

  /// <summary>
  ///  Entity for user.
  /// </summary>
  public class User : IEntity
  {
    /// <summary>User Id.</summary>
    public int Id { get; set; }

    [MaxLength(30)]
    /// <summary>User name.</summary>
    public string Name { get; set; } = string.Empty;

    [MaxLength(40)]
    /// <summary>User password.</summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>User role.</summary>
    public UserRoles Role { get; set; }
  }
}

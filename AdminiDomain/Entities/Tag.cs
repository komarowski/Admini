using System.ComponentModel.DataAnnotations;

namespace AdminiDomain.Entities
{
  /// <summary>
  /// Entity for tag.
  /// </summary>
  public class Tag : IEntity
  {
    /// <summary>Tag Id.</summary>
    public int Id { get; set; }

    /// <summary>User Id.</summary>
    public int UserId { get; set; }

    /// <summary>Tag number.</summary>
    public int Number { get; set; }

    /// <summary>Tag title.</summary>
    [MaxLength(20)]
    public string Title { get; set; } = string.Empty;
  }
}

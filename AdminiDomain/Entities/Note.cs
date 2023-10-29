using System.ComponentModel.DataAnnotations;

namespace AdminiDomain.Entities
{
  /// <summary>
  /// Note entity for database.
  /// </summary>
  public class Note : IEntity
  {
    /// <summary>Note Id.</summary>
    public int Id { get; set; }

    /// <summary>Note code.</summary>
    [MaxLength(35)]
    public string Code { get; set; } = string.Empty;

    /// <summary>Note title.</summary>
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Note description.</summary>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>Sum of note tag numbers.</summary>
    public int Tags { get; set; }

    /// <summary>User Id.</summary>
    public int UserId { get; set; }

    /// <summary>There is a mark on the map.</summary>
    public bool IsMark { get; set; }

    /// <summary>The latitude of the place in the note.</summary>
    public double Latitude { get; set; }

    /// <summary>The longitude of the place in the note.</summary>
    public double Longitude { get; set; }

    /// <summary>The time the note was last updated.</summary>
    public DateTime? LastUpdate { get; set; }
  }
}

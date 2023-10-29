using AdminiDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdminiDomain.Services
{
  /// <summary>
  /// Service for <see cref="Tag"/> entity management.
  /// </summary>
  public class TagService : GenericService<Tag>
  {
    public TagService(IRepository repository) 
      : base(repository)
    {
    }

    /// <summary>
    /// Gets tags string from tag sum.
    /// </summary>
    /// <param name="tagsSum">Tag sum.</param>
    /// <param name="userName">User name.</param>
    /// <returns>Tags string.</returns>
    public async Task<string> GetTagsString(int tagsSum, string userName)
    {
      var tags = await repository.GetQuery<Tag>()
        .Join(
          repository.GetQuery<User>(),
          tag => tag.UserId,
          user => user.Id,
          (tag, user) => new { tag, user })
        .Where(x => x.user.Name == userName && (tagsSum & x.tag.Number) == x.tag.Number)
        .Select(x => x.tag.Title)
        .ToArrayAsync();

      return string.Join(", ", tags);
    }

    /// <summary>
    /// Gets user's tags.
    /// </summary>
    /// <param name="userName">User name.</param>
    /// <returns>Array of <see cref="Tag"/>.</returns>
    public async Task<IList<Tag>> GetListAsync(string userName)
    {
      return await repository.GetQuery<Tag>()
        .Join(
          repository.GetQuery<User>(),
          tag => tag.UserId,
          user => user.Id,
          (tag, user) => new { tag, user })
        .Where(x => x.user.Name == userName)
        .Select(x => x.tag)
        .ToArrayAsync();
    }
  }
}

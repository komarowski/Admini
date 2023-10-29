using AdminiDomain.Entities;
using System.Linq.Expressions;

namespace AdminiDomain.Services
{
  /// <summary>
  /// Generic service for work with <see cref="IRepository"/>.
  /// </summary>
  /// <typeparam name="TEntity">Class that implements <see cref="IEntity"/>.</typeparam>
  public class GenericService<TEntity> where TEntity : class, IEntity
  {
    private protected readonly IRepository repository;

    public GenericService(IRepository repository)
    {
      this.repository = repository;
    }

    /// <summary>
    /// Gets entity.
    /// </summary>
    /// <param name="filter">Expression for filter.</param>
    /// <returns>Entity or null if not found.</returns>
    public virtual async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter)
    {
      return await repository.GetAsync(filter);
    }

    /// <summary>
    /// Gets a list of entities.
    /// </summary>
    /// <param name="filter">Expression for filter or null if no filter.</param>
    /// <returns>List of entities.</returns>
    public virtual async Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
      return await repository.GetListAsync(filter);
    }

    /// <summary>
    /// If entity id == 0 saves a new entity, otherwise updates the existing one.
    /// </summary>
    /// <returns>Saved entity.</returns>
    public virtual async Task<TEntity> SaveAsync(TEntity entity)
    {
      if (entity.Id == 0)
      {
        return await repository.AddAsync(entity);
      }
      return await repository.UpdateAsync(entity);
    }

    /// <summary>
    /// Deletes entity.
    /// </summary>
    /// <param name="id">PK of entity.</param>
    /// <returns>Deleted entity or null if not found.</returns>
    public virtual async Task<TEntity?> DeleteAsync(int id)
    {
      return await repository.DeleteAsync<TEntity>(id);
    }
  }
}

using System.Linq.Expressions;

namespace AdminiDomain
{
  /// <summary>
  /// This interface implemented base database operation with generic repository pattern.
  /// </summary>
  public interface IRepository
  {
    /// <summary>
    /// Performs apply filter to get entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    /// <param name="filter">Expression for filter.</param>
    /// <returns>Entity or null if not found.</returns>
    public Task<TEntity?> GetAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class;

    /// <summary>
    /// Performs apply filter to get entities.
    /// If there is no filter, get all entities.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    /// <param name="filter">Expression for filter or null if no filter.</param>
    /// <returns>IList of TEntity.</returns>
    public Task<IList<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>>? filter) where TEntity : class;

    /// <summary>
    /// Gets query of entities.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    /// <returns><see cref="IQueryable"/> of TEntity.</returns>
    public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class;

    /// <summary>
    /// Performs entity insert operation.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    /// <param name="entity">The entity to be added.</param>
    /// <returns>Returns added entity.</returns>
    public Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Performs entity update operation.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    /// <param name="entity">Updated entity.</param>
    /// <returns>Returns updated entity.</returns>
    public Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Performs entity delete operation.
    /// </summary>
    /// <typeparam name="TEntity">Type of Entity.</typeparam>
    /// <param name="id">PK of Entity.</param>
    public Task<TEntity?> DeleteAsync<TEntity>(int id) where TEntity : class;
  }
}

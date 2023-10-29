using AdminiDomain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdminiInfrastructure
{
  /// <summary>
  /// Implementation of <seealso cref="IRepository"/> for Microsoft SQL Server.
  /// </summary>
  public class RepositorySqlServer : IRepository
  {
    private readonly AdminiContext context;

    public RepositorySqlServer(AdminiContext context)
    {
      this.context = context;
    }

    /// <summary>
    /// Implementation of <seealso cref="IRepository.GetAsync{TEntity}(Expression{Func{TEntity, bool}})"/>
    /// </summary>
    public async Task<TEntity?> GetAsync<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
    {
      return await context.Set<TEntity>().FirstOrDefaultAsync(filter);
    }

    /// <summary>
    /// Implementation of <seealso cref="IRepository.GetListAsync{TEntity}(Expression{Func{TEntity, bool}}?)"/>
    /// </summary>
    public async Task<IList<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>>? filter) where TEntity : class
    {
      if (filter is null)
      {
        return await context.Set<TEntity>().ToArrayAsync();
      }
      return await context.Set<TEntity>().Where(filter).ToArrayAsync();
    }

    /// <summary>
    /// Implementation of <seealso cref="IRepository.GetListAsync{TEntity}(Expression{Func{TEntity, bool}}?)"/>
    /// </summary>
    public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : class
    {
      return context.Set<TEntity>();
    }

    /// <summary>
    /// Implementation of <seealso cref="IRepository.AddAsync{TEntity}(TEntity)"/>
    /// </summary>
    public async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
      await context.Set<TEntity>().AddAsync(entity);
      await context.SaveChangesAsync();
      return entity;
    }

    /// <summary>
    /// Implementation of <seealso cref="IRepository.UpdateAsync{TEntity}(TEntity)"/>
    /// </summary>
    public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
    {
      context.Attach(entity).State = EntityState.Modified;
      await context.SaveChangesAsync();
      return entity;
    }

    /// <summary>
    /// Implementation of <seealso cref="IRepository.DeleteAsync{TEntity}(int)"/>
    /// </summary>
    public async Task<TEntity?> DeleteAsync<TEntity>(int id) where TEntity : class
    {
      var entity = await context.Set<TEntity>().FindAsync(id);
      if (entity is not null)
      {
        context.Remove(entity);
        await context.SaveChangesAsync();
      }
      return entity;
    }
  }
}

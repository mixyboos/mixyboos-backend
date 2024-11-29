using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Models;
using MixyBoos.Api.Services.Extensions;

namespace MixyBoos.Api.Data.Repositories;

public interface IRepository<T> where T : BaseEntity {
  IQueryable<T> GetAll();

  Task<T> Get(Guid id);

  Task Insert(T entity);

  Task Update(T entity);

  Task Delete(T entity);
}

public class Repository<T> : IRepository<T> where T : BaseEntity {
  private readonly MixyBoosContext context;
  protected DbSet<T> entities;
  string errorMessage = string.Empty;

  public Repository(MixyBoosContext context) {
    this.context = context;
    entities = context.Set<T>();
  }

  public IQueryable<T> GetAll() {
    return entities.AsQueryable<T>();
  }

  public async Task<T> Get(Guid id) {
    return await entities.SingleOrDefaultAsync(s => s.Id.Equals(id));
  }

  public async Task Insert(T entity) {
    ArgumentNullException.ThrowIfNull(entity);

    entities.Add(entity);
    await context.SaveChangesAsync();
  }

  public async Task Update(T entity) {
    ArgumentNullException.ThrowIfNull(entity);

    await context.SaveChangesAsync();
  }

  public async Task AddOrUpdate(T entity) {
    ArgumentNullException.ThrowIfNull(entity);
    await context.AddOrUpdate(entity);
    await context.SaveChangesAsync();
  }

  public async Task Delete(T entity) {
    ArgumentNullException.ThrowIfNull(entity);

    entities.Remove(entity);
    await context.SaveChangesAsync();
  }
}

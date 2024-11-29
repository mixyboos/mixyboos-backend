using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Data.Repositories;

public class MixRepository : Repository<Mix> {
  public MixRepository(MixyBoosContext context) : base(context) { }

  private IQueryable<Mix> _internalGet(Expression<Func<Mix, bool>> predicate) {
    return entities
      .Where(predicate)
      .Where(m => m.IsProcessed)
      .Include(m => m.User)
      .Include(m => m.Likes)
      .Include(m => m.Plays)
      .Include(m => m.Shares)
      .Include(m => m.Downloads);
  }

  public async Task<IEnumerable<Mix>> GetByUser(string userSlug) {
    return await _internalGet(m => m.User.Slug.Equals(userSlug))
      .Where(m => m.IsProcessed)
      .ToListAsync();
  }

  public async Task<Mix> GetByUserAndSlug(string userSlug, string mixSlug) {
    return await _internalGet(m => m.User.Slug.Equals(userSlug) && m.Slug.Equals(mixSlug))
      .FirstOrDefaultAsync();
  }

  public async Task<IEnumerable<Mix>> GetFeedForUser(Guid id) {
    return await _internalGet(m => m.User.Id.Equals(id))
      .Where(m => m.IsProcessed)
      .OrderByDescending(m => m.DateCreated)
      .ToListAsync();
  }
}

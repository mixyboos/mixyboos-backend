#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Data.Repositories;

//TODO: Perhaps refactor this out to methods on Mix ?
public class MixRepository(MixyBoosContext context) : Repository<Mix>(context) {
  private IQueryable<Mix> _internalGet(Expression<Func<Mix, bool>> predicate, MixyBoosUser? requestingUser = null) {
    return entities
      .Where(predicate)
      .OrderByDescending(m => m.DateUpdated)
      .Where(m => requestingUser != null && m.User.Id.Equals(requestingUser.Id) || m.IsProcessed)
      .Include(m => m.User)
      .Include(m => m.Tags)
      .Include(m => m.Likes)
      .Include(m => m.Plays)
      .Include(m => m.Shares)
      .Include(m => m.Downloads);
  }

  public async Task<IEnumerable<Mix>> GetMyMixes(Guid userId) {
    return await _internalGet(m => m.User.Id.Equals(userId))
      .OrderByDescending(m => m.DateUpdated)
      .ToListAsync();
  }

  public async Task<Mix?> GetById(Guid id, MixyBoosUser requestingUser) {
    return await _internalGet(m => m.Id.Equals(id), requestingUser)
      .FirstOrDefaultAsync();
  }

  public async Task<IEnumerable<Mix>> GetByUser(string userSlug, MixyBoosUser requestingUser) {
    return await _internalGet(m => m.User.Slug.Equals(userSlug), requestingUser)
      .Where(m => m.IsProcessed)
      .ToListAsync();
  }

  public async Task<Mix?> GetByUserAndSlug(string userSlug, string mixSlug, MixyBoosUser requestingUser) {
    return await _internalGet(
        m => m.User.Slug.Equals(userSlug) && m.Slug != null && m.Slug.Equals(mixSlug),
        requestingUser)
      .FirstOrDefaultAsync();
  }

  public async Task<IEnumerable<Mix>> GetFeedForUser(Guid id) {
    return await _internalGet(m => m.User.Id.Equals(id))
      .Where(m => m.IsProcessed)
      .OrderByDescending(m => m.DateCreated)
      .ToListAsync();
  }
}

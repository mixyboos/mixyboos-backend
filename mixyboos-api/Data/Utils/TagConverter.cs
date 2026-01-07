using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MixyBoos.Api.Data.Models;

namespace MixyBoos.Api.Data.Utils;

public class TagConverter {
  private readonly MixyBoosContext _context;

  public TagConverter(MixyBoosContext context) {
    _context = context;
  }

  /// <summary>
  /// Processes a collection of tag names, finding existing tags and creating new ones as needed.
  /// This method is called by Mapster's AfterMappingAsync to handle tag processing during DTO to entity mapping.
  /// </summary>
  /// <param name="tagNames">Collection of tag names to process</param>
  /// <returns>List of Tag entities (both existing and newly created)</returns>
  public async Task<List<Tag>> ProcessTagsPayload(IEnumerable<string> tagNames) {
    if (tagNames == null) {
      return [];
    }

    // Normalise tag names (trim and filter out empty strings)
    var normalisedTagNames = tagNames
      .Where(t => !string.IsNullOrWhiteSpace(t))
      .Select(t => t.Trim())
      .Distinct()
      .ToList();

    if (normalisedTagNames.Count == 0) {
      return [];
    }

    // Find existing tags
    var existingTags = await _context.Tags
      .Where(t => normalisedTagNames.Contains(t.Name))
      .ToListAsync();

    var existingTagNames = existingTags.Select(t => t.Name).ToHashSet();

    // Create new tags for names that don't exist
    var newTagNames = normalisedTagNames
      .Where(name => !existingTagNames.Contains(name))
      .ToList();

    var newTags = new List<Tag>();
    foreach (var tag in newTagNames.Select(tagName => new Tag {Name = tagName})) {
      _context.Tags.Add(tag);
      newTags.Add(tag);
    }

    if (newTags.Count != 0) {
      await _context.SaveChangesAsync();
    }

    // Combine existing and new tags
    var allTags = existingTags.Concat(newTags).ToList();
    return allTags;
  }
}

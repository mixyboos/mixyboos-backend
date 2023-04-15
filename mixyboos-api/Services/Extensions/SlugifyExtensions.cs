using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Bogus;
using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MixyBoos.Api.Data;
using MixyBoos.Api.Data.Utils;
using MixyBoos.Api.Services.Helpers;

namespace MixyBoos.Api.Services.Extensions; 

/// <summary>Class <c>UniqueGeneratedFieldExtensions</c>
/// Various methods for slugifying/unique keying entities.</summary>
///
public class GenerateSlugFailureException : Exception {
    public GenerateSlugFailureException(string message) : base(message) { }
}

public static class UniqueGeneratedFieldExtensions {
    /// <summary>
    /// Simple ViewModel to easily work with ExecSQL 
    /// </summary>
    private class ProxySluggedModel : ISluggedEntity {
        public string Slug { get; set; }
    }

    public static IEnumerable<T> Select<T>(this IDataReader reader,
        Func<IDataReader, T> projection) {
        while (reader.Read()) {
            yield return projection(reader);
        }
    }

    public static IEnumerable<T> ExecSQL<T>(this DbContext context, string query)
        where T : class, ISluggedEntity, new() {
        using (var command = context.Database.GetDbConnection().CreateCommand()) {
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            context.Database.OpenConnection();

            using (var reader = command.ExecuteReader()) {
                var result = reader.Select(r => new T {
                    Slug = r["Slug"] is DBNull ? string.Empty : r["Slug"].ToString()
                });
                return result.ToList();
            }
        }
    }

    public static string RemoveNonAlphaChars(this string str) {
        var rgx = new Regex("[^a-zA-Z0-9 -]");
        return rgx.Replace(str, "");
    }

    public static string RemoveInvalidUrlChars(this string str) {
        var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        var r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        return r.Replace(str, "");
    }

    public static string Slugify(this string phrase, IEnumerable<string> source) {
        var str = phrase.RemoveAccent().ToLower().RemoveInvalidUrlChars().RemoveNonAlphaChars();
        // invalid chars           
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        // convert multiple spaces into one space   
        str = Regex.Replace(str, @"\s+", " ").Trim();
        // cut and trim 
        str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
        str = Regex.Replace(str, @"\s", "-"); // hyphens   
        str = str.RemoveAccent().ToLower();

        str = str.Replace(" ", "");
        var count = 1;
        var origStr = str;
        while (source != null && source.Count() != 0 &&
               !string.IsNullOrEmpty(source.Where(e => e == str).Select(e => e).DefaultIfEmpty("")
                   .FirstOrDefault())) {
            str = $"{origStr}-{count++}";
        }

        return str;
    }

    public static string GenerateSlug(this IUniqueFieldEntity entity, DbContext context,
        ILogger logger = null) {
        try {
            var property = entity.GetType()
                .GetProperties()
                .FirstOrDefault(prop => Attribute.IsDefined(prop, typeof(SlugFieldAttribute)));
            if (property != null) {
                var attribute = property
                    .GetCustomAttributes(typeof(SlugFieldAttribute), false)
                    .FirstOrDefault();

                var t = entity.GetType();
                var tableName = context.Model.FindEntityType(t).GetTableName();
                var schemaName = context.Model.FindEntityType(t).GetSchema() ?? "public";
                if (!string.IsNullOrEmpty(tableName)) {
                    var sourceField = (attribute as SlugFieldAttribute)?.SourceField;
                    if (string.IsNullOrEmpty(sourceField)) {
                        logger?.LogError($"Error slugifying - Entry title is blank, cannot slugify");
                        // need to throw here, shouldn't save without slug
                        throw new GenerateSlugFailureException("Entry title is blank, cannot slugify");
                    }

                    var slugSource = entity.GetType()
                        .GetProperty(sourceField)
                        ?.GetValue(entity, null)
                        ?.ToString() ?? string.Empty;

                    var source = context.ExecSQL<ProxySluggedModel>($"SELECT Slug FROM {schemaName}.{tableName}")
                        .Select(m => m.Slug);

                    return slugSource.Slugify(source);
                    //TODO: for loop to check for unique slugs
                }
            }
        } catch (Exception ex) {
            logger?.LogError($"Error slugifying {entity.GetType().Name} - {ex.Message}");
            // need to throw here, shouldn't save without slug
            throw new GenerateSlugFailureException(ex.Message);
        }

        return string.Empty;
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MixyBoos.Api.Services.Extensions;

public static class CollectionExtensions {
    public static Collection<T> ToCollection<T>(this List<T> items) {
        Collection<T> collection = new Collection<T>();

        foreach (var t in items) {
            collection.Add(t);
        }

        return collection;
    }
}

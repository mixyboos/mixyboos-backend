using System;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MixyBoos.Api.Data;

public class MixyBoosDbEventsInterceptor : SaveChangesInterceptor {
    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result) {
        Console.WriteLine(eventData.Context.ChangeTracker.DebugView.LongView);
        return base.SavedChanges(eventData, result);
    }
}

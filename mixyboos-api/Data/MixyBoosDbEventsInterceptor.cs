using System;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MixyBoos.Api.Data;

public class MixyBoosDbEventsInterceptor : SaveChangesInterceptor {
    public override InterceptionResult<int>
        SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) {
        Console.WriteLine(eventData.Context.ChangeTracker.DebugView.LongView);
        return base.SavingChanges(eventData, result);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result) {
        Console.WriteLine(eventData.Context.ChangeTracker.DebugView.LongView);
        return base.SavedChanges(eventData, result);
    }
}

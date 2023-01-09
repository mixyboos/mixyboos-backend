using Microsoft.Extensions.DependencyInjection;
using MixyBoos.Api.Services.Jobs;
using Quartz;

namespace MixyBoos.Api.Services.Startup;

public static class SchedulerBuilders {
    public static void LoadScheduler(this IServiceCollection services) {
        services.AddQuartz(q => {
            q.SchedulerId = "MixyBoos-Server-Core";
            q.SchedulerName = "MixyBoos Scheduler";

            q.UseMicrosoftDependencyInjectionJobFactory();
            q.AddJob<ConvertAudioJob>(opts => {
                opts.WithIdentity(new JobKey("ProcessUploadedAudioJob")).StoreDurably();
            });
            q.AddJob<CheckLiveStreamJob>(opts => {
                opts.WithIdentity(new JobKey("CheckLiveStreamJob")).StoreDurably();
            });
            q.AddJob<SaveLiveShowJob>(opts => {
                opts.WithIdentity(new JobKey("SaveLiveShowJob")).StoreDurably();
            });
        });
        services.AddQuartzServer(options => {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });
    }
}

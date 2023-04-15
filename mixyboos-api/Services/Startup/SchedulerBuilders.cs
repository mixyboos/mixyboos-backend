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
            q.AddJob<ProcessUploadedAudioJob>(opts => {
                opts.WithIdentity(new JobKey("ProcessUploadedAudioJob")).StoreDurably();
            });
            q.AddJob<ProcessUploadedImageJob>(opts => {
                opts.WithIdentity(new JobKey("ProcessUploadedImageJob")).StoreDurably();
            });
            q.AddJob<CheckLiveStreamJob>(opts => {
                opts.WithIdentity(new JobKey("CheckLiveStreamJob")).StoreDurably();
            });
            q.AddJob<SaveLiveShowJob>(opts => {
                opts.WithIdentity(new JobKey("SaveLiveShowJob")).StoreDurably();
            });
            q.AddJob<CheckAudioIsProcessedJob>(opts => {
                opts.WithIdentity(new JobKey("CheckAudioIsProcessedJob")).StoreDurably();
            });

            q.AddTrigger(opts => opts
                .ForJob(new JobKey("CheckAudioIsProcessedJob"))
                .WithIdentity("trigger-CheckAudioIsProcessedJob")
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(5)
                    .RepeatForever()));
        });
        services.AddQuartzServer(options => {
            // when shutting down we want jobs to complete gracefully
            options.WaitForJobsToComplete = true;
        });
    }
}

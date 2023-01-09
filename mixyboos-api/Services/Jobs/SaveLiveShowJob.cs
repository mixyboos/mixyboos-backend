using System;
using System.Threading.Tasks;
using Quartz;

namespace MixyBoos.Api.Services.Jobs;

public class SaveLiveShowJob : IJob {
    public async Task Execute(IJobExecutionContext context) {
        await Task.Factory.StartNew(() => { Console.WriteLine($"Farts"); });
    }
}

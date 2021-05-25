using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MixyBoos.Api.Tests.Services {
    public class InspectingDelegatingHandler : DelegatingHandler {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken) {
            return base.SendAsync(request, cancellationToken);
        }
    }
}

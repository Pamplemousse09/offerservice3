using Kikai.Logging.DTO;
using Kikai.Logging.Utils;
using Kikai.Logging.Extensions;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kikai.WebApi.Handlers
{
    public class MonitorEventHandler : DelegatingHandler
    {

        protected async override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string _requestId = Guid.NewGuid().ToString();
            request.Properties.Add("requestId", _requestId);

            // Call the inner handler.
            var response = await base.SendAsync(request, cancellationToken);

            sw.Stop();
            MonitorObject MonitorObject = new Methods().ToMonitorObject(request.Method.ToString(), request.RequestUri.PathAndQuery, _requestId, response.IsSuccessStatusCode, response.StatusCode.ToString(), (int)Math.Round(sw.Elapsed.TotalMilliseconds));
            LoggingUtility Log = LoggerFactory.GetLogger("MonitorLogger");
            Log.InfoMonitor(MonitorObject);

            //Log.DebugFormat("{0} {1}", request.Properties["requestId"].ToString(), request.RequestUri.OriginalString);
            return response;
        }
    }
}

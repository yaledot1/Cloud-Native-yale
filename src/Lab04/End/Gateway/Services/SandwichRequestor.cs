using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using RabbitQueue;
using Polly;

namespace Gateway.Services
{
  public class SandwichRequestor : ISandwichRequestor
  {
    readonly IConfiguration _config;
    readonly IWorkInProgress _wip;
    readonly Policy _retryPolicy = Policy.
      Handle<Exception>().
      WaitAndRetry(3, r => TimeSpan.FromSeconds(Math.Pow(2, r)));
    readonly IServiceBus _bus;

    public SandwichRequestor(IConfiguration config, IWorkInProgress wip, IServiceBus bus)
    {
      _config = config;
      _wip = wip;
      _bus = bus;
    }

    public async Task<Messages.SandwichResponse> RequestSandwich(Messages.SandwichRequest request)
    {
      var result = new Messages.SandwichResponse();
      var correlationId = Guid.NewGuid().ToString();
      var lockEvent = new AsyncManualResetEvent();
      _wip.StartWork(correlationId, lockEvent);
      try
      {
        _retryPolicy.Execute(() =>
        {
          _bus.Publish("SandwichRequest", correlationId, request);
        });
        var messageArrived = lockEvent.WaitAsync();
        if (await Task.WhenAny(messageArrived, Task.Delay(10000)) == messageArrived)
        {
          result = _wip.FinalizeWork(correlationId);
        }
        else
        {
          result.Error = "The cook didn't get back to us in time, no sandwich";
          result.Success = false;
        }
      }
      finally
      {
          _wip.FinalizeWork(correlationId);
      }

      return result;
    }
  }
}
﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitQueue;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeatService
{
  class MeatBin
  {
    private static IServiceBus _bus;

    static async Task Main(string[] args)
    {
      var config = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();

      if (_bus == null)
        _bus = new ServiceBus(config["rabbitmq:url"], "sandwichBus");

      Console.WriteLine("### Meat bin service starting to listen");
      _bus.Subscribe<Messages.MeatBinRequest>("MeatBinRequest", HandleMessage);

      // wait forever - we run until the container is stopped
      await new AsyncManualResetEvent().WaitAsync();
    }

    private volatile static int _inventory = 10;

    private static void HandleMessage(BasicDeliverEventArgs ea, Messages.MeatBinRequest request)
    {
      var response = new Messages.MeatBinResponse();
      lock (_bus)
      {
        Thread.Sleep(2000);
        if (request.Returning)
        {
          Console.WriteLine($"### Request for {request.GetType().Name} - returned");
          _inventory++;
        }
        else if (_inventory > 0)
        {
          Console.WriteLine($"### Request for {request.GetType().Name} - filled");
          _inventory--;
          response.Success = true;
          _bus.Publish("MeatBinResponse", ea.BasicProperties.CorrelationId, response);
        }
        else
        {
          Console.WriteLine($"### Request for {request.GetType().Name} - no inventory");
          response.Success = false;
          _bus.Publish("MeatBinResponse", ea.BasicProperties.CorrelationId, response);
        }
      }
    }
  }
}

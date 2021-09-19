using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ccamposh.BalancedSculptures;
using ccamposh.BalancedSculptures.Data;
using Microsoft.Extensions.Logging;

namespace ccamposh.BalancedSculptures
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = LoggerFactory.Create(i => i.AddConsole());
            var service = new MainService(factory.CreateLogger<MainService>(), new SculptureRepository(), new SculptureRepository());
            var result = service.CalculateSculptures(15);
        }
    }
}

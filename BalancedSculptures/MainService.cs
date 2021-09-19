using System;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using ccamposh.BalancedSculptures.Dto;
using ccamposh.BalancedSculptures.Interfaces;
using Microsoft.Extensions.Logging;

namespace ccamposh.BalancedSculptures
{
    public class MainService
    {
        private ISculptureRepository _balancedSculptures;
        private ISculptureRepository _incompleteSculptures;
        private ILogger _logger;
        public static ActionBlock<Sculpture> actionBlock;
        private static long _processingThreads = 0;
        private static object _lock = new Object();
        private static int currentSize = 0;

        public MainService(ILogger logger, ISculptureRepository balancedSculptures, ISculptureRepository incompletedSculptures)
        {
            _logger = logger;
            _balancedSculptures = balancedSculptures;
            _incompleteSculptures = incompletedSculptures;
        }

        public long CalculateSculptures(byte size)
        {
            var startTime = DateTime.Now;
            Sculpture.SetupSize(size);
            actionBlock = new ActionBlock<Sculpture>(i => getSculptures(i), new ExecutionDataflowBlockOptions {MaxDegreeOfParallelism = 30});
            _processingThreads = 1;
            getSculptures(new Sculpture());
            while ( _processingThreads > 0 || actionBlock.InputCount > 0)
            {
                Thread.Sleep(1000);
                _logger.LogInformation($"Level {currentSize} InputCount: {actionBlock.InputCount} Incomplete {_incompleteSculptures.Count} Balanced {_balancedSculptures.Count} Threads {_processingThreads}");
            }
            actionBlock.Complete();
            actionBlock.Completion.Wait();
            var endTime = DateTime.Now;
            Console.WriteLine("Total Sculptures: " + _balancedSculptures.Count.ToString() + " in " + (DateTime.Now - startTime));
            return _balancedSculptures.Count;
        }

        public void getSculptures(Sculpture sculpture)
        {
            if (currentSize < sculpture.CurrentSize)
            {
                currentSize = sculpture.CurrentSize;
            }
            var childSculptures = sculpture.GetChildSculptures();
            foreach (var childSculpture in childSculptures)
            {
                if (childSculpture.IsComplete)
                {
                    var inserted = _balancedSculptures.TryInsert(childSculpture);
                }
                else 
                {
                    if (_incompleteSculptures.TryInsert(childSculpture))
                    {
                        actionBlock.Post(childSculpture);
                        lock(_lock)
                        {
                            _processingThreads++;
                        }
                    }
                }
            }
            lock(_lock)
            {
                _processingThreads--;
            }            
        }

    }
}
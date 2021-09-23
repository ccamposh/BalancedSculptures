using System;
using System.Linq;
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
        public static BufferBlock<byte[]> bufferBlock;
        public static ActionBlock<byte[]> actionBlock;
        private static long _processingThreads = 0;
        private static object _lockComplete = new Object();
        private static object _lockIncomplete = new Object();
        private static object _lockCounter = new Object();
        private static int currentSize = 0;

        public MainService( ILogger logger, ISculptureRepository balancedSculptures, ISculptureRepository incompletedSculptures )
        {
            _logger = logger;
            _balancedSculptures = balancedSculptures;
            _incompleteSculptures = incompletedSculptures;
        }

        public long CalculateSculptures( byte size, byte[] baseSculpture )
        {
            var startTime = DateTime.Now;
            Sculpture.SetupSize( size );
            bufferBlock = new BufferBlock<byte[]>();
            actionBlock = new ActionBlock<byte[]>( i => getSculptures( i ), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 30, BoundedCapacity = 1000 } );
            bufferBlock.LinkTo( actionBlock );
            _processingThreads = 1;
            getSculptures( baseSculpture );
            var lastGC = DateTime.Now;
            while ( _processingThreads > 0 || actionBlock.InputCount > 0 )
            {
                Thread.Sleep( 1000 );
                _logger.LogInformation( $"Level {currentSize} Incomplete {_incompleteSculptures.Count} Balanced {_balancedSculptures.Count} Threads {_processingThreads} InputCount: {actionBlock.InputCount} Buffered: {bufferBlock.Count}" );
                if ((DateTime.Now - lastGC).TotalSeconds > 60)
                {
                    GC.Collect();
                    lastGC = DateTime.Now;
                }
            }
            actionBlock.Complete();
            actionBlock.Completion.Wait();
            var endTime = DateTime.Now;
            _logger.LogInformation( $"Total Sculptures: {_balancedSculptures.Count} in {DateTime.Now - startTime}" );
            return _balancedSculptures.Count;
        }

        public void getSculptures( byte[] array )
        {
            if ( currentSize < (array[0] & 127) )
            {
                currentSize = (array[0] & 127);
            }
            var childSculptures = Sculpture.GetChildSculptures(array);
            if (childSculptures.Count > 0)
            {
                if ((childSculptures.First()[0] & 127) == Sculpture.MaxBlocks) //all are complete
                {
                    lock (_lockComplete)
                    {
                        foreach ( var childSculpture in childSculptures )
                        {
                            var inserted = _balancedSculptures.TryInsert(childSculpture);
                        }
                    }
                }
                else 
                {
                    var inserted = 0;
                    lock (_lockIncomplete)
                    {
                        foreach ( var childSculpture in childSculptures )
                        {           
                            if ( _incompleteSculptures.TryInsert( childSculpture ) )
                            {
                                bufferBlock.Post( childSculpture );
                                inserted++;
                            }
                        }
                    }
                    if (inserted > 0)
                    {
                        lock(_lockCounter)
                        {
                            _processingThreads = _processingThreads + inserted;
                        }
                    }
                }
            }
            lock (_lockCounter)
            {
                _processingThreads--;            
            }
        }
    }
}
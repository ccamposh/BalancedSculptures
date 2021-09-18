using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BalancedSculptures
{
    class Program
    {
        private static Collection collection = new Collection();
        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            Sculpture.SetupSize(12);
            getSculptures(new Sculpture(), 1);
            var endTime = DateTime.Now;
            Console.WriteLine("Total Sculptures: " + collection.Count.ToString() + " in " + (endTime - startTime));
            Console.ReadLine();
        }

        private static void getSculptures(Sculpture sculpture, byte currentSize)
        {
            var validPositions = sculpture.getNextValidPositions();
            foreach (var validPosition in validPositions)
            {
                var childSculpture = new Sculpture(sculpture, validPosition);
                //check if potencially the sculpture can be balanced with the pending blocks
                if (childSculpture.CanBeBalanced(currentSize + 1))
                {
                    if (collection.Add(childSculpture, currentSize))
                    {
                        //if there are more pending blocks to put, recursion
                        if (currentSize + 1 < Sculpture.MaxBlocks)
                        {
                            getSculptures(childSculpture, (byte)(currentSize + 1));
                        }
                    }                    
                }
            }
        }
    }
}

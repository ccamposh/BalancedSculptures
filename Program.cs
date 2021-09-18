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
            Sculpture.SetupSize(10);
            var base0 = new Sculpture();
            var base1 = new Sculpture(base0, (7,0));
            var base2 = new Sculpture(base0, (8,1));
            var a = Task.Run(() => {getSculptures(base1, 2);});
            var b = Task.Run(() => {getSculptures(base2, 2);});
            a.Wait();
            b.Wait();
            //getSculptures(base2, 2);
            var endTime = DateTime.Now;
            Console.WriteLine("Total Sculptures: " + collection.Count.ToString() + " in " + (endTime - startTime));
            Console.ReadLine();
        }

        private static void getSculptures(Sculpture sculpture, int currentSize)
        {
            var validPositions = sculpture.getNextValidPositions();
            foreach (var validPosition in validPositions)
            {
                var childSculpture = new Sculpture(sculpture, validPosition);
                if (Sculpture.MaxBlocks == currentSize + 1)
                {
                    //the sculpture is complete, check if its balanced
                    if (childSculpture.IsBalanced())
                    {
                        collection.Add(childSculpture);
                    }
                }
                else 
                {
                    getSculptures(childSculpture, currentSize + 1);
                }
            }
        }
    }
}

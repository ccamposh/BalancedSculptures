using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BalancedSculptures
{
    public class Collection
    {
        private HashSet<byte[]> completeSculptures = new HashSet<byte[]>(new ByteArrayComparer());
        private HashSet<byte[]> incompleteSculptures = new HashSet<byte[]>(new ByteArrayComparer());
        
        
        public bool Add(Sculpture sculpture, byte currentSize)
        {
            //var newSculptureString = sculpture.ToString();
            var newSculpture = sculpture.ToArray((byte)(currentSize+1));
            var collection = incompleteSculptures;
            if (currentSize + 1 == Sculpture.MaxBlocks)
            {
                collection = completeSculptures;
                if (sculpture.IsBalanced() && !collection.Contains(newSculpture))
                {
                    collection.Add(newSculpture);
                    return true;
                }
            }
            //if the sculpture is incomplete
            else if (!collection.Contains(newSculpture))
            {
                collection.Add(newSculpture);
                return true;
            }
            return false;
        }

        public long Count => completeSculptures.Count;
    }
}
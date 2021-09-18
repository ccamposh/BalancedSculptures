using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BalancedSculptures
{
    public class Collection
    {
        private ConcurrentDictionary<byte[], object> completeSculptures = new ConcurrentDictionary<byte[], object>(new ByteArrayComparer());
        private ConcurrentDictionary<byte[], object> incompleteSculptures = new ConcurrentDictionary<byte[], object>(new ByteArrayComparer());
        
        
        public bool Add(Sculpture sculpture, byte currentSize)
        {
            var newSculpture = sculpture.ToArray((byte)(currentSize+1));
            var collection = incompleteSculptures;
            if (currentSize + 1 == Sculpture.MaxBlocks)
            {
                collection = completeSculptures;
                if (sculpture.IsBalanced() && !collection.ContainsKey(newSculpture))
                {
                    collection.TryAdd(newSculpture, null);
                    return true;
                }
            }
            //if the sculpture is incomplete, add it to the 
            else if (!collection.ContainsKey(newSculpture))
            {
                collection.TryAdd(newSculpture, null);
                return true;
            }
            return false;
        }

        public void Save(string filename)
        {
            var result = "";
            foreach(var item in completeSculptures.Keys)
            {
                foreach( var b in item)
                {
                    result += b.ToString("00");
                }
                result += System.Environment.NewLine;
            }
            File.AppendAllText(filename, result);
        }

        public long Count => completeSculptures.Count;
    }
}
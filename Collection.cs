using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BalancedSculptures
{
    public class Collection
    {
        private HashSet<string> collection = new HashSet<string>();
        
        public bool Add(Sculpture sculpture)
        {
            var newSculpture = sculpture.ToString();
            if (!collection.Contains(newSculpture))
            {
                collection.Add(newSculpture);
                return true;
            }
            return false;
        }

        public long Count => collection.Count;
    }
}
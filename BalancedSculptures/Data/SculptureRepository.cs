using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ccamposh.BalancedSculptures.Dto;
using ccamposh.BalancedSculptures.Interfaces;
using ccamposh.BalancedSculptures.Utils;

namespace ccamposh.BalancedSculptures.Data
{
    public class SculptureRepository : ISculptureRepository
    {
        private HashSet<byte[]> storage;

        public SculptureRepository()
        {
            storage = new HashSet<byte[]>(new ByteArrayComparer());
        }
        public long Count => storage.Count;

        public bool TryInsert( byte[] key )
        {
            return storage.Add(key);
        }

        public void SaveToFile( string filename )
        {
            var sb = new StringBuilder();
            foreach(var item in storage)
            {
                foreach( var b in item)
                {
                    sb.Append(b.ToString("00"));
                }
                sb.Append(System.Environment.NewLine);
            }
            File.WriteAllText(filename, sb.ToString());
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using ccamposh.BalancedSculptures.Dto;
using ccamposh.BalancedSculptures.Interfaces;
using ccamposh.BalancedSculptures.Utils;

namespace ccamposh.BalancedSculptures.Data
{
    public class SculptureRepository : ISculptureRepository
    {
        private ConcurrentDictionary<Guid, object> storage;

        public SculptureRepository()
        {
            storage = new ConcurrentDictionary<Guid, object>();
        }
        public long Count => storage.Count;

        public bool TryInsert( Guid key )
        {
            return storage.TryAdd(key, null);
        }

        public void SaveToFile( string filename )
        {
            var sb = new StringBuilder();
            foreach(var item in storage.Keys)
            {
                //foreach( var b in item)
                //{
                //    sb.Append(b.ToString("00"));
                //}
                sb.Append(System.Environment.NewLine);
            }
            File.WriteAllText(filename, sb.ToString());
        }
    }
}
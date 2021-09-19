using System.Collections.Generic;

namespace ccamposh.BalancedSculptures.Utils
{
    public class ByteArrayComparer : IEqualityComparer<byte[]> {
        public bool Equals(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
                if (a[i] != b[i]) return false;
            return true;
        }
        public int GetHashCode(byte[] a)
        {
            uint b = 0;
            for (int i = 0; i < a.Length; i++)
                b = ((b << 23) | (b >> 9)) ^ a[i];
            return unchecked((int)b);
        }

        public static int Compare(byte[] a, byte[] b)
        {
            for (byte i = 0; i < a.Length; i++ )
            {
                if (a[i] < b[i]) return -1;
                if (a[i] > b[i]) return 1;
            }
            return 0;
        }
    }    
}
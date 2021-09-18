using System.Collections.Generic;
using System.Text;

namespace BalancedSculptures
{
    public class Sculpture
    {
        const byte maxX = 17;
        const byte maxY = 18;
        const byte center = 8;
        public bool[,] Map = new bool[ maxX, maxY ];
        public static byte MaxBlocks;
        private static byte minimum;
        public static byte maximum;

        public Sculpture()
        {
            Map[ 8, 0 ] = true;
        }

        public static void SetupSize( byte size )
        {
            MaxBlocks = size;
            minimum = (byte)(center - ( size / 2 + 1 ));
            maximum = (byte)(center + size / 2 + 1);
        }

        public Sculpture( Sculpture sculpture, (byte x, byte y) position )
        {
            for ( var x = minimum; x < maximum; x++ )
            {
                for ( var y = 0; y < MaxBlocks; y++ )
                {
                    Map[ x, y ] = sculpture.Map[ x, y ];
                }
            }
            Map[ position.x, position.y ] = true;
        }

        public List<(byte x, byte y)> getNextValidPositions()
        {
            var result = new List<(byte x, byte y)>();
            for ( byte x = minimum; x < maximum; x++ )
            {
                for ( byte y = 0; y < MaxBlocks; y++ )
                {
                    if ( Map[ x, y ] )
                    {
                        if ( x > minimum && !Map[ x - 1, y ] )
                        {
                            result.Add( ((byte)(x - 1), y) );
                        }
                        if ( x < maximum && !Map[ x + 1, y ] )
                        {
                            result.Add( ((byte)(x + 1), y) );
                        }
                        if ( y < MaxBlocks - 1 && !Map[ x, y + 1 ] )
                        {
                            result.Add( (x, (byte)(y + 1)) );
                        }
                        if ( y >= 1 && !Map[ x, y - 1 ] )
                        {
                            result.Add( (x, (byte)(y - 1)) );
                        }
                    }
                }
            }
            return result;
        }

        public bool IsBalanced()
        {
            var count = 0;
            for ( byte x = minimum; x < maximum; x++ )
            {
                for ( byte y = 0; y < MaxBlocks; y++ )
                {
                    if ( Map[ x, y ] )
                    {
                        count += ( x - 8 );
                    }
                }
            }
            return count == 0;
        }

        public override string ToString()
        {
            var sb1 = new StringBuilder(2 * MaxBlocks);
            var sb2 = new StringBuilder(2 * MaxBlocks);
            for ( int y = 0; y < MaxBlocks; y++ )
            {
                for ( int x = minimum; x < maximum; x++ )
                {
                    if ( Map[ x, y ] )
                    {
                        sb1.Append(x.ToString("00"));
                    }
                }
                for ( int x = maximum; x >= minimum; x-- )
                {
                    if ( Map[ x, y ] )
                    {
                        sb2.Append((maxX - 1 - x).ToString("00"));
                    }
                }
            }
            var result1 = sb1.ToString();
            var result2 = sb2.ToString();   
            if (string.Compare(result1, result2) <= 0)
                return result1;
            return result2;
        }

        public byte[] ToArray(byte size)
        {
            var result1 = new byte[size];
            var r1 = 0;
            var result2 = new byte[size];
            var r2 = 0;
            var j = 0;
            for ( byte y = 0; y < MaxBlocks; y++ )
            {
                for ( byte x = minimum; x < maximum; x++ )
                {
                    if ( Map[ x, y ] )
                    {
                        result1[r1] = (byte)x;
                        r1++;
                    }
                }
                //when the minimum is 0, 
                for ( byte x = maximum; x >= minimum && x <= maximum; x-- )
                {
                    if ( Map[ x, y ] )
                    {
                        result2[r2] = (byte)(maxX - 1 - x);
                        r2++;
                    }
                }
                j++;
            }

            //Compare the two arrays
            if (ByteArrayComparer.Compare(result1, result2) <= 0)
            {
                return result1;
            }
            return result2;
        }        
    }
}
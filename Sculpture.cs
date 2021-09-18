using System.Collections.Generic;
using System.Text;

namespace BalancedSculptures
{
    public class Sculpture
    {
        const int maxX = 17;
        const int maxY = 18;
        const int center = 8;
        public bool[,] Map = new bool[ maxX, maxY ];
        public static int MaxBlocks;
        private static int minimum;
        public static int maximum;

        public Sculpture()
        {
            Map[ 8, 0 ] = true;
        }

        public static void SetupSize( int size )
        {
            MaxBlocks = size;
            minimum = center - ( size / 2 + 1 );
            maximum = center + size / 2 + 1;
        }

        public Sculpture( Sculpture sculpture, (int x, int y) position )
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

        public List<(int x, int y)> getNextValidPositions()
        {
            var result = new List<(int x, int y)>();
            for ( int x = minimum; x < maximum; x++ )
            {
                for ( int y = 0; y < MaxBlocks; y++ )
                {
                    if ( Map[ x, y ] )
                    {
                        if ( x > minimum && !Map[ x - 1, y ] )
                        {
                            result.Add( (x - 1, y) );
                        }
                        if ( x < maximum && !Map[ x + 1, y ] )
                        {
                            result.Add( (x + 1, y) );
                        }
                        if ( y < MaxBlocks - 1 && !Map[ x, y + 1 ] )
                        {
                            result.Add( (x, y + 1) );
                        }
                        if ( y >= 1 && !Map[ x, y - 1 ] )
                        {
                            result.Add( (x, y - 1) );
                        }
                    }
                }
            }
            return result;
        }

        public bool IsBalanced()
        {
            var count = 0;
            for ( int x = minimum; x < maximum; x++ )
            {
                for ( int y = 0; y < MaxBlocks; y++ )
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
            var sb1 = new StringBuilder(4 * MaxBlocks);
            var sb2 = new StringBuilder(4 * MaxBlocks);
            for ( int y = 0; y < MaxBlocks; y++ )
            {
                for ( int x = minimum; x < maximum; x++ )
                {
                    if ( Map[ x, y ] )
                    {
                        sb1.Append(x.ToString("00") + y.ToString("00"));
                    }
                }
                for ( int x = maximum; x >= minimum; x-- )
                {
                    if ( Map[ x, y ] )
                    {
                        sb2.Append((maxX - 1 - x).ToString("00") + y.ToString("00"));
                    }
                }

            }
            var result1 = sb1.ToString();
            var result2 = sb2.ToString();   
            if (string.Compare(result1, result2) <= 0)
                return result1;
            return result2;
        }
    }
}
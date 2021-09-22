using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using ccamposh.BalancedSculptures.Utils;

[assembly: InternalsVisibleTo( "BalancedSculptures.Tests" )]
namespace ccamposh.BalancedSculptures.Dto
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

        public byte CurrentSize = 0;

        public Sculpture()
        {
            Map[ 8, 0 ] = true;
            CurrentSize = 1;
        }

        public bool IsComplete => CurrentSize == Sculpture.MaxBlocks;

        public static void SetupSize( byte size )
        {
            MaxBlocks = size;
            if ( size % 2 == 0 )
            {
                minimum = ( byte )( center - ( size / 2 - 1 ) );
                maximum = ( byte )( center + size / 2 - 1 );
            }
            else
            {
                minimum = ( byte )( center - ( size / 2 ) );
                maximum = ( byte )( center + size / 2 );
            }
        }

        public Sculpture( Sculpture sculpture, (byte x, byte y) position )
        {
            for ( var x = minimum; x <= maximum; x++ )
            {
                for ( var y = 0; y < MaxBlocks; y++ )
                {
                    Map[ x, y ] = sculpture.Map[ x, y ];
                }
            }
            Map[ position.x, position.y ] = true;
            CurrentSize = ( byte )( sculpture.CurrentSize + 1 );
        }

        public Sculpture( string definition )
        {
            sbyte y = -1;
            byte lastX = 20;
            for ( byte i = 0; i < definition.Length; i += 2 )
            {
                var x = byte.Parse( definition.Substring( i, 2 ) );
                if ( x <= lastX )
                {
                    y++;
                }
                Map[ x, y ] = true;
                CurrentSize++;
                lastX = x;
            }
        }

        public static Sculpture FromArray( byte[] zipArray )
        {
            var array = DecompressArray( zipArray );
            var result = new Sculpture();
            result.CurrentSize = array[ 0 ];
            byte y = 0;
            byte lastX = 0;
            bool first = true;
            for ( byte i = 1; i <= array[ 0 ]; i++ )
            {
                byte x = array[ i ];
                if ( x <= lastX && !first )
                {
                    y++;
                }
                first = false;
                result.Map[ x, y ] = true;
                lastX = x;
            }
            return result;
        }

        public IDictionary<byte[], Sculpture> GetChildSculptures()
        {
            var result = new Dictionary<byte[], Sculpture>( new ByteArrayComparer() );
            var newBlocks = getNextValidPositions();
            foreach ( var block in newBlocks )
            {
                var childSculpture = new Sculpture( this, block );
                if ( ( childSculpture.IsComplete && childSculpture.IsBalanced )
                || ( !childSculpture.IsComplete && childSculpture.CanBeBalanced ) )
                {
                    //a mirror sculpture might already be added
                    var key = childSculpture.ToArray();
                    if ( !result.ContainsKey( key ) )
                    {
                        result.Add( key, childSculpture );
                    }
                }
            }
            return result;
        }
        internal HashSet<(byte x, byte y)> getNextValidPositions()
        {
            var result = new HashSet<(byte x, byte y)>();
            var max = MaxBlocks / 2 + 1;
            var limitedX = minimum;
            var limitedY = maximum;
            for ( byte y = 0; y < MaxBlocks; y++ )
            {
                for ( byte x = minimum; x <= maximum; x++ )
                {
                    if ( Map[ x, y ] )
                    {
                        if ( x > minimum && !Map[ x - 1, y ] )
                        {
                            if ( Math.Abs( x - 1 - center ) + ( ( y / 2 ) + 1 ) <= max )
                                result.Add( (( byte )( x - 1 ), y) );
                        }
                        if ( x < maximum && !Map[ x + 1, y ] )
                        {
                            if ( Math.Abs( x + 1 - center ) + ( ( y / 2 ) + 1 ) <= max )
                                result.Add( (( byte )( x + 1 ), y) );
                        }
                        if ( y < MaxBlocks - 1 && !Map[ x, y + 1 ] )
                        {
                            result.Add( (x, ( byte )( y + 1 )) );
                        }
                        if ( y >= 1 && !Map[ x, y - 1 ] )
                        {
                            result.Add( (x, ( byte )( y - 1 )) );
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Obtain the fartest blocks from the center, it is used to identify if a sculpture can be balanced
        /// </summary>
        internal (int min, int max) GetMostExternalsBlocks()
        {
            byte min = maximum;
            byte max = minimum;
            var blockCount = 0;
            for ( byte y = 0; y < MaxBlocks; y++ )
            {
                for ( byte x = minimum; x <= maximum; x++ )
                {
                    if ( Map[ x, y ] )
                    {
                        blockCount++;
                        if ( x < min )
                            min = x;
                        if ( x > max )
                            max = x;
                    }
                }
                if ( blockCount == CurrentSize ) break;
            }
            return (min, max);
        }

        public bool CanBeBalanced
        {
            get
            {
                var balance = GetBalance();
                //if it already balance, nothing to calculate
                if ( balance == 0 )
                {
                    return true;
                }
                var pendingBlocks = MaxBlocks - CurrentSize;
                var corners = GetMostExternalsBlocks();
                if ( balance < 0 )
                {
                    corners.max = corners.max - center;
                    for ( int i = 1; i <= pendingBlocks; i++ )
                    {
                        balance += ( i + corners.max );
                        if ( balance >= 0 )
                            return true;
                    }
                    return false;
                }
                else if ( balance > 0 )
                {
                    corners.min = corners.min - center;
                    for ( int i = 1; i <= pendingBlocks; i++ )
                    {
                        balance -= ( i - corners.min );
                        if ( balance <= 0 )
                            return true;
                    }
                    return false;
                }
                return true;
            }
        }

        public int GetBalance()
        {
            var count = 0;
            var blockCount = 0;
            for ( byte y = 0; y < MaxBlocks; y++ )
            {
                for ( byte x = minimum; x <= maximum; x++ )
                {
                    if ( Map[ x, y ] )
                    {
                        count += ( x - 8 );
                        blockCount++;
                    }
                }
                //if all blocks get counted, return
                if ( blockCount == CurrentSize ) return count;
            }
            return count;
        }

        public bool IsBalanced
        {
            get
            {
                return GetBalance() == 0;
            }
        }

        public override string ToString()
        {
            var sb1 = new StringBuilder( 2 * MaxBlocks );
            var sb2 = new StringBuilder( 2 * MaxBlocks );
            for ( int y = 0; y < MaxBlocks; y++ )
            {
                for ( int x = minimum; x <= maximum; x++ )
                {
                    if ( Map[ x, y ] )
                    {
                        sb1.Append( x.ToString( "00" ) );
                    }
                }
                for ( int x = maximum; x >= minimum; x-- )
                {
                    if ( Map[ x, y ] )
                    {
                        sb2.Append( ( maxX - 1 - x ).ToString( "00" ) );
                    }
                }
            }
            var result1 = sb1.ToString();
            var result2 = sb2.ToString();
            if ( string.Compare( result1, result2 ) <= 0 )
                return result1;
            return result2;
        }

        internal static byte[] DecompressArray( byte[] source )
        {
            if ( ( source[ 0 ] & 128 ) == 0 )
            {
                //array is not compressed
                return source;
            }
            source[ 0 ] = ( byte )( source[ 0 ] & 127 );
            var result = new byte[ source[ 0 ] + 1 ];
            result[ 0 ] = source[ 0 ];
            for ( byte i = 1; i < source.Length; i++ )
            {
                var first = ( byte )( source[ i ] >> 4 );
                result[ i * 2 - 1 ] = first;
                if ( i * 2 <= source[ 0 ] )
                {
                    var second = ( byte )( source[ i ] & 0x0F );
                    result[ i * 2 ] = second;
                }
            }
            return result;
        }

        ///<summary>
        /// Takes an array with size and all x positions and group two x position in a single byte
        /// It is required all x values should be 15 or less
        /// In case it is zipped the first bit of the array is turned on
        ///</summary>
        internal static byte[] CompressArray( byte[] source )
        {
            //if the content of the source has a value over 15 it cannot be zipped
            for ( int i = 1; i <= source[ 0 ]; i++ )
            {
                if ( source[ i ] > 15 )
                {
                    return source;
                }
            }
            var zipKey = new byte[ source[ 0 ] / 2 + source[ 0 ] % 2 + 1 ];
            zipKey[ 0 ] = source[ 0 ];
            for ( var i = 0; i < source[ 0 ]; i++ )
            {
                var index = ( i / 2 ) + 1;
                var position = i % 2;
                if ( position == 0 )
                {
                    zipKey[ index ] = ( byte )( source[ i + 1 ] << 4 );
                }
                else
                {
                    zipKey[ index ] = ( byte )( zipKey[ index ] + source[ i + 1 ] );
                }
            }
            zipKey[ 0 ] = ( byte )( zipKey[ 0 ] | 128 );
            return zipKey;
        }

        ///<summary>
        /// Takes an sculpture definitions and generates an array of the x positions. 
        /// The y position of each block is ignored, since it can be inferred
        ///<summary>
        public byte[] ToArray()
        {
            //the first byte in the array is the size of the sculpture
            var result1 = new byte[ CurrentSize + 1 ];
            result1[ 0 ] = CurrentSize;
            var r1 = 1;
            var result2 = new byte[ CurrentSize + 1 ];
            result2[ 0 ] = CurrentSize;
            var r2 = 1;
            for ( byte y = 0; y < MaxBlocks; y++ )
            {
                for ( byte x = minimum; x <= maximum; x++ )
                {
                    if ( Map[ x, y ] )
                    {
                        result1[ r1 ] = ( byte )x;
                        r1++;
                    }
                    //the mirror of x is two times the distance to the center
                    //for example, if x = 3, it is 5 positions from the center, so the mirror position is 3 + 5 + 5 = 13
                    //if x = 10, it is -2 positions from the center, so the mirror is 10 + -2 + -2 = 6
                    var mirrorX = x + ( ( center - x ) * 2 );
                    if ( Map[ mirrorX, y ] )
                    {
                        result2[ r2 ] = ( byte )x;
                        r2++;
                    }
                }
                if ( r1 > CurrentSize ) break;
            }

            //Compare the two arrays (the original and its mirror)
            byte[] result;
            if ( ByteArrayComparer.Compare( result1, result2 ) <= 0 )
            {
                result = result1;
            }
            else
            {
                result = result2;
            }
            return CompressArray( result );
        }
    }
}
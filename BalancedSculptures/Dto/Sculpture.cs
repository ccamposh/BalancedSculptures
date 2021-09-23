using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ccamposh.BalancedSculptures.Utils;

[assembly: InternalsVisibleTo( "BalancedSculptures.Tests" )]
namespace ccamposh.BalancedSculptures.Dto
{
    public static class Sculpture
    {
        const byte maxX = 17;
        const byte maxY = 18;
        const byte center = 8;
        public static byte MaxBlocks;
        private static byte minimum;
        public static byte maximum;

        public static byte[] BaseSculpture = new byte[] {1, 8};

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

        public static byte[] StringToArray( string definition)
        {
            return ToArray(StringToCoordinates(definition));
        }

        internal static HashSet<(byte x, byte y)> StringToCoordinates( string definition )
        {
            var result = new HashSet<(byte x, byte y)>();
            sbyte y = -1;
            byte lastX = 20;
            for ( byte i = 0; i < definition.Length; i += 2 )
            {
                var x = byte.Parse( definition.Substring( i, 2 ) );
                if ( x <= lastX )
                {
                    y++;
                }
                result.Add(((byte)x,(byte)y));
                lastX = x;
            }
            return result;
        }

        ///<summary>
        /// Takes an incoming array, infer the y value of each block, and generates the blocks coordinates
        ///</summary>
        public static HashSet<(byte x, byte y)> ArrayToCoordinates(byte[] array)
        {
            array = DecompressArray( array );
            var result = new HashSet<(byte x, byte y)>();
            sbyte y = -1;
            byte lastX = 20;
            for ( byte i = 1; i <= array[ 0 ]; i++ )
            {
                byte x = array[ i ];
                if ( x <= lastX )
                {
                    y++;
                }
                result.Add((x, (byte)y));
                lastX = x;
            }
            return result;            
        }

        public static HashSet<byte[]> GetChildSculptures(byte[] array)
        {
            var result = new HashSet<byte[]>(new ByteArrayComparer());
            var baseSculpture = ArrayToCoordinates(array);
            var newBlocks = GetNextValidPositions(baseSculpture);
            foreach ( var block in newBlocks )
            {
                var childSculpture = new HashSet<(byte x, byte y)>(baseSculpture);
                childSculpture.Add(block);
                var childArray = Sculpture.ToArray(childSculpture);
                if ( ( childSculpture.Count == MaxBlocks && IsBalanced(childArray) )
                || ( childSculpture.Count < MaxBlocks && CanBeBalanced(childArray) ) )
                {
                    //a mirror sculpture might already be added
                    var mirrorArray = Sculpture.ToArray(GetMirror(childSculpture));
                    var toInsert = ByteArrayComparer.Compare(childArray, mirrorArray) <= 0? childArray: mirrorArray;
                    if ( !result.Contains( toInsert ) )
                    {
                        result.Add( toInsert );
                    }
                }
            }
            return result;
        }

        internal static HashSet<(byte x, byte y)> GetNextValidPositions(HashSet<(byte x, byte y)> currentBlocks)
        {
            var result = new HashSet<(byte x, byte y)>();
            var max = MaxBlocks / 2 + 1;
            foreach ( var block in currentBlocks )
            {
                var nextBlock = ((byte)(block.x - 1), block.y);
                if ( block.x > minimum && !currentBlocks.Contains(nextBlock))
                {
                    result.Add(nextBlock);
                }
                nextBlock = ((byte)(block.x + 1), block.y);
                if ( block.x < maximum && !currentBlocks.Contains(nextBlock))
                {
                    result.Add(nextBlock);
                }
                nextBlock = (block.x, (byte)(block.y + 1));
                if ( block.y < MaxBlocks - 1 && !currentBlocks.Contains(nextBlock))
                {
                    result.Add( nextBlock );
                }
                nextBlock = (block.x, (byte)(block.y - 1));
                if ( block.y >= 1 && !currentBlocks.Contains(nextBlock))
                {
                    result.Add( nextBlock );
                }
            }
            return result;
        }

        internal static int GetMax(byte[] array)
        {
            var result = -1;
            for(var i = 1; i <= array[0]; i++)
                if (result < array[i]) result = array[i];
            return result;
        }

        internal static int GetMin(byte[] array)
        {
            var result = 20;
            for(var i = 1; i <= array[0]; i++)
                if (result > array[i]) result = array[i];
            return result;
        }        

        internal static bool CanBeBalanced(byte[] array)
        {
            var balance = GetBalance(array);
            if ( balance == 0 )
            {
                return true;
            }
            var pendingBlocks = MaxBlocks - array[0];            
            if ( balance < 0 )
            {
                var max = GetMax(array) - center;
                for ( int i = 1; i <= pendingBlocks; i++ )
                {
                    balance += ( i + max );
                    if ( balance >= 0 )
                        return true;
                }
                return false;
            }
            else 
            {
                var min = GetMin(array) - center;
                for ( int i = 1; i <= pendingBlocks; i++ )
                {
                    balance -= ( i - min );
                    if ( balance <= 0 )
                        return true;
                }
                return false;
            }
        }

        internal static int GetBalance(byte[] array)
        {
            var result = array[0] * -8;
            for (int i = 1; i <= array[0]; i++)
                result += array[i];
            return result;
        }        

        internal static bool IsBalanced(byte[] array)
        {
            return GetBalance(array) == 0;
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

        internal static HashSet<(byte x, byte y)> GetMirror(HashSet<(byte x, byte y)> sculpture)
        {
            var result = new HashSet<(byte x, byte y)>();
            foreach(var block in sculpture)
            {
                if (block.x == center)
                    result.Add(block);
                else if (block.x < center)
                    result.Add(((byte)(block.x + ((center - block.x) * 2)), block.y));
                else 
                    result.Add(((byte)(block.x + ((center - block.x) * 2)), block.y));
            }
            return result;
        }

        public static byte[] ToArray(HashSet<(byte x, byte y)> sculpture)
        {
            var result = new byte[ sculpture.Count + 1 ];
            result[0] = (byte)sculpture.Count;
            var position = 1; 
            foreach (var block in sculpture.OrderBy(i => i.y).ThenBy(i => i.x))
            {
                result[position++] = block.x;
            }
            return result;
        }
    }
}
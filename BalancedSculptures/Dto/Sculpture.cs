using System;
using System.Collections.Generic;
using System.Text;
using ccamposh.BalancedSculptures.Utils;

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
            if (size % 2 == 0)
            {
                minimum = (byte)(center - ( size / 2 - 1));
                maximum = (byte)(center + size / 2 - 1);
            }
            else 
            {
                minimum = (byte)(center - ( size / 2));
                maximum = (byte)(center + size / 2);
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
            CurrentSize = (byte)(sculpture.CurrentSize + 1);
        }

        public Sculpture( string definition )
        {
            byte y = 0;
            byte lastX = 0;
            for (byte i = 0; i < definition.Length; i += 2 )
            {
                var x = byte.Parse(definition.Substring(i, 2));
                if (x <= lastX)
                {
                    y++;
                }
                Map[x,y] = true;
                CurrentSize++;
                lastX = x;
            }
        }

        public IEnumerable<Sculpture> GetChildSculptures()
        {
            var result = new Dictionary<byte[],Sculpture>(new ByteArrayComparer());
            var newBlocks = getNextValidPositions();
            foreach(var block in newBlocks)
            {
                var childSculpture = new Sculpture(this, block);
                if ((childSculpture.IsComplete && childSculpture.IsBalanced) 
                || (!childSculpture.IsComplete && childSculpture.CanBeBalanced))
                {
                    if (!result.ContainsKey(childSculpture.ToArray()))
                    {
                        result.Add(childSculpture.ToArray(), childSculpture);
                    }
                }
            }
            return result.Values;
        }
        internal List<(byte x, byte y)> getNextValidPositions()
        {
            var result = new List<(byte x, byte y)>();
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
                            if (Math.Abs(x - 1 - center) + ((y / 2) + 1) <= max)
                                result.Add( ((byte)(x - 1), y) );
                        }
                        if ( x < maximum && !Map[ x + 1, y ] )
                        {
                            if (Math.Abs(x + 1 - center) + ((y / 2) + 1) <= max)
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

        private (int min, int max) getCorners()
        {
            byte min = maximum;
            byte max = minimum;
            for ( byte y = 0; y < MaxBlocks; y++ )
            {
                for ( byte x = minimum; x <= maximum; x++ )
                {
                    if (Map[x,y])
                    {
                        if (x < min)
                            min = x;
                        if (x > max)
                            max = x;
                    }
                }
            }
            return (min - center, max - center);
        }

        public bool CanBeBalanced
        {
            get
            {
                var balance = GetBalance();
                var corners = getCorners();
                if (balance < 0)
                {
                    for (int i = 1; i <= (MaxBlocks - CurrentSize); i++)
                    {
                        balance += (i + corners.max); 
                        if (balance >= 0)
                            return true;
                    }
                    return false;
                }
                else if (balance > 0)
                {
                    for (int i = 1; i <= (MaxBlocks - CurrentSize); i++)
                    {
                        balance -= (i - corners.min); 
                        if (balance <= 0)
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
            for ( byte x = minimum; x <= maximum; x++ )
            {
                for ( byte y = 0; y < MaxBlocks; y++ )
                {
                    if ( Map[ x, y ] )
                    {
                        count += ( x - 8 );
                    }
                }
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

        public byte[] ToArray()
        {
            var result1 = new byte[CurrentSize];
            var r1 = 0;
            var result2 = new byte[CurrentSize];
            var r2 = 0;
            var j = 0;
            for ( byte y = 0; y < MaxBlocks; y++ )
            {
                for ( byte x = minimum; x <= maximum; x++ )
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
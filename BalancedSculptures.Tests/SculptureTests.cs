
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ccamposh.BalancedSculptures.Dto;
using ccamposh.BalancedSculptures.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CREngland.CompanyService.Tests
{
    [TestClass]
    public class SculptureTests
    {
        [TestMethod]
        public void GetChildSculptures_StringConstructor()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToArray( "0506070809100608091011" );
            Assert.IsTrue( ByteArrayComparer.Compare(new byte[] {11, 5,6,7,8,9,10,6,8,9,10,11}, sculpture ) == 0);
        }

        [TestMethod]
        public void GetChildSculptures_NoDuplicate()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToArray( "0506070809100608091011" );
            var list = Sculpture.GetChildSculptures(sculpture);
            Assert.AreEqual( 1, list.Count() );
        }

        [TestMethod]
        public void StaticGetChildSculptures_NoDuplicate()
        {
            Sculpture.SetupSize( 12 );
            var array = new byte[] {2, 7, 8};
            var list = Sculpture.GetChildSculptures(array);
            Assert.AreEqual( 4, list.Count() );
        }        

        [TestMethod]
        public void GetBalance_Logic()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToArray( "0506070809100608091011" );
            var result = Sculpture.GetBalance(sculpture);
            Assert.AreEqual( 1, result );
        }

        [TestMethod]
        public void GetBalanceArray_Logic()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToArray( "0506070809100608091011" );
            var result = Sculpture.GetBalance(sculpture);
            Assert.AreEqual( 1, result );
        }        

        [TestMethod]
        public void CanBeBalance_Performance()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToArray( "0506070809100608091011" );
            var watch = Stopwatch.StartNew();
            for ( int i = 1; i < 5000; i++ )
            {
                Sculpture.CanBeBalanced(sculpture);
            }
            watch.Stop();
            Assert.IsTrue( 750000 > watch.ElapsedTicks, "Duration: " + watch.ElapsedTicks);
        }        

        [TestMethod]
        public void GetBalanceArray_Performance()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToArray( "0506070809100608091011" );
            var watch = Stopwatch.StartNew();
            for ( int i = 1; i < 5000; i++ )
            {
                Sculpture.GetBalance(sculpture);
            }
            watch.Stop();
            Assert.IsTrue( 250000 > watch.ElapsedTicks, "Duration: " + watch.ElapsedTicks);
        }        

        [TestMethod]
        public void ToArray_CheckValue()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToCoordinates( "0506070809100608091011" );
            var result = Sculpture.ToArray(sculpture);
            Assert.IsTrue( ByteArrayComparer.Compare( new byte[] { 11, 5, 6, 7, 8, 9, 10, 6, 8, 9, 10, 11 }, result ) == 0 );
        }

        [TestMethod]
        public void ToArray_CheckStaticMirror()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = new byte[] { 11, 6,7,8,9,10,11,5,6,7,8,10 };
            Assert.IsTrue( ByteArrayComparer.Compare( new byte[] { 11, 5, 6, 7, 8, 9, 10, 6, 8, 9, 10, 11 }, Sculpture.ToArray(Sculpture.GetMirror(Sculpture.ArrayToCoordinates(sculpture)))) == 0 );
        }   

        [TestMethod]
        public void ToArray_CheckStaticMirror2()
        {
            Sculpture.SetupSize( 12 );
            var array = new byte[] {2, 8, 9};
            var sculpture = Sculpture.ArrayToCoordinates(array);
            Assert.IsTrue( ByteArrayComparer.Compare( new byte[] { 2, 7, 8 }, Sculpture.ToArray(Sculpture.GetMirror(sculpture))) == 0 );
        }              

        [TestMethod]
        public void CanBeBalanced_Logic()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToArray( "0506070809100608091011" );
            var result = Sculpture.CanBeBalanced(sculpture);
            Assert.IsTrue( result );
        }

        [TestMethod]
        public void CompressDeCompress_BeingZippedEven()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToArray( "050607080910060809101112" );
            var zipped = Sculpture.CompressArray(sculpture);
            Assert.IsTrue( ByteArrayComparer.Compare( new byte[] { 0x8C, 0x56, 0x78, 0x9A, 0x68, 0x9A, 0xBC }, zipped ) == 0 );
            var decompressed = Sculpture.DecompressArray( zipped );
            Assert.IsTrue( ByteArrayComparer.Compare( new byte[] { 12, 5, 6, 7, 8, 9, 10, 6, 8, 9, 10, 11, 12 }, decompressed ) == 0 );
        }

        [TestMethod]
        public void CompressDeCompress_BeingZippedSmall()
        {
            Sculpture.SetupSize( 12 );
            var sculpture = Sculpture.StringToArray( "0808" );
            var zipped = Sculpture.CompressArray(sculpture);
            Assert.IsTrue( ByteArrayComparer.Compare( new byte[] { 0x82, 0x88 }, zipped ) == 0 );
            var decompressed = Sculpture.DecompressArray( zipped );
            Assert.IsTrue( ByteArrayComparer.Compare( new byte[] { 2, 8, 8 }, decompressed ) == 0 );
        }

        [TestMethod]
        public void CompressDeCompress_CannotBeZipped()
        {
            Sculpture.SetupSize( 18 );
            var sculpture = Sculpture.StringToArray( "0001020304050607080910111213141516" );
            var zipped = Sculpture.CompressArray(sculpture);
            Assert.IsTrue( ByteArrayComparer.Compare( new byte[] { 17, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, zipped ) == 0 );
            var decompressed = Sculpture.DecompressArray( zipped );
            Assert.IsTrue( ByteArrayComparer.Compare( new byte[] { 17, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, decompressed ) == 0 );
        }

        [TestMethod]
        public void ArrayToCoordinatesTest()
        {
            Sculpture.SetupSize( 18 );
            var sculpture = Sculpture.StringToArray( "07080708" );
            var result = Sculpture.ArrayToCoordinates(sculpture);
            var list = result.ToList();
            Assert.AreEqual((7,0), list[0]);
            Assert.AreEqual((8,0), list[1]);
            Assert.AreEqual((7,1), list[2]);
            Assert.AreEqual((8,1), list[3]);
        }

        [TestMethod]
        public void GetNextValidPositionsTest()
        {
            Sculpture.SetupSize( 18 );
            var sculpture = Sculpture.StringToArray( "0708080910" );
            var result = Sculpture.GetNextValidPositions(Sculpture.ArrayToCoordinates(sculpture));
            var list = result.ToList();
            Assert.AreEqual(8, list.Count());
            Assert.AreEqual((6,0), list[0]);
            Assert.AreEqual((7,1), list[1]);
            Assert.AreEqual((9,0), list[2]);
            Assert.AreEqual((8,2), list[3]);
            Assert.AreEqual((9,2), list[4]);
            Assert.AreEqual((11,1), list[5]);
            Assert.AreEqual((10,2), list[6]);
            Assert.AreEqual((10,0), list[7]);
        }  

        [TestMethod]
        public void IsBalancedTest1()
        {
            Sculpture.SetupSize( 18 );
            var sculpture = Sculpture.StringToArray( "060708080910" );
            Assert.IsTrue(Sculpture.IsBalanced(sculpture));
        }    

        [TestMethod]
        public void IsBalancedTest2()
        {
            Sculpture.SetupSize( 18 );
            var sculpture = Sculpture.StringToArray( "0708080910" );
            Assert.AreEqual(2, Sculpture.GetBalance(sculpture));
        }                   
    }
}
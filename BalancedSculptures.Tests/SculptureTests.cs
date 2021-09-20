
using System;
using System.Diagnostics;
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
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            Assert.AreEqual("0506070809100608091011", sculpture.ToString());
            Assert.AreEqual(11, sculpture.CurrentSize);
        }

        [TestMethod]
        public void GetChildSculptures_NoDuplicate()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var list = sculpture.GetChildSculptures();
            Assert.AreEqual(1, list.Count());
        }  

        [TestMethod]
        public void GetBalance_Logic()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var result = sculpture.GetBalance();
            Assert.AreEqual(1, result);
        }      

        [TestMethod]
        public void GetBalance_Performance()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var watch = Stopwatch.StartNew();
            for (int i = 1; i < 5000; i++)
            {
                sculpture.GetBalance();
            }
            watch.Stop();
            Assert.IsTrue(1000000 > watch.ElapsedTicks);
        }   

        [TestMethod]
        public void ToArray_CheckValue()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var result = sculpture.ToArray();
            Assert.IsTrue(ByteArrayComparer.Compare(new byte[] {5, 6, 7, 8, 9, 10, 6, 8, 9, 10, 11}, result) == 0);
        }

        [TestMethod]
        public void ToArray_CheckMirror()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0607080910110506070810");
            var result = sculpture.ToArray();
            Assert.IsTrue(ByteArrayComparer.Compare(new byte[] {5, 6, 7, 8, 9, 10, 6, 8, 9, 10, 11}, result) == 0);
        }        

        [TestMethod]
        public void ToArray_Performance()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var watch = Stopwatch.StartNew();
            for (int i = 1; i < 5000; i++)
            {
                sculpture.ToArray();
            }
            watch.Stop();
            Assert.IsTrue(2000000 > watch.ElapsedTicks);
        }

        [TestMethod]
        public void GetMostExternalsBlocks_Logic()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var result = sculpture.GetMostExternalsBlocks();
            Assert.AreEqual((5, 11), result);
        }    

        [TestMethod]
        public void GetMostExternalsBlocks_Performance()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var watch = Stopwatch.StartNew();
            for (int i = 1; i < 5000; i++)
            {
                sculpture.GetMostExternalsBlocks();
            }
            watch.Stop();
            Console.WriteLine("Duration " + watch.ElapsedTicks);
            Assert.IsTrue(1500000 > watch.ElapsedTicks);
        }    

        [TestMethod]
        public void CanBeBalanced_Logic()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var result = sculpture.CanBeBalanced;
            Assert.IsTrue(result);
        } 

        [TestMethod]
        public void CanBeBalanced_Performance()
        {
            Sculpture.SetupSize(12);
            var sculpture = new Sculpture("0506070809100608091011");
            var watch = Stopwatch.StartNew();
            for (int i = 1; i < 5000; i++)
            {
                var r = sculpture.CanBeBalanced;
            }
            watch.Stop();
            Console.WriteLine("Duration " + watch.ElapsedTicks);
            Assert.IsTrue(2500000 > watch.ElapsedTicks);
        }         
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MCC.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCC.Core.Tests
{
    [TestClass()]
    public class MultiCommentCollectorTests
    {
        [TestMethod()]
        public void ServerStartTest()
        {
            MultiCommentCollector.GetInstance().ServerStart();
        }
    }
}
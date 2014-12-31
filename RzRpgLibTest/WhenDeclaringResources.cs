using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenDeclaringResources
    {
        [TestMethod]
        public void SimpleAddResourceCaseWorks()
        {
            GameEntity player = new GameEntity();
            player.AddResource( "HitPoints", 100, 100 );

            Assert.IsTrue( player["HitPoints"].Value == 100 );
        }
    }
}
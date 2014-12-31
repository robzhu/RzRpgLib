using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenConstructingGameEntities
    {
        [TestMethod]
        public void DefaultConstructorDoesNotThrow()
        {
            GameEntity ent = new GameEntity();
        }

        [TestMethod]
        public void IndexingUndefinedPropertyDoesNotThrow()
        {
            GameEntity ent = new GameEntity();
            var mana = ent["Mana"];
        }
    }
}
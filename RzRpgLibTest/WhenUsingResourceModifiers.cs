using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingResourceModifiers
    {
        [TestMethod]
        public void CallingApplyPersistOnEffectAddsItToEffectsCollection()
        {
            GameEntity player = new GameEntity();
            var property = new ResourceProperty( "Shield", 0, 100 );
            player.AddProperty( property );

            ResourceModifier effect = new ResourceModifier( "Shield", 10 );
            Assert.IsTrue( player.HasPersistentEffect( effect ) == false );
            Assert.IsTrue( effect.IsPersisted == false );

            effect.ApplyPersist( player );

            Assert.IsTrue( player.HasPersistentEffect( effect ) == true );
            Assert.IsTrue( effect.IsPersisted == true );
            Assert.IsTrue( player[ "Shield" ].Value == 10 );
        }

        [TestMethod]
        public void CallingUnapplyOnEffectRemovesItFromEffectsCollection()
        {
            GameEntity player = new GameEntity();
            var property = new ResourceProperty( "Shield", 0, 100 );
            player.AddProperty( property );

            ResourceModifier effect = new ResourceModifier( "Shield", 10 );
            effect.ApplyPersist( player );

            Assert.IsTrue( player.HasPersistentEffect( effect ) == true );
            Assert.IsTrue( effect.IsPersisted == true );
            Assert.IsTrue( player[ "Shield" ].Value == 10 );

            effect.Unapply();

            Assert.IsTrue( player.HasPersistentEffect( effect ) == false );
            Assert.IsTrue( effect.IsPersisted == false );
            Assert.IsTrue( player[ "Shield" ].Value == 0 );
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenApplyingPersistentEffects
    {
        [TestMethod]
        public void DefaultUniquePersistenceIdIsNull()
        {
            var effect = new StatisticModifier( "HP", 100 );
            Assert.IsTrue( effect.UniquePersistenceId == null );
        }

        [TestMethod]
        public void UniquePersistenceIdOverloadWorks()
        {
            var effect = new StatisticModifier( "HP", "UniqueEffect", 100 );
            Assert.IsTrue( effect.UniquePersistenceId == "UniqueEffect" );
        }

        [TestMethod]
        public void ApplyingWeakerDuplicateEffectDoesNothing()
        {
            var player = new GameEntity();
            player.AddProperty( new StatisticProperty( "HP", 100 ) );

            //strong and weak effects below share the same Unique identifier: meow
            //Only the stronger one should perist.
            var effectStrong = new StatisticModifier( "HP", "meow", 100 );
            var effectWeak = new StatisticModifier( "HP", "meow", 50 );

            player.AddPersistentEffect( effectStrong );
            player.AddPersistentEffect( effectWeak );

            Assert.IsTrue( player.PersistentEffects.Count == 1 );
            Assert.IsTrue( player.PersistentEffects[ 0 ] == effectStrong );
            Assert.IsTrue( player.GetProperty( "HP" ).Value.Value == 200 );
        }

        [TestMethod]
        public void StrongerUniqueEffectReplacesWeakerOne()
        {
            var player = new GameEntity();
            player.AddProperty( new StatisticProperty( "HP", 100 ) );

            //strong and weak effects below share the same Unique identifier: meow
            //Only the stronger one should perist.
            var effectStrong = new StatisticModifier( "HP", "meow", 100 );
            var effectWeak = new StatisticModifier( "HP", "meow", 50 );

            player.AddPersistentEffect( effectWeak );
            player.AddPersistentEffect( effectStrong );

            Assert.IsTrue( player.PersistentEffects.Count == 1 );
            Assert.IsTrue( player.PersistentEffects[ 0 ] == effectStrong );
            Assert.IsTrue( player.GetProperty( "HP" ).Value.Value == 200 );
        }

        [TestMethod]
        public void RemovingStrongestEffectYieldsNextStrongestEffect()
        {
            var player = new GameEntity();
            player.AddProperty( new StatisticProperty( "HP", 100 ) );

            //effects below share the same Unique identifier: meow
            //Only the strongest one should perist at any given time.
            var effectStrong = new StatisticModifier( "HP", "meow", 100 );
            var effectMedium = new StatisticModifier( "HP", "meow", 75 );
            var effectWeak = new StatisticModifier( "HP", "meow", 50 );

            player.AddPersistentEffect( effectWeak );
            player.AddPersistentEffect( effectStrong );
            player.AddPersistentEffect( effectMedium );

            Assert.IsTrue( player.PersistentEffects.Count == 1 );
            Assert.IsTrue( player.PersistentEffects[ 0 ] == effectStrong );
            Assert.IsTrue( player.GetProperty( "HP" ).Value.Value == 200 );

            player.RemovePersistentEffect( effectStrong );

            Assert.IsTrue( player.PersistentEffects.Count == 1 );
            Assert.IsTrue( player.PersistentEffects[ 0 ] == effectMedium );
            Assert.IsTrue( player.GetProperty( "HP" ).Value.Value == 175 );

            player.RemovePersistentEffect( effectMedium );

            Assert.IsTrue( player.PersistentEffects.Count == 1 );
            Assert.IsTrue( player.PersistentEffects[ 0 ] == effectWeak );
            Assert.IsTrue( player.GetProperty( "HP" ).Value.Value == 150 );
        }
    }
}
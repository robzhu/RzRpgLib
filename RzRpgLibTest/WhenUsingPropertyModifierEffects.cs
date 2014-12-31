using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingPropertyModifierEffects
    {
        [TestMethod]
        public void AddingEffectModifiesExistingStat()
        {
            //Create a player with 100 hp
            GameEntity player = new GameEntity();
            var property = new StatisticProperty( "HP", 100 );
            player.AddProperty( property );

            //Create a piece of armor with +50 hp
            StatisticModifier modifyHpEffect = StatisticModifierFactory.CreateFromFullString( "+50 HP" );

            Assert.IsTrue( property.Value.Value == 100 );
            player.AddPersistentEffect( modifyHpEffect );
            Assert.IsTrue( property.Value.Value == 150 );
        }

        [TestMethod]
        public void RemovingEffectUnmodifiesExistingStat()
        {
            //Create a player with 100 hp
            GameEntity player = new GameEntity();
            var property = new StatisticProperty( "HP", 100 );
            player.AddProperty( property );

            //Create an effect: +50 hp
            StatisticModifier modifyHpEffect = StatisticModifierFactory.CreateFromFullString( "+50 HP" );

            Assert.IsTrue( property.Value.Value == 100 );
            player.AddPersistentEffect( modifyHpEffect );
            Assert.IsTrue( property.Value.Value == 150 );
            player.RemovePersistentEffect( modifyHpEffect );
            Assert.IsTrue( property.Value.Value == 100 );
        }

        [TestMethod]
        public void AddOnEquipEffectPropertyExtensionsPositiveFlatAmountWork()
        {
            GameEntity player = new GameEntity();
            var property = new StatisticProperty( "Intellect", 100 );
            player.AddProperty( property );

            GameEntity gear = new GameEntity();
            gear.AddEquipEffectProperty( "+10 Intellect" );

            Assert.IsTrue( player["Intellect"].Value == player["Intellect"].BaseValue );

            player.Accessories.Add( gear );

            Assert.IsTrue( player["Intellect"].Value == ( player["Intellect"].BaseValue + 10 ) );
        }

        [TestMethod]
        public void AddOnEquipEffectPropertyExtensionsPositiveNegativeAmountWork()
        {
            GameEntity player = new GameEntity();
            var property = new StatisticProperty( "Intellect", 100 );
            player.AddProperty( property );

            GameEntity gear = new GameEntity();
            gear.AddEquipEffectProperty( "-10 Intellect" );

            Assert.IsTrue( player["Intellect"].Value == player["Intellect"].BaseValue );

            player.Accessories.Add( gear );

            Assert.IsTrue( player["Intellect"].Value == (player["Intellect"].BaseValue - 10) );
        }

        [TestMethod]
        public void AddOnEquipEffectPropertyExtensionsPositiveScaleAmountWork()
        {
            GameEntity player = new GameEntity();
            var property = new StatisticProperty( "Intellect", 100 );
            player.AddProperty( property );

            GameEntity gear = new GameEntity();
            gear.AddEquipEffectProperty( "-50% Intellect" );

            Assert.IsTrue( player["Intellect"].Value == player["Intellect"].BaseValue );

            player.Accessories.Add( gear );

            Assert.IsTrue( player["Intellect"].Value == (player["Intellect"].BaseValue / 2) );
        }

        [TestMethod]
        public void CallingApplyInstantOnEffectDoesNotAddItToEffectsCollection()
        {
            GameEntity player = new GameEntity();
            var property = new StatisticProperty( "Intellect", 100 );
            player.AddProperty( property );

            StatisticModifier effect = StatisticModifierFactory.CreateFromFullString( "+50 Intellect" );
            Assert.IsTrue( player.HasPersistentEffect( effect ) == false );
            Assert.IsTrue( effect.IsPersisted == false );
            effect.ApplyInstant( player );
            Assert.IsTrue( player.HasPersistentEffect( effect ) == false );
            Assert.IsTrue( effect.IsPersisted == false );
            Assert.IsTrue( player[ "Intellect" ].Value == 150 );
        }

        [TestMethod]
        public void CallingApplyPersistOnEffectAddsItToEffectsCollection()
        {
            GameEntity player = new GameEntity();
            var property = new StatisticProperty( "Intellect", 100 );
            player.AddProperty( property );

            StatisticModifier effect = StatisticModifierFactory.CreateFromFullString( "+50 Intellect" );
            Assert.IsTrue( player.HasPersistentEffect( effect ) == false );
            Assert.IsTrue( effect.IsPersisted == false );
            effect.ApplyPersist( player );
            Assert.IsTrue( player.HasPersistentEffect( effect ) == true );
            Assert.IsTrue( effect.IsPersisted == true );
            Assert.IsTrue( player[ "Intellect" ].Value == 150 );
        }

        [TestMethod]
        public void CallingUnapplyOnEffectRemovesItFromEffectsCollection()
        {
            GameEntity player = new GameEntity();
            var property = new StatisticProperty( "Intellect", 100 );
            player.AddProperty( property );

            StatisticModifier effect = StatisticModifierFactory.CreateFromFullString( "+50 Intellect" );

            effect.ApplyPersist( player );
            Assert.IsTrue( player.HasPersistentEffect( effect ) == true );
            Assert.IsTrue( effect.IsPersisted == true );
            Assert.IsTrue( player[ "Intellect" ].Value == 150 );

            effect.Unapply();
            Assert.IsTrue( player.HasPersistentEffect( effect ) == false );
            Assert.IsTrue( effect.IsPersisted == false );
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenEquippingAccessories
    {
        [TestMethod]
        public void EquippingItemAppliesItsEffects()
        {
            //Create a player with 100 hp
            GameEntity player = new GameEntity();
            player.AddStatistic( "HP", 100 );

            //Create a piece of armor with +50 hp

            StatisticModifier modifyHpEffect = StatisticModifierFactory.CreateFromFullString( "+50 HP" );

            GameEntity armor = new GameEntity();
            armor.AddProperty( new ApplyEffectToParentProperty( "OnEquip_ModifyParentHP", modifyHpEffect ) );

            Assert.IsFalse( player.HasPersistentEffect( modifyHpEffect ) );
            player.Accessories.Add( armor );
            Assert.IsTrue( player.HasPersistentEffect( modifyHpEffect ) );
        }

        [TestMethod]
        public void UnequippingItemRemovesItsEffects()
        {
            //Create a player with 100 hp
            GameEntity player = new GameEntity();
            player.AddStatistic( "HP", 100 );

            //Create a piece of armor with +50 hp
            StatisticModifier modifyHpEffect = StatisticModifierFactory.CreateFromFullString( "+50 HP" );

            GameEntity armor = new GameEntity();
            armor.AddProperty( new ApplyEffectToParentProperty( "OnEquip_ModifyParentHP", modifyHpEffect ) );

            Assert.IsFalse( player.HasPersistentEffect( modifyHpEffect ) );
            player.Accessories.Add( armor );
            player.Accessories.Remove( armor );
            Assert.IsFalse( player.HasPersistentEffect( modifyHpEffect ) );
        }
    }
}
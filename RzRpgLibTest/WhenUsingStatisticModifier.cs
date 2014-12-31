using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingStatisticModifier
    {
        [TestMethod]
        public void ChangingValueRaisesOnModifierPropertyChangedEvent()
        {
            var mod = StatisticModifierFactory.CreateFromFullString( "+50% Test" );

            bool eventRaised = false;
            mod.PropertyChanged += (s,e) =>
            {
                eventRaised = true;
            };

            Assert.IsFalse( eventRaised );

            mod.Value = 2;

            Assert.IsTrue( eventRaised );
        }

        [TestMethod]
        public void ChangingValueCausesStatisticToUpdate()
        {
            GameEntityProperty stat = new StatisticProperty( "hp", 10 );

            StatisticModifier mod = StatisticModifierFactory.CreateFromFullString( "+10 hp" );

            Assert.IsTrue( stat.Value.Value == 10 );
            stat.Modifiers.Add( mod );
            Assert.IsTrue( stat.Value.Value == 20 );

            mod.Value = 20;
            Assert.IsTrue( stat.Value.Value == 30 );

            mod.Value = 30;
            Assert.IsTrue( stat.Value.Value == 40 );

            stat.Modifiers.Remove( mod );
            Assert.IsTrue( stat.Value.Value == 10 );
        }

        [TestMethod]
        public void ChangingRemovedModifierDoesNotAffectStatistic()
        {
            GameEntityProperty stat = new StatisticProperty( "hp", 10 );

            StatisticModifier mod = StatisticModifierFactory.CreateFromFullString( "+10 hp" );

            Assert.IsTrue( stat.Value.Value == 10 );
            stat.Modifiers.Add( mod );
            stat.Modifiers.Remove( mod );
            Assert.IsTrue( stat.Value.Value == 10 );

            mod.Value = 30;
            Assert.IsTrue( stat.Value.Value == 10 );
        }

        [TestMethod]
        public void CallingModifyAddsTheEffectToTheModifierCollection()
        {
            GameEntityProperty stat = new StatisticProperty( "hp", 10 );

            StatisticModifier mod = StatisticModifierFactory.CreateFromFullString( "+10 hp" );

            Assert.IsFalse( stat.Modifiers.Contains( mod ) );

            Assert.IsTrue( stat.Value.Value == 10 );
            mod.Modify( stat );
            Assert.IsTrue( stat.Value.Value == 20 );

            Assert.IsTrue( stat.Modifiers.Contains( mod ) );
        }

        [TestMethod]
        public void NetEffectForAddingZeroValueIsNone()
        {
            StatisticModifier mod = new StatisticModifier( "hp", 0 );
            Assert.IsTrue( mod.NetEffect == NetStatisticModifierEffect.None );
        }

        [TestMethod]
        public void NetEffectForAddingPositiveValueIsPositive()
        {
            StatisticModifier mod = new StatisticModifier( "hp", 5 );
            Assert.IsTrue( mod.NetEffect == NetStatisticModifierEffect.Positive );
        }

        [TestMethod]
        public void NetEffectForAddingNegativeValueIsNegative()
        {
            StatisticModifier mod = new StatisticModifier( "hp", -5 );
            Assert.IsTrue( mod.NetEffect == NetStatisticModifierEffect.Negative );
        }

        [TestMethod]
        public void NetEffectForMultiplyingZeroIsNone()
        {
            StatisticModifier mod = new StatisticModifier( "hp", 0 ) { Type = ModifierOperation.Multiply };
            Assert.IsTrue( mod.NetEffect == NetStatisticModifierEffect.None );
        }

        [TestMethod]
        public void NetEffectForMultiplyingPositiveIsPositive()
        {
            StatisticModifier mod = new StatisticModifier( "hp", 01.1f ) { Type = ModifierOperation.Multiply };
            Assert.IsTrue( mod.NetEffect == NetStatisticModifierEffect.Positive );
        }

        [TestMethod]
        public void NetEffectForMultiplyingNegativeIsNegative()
        {
            StatisticModifier mod = new StatisticModifier( "hp", -0.1f ) { Type = ModifierOperation.Multiply };
            Assert.IsTrue( mod.NetEffect == NetStatisticModifierEffect.Negative );
        }

        [TestMethod]
        public void StrengthEqualsModifierFlatAmount()
        {
            StatisticModifier mod = new StatisticModifier( "hp", 100 );
            Assert.IsTrue( mod.Strength == 100 );
        }
    }
}
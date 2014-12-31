using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingStatistic
    {
        [TestMethod]
        public void ChangingBaseValueUpdatesValue()
        {
            Statistic stat = new Statistic( 100 );
            Assert.IsTrue( stat.Value == 100 );

            stat.BaseValue = 50;

            Assert.IsTrue( stat.Value == 50 );
        }

        [TestMethod]
        public void ChangingAddTotalUpdatesValue()
        {
            Statistic stat = new Statistic( 100 );
            Assert.IsTrue( stat.Value == 100 );

            stat.AddTotal = 50;

            Assert.IsTrue( stat.Value == 150 );
        }

        [TestMethod]
        public void ChangingMultiplyTotalUpdatesValue()
        {
            Statistic stat = new Statistic( 100 );
            Assert.IsTrue( stat.Value == 100 );

            stat.MultiplyTotal = 2;

            Assert.IsTrue( stat.Value == 200 );
        }

        [TestMethod]
        public void AddValueIsAppliedFirst()
        {
            Statistic stat = new Statistic( 100 );
            Assert.IsTrue( stat.Value == 100 );

            stat.AddTotal = 50;
            stat.MultiplyTotal = 2;

            Assert.IsTrue( stat.Value == 300 );
        }

        [TestMethod]
        public void MinValueCannotExceedMaxValue()
        {
            Statistic stat = new Statistic();
            stat.MaxValue = 5;
            try
            {
                stat.MinValue = 10;
                Assert.Fail( "should not reach here" );
            }
            catch( ArgumentException )
            {
            }
        }

        [TestMethod]
        public void ValueCannotBeBelowMin()
        {
            Statistic stat = new Statistic( 100 )
            {
                MinValue = 0
            };

            stat.BaseValue = -100;
            Assert.IsTrue( stat.Value == 0 );
        }

        [TestMethod]
        public void ValueCannotBeAboveMin()
        {
            Statistic stat = new Statistic( 100 )
            {
                MaxValue = 50
            };

            Assert.IsTrue( stat.Value == 50 );
        }

        [TestMethod]
        public void NewStatisticHasNoNetEffect()
        {
            Statistic stat = new Statistic( 100 );
            Assert.IsTrue( stat.NetEffect == NetStatisticModifierEffect.None );
        }

        [TestMethod]
        public void UnmodifyingStatOfZeroValue()
        {
            StatisticProperty stat = new StatisticProperty( "tenacity", 0 );
            StatisticModifier mod = StatisticModifierFactory.CreateFromFullString( "+30% tenacity" );
            stat.Modifiers.Add( mod );
            Assert.IsTrue( stat.Value.NetEffect == NetStatisticModifierEffect.Neutral );

            stat.Modifiers.Remove( mod );
            Assert.IsTrue( stat.Value.NetEffect == NetStatisticModifierEffect.None );
        }
    }
}
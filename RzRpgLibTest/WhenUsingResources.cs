using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingResources
    {
        private Resource BuildResourceWithCooldown( float cooldown = 1000 )
        {
            var res = new Resource();
            res.Cooldown.Period.BaseValue = cooldown;
            return res;
        }

        [TestMethod]
        public void DefaultResourceHasExpectedDefaultValues()
        {
            Resource res = new Resource();
            Assert.IsTrue( res.Cooldown.Period.Value == 0 );
            Assert.IsTrue( res.IsAvailable );
            Assert.IsTrue( res.Cooldown.RemainingCooldown == 0 );
        }

        [TestMethod]
        public void UsingDefaultResourceWorks()
        {
            Resource res = new Resource();
            Assert.IsTrue( res.Use() );
        }

        [TestMethod]
        public void UsingCooldownResourcePlacesTheResourceOnCooldown()
        {
            Resource res = BuildResourceWithCooldown();

            Assert.IsTrue( res.IsAvailable == true );
            Assert.IsTrue( res.Use() );
            Assert.IsTrue( res.IsAvailable == false );
        }

        [TestMethod]
        public void ResourceIsAvailableWhenCooldownIsAvailable()
        {
            Resource res = BuildResourceWithCooldown();

            Assert.IsTrue( res.Use() );
            Assert.IsTrue( res.IsAvailable == false );
            Assert.IsTrue( res.Cooldown.IsAvailable == false );
            res.Cooldown.Update( new RzAspects.UpdateTime()
            {
                TotalTime = 2000,
                ElapsedTime = 2000
            } );
            Assert.IsTrue( res.IsAvailable == true );
        }

        [TestMethod]
        public void SettingCurrentValueThatExceedsMaxValueResultsInMaxValue()
        {
            Resource res = new Resource();
            res.MaxValue.BaseValue = 100;

            res.CurrentValue = 200;
            Assert.IsTrue( res.Value == res.MaxValue.Value );
        }

        [TestMethod]
        public void SettingCurrentValueThatExceedsMinValueResultsInMinValue()
        {
            Resource res = new Resource();
            res.MinValue.BaseValue = -10;

            res.CurrentValue = -20;
            Assert.IsTrue( res.Value == res.MinValue.Value );
        }

        [TestMethod]
        public void IncreasingMaxValueIncreasesCurrentValue()
        {
            Resource res = new Resource();
            res.MaxValue.BaseValue = 100;
            res.CurrentValue = 90;

            Assert.IsTrue( res.Value == 90 );

            res.MaxValue.BaseValue = 150;

            Assert.IsTrue( res.Value == 140 );
        }

        [TestMethod]
        public void ReducingMaxValueTruncatesCurrentValue()
        {
            Resource res = new Resource();
            res.MaxValue.BaseValue = 100;
            res.CurrentValue = 100;
            
            Assert.IsTrue( res.Value == 100 );

            res.MaxValue.BaseValue = 50;

            Assert.IsTrue( res.Value == 50 );
        }
    }
}
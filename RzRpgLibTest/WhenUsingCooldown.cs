using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingCooldown
    {
        [TestMethod]
        public void DefaultInstanceHasExpectedDefaultValues()
        {
            Cooldown cd = new Cooldown();
            Assert.IsTrue( cd.Period.Value == 0 );
            Assert.IsTrue( cd.IsAvailable );
            Assert.IsTrue( cd.RemainingCooldown == 0 );
        }

        [TestMethod]
        public void UsingDefaultCooldownWorks()
        {
            Cooldown cd = new Cooldown();
            Assert.IsTrue( cd.Use() );
        }

        [TestMethod]
        public void UsingCooldownWithNonzeroPeriodMakesItUnavailable()
        {
            Cooldown cd = new Cooldown( 1 );

            Assert.IsTrue( cd.IsAvailable == true );
            Assert.IsTrue( cd.Use() );
            Assert.IsTrue( cd.IsAvailable == false );
        }

        [TestMethod]
        public void UpdatingReducesRemainingCooldown()
        {
            float cooldown = 5000;
            Cooldown cd = new Cooldown( cooldown );

            Assert.IsTrue( cd.Use() );
            Assert.IsTrue( cd.RemainingCooldown == cooldown );

            //If 2000 ms elapses, the remaining cooldown should = 5000 - 2000 (3000 ms)
            cd.Update( new UpdateTime() { ElapsedTime = 2000, TotalTime = 2000 } );
            Assert.IsTrue( cd.RemainingCooldown == 3000 );
        }

        [TestMethod]
        public void UpdatingWithTimeGreaterOrEqualThanCooldownCausesItToBecomeAvailable()
        {
            float cooldown = 5000;
            Cooldown cd = new Cooldown( cooldown );

            Assert.IsTrue( cd.Use() );

            Assert.IsTrue( cd.IsAvailable == false );
            cd.Update( new UpdateTime() { ElapsedTime = cooldown, TotalTime = cooldown } );

            Assert.IsTrue( cd.IsAvailable == true );
        }
    }
}
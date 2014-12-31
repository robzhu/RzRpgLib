using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenInheritingGameEntityEffect
    {
        class MockEffect : GameEntityEffect
        {
            public Action<IGameEntity> ApplyInternalCallback { get; set; }
            public Action<IGameEntity> UnapplyCallback { get; set; }

            public MockEffect( object source = null ): base( null, source ) { }

            protected override void ApplyPersistInternal(IGameEntity target)
            {
                if (null != ApplyInternalCallback) ApplyInternalCallback( target );
            }

            protected override void ApplyInstantInternal( IGameEntity target )
            {
            }

            protected override void UnapplyInternal(IGameEntity target)
            {
                if (null != UnapplyCallback) UnapplyCallback( target );
            }
        }

        [TestMethod]
        public void AddingEffectToEntityCallsApply()
        {
            GameEntity player = new GameEntity();

            bool applyCalled = false;
            MockEffect effect = new MockEffect( "" )
            {
                ApplyInternalCallback = (target) =>
                    {
                        applyCalled = true;
                    }
            };

            Assert.IsFalse( applyCalled );
            player.AddPersistentEffect( effect );
            Assert.IsTrue( applyCalled );
        }

        [TestMethod]
        public void RemovingEffectFromEntityCallsUnapply()
        {
            GameEntity player = new GameEntity();

            bool unapplyCalled = false;
            MockEffect effect = new MockEffect( "" )
            {
                UnapplyCallback = (target) =>
                {
                    unapplyCalled = true;
                }
            };

            Assert.IsFalse( unapplyCalled );
            player.AddPersistentEffect( effect );
            player.RemovePersistentEffect( effect );
            Assert.IsTrue( unapplyCalled );
        }
    }
}
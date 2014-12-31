using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingCompositeGameEntityEffect
    {
        class MockEffect : GameEntityEffect
        {
            public Action<IGameEntity> ApplyInternalCallback { get; set; }
            public Action<IGameEntity> UnapplyCallback { get; set; }

            public MockEffect( object source = null ) : base( null, source ) { }

            protected override void ApplyPersistInternal( IGameEntity target )
            {
                if( null != ApplyInternalCallback ) ApplyInternalCallback( target );
            }

            protected override void ApplyInstantInternal( IGameEntity target )
            {
            }

            protected override void UnapplyInternal( IGameEntity target )
            {
                if( null != UnapplyCallback ) UnapplyCallback( target );
            }
        }

        [TestMethod]
        public void AddingEffectToEntityCallsApplyOnChildren()
        {
            GameEntity player = new GameEntity();

            bool applyCalledOnChild1 = false;
            MockEffect childEffect1 = new MockEffect( "" )
            {
                ApplyInternalCallback = ( target ) =>
                {
                    applyCalledOnChild1 = true;
                }
            };

            bool applyCalledOnChild2 = false;
            MockEffect childEffect2 = new MockEffect( "" )
            {
                ApplyInternalCallback = ( target ) =>
                {
                    applyCalledOnChild2 = true;
                }
            };

            CompositeGameEntityEffect compositeEffect = new CompositeGameEntityEffect( childEffect1, childEffect2 );

            Assert.IsFalse( applyCalledOnChild1 );
            Assert.IsFalse( applyCalledOnChild2 );
            player.AddPersistentEffect( compositeEffect );
            Assert.IsTrue( applyCalledOnChild1 );
            Assert.IsTrue( applyCalledOnChild2 );
        }

        [TestMethod]
        public void RemovingEffectFromEntityCallsUnapplyOnChildren()
        {
            GameEntity player = new GameEntity();

            bool unapplyCalledOnChild1 = false;
            MockEffect childEffect1 = new MockEffect( "" )
            {
                UnapplyCallback = ( target ) =>
                {
                    unapplyCalledOnChild1 = true;
                }
            };

            bool unapplyCalledOnChild2 = false;
            MockEffect childEffect2 = new MockEffect( "" )
            {
                UnapplyCallback = ( target ) =>
                {
                    unapplyCalledOnChild2 = true;
                }
            };

            CompositeGameEntityEffect compositeEffect = new CompositeGameEntityEffect( childEffect1, childEffect2 );

            Assert.IsFalse( unapplyCalledOnChild1 );
            Assert.IsFalse( unapplyCalledOnChild2 );
            player.AddPersistentEffect( compositeEffect );
            player.RemovePersistentEffect( compositeEffect );
            Assert.IsTrue( unapplyCalledOnChild1 );
            Assert.IsTrue( unapplyCalledOnChild2 );
        }

        [TestMethod]
        public void AddingCompositeEffectAddsCompositeEffectAndChildEffects()
        {
            GameEntity entity = new GameEntity();

            MockEffect childEffect1 = new MockEffect( "" );
            MockEffect childEffect2 = new MockEffect( "" );
            CompositeGameEntityEffect compositeEffect = new CompositeGameEntityEffect( childEffect1, childEffect2 );

            entity.AddPersistentEffect( compositeEffect );

            Assert.IsTrue( entity.PersistentEffects.Count == 3 );
            Assert.IsTrue( entity.PersistentEffects.Contains( compositeEffect ) );
            Assert.IsTrue( entity.PersistentEffects.Contains( childEffect1 ) );
            Assert.IsTrue( entity.PersistentEffects.Contains( childEffect2 ) );
        }

        [TestMethod]
        public void RemovingCompositeEffectRemovesCompositeEffectAndChildEffects()
        {
            GameEntity entity = new GameEntity();

            MockEffect childEffect1 = new MockEffect( "" );
            MockEffect childEffect2 = new MockEffect( "" );
            CompositeGameEntityEffect compositeEffect = new CompositeGameEntityEffect( childEffect1, childEffect2 );

            entity.AddPersistentEffect( compositeEffect );
            entity.RemovePersistentEffect( compositeEffect );

            Assert.IsTrue( entity.PersistentEffects.Count == 0 );
            Assert.IsTrue( !entity.PersistentEffects.Contains( compositeEffect ) );
            Assert.IsTrue( !entity.PersistentEffects.Contains( childEffect1 ) );
            Assert.IsTrue( !entity.PersistentEffects.Contains( childEffect2 ) );
        }

        [TestMethod]
        public void RemovingPartOfCompositeEffectsWorks()
        {
            GameEntity entity = new GameEntity();

            MockEffect childEffect1 = new MockEffect( "" );
            MockEffect childEffect2 = new MockEffect( "" );
            CompositeGameEntityEffect compositeEffect = new CompositeGameEntityEffect( childEffect1, childEffect2 );

            //Removing a child effect from its parent composite effect should remove it from the parent.
            
            entity.AddPersistentEffect( compositeEffect );
            compositeEffect.Children.Remove( childEffect1 );

            Assert.IsTrue( entity.PersistentEffects.Count == 2 );
            Assert.IsTrue( entity.PersistentEffects.Contains( compositeEffect ) );
            Assert.IsTrue( !entity.PersistentEffects.Contains( childEffect1 ) );
            Assert.IsTrue( entity.PersistentEffects.Contains( childEffect2 ) );
        }

        [TestMethod]
        public void AddingNewChildOfCompositeEffectsWorks()
        {
            GameEntity entity = new GameEntity();

            MockEffect childEffect1 = new MockEffect( "" );
            MockEffect childEffect2 = new MockEffect( "" );
            
            CompositeGameEntityEffect compositeEffect = new CompositeGameEntityEffect( childEffect1, childEffect2 );

            entity.AddPersistentEffect( compositeEffect );
            
            //Adding a new child effect while the compositeEffect is attached should add it to the parent.
            MockEffect childEffect3 = new MockEffect( "" );
            compositeEffect.Children.Add( childEffect3 );

            Assert.IsTrue( entity.PersistentEffects.Count == 4 );
            Assert.IsTrue( entity.PersistentEffects.Contains( compositeEffect ) );
            Assert.IsTrue( entity.PersistentEffects.Contains( childEffect1 ) );
            Assert.IsTrue( entity.PersistentEffects.Contains( childEffect2 ) );
            Assert.IsTrue( entity.PersistentEffects.Contains( childEffect3 ) );
        }
    }
}
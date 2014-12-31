using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingGameEntity
    {
        internal static class Properties
        {
            public static string Name = "Name";
        }

        internal static class MockGameEntityFactory
        {
            public static IGameEntity Create( string name )
            {
                var entity = new GameEntity();

                var prop = new GameEntityProperty( Properties.Name, name );
                entity.AddProperty( prop );

                return entity;
            }
        }

        [TestMethod]
        public void SimplePropertyAccessorsWork()
        {
            var entity = MockGameEntityFactory.Create( "meow" );
            Assert.AreEqual( entity[ Properties.Name ], "meow" );
        }

        [TestMethod]
        public void AddingNewPropertyThrowsPropertyBindingException()
        {
            var entity = MockGameEntityFactory.Create( "meow" );

            try
            {
                entity.AddProperty( new GameEntityProperty( Properties.Name, "woof" ) );
                Assert.Fail();
            }
            catch( GameEntityPropertyBindingException ) { }
        }

        [TestMethod]
        public void AddingNewPropertyRaisesCollectionProperChangedEvent()
        {
            var entity = new GameEntity();

            var propertyToAdd = new GameEntityProperty( Properties.Name, "meow" );
            IGameEntityProperty addedProperty = null;
            GameEntityPropertyCollectionChangedAction changeAction = GameEntityPropertyCollectionChangedAction.Remove;

            entity.GameEntityPropertyCollectionChanged += ( args ) =>
            {
                changeAction = args.Action;
                addedProperty = args.ChangedProperty;
            };

            entity.AddProperty( propertyToAdd );

            Assert.AreEqual( GameEntityPropertyCollectionChangedAction.Add, changeAction );
            Assert.AreEqual( propertyToAdd, addedProperty );
        }

        [TestMethod]
        public void RemovingPropertyRaisesCollectionProperChangedEvent()
        {
            var entity = new GameEntity();

            var propertyToAdd = new GameEntityProperty( Properties.Name, "meow" );
            entity.AddProperty( propertyToAdd );

            IGameEntityProperty removedProperty = null;
            GameEntityPropertyCollectionChangedAction changeAction = GameEntityPropertyCollectionChangedAction.Remove;

            entity.GameEntityPropertyCollectionChanged += ( args ) =>
            {
                changeAction = args.Action;
                removedProperty = args.ChangedProperty;
            };

            Assert.IsTrue( entity.RemoveProperty( propertyToAdd.Name ) );

            Assert.AreEqual( GameEntityPropertyCollectionChangedAction.Remove, changeAction );
            Assert.AreEqual( propertyToAdd, removedProperty );
        }

        [TestMethod]
        public void AddingBoundPropertyThrowsPropertyBindingException()
        {
            var e1 = new GameEntity();
            var e2 = new GameEntity();

            var propertyToAdd = new GameEntityProperty( Properties.Name, "meow" );

            e1.AddProperty( propertyToAdd );

            try
            {
                e2.AddProperty( propertyToAdd );
                Assert.Fail();
            }
            catch( GameEntityPropertyBindingException ) { }
        }

        [TestMethod]
        public void AddingPropertySetsParent()
        {
            var entity = new GameEntity();

            var propertyToAdd = new GameEntityProperty( Properties.Name, "meow" );

            Assert.AreEqual( null, propertyToAdd.Parent );
            entity.AddProperty( propertyToAdd );
            Assert.AreEqual( entity, propertyToAdd.Parent );
        }

        [TestMethod]
        public void RemoveNonExistentPropertyByNameReturnsFalse()
        {
            var entity = new GameEntity();
            Assert.IsFalse( entity.RemoveProperty( "meow" ) );
        }

        [TestMethod]
        public void RemoveNonExistentPropertyByReferenceReturnsFalse()
        {
            var entity = new GameEntity();
            var prop = new GameEntityProperty( Properties.Name, "meow" );
            Assert.IsFalse( entity.RemoveProperty( prop ) );
        }

        [TestMethod]
        public void RemoveNonMatchingExistentPropertyByReferenceReturnsFalse()
        {
            var entity = new GameEntity();
            var prop = new GameEntityProperty( Properties.Name, "meow" );
            var prop2 = new GameEntityProperty( Properties.Name, "meow" );

            entity.AddProperty( prop );
            Assert.IsFalse( entity.RemoveProperty( prop2 ) );
        }

        [TestMethod]
        public void ChangingAddedPropertyRaisesItemChangeCallbackOnEntity()
        {
            var entity = new GameEntity();
            var prop = new GameEntityProperty( Properties.Name, "meow" );

            bool handlerCalled = false;
            entity.PropertyPropertyChanged += (item, e) =>
                {
                    if( e.PropertyName == "Value" )
                        handlerCalled = true;
                };

            entity.AddProperty( prop );

            Assert.IsFalse( handlerCalled );

            prop.Value = "woof";

            Assert.IsTrue( handlerCalled );
        }

        [TestMethod]
        public void ChangingRemovedPropertyDoesNotRaiseItemChangeCallbackOnEntity()
        {
            var entity = new GameEntity();
            var prop = new GameEntityProperty( Properties.Name, "meow" );

            bool handlerCalled = false;
            entity.PropertyPropertyChanged += (item, e) =>
            {
                if (e.PropertyName == "Value")
                    handlerCalled = true;
            };

            entity.AddProperty( prop );
            entity.RemoveProperty( prop );

            Assert.IsFalse( handlerCalled );

            prop.Value = "woof";

            Assert.IsFalse( handlerCalled );
        }

        [TestMethod]
        public void RemovingPropertySetsParentToNull()
        {
            var entity = new GameEntity();

            var prop = new GameEntityProperty( Properties.Name, "meow" );

            entity.AddProperty( prop );
            entity.RemoveProperty( prop );

            Assert.AreEqual( null, prop.Parent );
        }
    }
}
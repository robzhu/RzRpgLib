using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingGameEntityProperty
    {
        class MockPropertyValue : ModelBase
        {
            public string PropertyMaxValue { get { return "MaxValue"; } }
            private float _MaxValue;
            public float MaxValue
            {
                get { return _MaxValue; }
                set { SetProperty( PropertyMaxValue, ref _MaxValue, value ); }
            }

            public string PropertyCurrentValue { get { return "CurrentValue"; } }
            private float _CurrentValue;
            public float CurrentValue
            {
                get { return _CurrentValue; }
                set { SetProperty( PropertyCurrentValue, ref _CurrentValue, value ); }
            }
        }

        class MockProperty : GameEntityProperty
        {
            public Action ResetCallback { get; set; }

            public override void Reset()
            {
                base.Reset();
                if (ResetCallback != null) ResetCallback();
            }

            public MockProperty( string name, object value ) : base( name, value ) { }
        }

        [TestMethod]
        public void ValueChangeNotificationsWork()
        {
            MockPropertyValue val = new MockPropertyValue();
            GameEntityProperty prop = new GameEntityProperty( "Mock", val );

            bool valuePropertyChangedCalled = false;
            prop.ValuePropertyChanged += (s, e) =>
                {
                    valuePropertyChangedCalled = true;
                };

            Assert.IsFalse( valuePropertyChangedCalled );

            val.MaxValue = 10;

            Assert.IsTrue( valuePropertyChangedCalled );
        }

        [TestMethod]
        public void AddingCleanPropertyToEntityWithExistingEffectsPicksUpThoseProperties()
        {
            //The player has a +50 HP effect but no "Rage" property.  When the "Rage" property is added, it should automatically pick up whatever effects are there.
            string propName = "Rage";
            GameEntity player = new GameEntity();
            StatisticModifier effect = new StatisticModifier( propName )
            {
                Value = 50,
                Type = ModifierOperation.Add
            };

            player.AddPersistentEffect( effect );

            GameEntityProperty property = new StatisticProperty( propName, 100 );

            Assert.IsTrue( property.Value.Value == property.Value.BaseValue );
            player.AddProperty( property );
            Assert.IsTrue( property.Value.Value == 150 );
        }

        [TestMethod]
        public void RemovingPropertyFromEntityCallsReset()
        {
            //The player has a +50 HP effect but no "Rage" property.  When the "Rage" property is added, it should automatically pick up whatever effects are there.
            GameEntity player = new GameEntity();

            MockProperty property = new MockProperty( "HP", new Statistic() { BaseValue = 100 } );
            bool resetCalled = false;
            property.ResetCallback = () => { resetCalled = true; };

            player.AddProperty( property );

            Assert.IsFalse( resetCalled );

            player.RemoveProperty( property );

            Assert.IsTrue( resetCalled );
        }

        [TestMethod]
        public void UpdateDescriptionIsCalledWhenFirstSet()
        {
            GameEntityProperty property = new GameEntityProperty( "HP", new Statistic() { BaseValue = 100 } );

            bool updateCalled = false;
            property.UpdateDescription = ( gep ) =>
            {
                updateCalled = true;
                return null;
            };

            Assert.IsTrue( updateCalled );
        }


        [TestMethod]
        public void UpdateDescriptionIsCalledWhenValueChanges()
        {
            GameEntityProperty property = new GameEntityProperty( "HP", new Statistic() { BaseValue = 100 } );

            bool updateCalled = false;
            property.UpdateDescription = ( gep ) =>
                {
                    updateCalled = true;
                    return null;
                };

            updateCalled = false;

            property.Value.AddTotal = 10;

            Assert.IsTrue( updateCalled );
        }
    }
}
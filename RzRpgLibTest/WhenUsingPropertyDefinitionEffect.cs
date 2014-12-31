using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingPropertyDefinitionEffect
    {
        [TestMethod]
        public void ApplyingEffectAddsProperty()
        {
            string propName = "HP";
            PropertyDefinitionEffect effect = new PropertyDefinitionEffect(
                () => { return new StatisticProperty( propName, 100 ); } );

            GameEntity player = new GameEntity();

            Assert.IsTrue( player[propName] == null );
            effect.ApplyPersist( player );
            Assert.IsTrue( player.GetProperty( propName ) != null );
        }

        [TestMethod]
        public void UnapplyingEffectRemovesProperty()
        {
            string propName = "HP";
            PropertyDefinitionEffect effect = new PropertyDefinitionEffect( 
                () => { return new StatisticProperty( propName, 100 ); } );

            GameEntity player = new GameEntity();

            Assert.IsTrue( player[propName] == null );
            effect.ApplyPersist( player );
            effect.Unapply();
            Assert.IsTrue( player[propName] == null );
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingGameEntityActor
    {
        [TestMethod]
        public void AddingAbilityWithResourceRequirementFindsResourceReference()
        {
            var actor = new GameEntityActor();
            var resource = ResourceProperty.FromFriendlyString( "50/50 Mana" );
            actor.AddProperty( resource );

            var ability = new GameEntityAction(){ Name = "Fireball", };
            var requirement = new ResourceRequirement( "Mana", 10 );
            ability.Requirements.Add( requirement );

            Assert.IsTrue( requirement.Resource == null );

            actor.Abilities.Add( ability );

            Assert.IsTrue( requirement.Resource == resource );
        }

        [TestMethod]
        public void RemovingAbilityResetsResourceReference()
        {
            var actor = new GameEntityActor();
            var resource = ResourceProperty.FromFriendlyString( "50/50 Mana" );
            actor.AddProperty( resource );

            var ability = new GameEntityAction() { Name = "Fireball", };
            var requirement = new ResourceRequirement( "Mana", 10 );
            ability.Requirements.Add( requirement );

            actor.Abilities.Add( ability );

            Assert.IsTrue( requirement.Resource == resource );

            actor.Abilities.Remove( ability );

            Assert.IsTrue( requirement.Resource == null );
        }
    }
}
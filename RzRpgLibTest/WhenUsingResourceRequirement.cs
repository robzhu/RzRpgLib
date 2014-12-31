using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingResourceRequirement
    {
        [TestMethod]
        public void ChangingResourceValueChangesCanExecute()
        {
            ResourceProperty resource = new ResourceProperty( "mana", 100 );
            ResourceRequirement requirement = new ResourceRequirement()
            {
                 Resource = resource,
            };
            requirement.Quantity.BaseValue = 50;

            Assert.IsTrue( requirement.RequirementMet == true );

            resource.Value.CurrentValue = 20;

            Assert.IsTrue( requirement.RequirementMet == false );

            resource.Value.CurrentValue = 60;

            Assert.IsTrue( requirement.RequirementMet == true );
        }

        [TestMethod]
        public void ChangingResourceIsAvailableChangesCanExecute()
        {
            ResourceProperty resource = new ResourceProperty( "mana", 100 );
            ResourceRequirement requirement = new ResourceRequirement()
            {
                Resource = resource,
            };
            requirement.Quantity.BaseValue = 50;

            Assert.IsTrue( requirement.RequirementMet == true );

            resource.Value.IsAvailable = false;

            Assert.IsTrue( requirement.RequirementMet == false );

            resource.Value.IsAvailable = true;

            Assert.IsTrue( requirement.RequirementMet == true );
        }
    }
}
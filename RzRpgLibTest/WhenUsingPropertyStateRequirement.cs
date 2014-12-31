using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingPropertyStateRequirement
    {
        [TestMethod]
        public void ChangingPropertyValueChangesRequirementMet()
        {
            GameEntityProperty booleanProperty = new GameEntityProperty( "Flag", false );

            PropertyStateRequirement stateRequirement = new PropertyStateRequirement()
            {
                TargetProperty = booleanProperty,
                RequiredState = true,
            };

            Assert.IsTrue( stateRequirement.RequirementMet == false );

            booleanProperty.Value = true;

            Assert.IsTrue( stateRequirement.RequirementMet == true );
        }

        [TestMethod]
        public void ChangingRequiredStateChangesRequirementMet()
        {
            GameEntityProperty booleanProperty = new GameEntityProperty( "Flag", false );

            PropertyStateRequirement stateRequirement = new PropertyStateRequirement()
            {
                TargetProperty = booleanProperty,
                RequiredState = false,
            };

            Assert.IsTrue( stateRequirement.RequirementMet == true );

            stateRequirement.RequiredState = true;

            Assert.IsTrue( stateRequirement.RequirementMet == false );
        }

        [TestMethod]
        public void SettingTargetPropertyChangesRequirementMet()
        {
            GameEntityProperty booleanProperty = new GameEntityProperty( "Flag", true );

            PropertyStateRequirement stateRequirement = new PropertyStateRequirement()
            {
                RequiredState = true,
            };

            Assert.IsTrue( stateRequirement.RequirementMet == false );

            stateRequirement.TargetProperty = booleanProperty;

            Assert.IsTrue( stateRequirement.RequirementMet == true );
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RzAspects;
using RzRpgLib;

namespace RzRpgLibTest
{
    [TestClass]
    public class WhenUsingGameEntityAction
    {
        class MockRequirement : ModelBase, IActionRequirement
        {
            public string PropertyRequirementMet { get { return "RequirementMet"; } }
            private bool _RequirementMet;
            public bool RequirementMet
            {
                get { return _RequirementMet; }
                set { SetProperty( PropertyRequirementMet, ref _RequirementMet, value ); }
            }

            public string ErrorString { get; set; }
            public void Use() { }

            public bool RefreshRequirementMet()
            {
                return true;
            }
        }

        [TestMethod]
        public void DefaultGameEntityActionValuesWork()
        {
            GameEntityAction action = new GameEntityAction();
            Assert.IsTrue( action.Requirements.IsEmpty == false );
            Assert.IsTrue( action.CanExecute == true );
        }

        [TestMethod]
        public void AddingUnmetRequirementChangesCanExecuteToFalse()
        {
            GameEntityAction action = new GameEntityAction();

            var requirement = new MockRequirement() { RequirementMet = false };

            Assert.IsTrue( action.CanExecute == true );
            action.Requirements.Add( requirement );
            Assert.IsTrue( action.CanExecute == false );
        }

        [TestMethod]
        public void RemovingUnmetRequirementChangesCanExecuteToTrue()
        {
            GameEntityAction action = new GameEntityAction();

            var requirement = new MockRequirement() { RequirementMet = false };

            action.Requirements.Add( requirement );
            Assert.IsTrue( action.CanExecute == false );
            action.Requirements.Remove( requirement );
            Assert.IsTrue( action.CanExecute == true );
        }

        [TestMethod]
        public void ChangingOnlyRequirementMetToFalseCausesCanExecuteToFalse()
        {
            GameEntityAction action = new GameEntityAction();

            var requirement = new MockRequirement() { RequirementMet = true };

            action.Requirements.Add( requirement );
            Assert.IsTrue( action.CanExecute == true );
            requirement.RequirementMet = false;
            Assert.IsTrue( action.CanExecute == false );
        }

        [TestMethod]
        public void ChangingOneOfRequirementMetToFalseCausesCanExecuteToFalse()
        {
            GameEntityAction action = new GameEntityAction();

            action.Requirements.Add( new MockRequirement() { RequirementMet = true } );
            action.Requirements.Add( new MockRequirement() { RequirementMet = true } );

            var requirement = new MockRequirement() { RequirementMet = true };

            action.Requirements.Add( requirement );
            Assert.IsTrue( action.CanExecute == true );
            requirement.RequirementMet = false;
            Assert.IsTrue( action.CanExecute == false );
        }

        [TestMethod]
        public void SetCooldownWorks()
        {
            GameEntityAction action = new GameEntityAction();

            //actions have an implicit CD requirement 
            Assert.IsTrue( action.Requirements.Count == 1 );
            Assert.IsTrue( action.InternalCooldown.Period.Value == 0 );

            action.SetCooldown( 100 );

            Assert.IsTrue( action.InternalCooldown.Period.Value == 100 );
            Assert.IsTrue( action.Requirements.Count == 1 );

            CooldownRequirement requirement = action.Requirements[ 0 ] as CooldownRequirement;
            Assert.IsTrue( requirement is CooldownRequirement );
        }
    }
}
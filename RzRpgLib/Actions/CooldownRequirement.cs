using System.ComponentModel;

namespace RzRpgLib
{
    /// <summary>
    /// Interface for a requirement that uses a cooldown.  
    /// Ex: a spell shares a cooldown with other spells of the same tree.
    /// </summary>
    public interface ICooldownRequirement : IActionRequirement
    {
        /// <summary>
        /// The cooldown that this action requires. 
        /// </summary>
        ICooldown Cooldown { get; set; }
    }

    public class CooldownRequirement : ActionRequirement, ICooldownRequirement
    {
        public string PropertyCooldown { get { return "Cooldown"; } }
        private ICooldown _Cooldown;
        public ICooldown Cooldown
        {
            get { return _Cooldown; }
            set { SetProperty( PropertyCooldown, ref _Cooldown, value ); }
        }

        public CooldownRequirement( ICooldown cooldown = null )
        {
            Cooldown = cooldown;
        }

        public override bool RefreshRequirementMet()
        {
            if( Cooldown == null ) return true;

            return Cooldown.IsAvailable;
        }

        public override void Use()
        {
            Cooldown.Use();
        }
    }
}

using RzAspects;
using System.ComponentModel;

namespace RzRpgLib
{
    /// <summary>
    /// Interface for an action requirement.  Actions can specify a set of requirements that 
    /// must be met before it can be executed.
    /// </summary>
    public interface IActionRequirement : ICompositeProperty
    {
        /// <summary>
        /// Gets whether this requirement has been met.
        /// </summary>
        bool RequirementMet { get; }

        /// <summary>
        /// Gets the error string associated with this Action if RequirementMet is false.
        /// </summary>
        string ErrorString { get; }

        bool RefreshRequirementMet();

        /// <summary>
        /// Uses the specified requirement.  
        /// </summary>
        void Use();
    }

    public abstract class ActionRequirement : CompositePropertyChangeNotificationBase, IActionRequirement
    {
        public string PropertyRequirementMet { get { return "RequirementMet"; } }
        private bool _RequirementMet;
        public bool RequirementMet
        {
            get { return _RequirementMet; }
            private set { SetProperty( PropertyRequirementMet, ref _RequirementMet, value ); }
        }

        public string ErrorString { get; protected set; }

        protected ActionRequirement()
        {
            //Whenever a property or desendent property on this class changes, refresh whether the requirement is met.
            PropertyChanged += OnPropertyChanged;
            DescendentPropertyChanged += OnPropertyChanged;
        }

        public abstract void Use();

        public abstract bool RefreshRequirementMet();

        private void OnPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            RequirementMet = RefreshRequirementMet();
        }        
    }
}

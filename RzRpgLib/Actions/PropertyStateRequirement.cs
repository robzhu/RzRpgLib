using System.ComponentModel;

namespace RzRpgLib
{
    /// <summary>
    /// Interface for a requirement is met if the entity's specific property is of a specific value.
    /// Ex: a spell requires 50 mana to case.
    /// </summary>
    public interface IPropertyStateRequirement : IActionRequirement
    {
        /// <summary>
        /// The name of the resource to bind to.
        /// </summary>
        string PropertyName { get; set; }

        /// <summary>
        /// The property on which this requirement acts.
        /// </summary>
        IGameEntityProperty TargetProperty{ get; set;}

        /// <summary>
        /// The state that the property must be in
        /// </summary>
        object RequiredState { get; }
    }

    public class PropertyStateRequirement : ActionRequirement, IPropertyStateRequirement
    {
        public string PropertyPropertyName { get { return "PropertyName"; } }
        private string _PropertyName;
        public string PropertyName
        {
            get{ return _PropertyName; }
            set{ SetProperty( PropertyPropertyName, ref _PropertyName, value ); }
        }

        public string PropertyTargetProperty { get { return "TargetProperty"; } }
        private IGameEntityProperty _TargetProperty;
        public IGameEntityProperty TargetProperty
        {
            get { return _TargetProperty; }
            set { SetProperty( PropertyTargetProperty, ref _TargetProperty, value, () => RefreshRequirementMet() ); }
        }
        
        public string PropertyRequiredState { get { return "RequiredState"; } }
        private object _RequiredState;
        public object RequiredState
        {
            get { return _RequiredState; }
            set { SetProperty( PropertyRequiredState, ref _RequiredState, value, () => RefreshRequirementMet() ); }
        }

        public PropertyStateRequirement() { }

        public PropertyStateRequirement( string targetPropertyName, object requiredState )
        {
            PropertyName = targetPropertyName;
            RequiredState = requiredState;
        }

        public override bool RefreshRequirementMet()
        {
            if( TargetProperty == null ) return false;

            if( RequiredState == null ) return ( TargetProperty.Value == null );
            else return RequiredState.Equals( TargetProperty.Value );
        }

        public override void Use() { }
    }
}

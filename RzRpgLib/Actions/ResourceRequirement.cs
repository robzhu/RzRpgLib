using System.ComponentModel;

namespace RzRpgLib
{
    /// <summary>
    /// Interface for a requirement that uses a resource.  
    /// Ex: a spell requires 50 mana to case.
    /// </summary>
    public interface IResourceRequirement : IActionRequirement
    {
        /// <summary>
        /// The name of the resource to bind to.
        /// </summary>
        string ResourceName { get; set; }

        /// <summary>
        /// The resource that this action requires.  Not valid unless 
        /// </summary>
        IResourceProperty Resource { get; set; }

        /// <summary>
        /// The quantity required of the resource.
        /// </summary>
        IStatistic Quantity { get; set; }
    }

    public class ResourceRequirement : ActionRequirement, IResourceRequirement
    {
        public string PropertyResourceName { get { return "ResourceName"; } }
        private string _ResourceName;
        public string ResourceName
        {
            get { return _ResourceName; }
            set { SetProperty( PropertyResourceName, ref _ResourceName, value ); }
        }

        public string PropertyResource { get { return "Resource"; } }
        private IResourceProperty _Resource = null;
        public IResourceProperty Resource
        {
            get { return _Resource; }
            set { SetProperty( PropertyResource, ref _Resource, value ); }
        }

        public string PropertyQuantity { get { return "Quantity"; } }
        private IStatistic _Quantity = new Statistic();
        public IStatistic Quantity
        {
            get { return _Quantity; }
            set { SetProperty( PropertyQuantity, ref _Quantity, value ); }
        }

        public ResourceRequirement() { }

        public ResourceRequirement( string resourceName, float cost = 0 )
        {
            ResourceName = resourceName;
            Quantity.BaseValue = cost;
            ErrorString = string.Format( "Needs at least {0} {1}", Quantity.Value, ResourceName );
        }

        public override bool RefreshRequirementMet()
        {
            if( Resource == null ) return false;
            if( Resource.Value == null ) return false;

            return ( Resource.Value.IsAvailable && ( Resource.Value.Value >= Quantity.Value ) );
        }

        public override void Use()
        {
            Resource.Value.Use( Quantity.Value );
        }
    }
}

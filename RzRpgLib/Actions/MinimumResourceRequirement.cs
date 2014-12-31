using System.ComponentModel;

namespace RzRpgLib
{
    public class MinimumResourceRequirement : ActionRequirement, IResourceRequirement
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

        public MinimumResourceRequirement( string resourceName, float minimum = 0 )
        {
            ResourceName = resourceName;
            Quantity.BaseValue = minimum;
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
            //this requirement does not actually consume the resource.
        }
    }
}

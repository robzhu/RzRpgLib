using System;

namespace RzRpgLib
{
    /// <summary>
    /// Effect that modifies a resource
    /// </summary>
    public class PeriodicResourceModifier : PropertyModifierEffect<IResource>
    {
        public string PropertyResourcePropertyTarget { get { return "ResourcePropertyTarget"; } }
        private ResourcePropertyTarget _ResourcePropertyTarget = ResourcePropertyTarget.Value;
        public ResourcePropertyTarget ResourcePropertyTarget
        {
            get { return _ResourcePropertyTarget; }
            set { SetProperty( PropertyResourcePropertyTarget, ref _ResourcePropertyTarget, value ); }
        }

        public PeriodicResourceModifier() : base( null, null ) { }
        public PeriodicResourceModifier( string target, float value )
            : base( target, null ) 
        {
            Value = value;
        }

        public override void Modify( IResource target )
        {
            ApplyValue( target );
        }

        public override void Unmodify( IResource target )
        {
            ApplyValue( target, true );
        }

        private void ApplyValue( IResource target, bool invertValue = false )
        {
            float valueToApply = invertValue ? -Value : Value;

            switch( ResourcePropertyTarget )
            {
                case ResourcePropertyTarget.Value:
                    target.CurrentValue += valueToApply;
                    break;
                case ResourcePropertyTarget.Max_AddTotal:
                    target.MaxValue.AddTotal += valueToApply;
                    break;
                case ResourcePropertyTarget.Max_BaseValue:
                    target.MaxValue.BaseValue += valueToApply;
                    break;
                case ResourcePropertyTarget.Max_MultiplyTotal:
                    target.MaxValue.MultiplyTotal += valueToApply;
                    break;

                case ResourcePropertyTarget.Min_AddTotal:
                    target.MaxValue.AddTotal += valueToApply;
                    break;
                case ResourcePropertyTarget.Min_BaseValue:
                    target.MaxValue.BaseValue += valueToApply;
                    break;
                case ResourcePropertyTarget.Min_MultiplyTotal:
                    target.MaxValue.MultiplyTotal += valueToApply;
                    break;
                default:
                    break;
            };
        }
    }
}

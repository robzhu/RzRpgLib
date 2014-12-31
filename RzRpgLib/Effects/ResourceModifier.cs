using System;
using RzAspects;

namespace RzRpgLib
{
    public enum ResourcePropertyTarget
    {
        Value,
        Max_BaseValue,
        Max_AddTotal,
        Max_MultiplyTotal,
        Min_BaseValue,
        Min_AddTotal,
        Min_MultiplyTotal,
    }

    /// <summary>
    /// Effect that modifies a resource
    /// </summary>
    public class ResourceModifier : PropertyModifierEffect<IResource>
    {
        public string PropertyResourcePropertyTarget { get { return "ResourcePropertyTarget"; } }
        private ResourcePropertyTarget _ResourcePropertyTarget = ResourcePropertyTarget.Value;
        public ResourcePropertyTarget ResourcePropertyTarget
        {
            get { return _ResourcePropertyTarget; }
            set { SetProperty( PropertyResourcePropertyTarget, ref _ResourcePropertyTarget, value, RefreshResourcePropertyTarget ); }
        }

        public ResourceModifier() : base( null, null ) { }
        public ResourceModifier( string target, float flatValue, float duration = 0 ) : base( target, null )
        {
            Value = flatValue;

            if ( duration != 0 )
            {
                Duration = new Duration( duration );
            }
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

        private void RefreshResourcePropertyTarget()
        {
            if( ResourcePropertyTarget == ResourcePropertyTarget.Max_MultiplyTotal ) Type = ModifierOperation.Multiply;
            RefreshNetEffect();
        }
    }

    public class AddMaxResourceModifier : ResourceModifier
    {
        public AddMaxResourceModifier( string target, float flatValue, float duration = 0 )
            : base( target, flatValue, duration )
        {
            ResourcePropertyTarget = ResourcePropertyTarget.Max_AddTotal;
        }
    }
}

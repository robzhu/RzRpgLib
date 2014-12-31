using System;
using System.ComponentModel;

namespace RzRpgLib
{
    public enum ModifierOperation : byte
    {
        Add,
        Multiply
    }

    public interface IPropertyModifierEffect : IGameEntityEffect
    {
        event Action<IPropertyModifierEffect> ValueChanging;
        event Action<IPropertyModifierEffect> ValueChanged;
        ModifierOperation Type { get; set; }
        NetStatisticModifierEffect NetEffect { get; }
        float Value { get; set; }
        string DisplayString { get; }

        /// <summary>
        /// Modifies the specified property and, if applicable, adds it to the property's modifier collection.
        /// </summary>
        /// <param name="property"></param>
        void Modify( IGameEntityProperty property );

        /// <summary>
        /// DEVELOPER SHOULD NOT BE CALLING THIS METHOD.
        /// Modifies the specified property without checking for TargetProperty or adding this modifier to the target property's modifier collection.
        /// </summary>
        /// <param name="property"></param>
        void ModifyOnly( IGameEntityProperty property );

        /// <summary>
        /// Unmodifies the specified property and, if applicable, removes it from the target property's modifier collection.
        /// </summary>
        /// <param name="property"></param>
        void Unmodify( IGameEntityProperty property );

        /// <summary>
        /// DEVELOPER SHOULD NOT BE CALLING THIS METHOD.
        /// Unmodifies the specified property without checking for TargetProperty or removing this modifier from the target property's modifier collection.
        /// </summary>
        /// <param name="property"></param>
        void UnmodifyOnly( IGameEntityProperty property );
    }

    public interface IPropertyModifierEffect<T> : IPropertyModifierEffect
    {
        void Modify( T propertyValue );
        void Unmodify( T propertyValue );
    }

    public abstract class PropertyModifierEffect<T> : GameEntityEffect, IPropertyModifierEffect<T>
    {
        public event Action<IPropertyModifierEffect> ValueChanging;
        public event Action<IPropertyModifierEffect> ValueChanged;

        public string PropertyType { get { return "Type"; } }
        private ModifierOperation _Type = ModifierOperation.Add;
        public ModifierOperation Type
        {
            get { return _Type; }
            set { SetProperty( PropertyType, ref _Type, value, RefreshNetEffect ); }
        }

        public string PropertyNetEffect { get { return "NetEffect"; } }
        private NetStatisticModifierEffect _NetEffect = NetStatisticModifierEffect.None;
        public NetStatisticModifierEffect NetEffect
        {
            get { return _NetEffect; }
            private set { SetProperty( PropertyNetEffect, ref _NetEffect, value ); }
        }

        public string PropertyValue { get { return "Value"; } }
        private float _Value;
        public float Value
        {
            get { return _Value; }
            set { SetProperty( PropertyValue, ref _Value, value, RefreshNetEffect ); }
        }

        public override float Strength
        {
            get { return Math.Abs( Value ); }
        }

        public string PropertyDisplayString { get { return "DisplayString"; } }
        private string _DisplayString;
        public string DisplayString
        {
            get { return _DisplayString; }
            private set { SetProperty( PropertyDisplayString, ref _DisplayString, value ); }
        }

        public string PropertyDisplayQuantity { get { return "DisplayQuantity"; } }
        private string _DisplayQuantity;
        public string DisplayQuantity
        {
            get { return _DisplayQuantity; }
            private set { SetProperty( PropertyDisplayQuantity, ref _DisplayQuantity, value ); }
        }

        public PropertyModifierEffect( string target, object source = null ) : base( target, source ) { }

        protected override bool BeforePropertyChange<U>( string propertyName, ref U property, U newValue )
        {
            if( ( propertyName == this.PropertyValue ) || ( propertyName == this.PropertyType ) )
            {
                if( ValueChanging != null ) ValueChanging( this );
            }

            return base.BeforePropertyChange<U>( propertyName, ref property, newValue );
        }

        protected override void AfterPropertyChange<U>( string propertyName, ref U property, U newValue )
        {
            if( ( propertyName == this.PropertyValue ) || ( propertyName == this.PropertyType ) )
            {
                if( ValueChanged != null ) ValueChanged( this );
            }

            base.AfterPropertyChange<U>( propertyName, ref property, newValue );
        }

        protected override void ApplyPersistInternal( IGameEntity target )
        {
            if( target != null )
            {
                IGameEntityProperty property = target.GetProperty( TargetPropertyName );
                Modify( property );
            }
        }

        protected override void ApplyInstantInternal( IGameEntity target )
        {
            if( target != null )
            {
                IGameEntityProperty property = target.GetProperty( TargetPropertyName );
                ModifyOnly( property );
            }
        }

        protected override void UnapplyInternal( IGameEntity target )
        {
            if( target != null )
            {
                IGameEntityProperty property = target.GetProperty( TargetPropertyName );
                Unmodify( property );
            }
        }

        public void Modify( IGameEntityProperty property )
        {
            if( ( property != null ) && ( TargetProperty == null ) )
            {
                ModifyOnly( property );
                TargetProperty = property;

                if( !property.Modifiers.Contains( this ) )
                {
                    property.Modifiers.Add( this );
                }
            }
        }

        public void ModifyOnly( IGameEntityProperty property )
        {
            if( property != null )
            {
                Modify( (T)property.Value );
            }
        }

        public void Unmodify( IGameEntityProperty property )
        {
            if( ( property != null ) && ( TargetProperty == property ) )
            {
                UnmodifyOnly( property );
                TargetProperty = null;

                if( property.Modifiers.Contains( this ) )
                {
                    property.Modifiers.Remove( this );
                }
            }
        }

        public void UnmodifyOnly( IGameEntityProperty property )
        {
            if( property != null )
            {
                Unmodify( (T)property.Value );
            }
        }

        public abstract void Modify( T property );
        public abstract void Unmodify( T property );

        protected void RefreshNetEffect()
        {
            if( Type == ModifierOperation.Add )
            {
                if( Value == 0 )
                {
                    NetEffect = NetStatisticModifierEffect.None;
                    DisplayQuantity = Value.ToString();
                    DisplayString = Value.ToString();
                }
                else if( Value > 0 )
                {
                    NetEffect = NetStatisticModifierEffect.Positive;
                    DisplayQuantity = string.Format( "+{0:0.00}", Value );
                    DisplayString = string.Format( "+{0:0.00} {1}", Value, TargetPropertyName );
                }
                else
                {
                    NetEffect = NetStatisticModifierEffect.Negative;
                    DisplayQuantity = string.Format( "{0:0.00}", Value );
                    DisplayString = string.Format( "{0:0.00} {1}", Value, TargetPropertyName );
                }
            }
            else if( Type == ModifierOperation.Multiply )
            {
                if( Value == 0 )
                {
                    NetEffect = NetStatisticModifierEffect.None;
                    DisplayString = Value.ToString();
                    DisplayQuantity = Value.ToString();
                }
                else if( Value > 0 )
                {
                    NetEffect = NetStatisticModifierEffect.Positive;
                    DisplayQuantity = string.Format( "+{0:0}%", Value * 100 );
                    DisplayString = string.Format( "+{0:0}% {1}", Value * 100, TargetPropertyName );
                }
                else
                {
                    NetEffect = NetStatisticModifierEffect.Negative;
                    DisplayQuantity = string.Format( "{0:0}%", Value * 100 );
                    DisplayString = string.Format( "{0:0}% {1}", Value * 100, TargetPropertyName );
                }
            }
        }
    }
}

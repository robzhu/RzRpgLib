using System;

namespace RzRpgLib
{
    public interface IPropertyDefinitionEffect : IGameEntityEffect
    {
        /// <summary>
        /// A callback method that returns an instance of the property to add/define.
        /// </summary>
        Func<IGameEntityProperty> PropertyProvider { get; }
    }

    /// <summary>
    /// Defines a property on the target entity.
    /// </summary>
    public class PropertyDefinitionEffect : GameEntityEffect, IPropertyDefinitionEffect
    {
        public string PropertyPropertyProvider { get { return "PropertyProvider"; } }
        private Func<IGameEntityProperty> _PropertyProvider;
        public Func<IGameEntityProperty> PropertyProvider
        {
            get { return _PropertyProvider; }
            private set { SetProperty( PropertyPropertyProvider, ref _PropertyProvider, value ); }
        }

        public PropertyDefinitionEffect( Func<IGameEntityProperty> propertyProvider, object source = null )
            : base( null, source ) 
        {
            PropertyProvider = propertyProvider;
        }

        protected override void ApplyPersistInternal(IGameEntity target)
        {
            var prop = PropertyProvider();

            if( ( target != null ) && ( prop != null ) && !target.HasProperty( prop.Name ) )
            {
                target.AddProperty( prop );
            }
        }

        protected override void ApplyInstantInternal( IGameEntity target )
        {
            var prop = PropertyProvider();

            if( ( target != null ) && ( prop != null ) && !target.HasProperty( prop.Name ) )
            {
                target.AddProperty( prop );
            }
        }

        protected override void UnapplyInternal(IGameEntity target)
        {
            var prop = PropertyProvider();

            if( target != null )
            {
                target.RemoveProperty( prop.Name );
            }
        }
    }
}

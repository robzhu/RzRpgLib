using System;
using RzAspects;

namespace RzRpgLib
{
    /// <summary>
    /// An effect is an object that modifies, defines, suppresses a property when applied to a game entity.
    /// </summary>
    public interface IGameEntityEffect : IUpdatable
    {
        /// <summary>
        /// The name of the effect, meant for displaying to the user.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// The icon that represents this effect.  Can be null.
        /// </summary>
        object Glyph { get; }

        /// <summary>
        /// The type of effect.  Ex:damage/healing/buff/debuff
        /// </summary>
        object EffectType { get; set; }

        /// <summary>
        /// The entity that this effect is acting upon.
        /// </summary>
        IGameEntity TargetEntity { get; }

        /// <summary>
        /// Whether this effect has been persisted on the target entity.
        /// </summary>
        bool IsPersisted { get; }

        /// <summary>
        /// If this value is not null or empty, then this effect is unique and cannot co-exist with other effects 
        /// that share the same UniquePersistenceId
        /// </summary>
        string UniquePersistenceId { get; }

        /// <summary>
        /// The strength of this effect, used to determine which effect should be persisted if multiple effects with the same UniquePersistenceId exist.
        /// </summary>
        float Strength { get; }

        /// <summary>
        /// The source object that is causing this effect.  Ex: a piece of equipment causes applies an effect that adds stats.
        /// </summary>
        object Source { get; set; }

        /// <summary>
        /// The name of the property that this effect applies to.  If Null, this effect does not apply to an existing property.
        /// </summary>
        string TargetPropertyName { get; set; }

        /// <summary>
        /// The entity property that this effect is acting upon.
        /// </summary>
        IGameEntityProperty TargetProperty { get; }

        /// <summary>
        /// The priority of this effect.  Used for resolving which effect takes precedence if this property conflicts with another.
        /// The effect with the higher priority takes precedence.  If priority is equal, either effect may take precedence.
        /// </summary>
        int Priority { get; set; }

        /// <summary>
        /// Modifies the specified entity with this effect instantly (does not persist).
        /// </summary>
        /// <param name="target">The value to modify.</param>
        void ApplyInstant( IGameEntity target );

        /// <summary>
        /// Modifies the specified entity with this effect and persists it.
        /// </summary>
        /// <param name="target">The value to modify.</param>
        void ApplyPersist( IGameEntity target );

        /// <summary>
        /// Unmodifies the specified entity from this effect.  Does nothing if the effect was not already on the property.
        /// </summary>
        void Unapply( IGameEntity target = null );

        string Description { get; }
    }

    public abstract class GameEntityEffect : UpdatableModel, IGameEntityEffect
    {
        public string PropertyDisplayName { get { return "DisplayName"; } }
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { SetProperty( PropertyDisplayName, ref _DisplayName, value ); }
        }

        public string PropertyGlyph { get { return "Glyph"; } }
        private object _Glyph;
        public object Glyph
        {
            get { return _Glyph; }
            set { SetProperty( PropertyGlyph, ref _Glyph, value ); }
        }

        public string PropertyEffectType { get { return "EffectType"; } }
        private object _EffectType;
        public object EffectType
        {
            get { return _EffectType; }
            set { SetProperty( PropertyEffectType, ref _EffectType, value ); }
        }

        public string PropertyTargetEntity { get { return "TargetEntity"; } }
        private IGameEntity _TargetEntity;
        public IGameEntity TargetEntity
        {
            get { return _TargetEntity; }
            private set { SetProperty( PropertyTargetEntity, ref _TargetEntity, value ); }
        }

        public string PropertyIsActive { get { return "IsPersisted"; } }
        private bool _IsPersisted = false;
        public bool IsPersisted
        {
            get { return _IsPersisted; }
            private set { SetProperty( PropertyIsActive, ref _IsPersisted, value ); }
        }

        public string PropertyUniquePersistenceId { get { return "UniquePersistenceId"; } }
        private string _UniquePersistenceId;
        public string UniquePersistenceId
        {
            get { return _UniquePersistenceId; }
            set { SetProperty( PropertyUniquePersistenceId, ref _UniquePersistenceId, value ); }
        }

        public virtual float Strength
        {
            get { return 0; }
        }

        public string PropertySource { get { return "Source"; } }
        private object _Source;
        public object Source
        {
            get { return _Source; }
            set { SetProperty( PropertySource, ref _Source, value ); }
        }

        public string PropertyTargetProperty { get { return "TargetProperty"; } }
        private IGameEntityProperty _TargetProperty;
        public IGameEntityProperty TargetProperty
        {
            get { return _TargetProperty; }
            protected set { SetProperty( PropertyTargetProperty, ref _TargetProperty, value ); }
        }

        public string PropertyTargetPropertyName { get { return "TargetPropertyName"; } }
        private string _TargetPropertyName;
        public string TargetPropertyName
        {
            get { return _TargetPropertyName; }
            set { SetProperty( PropertyTargetPropertyName, ref _TargetPropertyName, value ); }
        }

        public string PropertyPriority { get { return "Priority"; } }
        private int _Priority;
        public int Priority
        {
            get { return _Priority; }
            set { SetProperty( PropertyPriority, ref _Priority, value ); }
        }

        public string PropertyDescription { get { return "Description"; } }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { SetProperty( PropertyDescription, ref _Description, value ); }
        }

        public GameEntityEffect( string target, object source = null )
        {
            Source = source;
            TargetPropertyName = target;
        }

        public void ApplyInstant( IGameEntity target )
        {
            BeforeApply();
            ApplyInstantInternal( target );
            AfterApply();
        }

        public void ApplyPersist( IGameEntity target )
        {
            if( target == null ) return;
            if( IsPersisted ) throw new Exception( "Cannot apply an effect that is already active" );

            if( !target.HasPersistentEffect( this ) )
            {
                //Implementations of IGameEntity may try to apply this effect.
                target.AddPersistentEffect( this );
            }

            if( !IsPersisted )
            {
                BeforeApply();
                TargetEntity = target;
                IsPersisted = true;
                ApplyPersistInternal( target );
                AfterApply();
            }
        }

        public void Unapply( IGameEntity target = null )
        {
            if( IsPersisted )
            {
                if( TargetEntity == null ) throw new Exception( "TargetEntity cannot be null" );

                if( TargetEntity.HasPersistentEffect( this ) )
                {
                    TargetEntity.RemovePersistentEffect( this );
                }

                if( IsPersisted )
                {
                    BeforeUnapply();
                    UnapplyInternal( TargetEntity );
                    TargetEntity = null;
                    IsPersisted = false;
                    AfterUnapply();
                }
            }
            else
            {
                if( target == null ) throw new Exception( "TargetEntity cannot be null" );
                UnapplyInternal( target );
            }
        }

        protected virtual void BeforeApply() { }
        protected virtual void AfterApply() { }
        protected virtual void BeforeUnapply() { }
        protected virtual void AfterUnapply() { }

        protected abstract void ApplyPersistInternal( IGameEntity target );
        protected abstract void ApplyInstantInternal( IGameEntity target );
        protected abstract void UnapplyInternal( IGameEntity target );
    }
}

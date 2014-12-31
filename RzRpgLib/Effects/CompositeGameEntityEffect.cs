using System;
using RzAspects;

namespace RzRpgLib
{
    /// <summary>
    /// A game entity effect that is composed of other effects.
    /// </summary>
    public interface ICompositeGameEntityEffect : IGameEntityEffect
    {
        ObservableCollectionEx<IGameEntityEffect> Children { get; }
    }

    public class CompositeGameEntityEffect : GameEntityEffect, ICompositeGameEntityEffect
    {
        public ObservableCollectionEx<IGameEntityEffect> Children { get; private set; }

        public CompositeGameEntityEffect() : base( null, null )
        {
            Children = new ObservableCollectionEx<IGameEntityEffect>();
            Children.ItemAdded += ChildEffectAdded;
            Children.ItemRemoved += ChildEffectRemoved;
        }

        public CompositeGameEntityEffect( params IGameEntityEffect[] children ) : this( null, children ) { }
        public CompositeGameEntityEffect( object source, params IGameEntityEffect[] children )
            : base( null, source )
        {
            Children = new ObservableCollectionEx<IGameEntityEffect>();
            Children.AddRange( children );
            Children.ItemAdded += ChildEffectAdded;
            Children.ItemRemoved += ChildEffectRemoved;
        }

        private void ChildEffectAdded( IGameEntityEffect child )
        {
            if( TargetEntity != null )
                child.ApplyPersist( TargetEntity );
        }

        private void ChildEffectRemoved( IGameEntityEffect child )
        {
            if( TargetEntity != null )
                child.Unapply( TargetEntity );
        }

        protected override void ApplyPersistInternal( IGameEntity target )
        {
            foreach( var child in Children )
            {
                child.ApplyPersist( target );
            }
        }

        protected override void ApplyInstantInternal( IGameEntity target )
        {
            foreach( var child in Children )
            {
                child.ApplyInstant( target );
            }
        }

        protected override void UnapplyInternal( IGameEntity target )
        {
            foreach( var child in Children )
            {
                child.Unapply( target );
            }
        }

        protected override void AfterPropertyChange<T>( string propertyName, ref T property, T newValue )
        {
            base.AfterPropertyChange<T>( propertyName, ref property, newValue );
            if( propertyName == PropertySource )
            {
                foreach( var child in Children )
                {
                    child.Source = Source;
                }
            }
        }

        //TODO: duration on child effects should reflect the composite effect.
    }
}

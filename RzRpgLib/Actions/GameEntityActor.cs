using RzAspects;

namespace RzRpgLib
{
    /// <summary>
    /// An Actor is an entity that is capable of performing actions that affect other entities.  
    /// </summary>
    public interface IGameEntityActor : IGameEntity
    {
        ObservableCollectionEx<IGameEntityAction> Abilities { get; }
        ObservableCollectionEx<IGameEntityProperty> Resources { get; }
    }

    public class GameEntityActor : GameEntity, IGameEntityActor
    {
        /// <summary>
        /// NEVER MODIFY THIS COLLECTION DIRECTLY- use GameEntity.AddProperty() instead.
        /// The collection of resources for this entity.  It is
        /// passively synchronized with the Properties collection.  
        /// </summary>
        public ObservableCollectionEx<IGameEntityProperty> Resources { get; private set; }
        public ObservableCollectionEx<IGameEntityAction> Abilities { get; private set; }

        public GameEntityActor( string id = null ) : base( id )
        {
            Resources = new ObservableCollectionEx<IGameEntityProperty>();
            Abilities = new ObservableCollectionEx<IGameEntityAction>();

            Properties.ItemAdded += OnPropertyAdded;
            Properties.ItemRemoved += OnPropertyRemoved;

            Abilities.ItemAdded += OnAbilityAdded;
            Abilities.ItemRemoved += OnAbilityRemoved;
        }

        private void OnPropertyAdded( IGameEntityProperty item )
        {
            if( item is IResourceProperty )
            {
                Resources.Add( item );
            }
        }

        private void OnPropertyRemoved( IGameEntityProperty item )
        {
            if( item is IResourceProperty )
            {
                Resources.Remove( item );
            }
        }

        private void OnAbilityAdded( IGameEntityAction item )
        {
            item.Bind( this );
        }

        private void OnAbilityRemoved( IGameEntityAction item )
        {
            foreach( IActionRequirement requirement in item.Requirements )
            {
                IResourceRequirement resourceReq = requirement as IResourceRequirement;
                if( resourceReq != null )
                {
                    resourceReq.Resource = null;
                }
            }
        }
    }
}

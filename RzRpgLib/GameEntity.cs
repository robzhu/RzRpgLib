using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using RzAspects;

namespace RzRpgLib
{
    //Entity- an object that exists in and interacts with the game world.  
    //  Examples: player's character, a monster, a sword, etc.
    public interface IGameEntity : ICompositeProperty
    {
        string ID { get; }
        string Name { get; set; }
        string DisplayName { get; set; }

        event Action<GameEntityPropertyCollectionChangedEventArgs> GameEntityPropertyCollectionChanged;

        void AddProperty( IGameEntityProperty property );
        bool RemoveProperty( IGameEntityProperty property );
        bool RemoveProperty( string propertyName );
        bool HasProperty( string propertyName );
        IGameEntityProperty GetProperty( string propertyName );

        /// <summary>
        /// The accessories for this entity.  Examples: gear, components, subsystems, squad members, units
        /// </summary>
        ObservableCollectionEx<IGameEntity> Accessories { get; }

        /// <summary>
        /// The persistent effects acting on this entity.
        /// </summary>
        ObservableCollectionEx<IGameEntityEffect> PersistentEffects { get; }

        /// <summary>
        /// Adds the specified persistent effect. 
        /// </summary>
        /// <param name="effect">The effect to add.</param>
        /// <returns>Returns true if the effect is active immediately.  False if it was added but inactive because it conflicted with an existing UniquePersistenceId</returns>
        bool AddPersistentEffect( IGameEntityEffect effect );

        /// <summary>
        /// Removes the specified persistent effect.
        /// </summary>
        /// <param name="effect">The effect to remove.</param>
        /// <returns>Returns true if the effect was removed.  False otherwise.</returns>
        bool RemovePersistentEffect( IGameEntityEffect effect );

        /// <summary>
        /// Checks to see if the specified effect exists.  
        /// </summary>
        /// <param name="effect">The effect to check.</param>
        /// <returns>True if the effect exists (active or inactive).  False otherwise.</returns>
        bool HasPersistentEffect( IGameEntityEffect effect );

        ObservableCollectionEx<IGameEntityProperty> Properties { get; }

        dynamic this[ string propertyName ] { get; set; }
    }

    /// <summary>
    /// An object that exists in and interacts with the game world.  
    /// </summary>
    public class GameEntity : ModelBase, IGameEntity
    {
        public event Action<IGameEntityProperty> PropertyAdded
        {
            add { Properties.ItemAdded += value; }
            remove { Properties.ItemAdded -= value; }
        }

        public event Action<IGameEntityProperty> PropertyRemoved
        {
            add { Properties.ItemRemoved += value; }
            remove { Properties.ItemRemoved -= value; }
        }

        public event Action<IGameEntityProperty, PropertyChangedEventArgs> PropertyPropertyChanged
        {
            add { Properties.ItemPropertyChanged += value; }
            remove { Properties.ItemPropertyChanged -= value; }
        }

        public event Action<GameEntityPropertyCollectionChangedEventArgs> GameEntityPropertyCollectionChanged;

        public int ClassID { get; private set; }
        public string ID { get; set; }

        public string PropertyName { get { return "Name"; } }
        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty( PropertyName, ref _name, value ); }
        }

        public string PropertyDisplayName { get { return "DisplayName"; } }
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { SetProperty( PropertyDisplayName, ref _DisplayName, value ); }
        }

        private Dictionary<string, IGameEntityProperty> PropertyDictionary { get; set; }
        public ObservableCollectionEx<IGameEntityProperty> Properties { get; private set; }

        public ObservableCollectionEx<IGameEntity> Accessories { get; private set; }
        public ObservableCollectionEx<IGameEntityEffect> PersistentEffects { get; private set; }

        //The list of effects that have unique identifiers.
        private ObservableCollectionEx<IGameEntityEffect> ActiveUniquePersistentEffects { get; set; }
        private ObservableCollectionEx<IGameEntityEffect> InactiveUniquePersistentEffects { get; set; }

        public GameEntity( string id = null )
        {
            ID = ( id == null ) ? Guid.NewGuid().ToString() : id;
            PropertyDictionary = new Dictionary<string, IGameEntityProperty>();
            Properties = new ObservableCollectionEx<IGameEntityProperty>();
            Accessories = new ObservableCollectionEx<IGameEntity>( OnAccessoryAdded, OnAccessoryRemoved );
            PersistentEffects = new ObservableCollectionEx<IGameEntityEffect>( OnPersistentEffectAdded, OnPersistentEffectRemoved );
            ActiveUniquePersistentEffects = new ObservableCollectionEx<IGameEntityEffect>( OnActiveUniquePersistentEffectAdded, OnActiveUniquePersistentEffectRemoved );
            InactiveUniquePersistentEffects = new ObservableCollectionEx<IGameEntityEffect>( OnInactiveUniquePersistentEffectAdded, null );
        }

        public bool AddPersistentEffect( IGameEntityEffect effect )
        {
            if ( effect == null ) throw new ArgumentNullException( "effect" );
            if ( string.IsNullOrEmpty( effect.UniquePersistenceId ) )
            {   //The effect is not unique, add it to the set of persistent effects.
                PersistentEffects.Add( effect );
                return true;
            }
            else
            {
                //The effect is unique, check to see if an effect of the same name already exists in ActiveUniquePersistentEffects
                IGameEntityEffect existingEffectWithSameUniqueId = null;
                foreach ( var uniqueEffect in ActiveUniquePersistentEffects )
                {
                    if ( uniqueEffect.UniquePersistenceId == effect.UniquePersistenceId )
                    {
                        existingEffectWithSameUniqueId = uniqueEffect;
                        break;
                    }
                }

                if ( existingEffectWithSameUniqueId == null )
                {   //No existing effect with the same unique Id was found.
                    ActiveUniquePersistentEffects.Add( effect );
                    return true;
                }
                else
                {   //An existing effect with a conflicting unique Id was found.  Persist the stronger one and place the weaker one in the inactive list.
                    if ( effect.Strength > existingEffectWithSameUniqueId.Strength )
                    {
                        ActiveUniquePersistentEffects.Remove( existingEffectWithSameUniqueId );
                        InactiveUniquePersistentEffects.Add( existingEffectWithSameUniqueId );
                        ActiveUniquePersistentEffects.Add( effect );
                        return true;
                    }
                    else
                    {   //effect.Strength <= existingEffectWithSameUniqueId.Strength
                        InactiveUniquePersistentEffects.Add( effect );
                        return false;
                    }
                }
            }
        }

        public bool RemovePersistentEffect( IGameEntityEffect effect )
        {
            if( string.IsNullOrEmpty( effect.UniquePersistenceId ) )
            {   //effect is not unique, remove it from the main list.
                return PersistentEffects.Remove( effect );
            }
            else
            {   //effect is unique, it either lives in ActiveUniquePersistentEffects or InactiveUniquePersistentEffects
                if ( InactiveUniquePersistentEffects.Remove( effect ) )
                {
                    return true;
                }
                else if ( ActiveUniquePersistentEffects.Remove( effect ) )
                {
                    //we have removed an active unique effect.  Search for the next strongest unique effect in the Inactive list
                    IGameEntityEffect strongestInactiveEffectWithSameUniqueId = null;

                    foreach ( var inactiveEffect in InactiveUniquePersistentEffects )
                    {
                        if ( inactiveEffect.UniquePersistenceId == effect.UniquePersistenceId )
                        {
                            if ( strongestInactiveEffectWithSameUniqueId == null )
                            {
                                strongestInactiveEffectWithSameUniqueId = inactiveEffect;
                            }
                            else if ( inactiveEffect.Strength > strongestInactiveEffectWithSameUniqueId.Strength )
                            {
                                strongestInactiveEffectWithSameUniqueId = inactiveEffect;
                            }
                        }
                    }

                    if ( strongestInactiveEffectWithSameUniqueId != null )
                    {   //There is another effect with the same unique Id as the one we just removed.  Activate it now.  
                        InactiveUniquePersistentEffects.Remove( strongestInactiveEffectWithSameUniqueId );
                        ActiveUniquePersistentEffects.Add( strongestInactiveEffectWithSameUniqueId );
                    }

                    return true;
                }
                else return false;
            }
        }

        public bool HasPersistentEffect( IGameEntityEffect effect )
        {
            return ( PersistentEffects.Contains( effect ) || 
                     ActiveUniquePersistentEffects.Contains( effect ) || 
                     InactiveUniquePersistentEffects.Contains( effect ) );
        }

        private void OnPersistentEffectAdded( IGameEntityEffect effect )
        {
            if( effect != null )
            {
                effect.ApplyPersist( this );
                effect.OnExpired += ( o ) =>
                {
                    PersistentEffects.Remove( effect );
                    ActiveUniquePersistentEffects.Remove( effect );
                };
            }
        }

        private void OnPersistentEffectRemoved( IGameEntityEffect effect )
        {
            if( effect != null )
            {
                effect.Unapply();
            }
        }

        private void OnActiveUniquePersistentEffectAdded( IGameEntityEffect effect )
        {
            PersistentEffects.Add( effect );
        }

        private void OnActiveUniquePersistentEffectRemoved( IGameEntityEffect effect )
        {
            PersistentEffects.Remove( effect );
        }

        private void OnInactiveUniquePersistentEffectAdded( IGameEntityEffect effect )
        {
            if ( effect != null )
            {
                effect.OnExpired += ( o ) =>
                {
                    InactiveUniquePersistentEffects.Remove( effect );
                };
            }
        }

        private void OnAccessoryAdded( IGameEntity accessory )
        {
            foreach( var property in accessory.Properties )
            {
                IApplyEffectToParentProperty equipEffect = property as IApplyEffectToParentProperty;
                if( equipEffect != null )
                {
                    AddPersistentEffect( equipEffect.Effect );
                }
            }
        }

        private void OnAccessoryRemoved( IGameEntity accessory )
        {
            foreach( var property in accessory.Properties )
            {
                IApplyEffectToParentProperty equipEffect = property as IApplyEffectToParentProperty;
                if( equipEffect != null )
                {
                    RemovePersistentEffect( equipEffect.Effect );
                }
            }
        }

        #region Property Manipulation Methods
        public dynamic this[ string propertyName ]
        {
            get
            {
                if( PropertyDictionary.ContainsKey( propertyName ) )
                {
                    return PropertyDictionary[ propertyName ].Value;
                }
                return null;
            }
            set
            {
                if( PropertyDictionary.ContainsKey( propertyName ) )
                {
                    PropertyDictionary[ propertyName ].Value = value;
                }
            }
        }

        public void AddProperty( IGameEntityProperty property )
        {
            Debug.Assert( null != property );
            Debug.Assert( null != property.Name );

            if ( property.Parent != null ) 
                throw new GameEntityPropertyBindingException( this, property, "property already appears to have a parent." );
            if ( PropertyDictionary.ContainsKey( property.Name ) ) 
                throw new GameEntityPropertyBindingException( this, property, string.Format( "this entity already contains a property with name {0}", property.Name ) );

            property.Parent = this;
            ApplyExistingEffectsToProperty( property );
            PropertyDictionary.Add( property.Name, property );
            Properties.Add( property );
            OnPropertiesChanged( new GameEntityPropertyCollectionChangedEventArgs( GameEntityPropertyCollectionChangedAction.Add, property ) );
        }

        private void ApplyExistingEffectsToProperty( IGameEntityProperty property )
        {
            if( property != null )
            {
                //search over the existing effects and apply any that target this property.
                foreach( var effect in PersistentEffects )
                {
                    IPropertyModifierEffect modifier = effect as IPropertyModifierEffect;
                    if( ( modifier != null ) && ( modifier.TargetPropertyName == property.Name ) )
                    {
                        property.Modifiers.Add( modifier );
                    }
                }
            }
        }

        public bool RemoveProperty( string propertyName )
        {
            IGameEntityProperty toRemove = GetProperty( propertyName );

            if( null != toRemove )
            {
                RemovePropertyCore( toRemove );
                return true;
            }
            return false;
        }

        public bool RemoveProperty( IGameEntityProperty property )
        {
            Debug.Assert( null != property );

            IGameEntityProperty toRemove = GetProperty( property.Name );

            if( ( null != toRemove ) && ( toRemove == property ) )
            {
                RemovePropertyCore( property );
                return true;
            }
            return false;
        }

        public IGameEntityProperty GetProperty( string propertyName )
        {
            Debug.Assert( null != propertyName );

            IGameEntityProperty prop = null;

            if( PropertyDictionary != null )
            {
                PropertyDictionary.TryGetValue( propertyName, out prop );
            }
            return prop;
        }

        private void RemovePropertyCore( IGameEntityProperty property )
        {
            Debug.Assert( null != property );
            PropertyDictionary.Remove( property.Name );
            Properties.Remove( property );
            property.Reset();

            OnPropertiesChanged( new GameEntityPropertyCollectionChangedEventArgs( GameEntityPropertyCollectionChangedAction.Remove, property ) );
        }

        public bool HasProperty( string propertyName )
        {
            return PropertyDictionary.ContainsKey( propertyName );
        }

        protected void OnPropertiesChanged( GameEntityPropertyCollectionChangedEventArgs changeArgs )
        {
            if( null != GameEntityPropertyCollectionChanged )
            {
                GameEntityPropertyCollectionChanged( changeArgs );
            }
        }

        #endregion
    }
}

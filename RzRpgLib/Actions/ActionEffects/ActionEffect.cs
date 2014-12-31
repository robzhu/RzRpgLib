using System.Collections.Generic;
using RzAspects;

namespace RzRpgLib
{
    /// <summary>
    /// Interface for an action effect.  Action effects are what occur when an action is executed.
    /// </summary>
    public interface IActionEffect
    {
        /// <summary>
        /// The name of the effect, used to uniquely identify the type of the effect.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type of this action effect that will determine how it is processed.
        /// </summary>
        object EffectType { get; }

        /// <summary>
        /// The friendly, display name of this effect.
        /// </summary>
        string DisplayName { get; set; }

        string Description { get; set; }

        /// <summary>
        /// Builds a string that describes the effect.
        /// </summary>
        /// <param name="entry">The log entry that contains the effect's related parameters</param>
        /// <returns>The string for display in a message log.</returns>
        string GetMessageLogString( EffectLogEntry entry );

        void Apply( IGameEntity target, object context );

        /// <summary>
        /// Applies the effect to the target(s).
        /// </summary>
        /// <param name="targets">The target(s) to apply this effect to.</param>
        void Apply( IEnumerable<IGameEntity> targets, object context );
    }

    public abstract class ActionEffect : ModelBase, IActionEffect
    {
        public string PropertyName { get { return "Name"; } }
        private string _Name;
        public string Name
        {
            get { return _Name; }
            protected set { SetProperty( PropertyName, ref _Name, value ); }
        }

        public abstract object EffectType { get; }

        public string PropertyDisplayName { get { return "DisplayName"; } }
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { SetProperty( PropertyDisplayName, ref _DisplayName, value ); }
        }

        public string PropertyDescription { get { return "Description"; } }
        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { SetProperty( PropertyDescription, ref _Description, value ); }
        }

        public ActionEffect( string name = null )
        {
            Name = name;
            DisplayName = name;
        }

        public void Apply( IEnumerable<IGameEntity> targets, object context )
        {
            if( targets == null ) return;

            foreach( var target in targets )
            {
                Apply( target, context );
            }
        }

        public abstract void Apply( IGameEntity target, object context );
        public abstract string GetMessageLogString( EffectLogEntry entry );
    }
}

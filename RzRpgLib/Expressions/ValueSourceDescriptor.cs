using RzAspects;

namespace RzRpgLib
{
    public interface IValueSourceDesrciptor
    {
        /// <summary>
        /// Gets the name of the entity that owns the value source property.
        /// </summary>
        string SourceEntityName { get; }

        /// <summary>
        /// The name of the value source property.
        /// </summary>
        string SourcePropertyName { get; }

        /// <summary>
        /// The display name of the value source property.  This display name is independent of the actual property's display name.
        /// </summary>
        string SourcePropertyDisplayName { get; }

        /// <summary>
        /// Retrieves the value source from the specified entity.  
        /// </summary>
        /// <param name="entity">The entity on which to retrieve the property with name = SourcePropertyName</param>
        /// <returns>The value source for the described property.</returns>
        IValueSource Retrieve( IGameEntity entity );
    }

    public class ValueSourceDescriptor : ModelBase, IValueSourceDesrciptor
    {
        public string PropertySourceEntityName { get { return "SourceEntityName"; } }
        private string _SourceEntityName;
        public string SourceEntityName
        {
            get { return _SourceEntityName; }
            set { SetProperty( PropertySourceEntityName, ref _SourceEntityName, value ); }
        }

        public string PropertySourcePropertyName { get { return "SourcePropertyName"; } }
        private string _SourcePropertyName;
        public string SourcePropertyName
        {
            get { return _SourcePropertyName; }
            set { SetProperty( PropertySourcePropertyName, ref _SourcePropertyName, value ); }
        }

        public string PropertySourcePropertyDisplayName { get { return "SourcePropertyDisplayName"; } }
        private string _SourcePropertyDisplayName;
        public string SourcePropertyDisplayName
        {
            get { return _SourcePropertyDisplayName; }
            set { SetProperty( PropertySourcePropertyDisplayName, ref _SourcePropertyDisplayName, value ); }
        }

        public IValueSource Retrieve( IGameEntity target )
        {
            return ( target != null ) ? ( target.GetProperty( SourcePropertyName ).Value as IValueSource ) : null;
        }
    }
}

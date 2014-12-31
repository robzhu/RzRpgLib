using System;

namespace RzRpgLib
{
    public interface IResourceProperty : IGameEntityProperty
    {
    }

    public class ResourceProperty : GameEntityProperty, IResourceProperty
    {
        public ResourceProperty( string name, float currentValue )
            : this( name, currentValue, currentValue ) { }

        public ResourceProperty( string name, float currentValue, float maxValueBase, float minValueBase = 0 )
            : this( name, new Resource( currentValue, maxValueBase, minValueBase ) ) { }

        public ResourceProperty( string name, Resource resource )
            : base( name, resource ) { }

        /// <summary>
        /// A single string defining the property name and value defaults, taking on the format "CurrentValue/MaxValue PropertyName"
        /// </summary>
        /// <param name="friendString">The string defining the property.</param>
        /// <example>"50/50 Mana"</example>
        /// <example>"0/100 Rage"</example>
        /// <returns>The Resource property.</returns>
        public static ResourceProperty FromFriendlyString( string friendlyString )
        {
            try
            {
                string[] parts = friendlyString.Split( ' ' );
                string[] values = parts[ 0 ].Split( '/' );

                float currentValue = float.Parse( values[ 0 ] );
                float maxValue = float.Parse( values[ 1 ] );

                string name = parts[ 1 ];

                return new ResourceProperty( name, currentValue, maxValue );
            }
            catch( Exception )
            {
                throw new ArgumentException( "input was not of the expected friendlyString format." );
            }
        }
    }
}

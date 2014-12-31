using System;

namespace RzRpgLib
{
    public class GameEntityPropertyBindingException : Exception
    {
        public IGameEntity Entity { get; set; }
        public IGameEntityProperty Property { get; set; }

        public GameEntityPropertyBindingException( IGameEntity entity, IGameEntityProperty property ) : this( entity, property, null ) { }
        public GameEntityPropertyBindingException( IGameEntity entity, IGameEntityProperty property, string message ) : base( message )
        {
            Entity = entity;
            Property = property;
        }
    }
}

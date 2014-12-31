using System;
using System.Collections.Generic;

namespace RzRpgLib
{
    /// <summary>
    /// This class maps a series of effects (identified by their Name property) to a corresponding processor
    /// </summary>
    public class EffectProcessorMap : IEffectProcessor
    {
        Dictionary<Type, IEffectProcessor> _map = new Dictionary<Type, IEffectProcessor>();

        public void Process( IGameEntityAction action, IActionEffect effect, IGameEntityActor actor, object effectTargetContext )
        {
            if ( action == null ) throw new Exception( "action cannot be null" );
            if ( effect == null ) throw new Exception( "effect cannot be null" );
            IEffectProcessor processor = null;
            if ( !_map.TryGetValue( effect.GetType(), out processor ) ) throw new Exception( string.Format( "No mapping found for an effect type: {0}", action.Name ) );
            if ( processor == null ) throw new Exception( string.Format( "No mapping found for an effect type: {0}", effect.Name ) );

            processor.Process( action, effect, actor, effectTargetContext );
        }

        public void AddMapping( Type effectType, IEffectProcessor effectProcessor )
        {
            if ( effectType == null ) throw new ArgumentNullException( "effectType cannot be null" );
            if ( effectProcessor == null ) throw new ArgumentNullException( "processor cannot be null" );

            _map.Add( effectType, effectProcessor );
        }
    }
}

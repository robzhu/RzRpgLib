using System;
using System.Collections.Generic;

namespace RzRpgLib
{ 
    /// <summary>
    /// This class maps a series of actions (identified by their Name property) to a corresponding ActionExecutor
    /// </summary>
    public class ActionExecutorMap : IActionExecutor
    {
        Dictionary<string, IActionExecutor> _map = new Dictionary<string, IActionExecutor>();

        public ActionResult Execute( IGameEntityAction action, IGameEntityActor actor, object targetContext )
        {
            if( action == null ) throw new Exception( "action cannot be null" );
            IActionExecutor executor = null;
            if( !_map.TryGetValue( action.Name, out executor ) ) throw new Exception( string.Format( "No mapping found for an action type: {0}", action.Name ) );
            if( executor == null ) throw new Exception( string.Format( "No mapping found for an action type: {0}", action.Name ) );

            return executor.Execute( action, actor, targetContext );
        }

        public void AddMapping( string actionId, IActionExecutor executor )
        {
            if( string.IsNullOrEmpty( actionId ) ) throw new ArgumentNullException( "actionId cannot be null" );
            if( executor == null ) throw new ArgumentNullException( "executor cannot be null" );

            _map.Add( actionId, executor );
        }
    }
}

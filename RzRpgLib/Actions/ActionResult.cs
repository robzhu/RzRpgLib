using System;

namespace RzRpgLib
{
    public class ActionExecutionException : Exception
    {
        public ActionResult Result { get; private set; }

        public ActionExecutionException( string message ) : base( message )
        {
            Result = new ActionResult( message );
        }
    }

    /// <summary>
    /// The result of attempting to perform an action.
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// Gets whether the action successfully executed.  Executing successfully does not mean the action's effects were implemented.
        /// For example, an attack action could execute successfully but miss.  If Success is false, the Message field should contain a 
        /// an explanation of why the action could not execute.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// The reason why the action could not execute if Success is false.  If Success is true, this field should be ignored.
        /// </summary>
        public string Message { get; private set; }

        public ActionResult( string message = null )
        {
            Message = message;
            Success = (Message == null);
        }
    }
}

using System.Collections.Generic;

namespace RzRpgLib
{
    /// <summary>
    /// Interface for a component that evaluates itself only when bound to a parent entity.
    /// </summary>
    public interface IBoundEvaluationComponent
    {
        void Bind( IGameEntity parent );
    }
}

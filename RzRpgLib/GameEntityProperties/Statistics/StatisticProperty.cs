using RzAspects;

namespace RzRpgLib
{
    public class StatisticProperty : GameEntityProperty
    {
        public StatisticProperty( string name, float baseValue ) : base( name, new Statistic( baseValue ) ) { }
    }
}

using System;
using RzAspects;

namespace RzRpgLib
{
    /// <summary>
    /// Effect that modifies an existing numeric statistic
    /// </summary>
    public class StatisticModifier : PropertyModifierEffect<IStatistic>, IPropertyModifierEffect
    {
        public StatisticModifier() : base( null, null ) { }
        public StatisticModifier( string target ) : base( target, null ) { }

        public StatisticModifier( string target, float flatValue, ModifierOperation modType ) : base( target, null )
        {
            Value = flatValue;
            Type = modType;
        }

        public StatisticModifier( string target, string uniqueId, float flatValue, ModifierOperation modType ) : this( target, flatValue, modType )
        {
            UniquePersistenceId = uniqueId;
        }

        public StatisticModifier( string target, float flatValue = 0 ) : base( target, null )
        {
            Value = flatValue;
            if( flatValue == 0 ) Type = ModifierOperation.Add;
            else
            {
                Type = ( Math.Abs( flatValue ) > 1 ) ? ModifierOperation.Add : ModifierOperation.Multiply;
            }
        }

        public StatisticModifier( string target, string uniqueId, float flatValue ) : this( target, flatValue )
        {
            UniquePersistenceId = uniqueId;
        }

        public StatisticModifier( string target, float flatValue, float duration )
            : base( target, null )
        {
            Value = flatValue;
            Duration = new Duration( duration );
        }

        public StatisticModifier( string target, string uniqueId, float flatValue, float duration ) : this ( target, flatValue, duration )
        {
            UniquePersistenceId = uniqueId;
        }

        public override void Modify( IStatistic target )
        {
            if ( Type == ModifierOperation.Add )
                target.AddTotal += Value;
            else if ( Type == ModifierOperation.Multiply )
                target.MultiplyTotal += Value;
        }

        public override void Unmodify( IStatistic target )
        {
            if ( Type == ModifierOperation.Add )
                target.AddTotal -= Value;
            else if ( Type == ModifierOperation.Multiply )
                target.MultiplyTotal -= Value;
        }
    }

    public static class StatisticModifierFactory
    {
        public static StatisticModifier CreateFromFullString( string modifierString, object source = null )
        {
            StatisticModifier modifier = new StatisticModifier();

            if ( string.IsNullOrEmpty( modifierString ) ) throw new ArgumentNullException( "modifierString" );

            string[] strings = modifierString.Split( ' ' );
            if ( strings.Length > 2 ) throw new ArgumentNullException( "Invalid format for modifierString" );

            modifier.Source = source;

            if ( strings.Length == 1 )
            {
                //Simple value case.
                modifier.TargetPropertyName = modifierString;
            }
            else
            {
                //Full modifier string case.
                modifier.TargetPropertyName = strings[ 1 ];
                string valueStr = strings[ 0 ];
                modifier.Value = float.Parse( valueStr.TrimEnd( '%' ) );
                modifier.Type = ModifierOperation.Add;

                //scaling modifier
                if ( strings[ 0 ].EndsWith( @"%" ) )
                {
                    modifier.Type = ModifierOperation.Multiply;
                    modifier.Value /= 100;
                }
            }

            return modifier;
        }
    }
}

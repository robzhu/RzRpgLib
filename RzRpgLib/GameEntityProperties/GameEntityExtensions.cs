using System;

namespace RzRpgLib
{
    public static class GameEntityPropertyExtensions
    {
        public static void AddStatisticModifier( this GameEntityProperty property, string fullModifierString )
        {
            property.Modifiers.Add( StatisticModifierFactory.CreateFromFullString( fullModifierString ) );
        }

        public static void AddStatisticModifier( this GameEntityProperty property, string target, float value, ModifierOperation type = ModifierOperation.Add )
        {
            property.Modifiers.Add( new StatisticModifier( target ) { Value = value, Type = type } );
        }
    }

    public static class GameEntityExtensions
    {
        public static void AddStatistic( this GameEntity entity, string statName, float baseValue = 0, float min = 0 )
        {
            entity.AddProperty( new GameEntityProperty( statName, new Statistic() 
            { 
                BaseValue = baseValue,
            } ) );
        }

        public static void AddPercentStatistic( this GameEntity entity, string statName, float baseValue = 0, float min = 0 )
        {
            entity.AddProperty( new GameEntityProperty( statName, new Statistic()
            {
                IsPercentage = true,
                BaseValue = baseValue,
            } ) );
        }

        public static void AddNegativePercentStatistic( this GameEntity entity, string statName, float baseValue = 0, float min = 0 )
        {
            entity.AddProperty( new GameEntityProperty( statName, new Statistic()
            {
                IsPercentage = true,
                HigherIsBetter = false,
                BaseValue = baseValue,
            } ) );
        }

        /// <summary>
        /// Parses and adds the specified effect (from a single string format).  The format must be:
        /// 
        /// </summary>
        /// <param name="entity">The entity on which to add the property.</param>
        /// <param name="effectString">The full effect string.  Cannot be empty or null and must contain two parts: the value and the target property.</param>
        /// <example>+10 Intellect</example>
        /// <example>+5% Courage</example>
        public static void AddEquipEffectProperty( this GameEntity entity, string effectString )
        {
            AddEquipEffectProperty( entity, StatisticModifierFactory.CreateFromFullString( effectString ) );
        }

        public static void AddEquipEffectProperty( this GameEntity entity, string statName, float baseValue, ModifierOperation statModType = ModifierOperation.Add )
        {
            AddEquipEffectProperty( entity, new StatisticModifier( statName ) { Value = baseValue, Type = statModType } );
        }

        public static void AddEquipEffectProperty( this GameEntity entity, IGameEntityEffect effect )
        {
            entity.AddProperty( new ApplyEffectToParentProperty( "mod" + effect.TargetPropertyName, effect ) );
        }

        public static void AddResource( this GameEntity entity, string name, float currentValue, float maxValue )
        {
            entity.AddProperty( new ResourceProperty( name, currentValue, maxValue ) );
        }

        public static void AddResource( this GameEntity entity, IResourceProperty resource )
        {
            entity.AddProperty( resource );
        }
    }
}

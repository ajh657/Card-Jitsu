using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CardJitsu.Common
{
    public enum Effect
    {
        ReversePower,
        BlockSnow,
        DestroySnow,
        BlockWater,
        DestroyWater,
        BlockFire,
        DestroyFire,
        ConvertSnowToWater,
        ConvertWaterToFire,
        ConvertFireToSnow,
        Add2Power,
        Remove2Power,
        DestroyBlueCard,
        DestroyBlueCardAll,
        DestroyOrangeCard,
        DestroyOrangeCardAll,
        DestroyPurpleCard,
        DestroyPurpleCardAll,
        DestroyRedCard,
        DestroyRedCardAll,
        DestroyYellowCard,
        DestroyYellowCardAll,
        DestroyGreenCard,
        DestroyGreenCardAll,
    }

    public enum EffectDirection
    {
        Both,
        Self,
        Opponent,
    }

    public record CardEffect
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Effect Type { get; init; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EffectDirection Direction { get; init; }
    }
}

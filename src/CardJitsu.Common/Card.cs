using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CardJitsu.Common
{
    public enum Element
    {
        Snow,
        Water,
        Fire,
    }

    public enum Color
    {
        Blue,
        Orange,
        Purple,
        Red,
        Yellow,
        Green,
    }

    public record Card
    {
        public int Id { get; init; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Element Element { get; init; }
        public int Power { get; init; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Color Color { get; init; }
        public CardEffect? Effect { get; init; }
    }
}

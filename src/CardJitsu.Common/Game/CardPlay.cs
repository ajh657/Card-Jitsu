namespace CardJitsu.Common.Game
{
    public class CardPlay(Card card, Effect[] playerEffects)
    {
        public Card OriginalCard { get; private set; } = card;
        public int Id { get; set; } = card.Id;
        public Element Element { get; set; } = card.Element;
        public int Power { get; set; } = card.Power;
        public Color Color { get; set; } = card.Color;
        public Effect[] PlayerEffects { get; init; } = playerEffects;
    }
}

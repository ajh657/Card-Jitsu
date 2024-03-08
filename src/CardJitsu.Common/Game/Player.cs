namespace CardJitsu.Common.Game
{
    public class Player
    {
        public required string Name { get; set; }
        public CardBank CardBank { get; set; } = new CardBank();
        private readonly List<Effect> _effects = [];
        public Effect[] effects
        {
            get { return _effects.ToArray(); }
        }
        private readonly Card[] _hand = new Card[5];
        public Card[] Hand
        {
            get { return _hand; }
        }

        public void AddEffect(Effect effect)
        {
            if (!_effects.Contains(effect))
            {
                _effects.Add(effect);
            }
        }
    }
}

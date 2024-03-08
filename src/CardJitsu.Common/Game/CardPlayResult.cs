namespace CardJitsu.Common.Game
{
    public record CardPlayResult
    {
        public required WinResult Winner { get; init; }
        public required Effect[] P1Effects { get; init; }
        public required Effect[] P2Effects { get; init; }
        public required Effect[] GlobalEffects { get; init; }
    }
}

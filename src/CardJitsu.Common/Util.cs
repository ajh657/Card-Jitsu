using CardJitsu.Common.Game;

namespace CardJitsu.Common
{
    public static class Util
    {
        public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
        {
            return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
        }

        private static readonly Random _random = new();

        public static int RandomInt(int max)
        {
            return _random.Next(0, max + 1);
        }

        public static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_random.Next(v.Length));
        }

        public static Card[] GenerateRandomCards(int count, bool? isPowerCard = null)
        {
            return Enumerable.Range(0, count).Select((x, i) => GenerateRandomCard(i, isPowerCard: isPowerCard)).ToArray();
        }

        public static Card GenerateRandomCard(int index, Element? element = null, Color? color = null, int? power = null, bool? isPowerCard = null, Effect? powerCardEffect = null)
        {
            element ??= RandomEnumValue<Element>();
            color ??= RandomEnumValue<Color>();
            power ??= RandomInt(10);

            if (powerCardEffect != null)
            {
                isPowerCard = true;
            }
            isPowerCard ??= RandomBool();

            CardEffect? cardEffect = null;

            if ((bool)isPowerCard)
            {
                if (powerCardEffect != null)
                {
                    cardEffect = new CardEffect
                    {
                        Type = (Effect)powerCardEffect,
                        Direction = GetEffectDirection((Effect)powerCardEffect)
                    };
                }
                cardEffect = GenerateRandomPowerCard();
            }

            return new Card
            {
                Effect = cardEffect,
                Element = (Element)element,
                Color = (Color)color,
                Power = (int)power,
                Name = $"{color} {element} {power}",
                Id = index
            };
        }

        public static bool RandomBool()
        {
            return RandomInt(1) == 1;
        }

        public static CardEffect GenerateRandomPowerCard()
        {
            var effect = RandomEnumValue<Effect>();
            var direction = GetEffectDirection(RandomEnumValue<Effect>());
            return new CardEffect
            {
                Type = effect,
                Direction = direction
            };
        }

        public static EffectDirection GetEffectDirection(Effect? effect)
        {
            return effect switch
            {
                Effect.BlockSnow or
                Effect.DestroySnow or
                Effect.BlockWater or
                Effect.DestroyWater or
                Effect.BlockFire or
                Effect.DestroyFire or
                Effect.Remove2Power or
                Effect.DestroyBlueCard or
                Effect.DestroyBlueCardAll or
                Effect.DestroyOrangeCard or
                Effect.DestroyOrangeCardAll or
                Effect.DestroyPurpleCard or
                Effect.DestroyPurpleCardAll or
                Effect.DestroyRedCard or
                Effect.DestroyRedCardAll or
                Effect.DestroyYellowCard or
                Effect.DestroyYellowCardAll or
                Effect.DestroyGreenCard or
                Effect.DestroyGreenCardAll
                => EffectDirection.Opponent,

                Effect.Add2Power
                => EffectDirection.Self,

                Effect.ReversePower or
                Effect.ConvertSnowToWater or
                Effect.ConvertWaterToFire or
                Effect.ConvertFireToSnow
                => EffectDirection.Both,
                _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, null)
            };
        }

        public static (CardPlay updatedPlay, Effect[] playerEffects, Effect[] globalEffects) ApplyCardEffects(this CardPlay card, Effect[] PlayerEffects, Effect[] globalEffects)
        {
            var remainingPlayerEffects = PlayerEffects.ToList();
            var remainingGlobalEffects = globalEffects.ToList();
            if (PlayerEffects.Contains(Effect.Add2Power))
            {
                card.Power += 2;
                remainingPlayerEffects.Remove(Effect.Add2Power);
            }
            if (PlayerEffects.Contains(Effect.Remove2Power))
            {
                card.Power -= 2;
                remainingPlayerEffects.Remove(Effect.Remove2Power);
            }
            if (globalEffects.Contains(Effect.ConvertFireToSnow) && card.Element == Element.Fire)
            {
                card.Element = Element.Snow;
                remainingGlobalEffects.Remove(Effect.ConvertFireToSnow);
            }
            if (globalEffects.Contains(Effect.ConvertSnowToWater) && card.Element == Element.Snow)
            {
                card.Element = Element.Water;
                remainingGlobalEffects.Remove(Effect.ConvertSnowToWater);
            }
            if (globalEffects.Contains(Effect.ConvertWaterToFire) && card.Element == Element.Water)
            {
                card.Element = Element.Fire;
                remainingGlobalEffects.Remove(Effect.ConvertWaterToFire);
            }

            return (card, remainingPlayerEffects.ToArray(), remainingGlobalEffects.ToArray());
        }
    }
}

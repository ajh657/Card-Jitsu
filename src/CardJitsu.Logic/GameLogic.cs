using System;
using System.Collections.Generic;
using System.Linq;
using CardJitsu.Common;
using CardJitsu.Common.Game;

namespace CardJitsu.Logic
{
    public class GameLogic
    {

        private readonly Card[] _cardSet;
        private readonly Dictionary<Element, Element> _winRuleSet = new()
        {
            { Element.Water, Element.Fire },
            { Element.Snow, Element.Water},
            { Element.Fire, Element.Snow }
        };

        public GameLogic()
        {
            _cardSet = Util.GenerateRandomCards(Util.RandomInt(50));
        }

        public Card[] DealNewHand()
        {
            return [.. _cardSet.GetRandomElements(5)];
        }

        public CardPlayResult GetPlayWinner(CardPlay p1Play, CardPlay p2Play, Effect[] globalEffects)
        {
            var p1Application = p1Play.ApplyCardEffects(p1Play.PlayerEffects, globalEffects);
            var p2Application = p2Play.ApplyCardEffects(p2Play.PlayerEffects, globalEffects);
            var p1CardPlay = p1Application.updatedPlay;
            var p2CardPlay = p2Application.updatedPlay;
            List<Effect> globalEffectList = [.. p1Application.globalEffects, .. p2Application.globalEffects];
            globalEffectList = globalEffectList.Distinct().ToList();

            if (_winRuleSet[p1CardPlay.Element] == p2CardPlay.Element)
            {
                return new CardPlayResult
                {
                    Winner = WinResult.P1,
                    P1Effects = p1Application.playerEffects,
                    P2Effects = p2Application.playerEffects,
                    GlobalEffects = globalEffectList.ToArray()
                };
            }
            else if (_winRuleSet[p2CardPlay.Element] == p1CardPlay.Element)
            {

                return new CardPlayResult
                {
                    Winner = WinResult.P2,
                    P1Effects = p1Application.playerEffects,
                    P2Effects = p2Application.playerEffects,
                    GlobalEffects = globalEffectList.ToArray()
                };
            }

            var powerPlayResult = WinByPower(p1CardPlay, p2CardPlay);

            if (powerPlayResult == WinResult.Tie)
            {
                return new CardPlayResult()
                {
                    Winner = WinResult.Tie,
                    P1Effects = p1Application.playerEffects,
                    P2Effects = p2Application.playerEffects,
                    GlobalEffects = globalEffectList.ToArray()
                };
            }

            if (globalEffectList.Contains(Effect.ReversePower))
            {
                powerPlayResult = powerPlayResult switch
                {
                    WinResult.P1 => WinResult.P2,
                    WinResult.P2 => WinResult.P1,
                    _ => WinResult.Tie
                };
                globalEffectList.Remove(Effect.ReversePower);
            }

            return new CardPlayResult
            {
                Winner = powerPlayResult,
                P1Effects = p1Application.playerEffects,
                P2Effects = p2Application.playerEffects,
                GlobalEffects = globalEffectList.ToArray()
            };
        }

        private WinResult WinByPower(CardPlay p1CardPlay, CardPlay p2CardPlay)
        {
            if (p1CardPlay.Power > p2CardPlay.Power)
            {
                return WinResult.P1;
            }

            if (p1CardPlay.Power < p2CardPlay.Power)
            {
                return WinResult.P2;
            }

            return WinResult.Tie;
        }

        public (Player? Winner, Player? loser, Effect? globalEffect) ApplyWinningCardEffects(Player winner, CardPlay winningPlay, Player loser)
        {
            var effect = winningPlay.OriginalCard.Effect?.Type;
            var effectDirection = Util.GetEffectDirection(effect);
            Effect? globalEffect = null;

            if (effect == null)
            {
                throw new ArgumentNullException(nameof(effect));
            }

            if (effectDirection == EffectDirection.Self)
            {
                winner.AddEffect((Effect)effect);
            }
            if (effectDirection == EffectDirection.Opponent)
            {
                loser.AddEffect((Effect)effect);
            }
            if (effectDirection == EffectDirection.Both)
            {
                globalEffect = effect;
            }

            return (winner, loser, globalEffect);
        }
    }
}

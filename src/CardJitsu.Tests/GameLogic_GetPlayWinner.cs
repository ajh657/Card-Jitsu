using CardJitsu.Common;
using CardJitsu.Common.Game;
namespace CardJitsu.Logic.Test
{
    [TestFixture]
    public class GameLogic_GetPlayWinner
    {
        private readonly GameLogic _gameLogic = new();

        [Test]
        [TestCase(WinResult.P1, Element.Fire, Element.Snow)]
        [TestCase(WinResult.P1, Element.Water, Element.Fire)]
        [TestCase(WinResult.P1, Element.Snow, Element.Water)]
        [TestCase(WinResult.P2, Element.Snow, Element.Fire)]
        [TestCase(WinResult.P2, Element.Fire, Element.Water)]
        [TestCase(WinResult.P2, Element.Water, Element.Snow)]
        public void GetPlayWinner_CorrentPlayerWinsByElement(WinResult winner, Element p1Element, Element p2Element)
        {
            var firstCard = Util.GenerateRandomCard(1, p1Element);
            var secondCard = Util.GenerateRandomCard(2, p2Element);
            var result = _gameLogic.GetPlayWinner(new CardPlay(firstCard, []), new CardPlay(secondCard, []), []);
            Assert.That(result.Winner, Is.EqualTo(winner));
        }

        [Test]
        [TestCase(WinResult.P1, Element.Fire, Element.Water, Effect.ConvertFireToSnow)]
        [TestCase(WinResult.P2, Element.Water, Element.Fire, Effect.ConvertFireToSnow)]
        [TestCase(WinResult.P1, Element.Snow, Element.Fire, Effect.ConvertSnowToWater)]
        [TestCase(WinResult.P2, Element.Fire, Element.Snow, Effect.ConvertSnowToWater)]
        [TestCase(WinResult.P1, Element.Water, Element.Snow, Effect.ConvertWaterToFire)]
        [TestCase(WinResult.P2, Element.Snow, Element.Water, Effect.ConvertWaterToFire)]
        public void GetPlayWinner_CorrectPlayerWinsByElementWithConversion(WinResult winner, Element p1Element, Element p2Element, Effect conversionEffect)
        {
            var firstCard = Util.GenerateRandomCard(1, p1Element);
            var secondCard = Util.GenerateRandomCard(2, p2Element);
            var result = _gameLogic.GetPlayWinner(new CardPlay(firstCard, []), new CardPlay(secondCard, []), [conversionEffect]);
            Assert.That(result.Winner, Is.EqualTo(winner));
        }

        [Test]
        [TestCase(WinResult.P1, 5, 3)]
        [TestCase(WinResult.P2, 3, 5)]
        [TestCase(WinResult.Tie, 5, 5)]
        public void GetPlayWinner_CorrentPlayerWinsByPower(WinResult winner, int p1Power, int p2Power)
        {
            var firstCard = Util.GenerateRandomCard(1, Element.Fire, power: p1Power);
            var secondCard = Util.GenerateRandomCard(2, Element.Fire, power: p2Power);
            var result = _gameLogic.GetPlayWinner(new CardPlay(firstCard, []), new CardPlay(secondCard, []), []);
            Assert.That(result.Winner, Is.EqualTo(winner));
        }

        [Test]
        [TestCase(WinResult.P1, 3, 5)]
        [TestCase(WinResult.P2, 5, 3)]
        [TestCase(WinResult.Tie, 5, 5)]
        public void GetPlayWinner_CorrentPlayerWinByReversePower(WinResult winner, int p1Power, int p2Power)
        {
            var firstCard = Util.GenerateRandomCard(1, Element.Fire, power: p1Power);
            var secondCard = Util.GenerateRandomCard(2, Element.Fire, power: p2Power);
            var result = _gameLogic.GetPlayWinner(new CardPlay(firstCard, []), new CardPlay(secondCard, []), [Effect.ReversePower]);
            Assert.That(result.Winner, Is.EqualTo(winner));
        }

        [Test]
        public void GetPlayWinner_EffectsAreRemovedFromPlayer()
        {
            var firstCard = Util.GenerateRandomCard(1, Element.Fire);
            var secondCard = Util.GenerateRandomCard(2, Element.Fire);
            var result = _gameLogic.GetPlayWinner(new CardPlay(firstCard, [Effect.Add2Power]), new CardPlay(secondCard, [Effect.Remove2Power]), []);
            Assert.Multiple(() =>
            {
                Assert.That(result.P1Effects, Is.Empty);
                Assert.That(result.P2Effects, Is.Empty);
            });
        }

        [Test]
        public void GetPlayWinner_EffectsAreRemovedFromGlobal()
        {
            var firstCard = Util.GenerateRandomCard(1, Element.Fire);
            var secondCard = Util.GenerateRandomCard(2, Element.Fire);
            var result = _gameLogic.GetPlayWinner(new CardPlay(firstCard, []), new CardPlay(secondCard, []), [Effect.ConvertFireToSnow]);
            Assert.That(result.GlobalEffects, Is.Empty);
        }
    }
}

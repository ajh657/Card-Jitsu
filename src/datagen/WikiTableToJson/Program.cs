using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CardJitsu.Common.Game;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace WikiTableToJson
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var wikiTableString = File.ReadAllText(args[0]);
            var wikiTable = new HtmlDocument();
            wikiTable.LoadHtml(wikiTableString);

            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;

            var cardNodes = wikiTable.DocumentNode.SelectNodes("//table/tbody/tr");

            var cards = new List<Card>();

            foreach (var item in cardNodes)
            {
                var childNodeLength = item.ChildNodes.Where(x => x.NodeType == HtmlNodeType.Element).Count();

                CardEffect? effect = null;

                var name = textInfo.ToTitleCase(item.SelectSingleNode("//table/tbody/tr/td[2]").InnerText);
                var id = int.Parse(item.Id.Split('_')[1]);
                var element = GetElement(item.SelectSingleNode("//table/tbody/tr/td[5]/div/div/a"));
                var power = GetPower(item.SelectSingleNode("//table/tbody/tr/td[6]/div/div/a/img"));
                var color = GetColor(item.SelectSingleNode("//table/tbody/tr/td[7]/div/div/a"));

                if (childNodeLength == 10)
                {
                    effect = GetEffect(item.SelectSingleNode("//table/tbody/tr/td[8]"));
                }

                cards.Add(new Card
                {
                    Id = id,
                    Name = name,
                    Element = element,
                    Power = power,
                    Color = color,
                    Effect = effect
                });
            }

            Console.WriteLine(JsonConvert.SerializeObject(cards, Formatting.Indented));

        }

        public static int GetPower(HtmlNode element)
        {
            var powerAttribute = element.GetAttributeValue<string>("alt", "");

            return int.Parse(powerAttribute.Replace("Burbank ", null));
        }

        public static Element GetElement(HtmlNode element)
        {
            //title="Fire"
            var elementTitleAttribute = element.GetAttributeValue<string>("Title", "");

            if (elementTitleAttribute == "")
            {
                throw new Exception("Element not found");
            }

            return elementTitleAttribute switch
            {
                "Fire" => Element.Fire,
                "Snow" => Element.Snow,
                "Water" => Element.Water,
                _ => throw new Exception("???")
            };
        }

        public static Color GetColor(HtmlNode element)
        {
            var colorTitleAttribute = element.GetAttributeValue<string>("Title", "");

            if (colorTitleAttribute == "")
            {
                throw new Exception("Element not found");
            }

            return colorTitleAttribute switch
            {
                "Blue" => Color.Blue,
                "Orange" => Color.Orange,
                "Purple" => Color.Purple,
                "Red" => Color.Red,
                "Yellow" => Color.Yellow,
                "Green" => Color.Green,
                _ => throw new Exception("???")
            };
        }

        public static CardEffect GetEffect(HtmlNode element)
        {
            var direction = GetEffectDirection(element);
            var type = GetEffectType(element);

            return new CardEffect
            {
                Direction = direction,
                Type = type
            };
        }

        public static EffectDirection GetEffectDirection(HtmlNode element)
        {
            var effectDirection = element.InnerText;

            if (effectDirection.Contains("(opponent)"))
            {
                return EffectDirection.Opponent;
            }

            if (effectDirection.Contains("(user)"))
            {
                return EffectDirection.Self;
            }

            if (effectDirection.Contains("(both players)"))
            {
                return EffectDirection.Both;
            }

            throw new Exception("Effect direction not found");
        }

        public static Effect GetEffectType(HtmlNode element)
        {
            var effectTypeString = element.SelectSingleNode("//table/tbody/tr/td[8]/div/div/a/img").GetAttributeValue<string>("alt", "");

            return effectTypeString.Replace("CJ ", null) switch
            {
                "Power Reversal" => Effect.ReversePower,
                "+2 Power" => Effect.Add2Power,
                "-2 Power" => Effect.Remove2Power,
                "Discard Snow" => Effect.DestroySnow,
                "Discard Water" => Effect.DestroyWater,
                "Discard Fire" => Effect.DestroyFire,
                "Discard Red Card" => Effect.DestroyRedCard,
                "Discard Red Cards" => Effect.DestroyRedCardAll,
                "Discard Blue Card" => Effect.DestroyBlueCard,
                "Discard Blue Cards" => Effect.DestroyBlueCardAll,
                "Discard Green Card" => Effect.DestroyGreenCard,
                "Discard Green Cards" => Effect.DestroyGreenCardAll,
                "Discard Yellow Card" => Effect.DestroyYellowCard,
                "Discard Yellow Cards" => Effect.DestroyYellowCardAll,
                "Discard Orange Card" => Effect.DestroyOrangeCard,
                "Discard Orange Cards" => Effect.DestroyOrangeCardAll,
                "Discard Purple Card" => Effect.DestroyPurpleCard,
                "Discard Purple Cards" => Effect.DestroyPurpleCardAll,
                "Block Snow" => Effect.BlockSnow,
                "Block Water" => Effect.BlockWater,
                "Block Fire" => Effect.BlockFire,
                "Change Water to Fire" => Effect.ConvertWaterToFire,
                "Change Snow to Water" => Effect.ConvertSnowToWater,
                "Change Fire to Snow" => Effect.ConvertFireToSnow,
                _ => throw new Exception("Effect type not found"),
            };
        }
    }
}

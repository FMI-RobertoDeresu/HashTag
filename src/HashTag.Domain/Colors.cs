using System.Collections.Generic;
using System.Linq;

namespace HashTag.Domain
{
    public class Colors
    {
        public const string Grenadine = "#DC4C46";
        public const string Niagara = "#578CA9";
        public const string Kale = "#5A7247";
        public const string Fiesta = "#DD4132";
        public const string Buttercup = "#FAE03C";
        public const string GreenFlash = "#79C753";
        public const string SnorkelBlue = "#034F84";
        public const string AutumnMaple = "#D2691E";
        public const string NeutralGray = "#898E8C";
        public const string Emerald = "#009B77";

        public static int CountAll => All().Count();

        public static IEnumerable<string> All()
        {
            return new[]
            {
                Grenadine,
                Niagara,
                Kale,
                Fiesta,
                Buttercup,
                GreenFlash,
                SnorkelBlue,
                AutumnMaple,
                NeutralGray,
                Emerald
            };
        }
    }
}
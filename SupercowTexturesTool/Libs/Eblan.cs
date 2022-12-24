using System;

namespace EblanModule
{
    public static class Eblan
    {
        public enum EblanType
        { Question, Сonclusion }

        public static string EblanRnd(this string str)
        {
            var rnd = new Random().Next(0, 100);
            return rnd <= 25 ? "Eblan" : str;
        }

        public static string EblanRnd(this string str, int chance)
        {
            var rnd = new Random().Next(0, 100);
            return rnd <= chance ? "Eblan" : str;
        }

        public static string EblanRnd(this string str, EblanType type)
        {
            var eblantext = "Eblan";
            switch (type)
            {
                case EblanType.Question:
                    eblantext += "?";
                    break;
            }
            var rnd = new Random().Next(0, 100);
            return rnd <= 25 ? eblantext : str;
        }

        public static string EblanRnd(this string str, EblanType type, int chance)
        {
            var eblantext = "Eblan";
            switch (type)
            {
                case EblanType.Question:
                    eblantext += "?";
                    break;
            }
            var rnd = new Random().Next(0, 100);
            return rnd <= chance ? eblantext : str;
        }

        public static string EblanRnd(this string str, string customText)
        {
            var rnd = new Random().Next(0, 100);
            return rnd <= 25 ? customText : str;
        }

        public static string EblanRnd(this string str, string customText, int chance)
        {
            var rnd = new Random().Next(0, 100);
            return rnd <= chance ? customText : str;
        }
    }
}
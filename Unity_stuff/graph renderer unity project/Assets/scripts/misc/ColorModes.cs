using System;
using System.Linq;
using UnityEngine;

namespace Assets.scripts.misc
{
    // IMPORTANT: This Class is used to create a WEAKLY TYPED enum, it is convenient, but pay attention to your types!
    // this is generally a bad idea, but it is useful in this case because it allows us to use the same variable for
    // multiple purposes without bloating the vars script or other code
    public class ColorModes
    {
        // you only need to change the three functions below to implement a new color mode, the rest is handled automatically
        // you can skip all three temporarily and the program will run fine, but users will have no idea what is going on
        private static string GetColorModeName(ColorMode colorMode)
        {
            switch (colorMode)
            {
                case ColorMode.NONE:
                    return "NONE";
                case ColorMode.IN_RANGE:
                    return "IN_RANGE";
                case ColorMode.IS_SCANNED:
                    return "IS_SCANNED";
                case ColorMode.URL_LENGTH:
                    return "URL_LENGTH";
                case ColorMode.BY_BRANCH:
                    return "BY_BRANCH";
                case ColorMode.BY_SITE:
                    return "BY_SITE";
                default:
                    return "NOT_DEFINED";
            }
        }

        public static string GetColorModePurpose(ColorMode colorMode)
        {
            switch (colorMode)
            {
                case ColorMode.NONE:
                    return "all white";
                case ColorMode.IN_RANGE:
                    return "nodes in 'physics range' are red (helps with optimization)";
                case ColorMode.IS_SCANNED:
                    return "nodes that have been scanned are blue (helps with seeing 'explored regions')";
                case ColorMode.URL_LENGTH:
                    return "the shorter the URL, the greener (helps detect 'main' sites)";
                case ColorMode.BY_BRANCH:
                    return "colors mutate over connections, (helps detect webpages that are closely connected)";
                case ColorMode.BY_SITE:
                    return "colors generate based on a hash of the website";
                default:
                    return "all white";
            }
        }

        private static Color GetColorModeAssociatedColor(ColorMode colorMode)
        {
            switch (colorMode)
            {
                case ColorMode.NONE:
                    return Color.white;
                case ColorMode.IN_RANGE:
                    return new Color(255f, 37f, 37f);
                case ColorMode.IS_SCANNED:
                    return new Color(138f, 175f, 255f);
                case ColorMode.URL_LENGTH:
                    return new Color(56f, 255f, 79f);
                case ColorMode.BY_BRANCH:
                    return new Color(134f, 41f, 255f);
                case ColorMode.BY_SITE:
                    return new Color(134f, 41f, 255f);
                default:
                    return Color.white;
            }
        }

        // operator overrides
        public static implicit operator ColorMode(ColorModes colorMode) => ColorMode;
        public static implicit operator Color(ColorModes colorMode) => GetColorModeAssociatedColor(ColorMode);
        public static implicit operator string(ColorModes colorMode) => GetColorModeName(ColorMode);
        public static explicit operator int(ColorModes colorMode) => (int)ColorMode;
        public static bool operator ==(ColorModes colorMode, ColorMode colorMode2) => ColorMode == colorMode2;
        public static bool operator !=(ColorModes colorMode, ColorMode colorMode2) => ColorMode != colorMode2;

        public static implicit operator ColorModes(ColorMode colorMode)
        {
            ColorMode = colorMode;
            return new ColorModes();
        }
        public static ColorModes operator++(ColorModes colorMode)
        {
            NextColorMode();
            return colorMode;
        }

        public static ColorModes operator--(ColorModes colorMode)
        {
            PreviousColorMode();
            return colorMode;
        }

        [SerializeField]
        public static ColorMode ColorMode = ColorMode.NONE;
        private static ColorMode[] colorModes = Enum.GetValues(typeof(ColorMode)).Cast<ColorMode>().ToArray();

        public static ColorMode GetColorMode(int idx)
        {
            return colorModes[idx];
        }
        private static int index = 0;
        private static ColorMode NextColorMode()
        {
            index++;
            index %= colorModes.Length;
            ColorMode = colorModes[index];
            return ColorMode;
        }
        private static ColorMode PreviousColorMode()
        {
            index+= colorModes.Length - 1;
            index %= colorModes.Length;
            ColorMode = colorModes[index];
            return ColorMode;
        }

        public static int getNumberOfColorModes()
        {
            return Enum.GetNames(typeof(ColorMode)).Length;
        }
    }
}

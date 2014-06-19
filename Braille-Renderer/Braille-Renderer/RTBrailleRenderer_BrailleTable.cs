using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tud.mci.tangram.Braille_Renderer
{
    public static partial class RTBrailleRendererHelper
    {
        public static bool[,] unicode_to_matrix(String unicode)
        {
            bool[,] matrix;
            switch (unicode)
            {
                case "\\x2800":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { false, false }, { false, false } };
                    return matrix;

                case "\\x2801":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { false, false }, { false, false } };
                    return matrix;

                case "\\x2802":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { false, false }, { false, false } };
                    return matrix;

                case "\\x2803":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { false, false }, { false, false } };
                    return matrix;

                case "\\x2804":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { true, false }, { false, false } };
                    return matrix;

                case "\\x2805":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { true, false }, { false, false } };
                    return matrix;

                case "\\x2806":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { true, false }, { false, false } };
                    return matrix;

                case "\\x2807":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { true, false }, { false, false } };
                    return matrix;

                case "\\x2808":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { false, false }, { false, false } };
                    return matrix;

                case "\\x2809":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { false, false }, { false, false } };
                    return matrix;

                case "\\x280a":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { false, false }, { false, false } };
                    return matrix;

                case "\\x280b":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { false, false }, { false, false } };
                    return matrix;

                case "\\x280c":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { true, false }, { false, false } };
                    return matrix;

                case "\\x280d":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { true, false }, { false, false } };
                    return matrix;

                case "\\x280e":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { true, false }, { false, false } };
                    return matrix;

                case "\\x280f":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { true, false }, { false, false } };
                    return matrix;


                // Ende 1. Spalte ----------------------------------------------------------------------------------------
                case "\\x2810":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { false, false }, { false, false } };
                    return matrix;
                case "\\x2811":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { false, false }, { false, false } };
                    return matrix;
                case "\\x2812":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { false, false }, { false, false } };
                    return matrix;
                case "\\x2813":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { false, false }, { false, false } };
                    return matrix;
                case "\\x2814":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { true, false }, { false, false } };
                    return matrix;
                case "\\x2815":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { true, false }, { false, false } };
                    return matrix;
                case "\\x2816":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { true, false }, { false, false } };
                    return matrix;
                case "\\x2817":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { true, false }, { false, false } };
                    return matrix;
                case "\\x2818":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { false, false }, { false, false } };
                    return matrix;
                case "\\x2819":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { false, false }, { false, false } };
                    return matrix;
                case "\\x281a":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { false, false }, { false, false } };
                    return matrix;
                case "\\x281b":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { false, false }, { false, false } };
                    return matrix;
                case "\\x281c":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { true, false }, { false, false } };
                    return matrix;
                case "\\x281d":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { true, false }, { false, false } };
                    return matrix;
                case "\\x281e":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { true, false }, { false, false } };
                    return matrix;
                case "\\x281f":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { true, false }, { false, false } };
                    return matrix;

                // Ende 2. Spalte --------------------------------------------------------------------------------

                case "\\x2820":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2821":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2822":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2823":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2824":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { true, true }, { false, false } };
                    return matrix;
                case "\\x2825":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { true, true }, { false, false } };
                    return matrix;
                case "\\x2826":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { true, true }, { false, false } };
                    return matrix;
                case "\\x2827":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { true, true }, { false, false } };
                    return matrix;
                case "\\x2828":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2829":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { false, true }, { false, false } };
                    return matrix;
                case "\\x282a":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { false, true }, { false, false } };
                    return matrix;
                case "\\x282b":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { false, true }, { false, false } };
                    return matrix;
                case "\\x282c":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { true, true }, { false, false } };
                    return matrix;
                case "\\x282d":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { true, true }, { false, false } };
                    return matrix;
                case "\\x282e":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { true, true }, { false, false } };
                    return matrix;
                case "\\x282f":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { true, true }, { false, false } };
                    return matrix;

                //Ende 3. Spalte -------------------------------------------------------------------------------------------
                case "\\x2830":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2831":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2832":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2833":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2834":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { true, true }, { false, false } };
                    return matrix;
                case "\\x2835":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { true, true }, { false, false } };
                    return matrix;
                case "\\x2836":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { true, true }, { false, false } };
                    return matrix;
                case "\\x2837":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { true, true }, { false, false } };
                    return matrix;
                case "\\x2838":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { false, true }, { false, false } };
                    return matrix;
                case "\\x2839":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { false, true }, { false, false } };
                    return matrix;
                case "\\x283a":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { false, true }, { false, false } };
                    return matrix;
                case "\\x283b":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { false, true }, { false, false } };
                    return matrix;
                case "\\x283c":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { true, true }, { false, false } };
                    return matrix;
                case "\\x283d":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { true, true }, { false, false } };
                    return matrix;
                case "\\x283e":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { true, true }, { false, false } };
                    return matrix;
                case "\\x283f":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { true, true }, { false, false } };
                    return matrix;

                //Ende 4. Spalte

                case "\\x2840":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2841":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2842":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2843":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2844":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { true, false }, { true, false } };
                    return matrix;
                case "\\x2845":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { true, false }, { true, false } };
                    return matrix;
                case "\\x2846":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { true, false }, { true, false } };
                    return matrix;
                case "\\x2847":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { true, false }, { true, false } };
                    return matrix;
                case "\\x2848":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2849":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { false, false }, { true, false } };
                    return matrix;
                case "\\x284a":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { false, false }, { true, false } };
                    return matrix;
                case "\\x284b":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { false, false }, { true, false } };
                    return matrix;
                case "\\x284c":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { true, false }, { true, false } };
                    return matrix;
                case "\\x284d":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { true, false }, { true, false } };
                    return matrix;
                case "\\x284e":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { true, false }, { true, false } };
                    return matrix;
                case "\\x284f":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { true, false }, { true, false } };
                    return matrix;

                //Ende 5. Spalte

                case "\\x2850":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2851":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2852":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2853":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2854":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { true, false }, { true, false } };
                    return matrix;
                case "\\x2855":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { true, false }, { true, false } };
                    return matrix;
                case "\\x2856":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { true, false }, { true, false } };
                    return matrix;
                case "\\x2857":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { true, false }, { true, false } };
                    return matrix;
                case "\\x2858":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { false, false }, { true, false } };
                    return matrix;
                case "\\x2859":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { false, false }, { true, false } };
                    return matrix;
                case "\\x285a":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { false, false }, { true, false } };
                    return matrix;
                case "\\x285b":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { false, false }, { true, false } };
                    return matrix;
                case "\\x285c":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { true, false }, { true, false } };
                    return matrix;
                case "\\x285d":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { true, false }, { true, false } };
                    return matrix;
                case "\\x285e":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { true, false }, { true, false } };
                    return matrix;
                case "\\x285f":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { true, false }, { true, false } };
                    return matrix;

                // Ende 6. Spalte --------------------------------------------------------------------------
                case "\\x2860":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2861":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2862":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2863":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2864":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { true, true }, { true, false } };
                    return matrix;
                case "\\x2865":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { true, true }, { true, false } };
                    return matrix;
                case "\\x2866":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { true, true }, { true, false } };
                    return matrix;
                case "\\x2867":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { true, true }, { true, false } };
                    return matrix;
                case "\\x2868":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2869":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { false, true }, { true, false } };
                    return matrix;
                case "\\x286a":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { false, true }, { true, false } };
                    return matrix;
                case "\\x286b":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { false, true }, { true, false } };
                    return matrix;
                case "\\x286c":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { true, true }, { true, false } };
                    return matrix;
                case "\\x286d":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { true, true }, { true, false } };
                    return matrix;
                case "\\x286e":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { true, true }, { true, false } };
                    return matrix;
                case "\\x286f":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { true, true }, { true, false } };
                    return matrix;

                //Ende 7. Spalte --------------------------------------------------------------------------------

                case "\\x2870":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2871":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2872":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2873":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2874":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { true, true }, { true, false } };
                    return matrix;
                case "\\x2875":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { true, true }, { true, false } };
                    return matrix;
                case "\\x2876":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { true, true }, { true, false } };
                    return matrix;
                case "\\x2877":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { true, true }, { true, false } };
                    return matrix;
                case "\\x2878":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { false, true }, { true, false } };
                    return matrix;
                case "\\x2879":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { false, true }, { true, false } };
                    return matrix;
                case "\\x287a":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { false, true }, { true, false } };
                    return matrix;
                case "\\x287b":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { false, true }, { true, false } };
                    return matrix;
                case "\\x287c":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { true, true }, { true, false } };
                    return matrix;
                case "\\x287d":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { true, true }, { true, false } };
                    return matrix;
                case "\\x287e":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { true, true }, { true, false } };
                    return matrix;
                case "\\x287f":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { true, true }, { true, false } };
                    return matrix;

                //Ende 8. Spalte ------------------------------------------------------------------------------------

                case "\\x2880":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2881":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2882":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2883":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2884":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { true, false }, { false, true } };
                    return matrix;
                case "\\x2885":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { true, false }, { false, true } };
                    return matrix;
                case "\\x2886":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { true, false }, { false, true } };
                    return matrix;
                case "\\x2887":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { true, false }, { false, true } };
                    return matrix;
                case "\\x2888":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2889":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { false, false }, { false, true } };
                    return matrix;
                case "\\x288a":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { false, false }, { false, true } };
                    return matrix;
                case "\\x288b":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { false, false }, { false, true } };
                    return matrix;
                case "\\x288c":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { true, false }, { false, true } };
                    return matrix;
                case "\\x288d":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { true, false }, { false, true } };
                    return matrix;
                case "\\x288e":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { true, false }, { false, true } };
                    return matrix;
                case "\\x288f":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { true, false }, { false, true } };
                    return matrix;

                //Ende 9. Spalte -------------------------------------------------------------------------------------


                case "\\x2890":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2891":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2892":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2893":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2894":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { true, false }, { false, true } };
                    return matrix;
                case "\\x2895":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { true, false }, { false, true } };
                    return matrix;
                case "\\x2896":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { true, false }, { false, true } };
                    return matrix;
                case "\\x2897":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { true, false }, { false, true } };
                    return matrix;
                case "\\x2898":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { false, false }, { false, true } };
                    return matrix;
                case "\\x2899":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { false, false }, { false, true } };
                    return matrix;
                case "\\x289a":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { false, false }, { false, true } };
                    return matrix;
                case "\\x289b":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { false, false }, { false, true } };
                    return matrix;
                case "\\x289c":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { true, false }, { false, true } };
                    return matrix;
                case "\\x289d":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { true, false }, { false, true } };
                    return matrix;
                case "\\x289e":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { true, false }, { false, true } };
                    return matrix;
                case "\\x289f":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { true, false }, { false, true } };
                    return matrix;

                //Ende 10. Spalte ------------------------------------------------------------------------------------


                case "\\x28a0":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28a1":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28a2":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28a3":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28a4":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28a5":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28a6":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28a7":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28a8":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28a9":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28aa":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28ab":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28ac":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28ad":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28ae":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28af":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { true, true }, { false, true } };
                    return matrix;

                // ENde 11. Spalte ----------------------------------------------------------------------------------


                case "\\x28b0":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28b1":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28b2":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28b3":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28b4":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28b5":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28b6":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28b7":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28b8":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28b9":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28ba":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28bb":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { false, true }, { false, true } };
                    return matrix;
                case "\\x28bc":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28bd":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28be":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { true, true }, { false, true } };
                    return matrix;
                case "\\x28bf":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { true, true }, { false, true } };
                    return matrix;

                //Ende 12. Spalte ----------------------------------------------------------------------------------


                case "\\x28c0":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28c1":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28c2":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28c3":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28c4":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28c5":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28c6":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28c7":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28c8":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28c9":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28ca":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28cb":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28cc":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28cd":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28ce":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28cf":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { true, false }, { true, true } };
                    return matrix;

                //Ende 13. Spalte


                case "\\x28d0":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28d1":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28d2":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28d3":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28d4":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28d5":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28d6":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28d7":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { true, false }, { false, true } };
                    return matrix;
                case "\\x28d8":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28d9":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28da":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28db":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { false, false }, { true, true } };
                    return matrix;
                case "\\x28dc":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28dd":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28de":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { true, false }, { true, true } };
                    return matrix;
                case "\\x28df":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { true, false }, { true, true } };
                    return matrix;

                //Ende 14. Spalte -----------------------------------------------------------------------------------


                case "\\x28e0":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28e1":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28e2":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28e3":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28e4":
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28e5":
                    matrix = new bool[4, 2] { { true, false }, { false, false }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28e6":
                    matrix = new bool[4, 2] { { false, false }, { true, false }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28e7":
                    matrix = new bool[4, 2] { { true, false }, { true, false }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28e8":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28e9":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28ea":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28eb":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28ec":
                    matrix = new bool[4, 2] { { false, true }, { false, false }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28ed":
                    matrix = new bool[4, 2] { { true, true }, { false, false }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28ee":
                    matrix = new bool[4, 2] { { false, true }, { true, false }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28ef":
                    matrix = new bool[4, 2] { { true, true }, { true, false }, { true, true }, { true, true } };
                    return matrix;

                //Ende 15. Spalte ----------------------------------------------------------------------------------


                case "\\x28f0":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28f1":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28f2":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28f3":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28f4":
                    matrix = new bool[4, 2] { { false, false }, { false, true }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28f5":
                    matrix = new bool[4, 2] { { true, false }, { false, true }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28f6":
                    matrix = new bool[4, 2] { { false, false }, { true, true }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28f7":
                    matrix = new bool[4, 2] { { true, false }, { true, true }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28f8":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28f9":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28fa":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28fb":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { false, true }, { true, true } };
                    return matrix;
                case "\\x28fc":
                    matrix = new bool[4, 2] { { false, true }, { false, true }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28fd":
                    matrix = new bool[4, 2] { { true, true }, { false, true }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28fe":
                    matrix = new bool[4, 2] { { false, true }, { true, true }, { true, true }, { true, true } };
                    return matrix;
                case "\\x28ff":
                    matrix = new bool[4, 2] { { true, true }, { true, true }, { true, true }, { true, true } };
                    return matrix;
                default:
                    matrix = new bool[4, 2] { { false, false }, { false, false }, { false, false }, { false, false } };
                    return matrix;

                //Ende 16. Spalte --------------------------------------------------------------------------------------


            }

        }


    }
}

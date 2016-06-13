using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Renderer.BrailleInterpreter
{
    /// <summary>
    /// Interface for Braille interpreters. 
    /// They should transform strings or characters into dot patterns.
    /// </summary>
    public interface IBraileInterpreter
    {
        /// <summary>
        /// Converts a character (e.g. T) to a list of integer (e.g. 2,3,4,5,7) that 
        /// indicates the positions of raised pins in a Braille cell.
        /// </summary>
        /// <param name="c">The character to interpret.</param>
        /// <returns>
        /// a List of integer indicating the raised dots as a position in a Braille cell.
        /// 1 4
        /// 2 5
        /// 3 6
        /// 7 8
        /// </returns>
        List<int> GetDotsFromChar(char c);

        /// <summary>
        /// Gets the dot pattern lists from string.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>
        /// A list of interpreted characters. Each child list of this list 
        /// stands for one Braille cell.
        /// The Braille cell is given as a sublist, containing a list of 
        /// raised pin positions inside a Braille cell.
        /// </returns>
        List<List<int>> GetDotsFromString(String text);


        /// <summary>
        /// Gets the char from a dot pattern. Only one-cell patterns can be interpreted.
        /// </summary>
        /// <param name="dots">The dot pattern to interpret as a list of raised pin-positions 
        /// inside a Braille cell . E.g. 2,3,4,5,7 will become a 'T'</param>
        /// <returns>The correlated character to the requested dot pattern for one Braille cell.</returns>
        char GetCharFromDots(List<int> dots);


        /// <summary>
        /// Gets the string form a list of dot patterns.
        /// Each sublist stands for one Braille cell.
        /// </summary>
        /// <param name="dots">The dot patterns to interpret. 
        /// Each sublist is one Braille cell. The Sublist is a list of raised 
        /// pin positions inside one Braille cell.</param>
        /// <returns>A string of interpreted Braille dot patterns.</returns>
        String GetStringFormDots(List<List<int>> dots);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleRenderer.BrailleInterpreter
{
    public interface IBraileInterpreter
    {

        List<int> GetDotsFromChar(char c);
        List<List<int>> GetDotsFromString(String text);

        char GetCharFromDots(List<int> dots);
        String GetStringFormDots(List<List<int>> dots);

    }
}

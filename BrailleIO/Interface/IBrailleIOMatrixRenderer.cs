using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    interface IBrailleIOMatrixRenderer
    {
        bool[,] renderMatrix(IViewBoxModel view, bool[,] matrix);
    }
}

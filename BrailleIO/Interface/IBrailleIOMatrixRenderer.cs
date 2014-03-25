using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    public interface IBrailleIOMatrixRenderer : IBrailleIOContentRenderer
    {
        bool[,] renderMatrix(IViewBoxModel view, bool[,] matrix);
    }

    public interface IBrailleIOContentRenderer
    {
        bool[,] renderMatrix(IViewBoxModel view, object content);
    }
}

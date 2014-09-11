using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrailleIO.Interface
{
    public interface IBrailleIOAdapterManager
    {
        IBrailleIOAdapter ActiveAdapter
        {
            get;
            set;
        }

        bool AddAdapter(IBrailleIOAdapter adapter);
        bool RemoveAdapter(IBrailleIOAdapter adapter);
        IBrailleIOAdapter[] GetAdapters();
        bool Synchronize(bool[,] matrix);

    }
}

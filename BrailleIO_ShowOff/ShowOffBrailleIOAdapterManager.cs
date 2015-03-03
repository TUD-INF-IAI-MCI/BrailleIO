
using BrailleIO.Interface;
namespace BrailleIO
{
    public class ShowOffBrailleIOAdapterManager : AbstractBrailleIOAdapterManagerBase
    {
        #region Members

        public IBrailleIOShowOffMonitor Monitor { get; private set; }

        #endregion

        public ShowOffBrailleIOAdapterManager()
            : base()
        { init(); }
        public ShowOffBrailleIOAdapterManager(ref BrailleIOMediator io)
            : base(ref io)
        { init(); }

        void init()
        {
            Monitor = new ShowOff();
            IBrailleIOAdapter showOffAdapter = Monitor.InitializeBrailleIO();
            ActiveAdapter = showOffAdapter;
        }
    }
}

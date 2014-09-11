
using BrailleIO.Interface;
namespace BrailleIO
{
    public class ShowOffBrailleIOAdapterManager : AbstractBrailleIOAdapterManagerBase
    {
        public ShowOffBrailleIOAdapterManager()
            : base()
        { init(); }
        public ShowOffBrailleIOAdapterManager(ref BrailleIOMediator io)
            : base(ref io)
        { init(); }

        void init()
        {
            //push all supported devices and map events 
            IBrailleIOAdapterManager me = this;
            Adapters.Add(new BrailleIOAdapter_ShowOff(ref me));
        }
    }
}

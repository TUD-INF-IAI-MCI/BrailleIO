
using BrailleIO.Interface;
namespace BrailleIO
{
    /// <summary>
    /// Basic ApadapterManager automatically sets up a ShowOffAdaper and provide it as a global monitor field.
    /// </summary>
    /// <seealso cref="BrailleIO.AbstractBrailleIOAdapterManagerBase" />
    public class ShowOffBrailleIOAdapterManager : AbstractBrailleIOAdapterManagerBase
    {
        #region Members

        /// <summary>
        /// Gets the automatically instantiated monitor component.
        /// </summary>
        /// <value>
        /// The monitor.
        /// </value>
        public IBrailleIOShowOffMonitor Monitor { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowOffBrailleIOAdapterManager"/> class.
        /// </summary>
        public ShowOffBrailleIOAdapterManager()
            : base()
        { init(); }
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowOffBrailleIOAdapterManager"/> class.
        /// </summary>
        /// <param name="io">The <see cref="BrailleIOMediator"/> this relates to.</param>
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

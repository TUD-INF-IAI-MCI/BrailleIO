
namespace BrailleIO.Dialogs
{
    public interface IDialogComponent
    {
        /// <summary>
        /// Gets or sets the unique identifier for this element.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        string ID { get; }
        /// <summary>
        /// Gets or sets the visible text of this element.
        /// </summary>
        /// <value>
        /// The text to display.
        /// </value>
        string Title { get; set; }
        /// <summary>
        /// Gets or sets the help text for this element.
        /// </summary>
        /// <value>
        /// The help text explaining the usage or behavior.
        /// </value>
        string Help { get; set; }
    }
}

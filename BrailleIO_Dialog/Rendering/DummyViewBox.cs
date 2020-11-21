using BrailleIO.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BrailleIO.Dialogs.Rendering
{
    /// <summary>
    /// Dummy class for manipulating the view boy without accessing the real one.
    /// </summary>
    /// <seealso cref="BrailleIO.Interface.IViewBoxModel" />
    internal class DummyViewBox : IViewBoxModel
    {
        Rectangle _viewBox = new Rectangle();
        /// <summary>
        /// Gets or sets the view box. The viewBox defines the viewBox in size and offset to the content
        /// </summary>
        /// <value>
        /// The view box.
        /// </value>
        Rectangle IViewBoxModel.ViewBox
        {
            get
            {
                return _viewBox;
            }
            set
            {
                _viewBox = value;
            }
        }

        Rectangle _contentBox = new Rectangle();
        /// <summary>
        /// Gets or sets the content box. The real view box.
        /// The space that can be used to show content. It can maximum be the Size of the ViewBox.
        /// Normally it is less. The Size of the ContentBox depends on the size of the ViewBox with respect of margin, border and padding.
        /// </summary>
        /// <value>
        /// The content box.
        /// </value>
        Rectangle IViewBoxModel.ContentBox
        {
            get
            {
                return _contentBox;
            }
            set
            {
                _contentBox = value;
            }
        }

        int _cWidth = 0;
        /// <summary>
        /// Gets or sets the width of the content.
        /// This is used to show the Scrollbars and to estimate the ratio between the content box and the hidden content.
        /// </summary>
        /// <value>
        /// The width of the whole content.
        /// </value>
        int IViewBoxModel.ContentWidth
        {
            get
            {
                return _cWidth;
            }
            set
            {
                _cWidth = value;
            }
        }

        int _cHeight = 0;
        /// <summary>
        /// Gets or sets the height of the content.
        /// This is used to show the Scrollbars and to estimate the ratio between the content box and the hidden content.
        /// </summary>
        /// <value>
        /// The height of the whole content.
        /// </value>
        int IViewBoxModel.ContentHeight
        {
            get
            {
                return _cHeight;
            }
            set
            {
                _cHeight = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyViewBox"/> class.
        /// </summary>
        /// <param name="baseModel">The base model this dummy is copied from.</param>
        public DummyViewBox(IViewBoxModel baseModel)
        {
            if (baseModel != null)
            {
                _contentBox = baseModel.ContentBox;
                _viewBox = baseModel.ViewBox;
                _cWidth = baseModel.ContentWidth;
                _cHeight = baseModel.ContentHeight;
            }
        }

    } 
}

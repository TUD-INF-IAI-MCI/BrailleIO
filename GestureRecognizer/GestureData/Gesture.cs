using System;
using System.Collections.Generic;
using Gestures.Geometrie;
using System.Xml.Serialization;

namespace Gestures.Recognition.GestureData
{   
    
    /// <summary>
    /// Single multitouch gesture, consisting of several <see cref="GestureToken"/>.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Gesture", IsNullable = true)]
    public class Gesture
    {
        #region private fields
        private GestureToken[] token;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Gesture"/> class.
        /// </summary>
        public Gesture()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Gesture"/> class.
        /// </summary>
        /// <param name="token">Array of gestures tokens.</param>
        public Gesture(GestureToken[] token)            
        {
            Tokens = token;
        }
        #endregion

        #region public member

        /// <summary>
        /// Gets the <see cref="GestureToken"/> at the specified index.
        /// </summary>
        /// <value></value>
        [XmlIgnore]
        public GestureToken this[int index]
        {
            get { return token[index]; }
        }

        /// <summary>
        /// Gets the number of the gestures tokens.
        /// </summary>
        /// <value>The number of tokens.</value>
        [XmlAttribute("TokenCount")]
        public int Count { get { return token.Length; } }

        /// <summary>
        /// Gets or sets the tokens the gesture consists of.
        /// </summary>
        /// <value>The tokens.</value>
        [XmlArray("Tokens")]
        [XmlArrayItem("Token", Type = typeof(GestureToken), IsNullable = true)]        
        public GestureToken[] Tokens
        {
            get { return token; }
            set
            {
                this.token = value;
                if (token != null)
                {
                    Sort(this.token);
                }
            }
        }
        #endregion

        #region private members
        private void Sort(GestureToken[] token)
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList(token);
            al.Sort(new TokenComparer());
            for (int i = 0; i < token.Length; i++)
            {
                token[i] = al[i] as GestureToken;
            }
        }
        private class TokenComparer : IComparer<GestureToken>,System.Collections.IComparer
        {
            #region IComparer<GestureToken> Members

            public int Compare(GestureToken x, GestureToken y)
            {
                return DateTime.Compare(x.Samples[0].TimeStamp, y.Samples[0].TimeStamp);
            }

            #endregion

            #region IComparer Members

            public int Compare(object x, object y)
            {
                return Compare(x as GestureToken, y as GestureToken);
            }

            #endregion
        }
        #endregion
    }


    /// <summary>
    /// A gesture composed by the blob tracker and therefore no template but actual input.
    /// </summary>
    public class TrackedGesture : Gesture
    {
        Frame[] frameList;
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedGesture"/> class.
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        /// <param name="frameList">The frame list.</param>
        public TrackedGesture(GestureToken[] tokens, params Frame[] frameList)
            : base(tokens)
        {
            this.frameList = frameList;
        }

        /// <summary>
        /// Gets the frame list.
        /// </summary>
        /// <value>
        /// The frame list.
        /// </value>
        public Frame[] FrameList { get { return frameList; } }

    }
}
using System;
using System.Collections.Generic;
using Gestures.Geometrie;
using System.Xml.Serialization;

namespace Gestures.Recognition.GestureData
{   
    
    /// <summary>
    /// Single multitouch gesture, consisting of several <see cref="GestureToken"/>.
    /// </summary>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
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
                //TODO: destroys own token sorting, so needed anywhere?
                //if (token != null)
                //{
                //    Sort(this.token);
                //}

                //fix ids of potentially sorted tokens, TODO: maybe at sorting stage
                if (token != null)
                {
                    for (int i = 0; i < token.Length; i++)
                    {
                        token[i].Id = i;
                    }
                }
            }
        }
        #endregion

        #region private members
        protected virtual void Sort(GestureToken[] token)
        {
            System.Collections.ArrayList al = new System.Collections.ArrayList(token);
            al.Sort(new TokenComparer());
            for (int i = 0; i < token.Length; i++)
            {
                token[i] = al[i] as GestureToken;
            }
        }
        protected class TokenComparer : IComparer<GestureToken>, System.Collections.IComparer
        {
            #region IComparer<GestureToken> Members

            public virtual int Compare(GestureToken x, GestureToken y)
            {
                return DateTime.Compare(x.Samples[0].TimeStamp, y.Samples[0].TimeStamp);
            }

            #endregion

            #region IComparer Members

            public virtual int Compare(object x, object y)
            {
                return Compare(x as GestureToken, y as GestureToken);
            }

            #endregion
        }
        #endregion
    }



    /// <summary>
    /// Gesture gestureTemplate, actual a gesture with className.
    /// </summary>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    [XmlRoot(ElementName = "GestureTemplate", IsNullable = false)]
    [XmlType("GestureTemplate")]
    public class GestureTemplate : Gesture
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GestureTemplate"/> class.
        /// </summary>
        public GestureTemplate()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureTemplate"/> class.
        /// </summary>
        /// <param name="token">Array of gestures tokens.</param>
        public GestureTemplate(GestureToken[] token)
            : base(token)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureTemplate"/> class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="token">Array of gestures tokens.</param>
        public GestureTemplate(String className, GestureToken[] token)
            :base(token)
        {
            ClassName = className;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GestureTemplate"/> class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="gesture">A gestures instance belonging to specified gesture class.</param>
        public GestureTemplate(String className, Gesture gesture)
            :this(className,gesture.Tokens)
        {
        }

        #endregion

        /// <summary>
        /// Gets or sets the name of the gestures corresponding class.
        /// </summary>
        /// <value>The name of the class.</value>
        [System.Xml.Serialization.XmlAttribute()]
        public String ClassName{get;set;}

        /// <summary>
        /// Internal number to distinguish templates within same classes.
        /// </summary>
        public int TemplateNumber { get; set; }
    }


    /// <summary>
    /// </summary>
    /// <author>Dr. rer. nat. Michael Schmidt - Techniche Universität Dresden 2014.</author>
    /// <seealso cref="Gestures.Recognition.GestureData.Gesture" />
    public class TrackedGesture : Gesture
    {
        Frame[] frameList;
        public TrackedGesture(GestureToken[] tokens, params Frame[] frameList)
            : base(tokens)
        {
            this.frameList = frameList;
        }

        public Frame[] FrameList { get { return frameList; } }

    }
}
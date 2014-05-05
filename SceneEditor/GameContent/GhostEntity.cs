using KryptonEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuEditor.GameContent
{
    class GhostEntity : BaseObject
    {

        #region Singleton
        private GhostEntity mInstance;
        public GhostEntity Instance { get { if (mInstance != null) mInstance = new GhostEntity(); return mInstance; } }
        #endregion

        #region Properties

        private TiledSprite mSprite;
        private SpineObject mSpineObject;
        #endregion

        #region Getter & Setter
        #endregion

        #region Constructor
        public GhostEntity() { }
        #endregion

        #region Override Methods

        public override void Update()
        {
            
        }
        #endregion

        #region Methods
        #endregion
    }
}

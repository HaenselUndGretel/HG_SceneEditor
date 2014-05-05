using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MenuEditor.GameContent.Interface
{
    class SaveLoadWindow : Window
    {
        #region Porperties

        #endregion

        #region Getter & Setter

        #endregion

        #region Constructor

        public SaveLoadWindow(Vector2 pPosition, String pName, Vector2 pSize)
            : base(pPosition, pName, pSize)
        {
        }
        #endregion

        #region Override Methods

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
            base.Draw(spriteBatch);

        }
        #endregion

        #region Methods

        protected override void SortEntitesOnScreen(Vector2 pMoveOffset)
        {

        }
        #endregion
    }
}

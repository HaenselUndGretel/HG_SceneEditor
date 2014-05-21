using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using KryptonEngine.Entities;

namespace MenuEditor.GameContent.Interface
{
    class InfoBox : Box
    {
        #region Properties

        private const int FONT_PADDING_LEFT = 10;
        String mBoxName;
        //BaseObject mInfoObj;
        #endregion

        #region Getter & Setter
        //public BaseObject InfoObject { set { mInfoObj = value; } }
        #endregion

        #region Constructor
        public InfoBox(Vector2 pPosition, Rectangle pRectangle)
            : base(pPosition, pRectangle)
        {
            mBoxName = "Infobox:";
        }
        #endregion

        #region Methods

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!IsVisible) return;

            spriteBatch.DrawString(font, mBoxName, new Vector2(PositionX + FONT_PADDING_LEFT, PositionY + 5), Color.Black);
            if (GameLogic.SelectedEntity != null)
                spriteBatch.DrawString(font, GameLogic.SelectedEntity.GetInfo(), new Vector2(PositionX + FONT_PADDING_LEFT, PositionY + 30), Color.Black);

			spriteBatch.DrawString(font, "EditorState: " + GameLogic.EState, new Vector2(PositionX + FONT_PADDING_LEFT, PositionY + mCollisionBox.Height - 50), Color.Black);
            spriteBatch.DrawString(font, "Aktuelle Ebene: " + GameLogic.ParallaxLayerNow, new Vector2(PositionX + FONT_PADDING_LEFT, PositionY + mCollisionBox.Height - 30), Color.Black);
        }
        #endregion
    }
}

using Microsoft.Xna.Framework;
using SceneEditor.GameContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuEditor.GameContent
{
    public static class GameLogic
    {
        #region Properties

        private static int mParalllaxNow = 1;

        public static int ParallaxLayerNow
        {
            get { return mParalllaxNow; }
            set
            {
                if (value == -1)
                    value = 0;
                else if (value == 5)
                    value = 4;

                GameLogic.SelectEntityRectangle.IsSelected = false;
                GameLogic.SelectedEntity = null;
                mParalllaxNow = value;
            }
        }

        public static void Initialize()
        {
            SelectEntityRectangle = new Interface.SelectRectangle(Vector2.Zero, Rectangle.Empty);
        }

		public static MenuEditor.GameContent.Interface.ImageBox.Data GhostData;
        public static MenuEditor.GameContent.Interface.SelectRectangle SelectEntityRectangle;
        public static KryptonEngine.Entities.GameObject SelectedEntity; 
		public static EditorState EState;
        #endregion
    }
}

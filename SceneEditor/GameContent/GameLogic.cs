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
                mParalllaxNow = value;
            }
        }

		public static MenuEditor.GameContent.Interface.ImageBox.Data GhostData;
		public static EditorState EState;
        #endregion
    }
}

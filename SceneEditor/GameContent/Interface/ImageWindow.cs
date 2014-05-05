using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.Interface;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using KryptonEngine.Entities;
using KryptonEngine.Manager;

namespace MenuEditor.GameContent.Interface
{
    public class ImageWindow : Window
    {
        private struct Data
        {
            public Thumbnail Texture;
            public int Index;
        }

        #region Porperties

        private const int THUMBNAIL_WIDTH = 64;
        private const int THUMBNAIL_HEIGHT = 64;
        private List<Data> mEntity = new List<Data>();
        #endregion

        #region Getter & Setter
        #endregion

        #region Constructor

        public ImageWindow(Vector2 pPosition, String pName, Vector2 pSize)
            : base(pPosition, pName, pSize)
        {
            SortEntitesOnScreen(Vector2.Zero);
        }
        #endregion

        #region Override Methods

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
            base.Draw(spriteBatch);

            DrawThumbnails(spriteBatch);
        }
        #endregion

        #region Methods

        protected override void SortEntitesOnScreen(Vector2 pMoveOffset)
        {
            mEntity.Clear();

            Dictionary<string, Texture2D> Resourcen = TextureManager.Instance.GetAllEntitys();
            int posX = 10;
            int posY = 10;
            int index = 0;

            int EntityInRow = (int)(mWindowDimension.Y - 20) / THUMBNAIL_HEIGHT;
            int EntityInColumn = (int)(mWindowDimension.X - 20) / THUMBNAIL_WIDTH;

            for (int j = 0; j < EntityInRow; j++)
            {
                for (int k = 0; k < EntityInColumn; k++)
                {
                    if (Resourcen.Count > index)
                    {
                        Data tmpData = new Data();
                        tmpData.Texture = new Thumbnail(Position + new Vector2(posX, posY), Resourcen.ElementAt(index).Key);
                        tmpData.Index = index;

                        mEntity.Add(tmpData);
                        posX += THUMBNAIL_WIDTH + 10;
                        index++;
                    }
                    else
                        break;
                }
                if (Resourcen.Count > index)
                {
                    posX = 10;
                    posY += THUMBNAIL_HEIGHT;
                }
                else
                    break;
            }
        }

        private void DrawThumbnails(SpriteBatch spriteBatch)
        {
            foreach (Data d in mEntity)
                d.Texture.Draw(spriteBatch);
        }
        #endregion
    }
}

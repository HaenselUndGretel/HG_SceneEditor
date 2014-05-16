using KryptonEngine.Controls;
using KryptonEngine.Entities;
using KryptonEngine.Interface;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SceneEditor.GameContent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MenuEditor.GameContent.Interface
{
    public class ImageBox : Box
    {
        public struct Data
        {
            public Thumbnail Texture;
			public int Index;
            public String Name;
        }

        private class SelectRectangle
        {
            public Vector2 Position;

            private Rectangle Top;
            private Rectangle Bottom;
            private Rectangle Left;
            private Rectangle Right;
            private Rectangle Select;
            private Texture2D mTexture;

            public SelectRectangle(Vector2 pPosition, Rectangle pRectangleSize)
            {
                Position = pPosition;

                // Many Magic Numbers !!!
                Top = new Rectangle(0, 0, pRectangleSize.Width-2, 1);
                Bottom = new Rectangle(0, pRectangleSize.Height-2, pRectangleSize.Width-1, 1);
                Left = new Rectangle(0, 0, 1, pRectangleSize.Height-2);
                Right = new Rectangle(pRectangleSize.Width-2, 0, 1, pRectangleSize.Height-2);
                Select = new Rectangle(1 , 1, pRectangleSize.Width-3, pRectangleSize.Height-3);

                mTexture = TextureManager.Instance.GetElementByString("pixel");
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(mTexture, new Vector2(Position.X + Top.X, Position.Y + Top.Y), Top, Color.Blue);
                spriteBatch.Draw(mTexture, new Vector2(Position.X + Bottom.X, Position.Y + Bottom.Y), Bottom, Color.Blue);
                spriteBatch.Draw(mTexture, new Vector2(Position.X + Left.X, Position.Y + Left.Y), Left, Color.Blue);
                spriteBatch.Draw(mTexture, new Vector2(Position.X + Right.X, Position.Y + Right.Y), Right, Color.Blue);
                spriteBatch.Draw(mTexture, new Vector2(Position.X + Select.X, Position.Y + Select.Y), Select, Color.Red * 0.5f);
            }
        }

        #region Porperties
        private static SelectRectangle mSelectRectangle;
        private bool mIsSelected;
        private List<Data> mEntity = new List<Data>();
        #endregion

        #region Getter & Setter
		public Data SelectedEntity;
        #endregion

        #region Constructor

        public ImageBox(Vector2 pPosition, Rectangle pSize)
            : base(pPosition, pSize)
        {
			mCollisionBox = pSize;
			mSelectRectangle = new SelectRectangle(Position, new Rectangle(0, 0, Thumbnail.THUMBNAIL_WIDTH + 2, Thumbnail.THUMBNAIL_HEIGHT + 2));
            SortEntitesOnScreen(Vector2.Zero);

        }
        #endregion

        #region Override Methods

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
            base.Draw(spriteBatch);

            DrawThumbnails(spriteBatch);
            if(mIsSelected)
                mSelectRectangle.Draw(spriteBatch);
        }

        public override void Update()
        {
            if (mDrawRectangle.Contains(MouseHelper.PositionPoint) && MouseHelper.Instance.IsClickedLeft)
            {
                foreach(Data d in mEntity)
                {
					if (d.Texture.CollisionBox.Contains(MouseHelper.PositionPoint) && MouseHelper.Instance.IsClickedLeft)
					{
						mSelectRectangle.Position = d.Texture.Position;
						mIsSelected = true;
						GameLogic.GhostData = d;
						if (d.Name == "IconMoveArea")
							GameLogic.EState = EditorState.PlaceWayPoint;
						else if (InteractiveObjectDataManager.Instance.HasElement(d.Name))
							GameLogic.EState = EditorState.PlaceInteractiveObject;
						else
							GameLogic.EState = EditorState.PlaceSprites;
						break;
					}
					else
					{
						mIsSelected = false;
						GameLogic.EState = EditorState.Standard;
					}
                }
                MouseHelper.ResetClick();
            }
 
        }
        #endregion

        #region Methods

        protected void SortEntitesOnScreen(Vector2 pMoveOffset)
        {
            mEntity.Clear();

			Dictionary<string, Texture2D> Resourcen = TextureManager.Instance.GetAllGameEntities();
            int posX = 10;
            int posY = 10;
            int index = 0;

            int EntityInRow = (mCollisionBox.Height - 20) / Thumbnail.THUMBNAIL_HEIGHT;
			int EntityInColumn = (mCollisionBox.Width - 20) / Thumbnail.THUMBNAIL_WIDTH;

            for (int j = 0; j < EntityInRow; j++)
            {
                for (int k = 0; k < EntityInColumn; k++)
                {
                    if (Resourcen.Count > index)
                    {
                        Data tmpData = new Data();
                        tmpData.Texture = new Thumbnail(Position + new Vector2(posX, posY), Resourcen.ElementAt(index).Key);
						tmpData.Name = tmpData.Texture.TextureName;
                        tmpData.Index = index;

                        mEntity.Add(tmpData);
						posX += Thumbnail.THUMBNAIL_WIDTH + 10;
                        index++;
                    }
                    else
                        break;
                }
                if (Resourcen.Count > index)
                {
                    posX = 10;
					posY += Thumbnail.THUMBNAIL_HEIGHT;
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

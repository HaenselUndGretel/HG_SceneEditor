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
    public class SelectRectangle
    {
        public Vector2 Position;

        private Rectangle Top;
        private Rectangle Bottom;
        private Rectangle Left;
        private Rectangle Right;
        private Rectangle Select;
        private Texture2D mTexture;

        private bool mIsSelected;
        public bool IsSelected { get { return mIsSelected; } set { mIsSelected = value; } }

        public SelectRectangle(Vector2 pPosition, Rectangle pRectangleSize)
        {
            Position = pPosition;

            // Many Magic Numbers !!!
            Top = new Rectangle(-1, -1, pRectangleSize.Width + 2, 1);
            Bottom = new Rectangle(-1, pRectangleSize.Height , pRectangleSize.Width + 2, 1);
            Left = new Rectangle(-1, -1, 1, pRectangleSize.Height + 1);
            Right = new Rectangle(pRectangleSize.Width, -1, 1, pRectangleSize.Height + 1);
            Select = new Rectangle(0, 0, pRectangleSize.Width, pRectangleSize.Height);

            IsSelected = true;
            mTexture = TextureManager.Instance.GetElementByString("pixel");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Vector2(Position.X + Top.X, Position.Y + Top.Y), Top, Color.Blue);
            spriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Vector2(Position.X + Bottom.X, Position.Y + Bottom.Y), Bottom, Color.Blue);
            spriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Vector2(Position.X + Left.X, Position.Y + Left.Y), Left, Color.Blue);
            spriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Vector2(Position.X + Right.X, Position.Y + Right.Y), Right, Color.Blue);
            spriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Vector2(Position.X + Select.X, Position.Y + Select.Y), Select, Color.Red * 0.5f);
        }
    }

    public class ImageBox : Box
    {
        public struct Data
        {
            public Thumbnail Texture;
			public int Index;
            public String Name;
        }

        #region Porperties
        private List<Data> mEntity = new List<Data>();
		private const int THUMBNAIL_PADDING = 10;
        #endregion

        #region Getter & Setter
		public Data SelectedEntity;
        public SelectRectangle SelectRectangleObject;
        #endregion

        #region Constructor

        public ImageBox(Vector2 pPosition, Rectangle pSize)
            : base(pPosition, pSize)
        {
			mCollisionBox = pSize;
			SelectRectangleObject = new SelectRectangle(Position, new Rectangle(0, 0, Thumbnail.THUMBNAIL_WIDTH, Thumbnail.THUMBNAIL_HEIGHT));
            SortEntitesOnScreen(Vector2.Zero);
            SelectRectangleObject.IsSelected = false;
        }
        #endregion

        #region Override Methods

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
            base.Draw(spriteBatch);

            DrawThumbnails(spriteBatch);
            if(SelectRectangleObject.IsSelected)
                SelectRectangleObject.Draw(spriteBatch);
        }

        public override void Update()
        {
            if (mDrawRectangle.Contains(MouseHelper.PositionPoint) && MouseHelper.Instance.IsClickedLeft)
            {
                foreach(Data d in mEntity)
                {
					if (d.Texture.CollisionBox.Contains(MouseHelper.PositionPoint) && MouseHelper.Instance.IsClickedLeft)
					{
						SelectRectangleObject.Position = d.Texture.Position;
                        SelectRectangleObject.IsSelected = true;
						GameLogic.GhostData = d;
						if (d.Name == "IconMoveArea")
							GameLogic.EState = EditorState.PlaceWayPoint;
						else if (d.Name == "IconEventArea")
							GameLogic.EState = EditorState.PlaceEventArea;
						else if (d.Name.Contains("Enemy"))
							GameLogic.EState = EditorState.PlaceEnemy;
						else if (d.Name.Contains("Collectable"))
							GameLogic.EState = EditorState.PlaceCollectable;
						else if (d.Name.Contains("Item"))
							GameLogic.EState = EditorState.PlaceItem;
						else if (d.Name.Contains("Light"))
							GameLogic.EState = EditorState.PlaceLight;
						else if (InteractiveObjectDataManager.Instance.HasElement(d.Name))
							GameLogic.EState = EditorState.PlaceInteractiveObject;
						else if (d.Name.Contains("Ground"))
							GameLogic.EState = EditorState.PlaceGround;

						else
							GameLogic.EState = EditorState.PlaceSprites;
						break;
                    }
					else
					{
						SelectRectangleObject.IsSelected = false;
                        GameLogic.SelectedEntity = null;
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

			Dictionary<string, Texture2D> Resourcen = TextureManager.Instance.GetAllEntetiesWithout(new List<string>() { "pixel", "Normal", "Depth" });
			int posX = THUMBNAIL_PADDING;
			int posY = THUMBNAIL_PADDING;
            int index = 0;


			while(Resourcen.Count > index)
			{
				while(posX < mDrawRectangle.Width - Thumbnail.THUMBNAIL_WIDTH)
				{
					if (Resourcen.Count > index)
					{
						Data tmpData = new Data();
						tmpData.Texture = new Thumbnail(Position + new Vector2(posX, posY), Resourcen.ElementAt(index).Key);
						tmpData.Name = tmpData.Texture.TextureName;
						tmpData.Index = index;

						mEntity.Add(tmpData);
						posX += Thumbnail.THUMBNAIL_WIDTH + THUMBNAIL_PADDING;
						index++;
					}
					else
						break;
				}
				posX = 10;
				posY += Thumbnail.THUMBNAIL_HEIGHT + THUMBNAIL_PADDING;
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

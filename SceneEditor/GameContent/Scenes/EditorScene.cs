using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KryptonEngine.SceneManagement;
using KryptonEngine.Manager;
using Microsoft.Xna.Framework;
using KryptonEngine.Entities;
using KryptonEngine.Interface;
using KryptonEngine;
using Microsoft.Xna.Framework.Graphics;
using KryptonEngine.Controls;
using MenuEditor.GameContent.Interface;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using HanselAndGretel.Data;
using SceneEditor.GameContent;
using Microsoft.Xna.Framework.Input;

namespace MenuEditor.GameContent.Scenes
{
    class EditorScene : Scene
    {
        #region Properties
		protected SceneData mlevelSceneData;

        /// <summary>
        /// Hier sind Objekte für die Scene
        /// </summary>

        private List<InterfaceObject> mInterfaceObjects = new List<InterfaceObject>();
        private DropDownMenue mRightClickDropDown;

		private bool[] mPlaneVisible;
        private SpriteFont mDebugFont;

        private FormNewGame mFormNewGame;

        private InfoBox mObjectInfoBox;
        private ImageBox mObjectBox;
        private Box mIconBox;


		#region MoveArea
		private bool IsDrawingRectangle;
		private Vector2 mRectangleSelectPos1;
		private Vector2 mRectangleSelectPos2;
		private Rectangle tmpRectangle;
		#endregion

		#region MouseMove

		private const float SPEED_CAMERA_MOVE = 15.0f;
        private Vector2 mMouseCenter;
        #endregion
        #endregion

        #region Getter & Setter

        #endregion

        #region Constructor

        public EditorScene(String pSceneName)
            : base(pSceneName)
        {
			mCamera = new Camera(new Vector2(0, 20));
			mCamera.GameScreen = new Rectangle(0, 0, 1280, 720);
			mCamera.BoundSize = 200;
			mlevelSceneData = new SceneData();
			
        }
        #endregion

        #region Override Methods

        public override void Initialize()
        {
			mlevelSceneData.Initialize();
			mlevelSceneData.GamePlane = mCamera.GameScreen;
			mClearColor = Color.Red;

            mMouseCenter = Vector2.Zero;
			GameLogic.EState = EditorState.Standard;

			mPlaneVisible = new bool[5] { true, true, true, true, true };
        }

        public override void LoadContent()
        {
            // Images
			mRightClickDropDown = new DropDownMenue(Vector2.Zero, new List<string>() { "" }, new List<Action>() { });

            /// Relevante Sachen für HUD 

            mObjectBox = new ImageBox(new Vector2(EngineSettings.VirtualResWidth - 1440 - 1, 1), new Rectangle(0, 0, 1440 - 440, EngineSettings.VirtualResHeight - 2));
            mObjectInfoBox = new InfoBox(new Vector2(EngineSettings.VirtualResWidth - 440 - 1, 1), new Rectangle(0, 0, 440, (EngineSettings.VirtualResHeight / 2) - 2));
            mObjectInfoBox.BackgroundColor = Color.DarkGray;
            mIconBox = new Box(new Vector2(EngineSettings.VirtualResWidth - 440, EngineSettings.VirtualResHeight / 2 + 1), new Rectangle(0, 0, 440, (EngineSettings.VirtualResHeight / 2) - 2));
            DropDownMenue Main = new DropDownMenue(Vector2.Zero, 
				new List<string>() { "Neu...", "Laden", "Speichern unter...", "DebugMode" }, 
				new List<Action>() { ShowFormNewGame, LoadScene, SaveScene, SetDebugMode });
			DropDownMenue Ebenen = new DropDownMenue(Vector2.Zero, 
				new List<string>() { "Alle Ebenen anzeigen", "Ebene 1", "Ebene 2", "Ebene 3", "Ebene 4", "Ebene 5", "Wegsetzding(PathStuff)" }, 
				new List<Action>() { SetAllPlanesVisible, SetPlane0Visible, SetPlane1Visible, SetPlane2Visible, SetPlane3Visible, SetPlane4Visible, SetMoveRectangleState });

			MenueBar mMenuBarTop = new MenueBar(Vector2.Zero);
            mMenuBarTop.AddMenueItem("Datei", Main);
            mMenuBarTop.AddMenueItem("Ebenen", Ebenen);
            mMenuBarTop.OrganizeButtons();

            mFormNewGame = new FormNewGame();

			mDebugFont = FontManager.Instance.GetElementByString("font");

            // Addet alle InterfaceObjekte in die InterfaceObject Liste
            mInterfaceObjects.Add(mMenuBarTop);
            mInterfaceObjects.Add(mRightClickDropDown);
			mInterfaceObjects.Add(mObjectInfoBox);
            mInterfaceObjects.Add(mIconBox);
            mInterfaceObjects.Add(mObjectBox);
        }

        public override void Update()
        {
            foreach (InterfaceObject io in mInterfaceObjects)
                io.Update();

            UpdateMouseInput();
        }

        public override void Draw()
        {
            EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mRenderTarget);
            
            DrawBackground();

            mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null , mCamera.GetTranslationMatrix());
            mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), mlevelSceneData.GamePlane, Color.SpringGreen);
            mSpriteBatch.End();

			DrawPlanes();
			DrawMoveRectangle();

			switch(GameLogic.EState)
			{
				case EditorState.PlaceWayPoint:
					DrawWayPoints();
					break;
			}

            DrawMenueItems();

            DrawOnScene();
        }

        #endregion

        #region Methods

		#region DrawMethods
		public void DrawMenueItems()
        {
            mSpriteBatch.Begin();
			foreach (InterfaceObject io in mInterfaceObjects)
				io.Draw(mSpriteBatch);

            Vector2 msPos = MouseHelper.Position - mCamera.Position;
            mSpriteBatch.DrawString(mDebugFont, "MousePosition: " + msPos, new Vector2(0, 20), Color.White);
			mSpriteBatch.DrawString(mDebugFont, "Rectangle Position: " + tmpRectangle, new Vector2(0, 40), Color.White);
			mSpriteBatch.DrawString(mDebugFont, "EditorState: " + GameLogic.EState, new Vector2(0, 60), Color.White);
            mRightClickDropDown.Draw(mSpriteBatch);
            
            // MouseMoveIcon
            if(mMouseCenter != Vector2.Zero)
              mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), mMouseCenter, new Rectangle(-2, -2, 5, 5), Color.Black);

            mSpriteBatch.End();
        }

		private void DrawWayPoints()
		{
			mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
			foreach (Waypoint w in mlevelSceneData.Waypoints)
				w.Draw(mSpriteBatch);
			mSpriteBatch.End();
		}

		private void DrawGhostEntity()
		{
			if (GameLogic.GhostData.Texture != null)
			{
				mSpriteBatch.Begin();
				mSpriteBatch.Draw(GameLogic.GhostData.Texture.Texture, MouseHelper.Position - GameLogic.GhostData.Texture.Origin, Color.White);
				mSpriteBatch.End();
			}
		}

		public void DrawPlanes()
		{

			for (int i = 4; i >= 0; i-- )
			{
				if(mPlaneVisible[i])
				{
					mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
					foreach (GameObject go in mlevelSceneData.ParallaxPlanes[i])
						go.Draw(mSpriteBatch);
					mSpriteBatch.End();
				}

				if (GameLogic.ParallaxLayerNow == i)
					DrawGhostEntity();
			}
		}

		private void DrawMoveRectangle()
		{
			mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
			if(IsDrawingRectangle)
				mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), tmpRectangle, Color.White);
			foreach (Rectangle r in mlevelSceneData.MoveArea)
				mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), r, Color.DarkRed * 0.8f);
			mSpriteBatch.End();
		}

		#endregion
		public void UpdateMouseInput()
        {
            if (MouseHelper.Instance.IsPressedMiddle)
                MoveCameraByMouse();
            else
                mMouseCenter = Vector2.Zero;

			if(MouseHelper.Instance.IsClickedLeft)
				UpdateSelectedEntity();
			
			if (GameLogic.EState != EditorState.PlaceMoveArea)
			{
				if (MouseHelper.Instance.IsWheelUp)
					GameLogic.ParallaxLayerNow++;
				if (MouseHelper.Instance.IsWheelDown)
					GameLogic.ParallaxLayerNow--;
			}

			switch(GameLogic.EState)
			{ 
				case EditorState.Standard:
					break;
				case EditorState.PlaceMoveArea:
					UpdateRectInput();
					if(MouseHelper.Instance.IsClickedRight)
						UpdateMoveRectDelete();
					break;
				case EditorState.PlaceWayPoint:
					UpdateRectInput();
					if(MouseHelper.Instance.IsClickedRight)
						UpdateMoveRectDelete();
					break;
				case EditorState.PlaceInteractiveObject:
					if (MouseHelper.Instance.IsClickedLeft)
						PlaceInteractiveObject();
					break;
				case EditorState.PlaceSprites:
					if (MouseHelper.Instance.IsClickedLeft)
						PlaceSprite();
					break;
			}
        }

		private void UpdateRectInput()
		{
			Vector2 msPos = MouseHelper.Position - mCamera.Position;
			if (!IsDrawingRectangle && MouseHelper.Instance.IsPressedLeft)
			{
				IsDrawingRectangle = true;
				mRectangleSelectPos1 = new Vector2(msPos.X, msPos.Y);
			}

			if (IsDrawingRectangle && MouseHelper.Instance.IsPressedLeft)
			{
				mRectangleSelectPos2 = new Vector2(msPos.X, msPos.Y);
				tmpRectangle = new Rectangle((int)mRectangleSelectPos1.X, (int)mRectangleSelectPos1.Y, (int)(mRectangleSelectPos2.X - mRectangleSelectPos1.X), (int)(mRectangleSelectPos2.Y - mRectangleSelectPos1.Y));
			}

			if (IsDrawingRectangle && MouseHelper.Instance.IsReleasedLeft)
			{
				if(!tmpRectangle.IsEmpty)
					switch(GameLogic.EState)
					{
						case EditorState.PlaceMoveArea:
							mlevelSceneData.MoveArea.Add(tmpRectangle);
							break;
						case EditorState.PlaceWayPoint:
							Waypoint w = new Waypoint();
							w.CollisionBox = tmpRectangle;
							w.Texture = GameLogic.GhostData.Texture.Texture;
							mlevelSceneData.Waypoints.Add(w);
							break;
					}
				IsDrawingRectangle = false;
			}
		}

		private void UpdateMoveRectDelete()
		{
			int delRect = -1;
			for (int i = 0; i < mlevelSceneData.MoveArea.Count; i++ )
				if (mlevelSceneData.MoveArea[i].Contains(MouseHelper.PositionPoint))
					delRect = i;
			if (delRect != -1)
				mlevelSceneData.MoveArea.RemoveAt(delRect);
		}

		private void UpdateGameObjectDelete(List<GameObject> go)
		{
			int delRect = -1;
			for (int i = 0; i < go.Count; i++)
				if (go[i].CollisionBox.Contains(MouseHelper.PositionPoint))
					delRect = i;
			if (delRect != -1)
				go.RemoveAt(delRect);
		}

		private void PlaceInteractiveObject()
		{
			InteractiveObject ioDefault = new InteractiveObject();
			InteractiveObject ioNew = new InteractiveObject();
			ioDefault = InteractiveObjectDataManager.Instance.GetElementByString(GameLogic.GhostData.Name);

			MouseState ms = Mouse.GetState();

			ioNew.Texture = ioDefault.Texture;
			ioNew.TextureName = ioDefault.TextureName;
			ioNew.Position = MouseHelper.Position - new Vector2(ioNew.Texture.Width / 2, ioNew.Texture.Height / 2) - mCamera.Position;

			for (int i = 0; i < ioDefault.ActionRectList.Count; i++)
			{
				ioNew.ActionRectList.Add(new Rectangle
					(ioDefault.ActionRectList[i].X + ioNew.PositionX,// - ioNew.Texture.Width / 2,// - (int)mCamera.Position.X,
					 ioDefault.ActionRectList[i].Y + ioNew.PositionY,// - ioNew.Texture.Height / 2,// - (int)mCamera.Position.Y,
					 ioDefault.ActionRectList[i].Width,
					 ioDefault.ActionRectList[i].Height));
			}

			for (int i = 0; i < ioDefault.CollisionRectList.Count; i++)
			{
				ioNew.CollisionRectList.Add(new Rectangle
					(ioDefault.CollisionRectList[i].X + ioNew.PositionX,// - ioNew.Texture.Width / 2,// - (int)mCamera.Position.X,
					 ioDefault.CollisionRectList[i].Y + ioNew.PositionY,// - ioNew.Texture.Height / 2,// - (int)mCamera.Position.Y,
					 ioDefault.CollisionRectList[i].Width,
					 ioDefault.CollisionRectList[i].Height));
			}

			if (ioDefault.ActionPosition1 != Vector2.Zero)
				ioNew.ActionPosition1 = ioDefault.ActionPosition1 + ioNew.Position;
			if (ioDefault.ActionPosition2 != Vector2.Zero)
				ioNew.ActionPosition2 = ioDefault.ActionPosition2 + ioNew.Position;

			if (ioDefault.DrawZ != 0)
				ioNew.DrawZ = ioDefault.DrawZ + ioNew.PositionY - ioNew.Texture.Height / 2;

			PlaceElementInPlane(ioNew);
		}

		private void PlaceSprite()
		{
			Sprite s = new Sprite(Vector2.Zero, GameLogic.GhostData.Name);
			s.Position = MouseHelper.Position - new Vector2(s.Texture.Width / 2, s.Texture.Height / 2) - mCamera.Position;
			PlaceElementInPlane(s);
		}

		private void UpdateSelectedEntity()
		{
			switch(GameLogic.EState)
			{
				case EditorState.PlaceWayPoint:
					foreach (Waypoint w in mlevelSceneData.Waypoints)
						if (w.CollisionBox.Contains(MouseHelper.PositionPoint))
						{
							mObjectInfoBox.InfoObject = w;
							MouseHelper.ResetClick();
							return;
						}
					break;
			}
		}

		/// <summary>
		/// Setzt das Gameobject auf die richtige Plane
		/// </summary>
		/// <param name="go"></param>
		private void PlaceElementInPlane(GameObject go)
		{
			mlevelSceneData.ParallaxPlanes[GameLogic.ParallaxLayerNow].Add(go);
		}

        private void MoveCameraByMouse()
        {
            if (mMouseCenter == Vector2.Zero) mMouseCenter = MouseHelper.Position;

            Vector2 Mousepos;
            Mousepos.X = MouseHelper.Position.X;
            Mousepos.Y = -MouseHelper.Position.Y;

            Vector2 DirectionNormalize;

            Vector2 Direction = Mousepos - mMouseCenter;
            Vector2 Direction2 = mMouseCenter - Mousepos * -1;

            DirectionNormalize = new Vector2(Direction.X, -Direction2.Y); ;
            if(DirectionNormalize != Vector2.Zero)
                DirectionNormalize.Normalize();

            Vector2 DirectionSpeed = DirectionNormalize * SPEED_CAMERA_MOVE;

            mCamera.MoveCamera(DirectionSpeed);
        }

		#region MainDropDown Methods
		private void ShowFormNewGame()
        {
            if (!mFormNewGame.Visible)
                mFormNewGame.ShowDialog();
			MouseHelper.ResetClick();
        }

		private void SaveScene()
		{
			Stream myStream;
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

			saveFileDialog1.InitialDirectory = Environment.CurrentDirectory + @"\Content\hug\";
			saveFileDialog1.Filter = "Ultrageiles Hansel und Gretel Level von mega geilen GD's gemacht !!! (*.hug)|*.hug";
			saveFileDialog1.FilterIndex = 2;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((myStream = saveFileDialog1.OpenFile()) != null)
				{
					XmlSerializer xml = new XmlSerializer(typeof(SceneData));

					TextWriter writer = new StreamWriter(myStream);
					xml.Serialize(writer, mlevelSceneData);
					writer.Close();
					myStream.Close();
				}
			}
			MouseHelper.ResetClick();
		}

		public void LoadScene()
		{
			mlevelSceneData.ResetLevel();
			Stream myStream;
			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = Environment.CurrentDirectory + @"\Content\hug\";
			openFileDialog1.Filter = "Ultrageiles Hansel und Gretel Level von mega geilen GD's gemacht !!! (*.hug)|*.hug";
			openFileDialog1.FilterIndex = 2;
			openFileDialog1.RestoreDirectory = true;

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				if ((myStream = openFileDialog1.OpenFile()) != null)
				{
					XmlSerializer xml = new XmlSerializer(typeof(SceneData));

					TextReader reader = new StreamReader(myStream);
					mlevelSceneData = (SceneData)xml.Deserialize(myStream);
					reader.Close();
					myStream.Close();

					// Ersten 5 Listen löschen wichtig!!!
					for(int i = 0; i < 5; i++)
					{
						mlevelSceneData.ParallaxPlanes.RemoveAt(0);
					}
				}
			LoadAllTexturesInPlanes();
			}
			MouseHelper.ResetClick();
			
		}
        
        public void CreateNewScene(Rectangle pGameScreenSize)
        {
            mCamera.Position = new Vector2(0,20);
            mCamera.GameScreen = pGameScreenSize;
			mlevelSceneData.ResetLevel();
			mlevelSceneData.GamePlane = pGameScreenSize;
		}

		protected void SetDebugMode()
		{
			EngineSettings.IsDebug = !EngineSettings.IsDebug;
			MouseHelper.ResetClick();
		}

		// Muss Ingame auch getätigt werden.
		private void LoadAllTexturesInPlanes()
		{
			for (int i = 4; i >= 0; i-- )
			{
				foreach (GameObject go in mlevelSceneData.ParallaxPlanes[i])
				{
					if (go.GetType() == typeof(Sprite))
					{
						Sprite s = (Sprite)go;
						s.Texture = TextureManager.Instance.GetElementByString(s.TextureName);
					}
					if (go.GetType() == typeof(InteractiveObject))
					{
						InteractiveObject io = (InteractiveObject)go;
						io.Texture = TextureManager.Instance.GetElementByString(io.TextureName);
					}
				}
			}
		}

		#endregion

		#region PlaneDropDownMenu
		private void SetAllPlanesVisible()
		{
			for (int i = 0; i < mPlaneVisible.Length; i++)
				mPlaneVisible[i] = true;
			MouseHelper.ResetClick();
		}
		private void SetPlane0Visible()
		{
			mPlaneVisible[0] = !mPlaneVisible[0];
			MouseHelper.ResetClick();
		}
		private void SetPlane1Visible()
		{
			mPlaneVisible[1] = !mPlaneVisible[1];
			MouseHelper.ResetClick();
		}
		private void SetPlane2Visible()
		{
			mPlaneVisible[2] = !mPlaneVisible[2];
			MouseHelper.ResetClick();
		}
		private void SetPlane3Visible()
		{
			mPlaneVisible[3] = !mPlaneVisible[3];
			MouseHelper.ResetClick();
		}
		private void SetPlane4Visible()
		{
			mPlaneVisible[4] = !mPlaneVisible[4];
			MouseHelper.ResetClick();
		}
		private void SetMoveRectangleState()
		{
			GameLogic.EState = EditorState.PlaceMoveArea;
			MouseHelper.ResetClick();
		}
		#endregion

		#endregion
	}
}

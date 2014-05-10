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

		protected List<GameObject> mPlane0Objects;
		protected List<GameObject> mPlane1Objects;
		protected List<GameObject> mPlane2Objects;
		protected List<GameObject> mPlane3Objects;
		protected List<GameObject> mPlane4Objects;

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
			mlevelSceneData.GamePlane = mCamera.GameScreen;
        }
        #endregion

        #region Override Methods

        public override void Initialize()
        {
			mlevelSceneData.Initialize();
            mClearColor = Color.Red;
            mMouseCenter = Vector2.Zero;
			GameLogic.EState = EditorState.Standart;

			mPlane0Objects = new List<GameObject>();
			mPlane1Objects = new List<GameObject>();
			mPlane2Objects = new List<GameObject>();
			mPlane3Objects = new List<GameObject>();
			mPlane4Objects = new List<GameObject>();
			mPlaneVisible = new bool[5];
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
				new List<string>() { "Neu...", "Laden", "Speichern unter..." }, 
				new List<Action>() { ShowFormNewGame, LoadScene, SaveScene });
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

			switch(GameLogic.EState)
			{
				case EditorState.Standart: DrawPlanes();
					DrawGhostEntity();
					break;
				case EditorState.PlaceMoveArea: DrawMoveRectangle();
					break;
				case EditorState.PlaceWayPoint:
					DrawPlanes();
					DrawWayPoints();
					DrawGhostEntity();
					break;
			}



            DrawMenueItems();

            DrawOnScene();
        }

        #endregion

        #region Methods
        public void DrawMenueItems()
        {
            mSpriteBatch.Begin();
			foreach (InterfaceObject io in mInterfaceObjects)
				io.Draw(mSpriteBatch);

            Vector2 msPos = MouseHelper.Position - mCamera.Position;
            mSpriteBatch.DrawString(mDebugFont, "MousePosition: " + msPos, new Vector2(0, 20), Color.White);
			mSpriteBatch.DrawString(mDebugFont, "Rectangle Position: " + tmpRectangle, new Vector2(0, 40), Color.White);
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
			if (mPlaneVisible[0])
			{
				// Camera für verschiebung einbinden.
				mSpriteBatch.Begin();
				foreach (GameObject go in mPlane0Objects)
					go.Draw(mSpriteBatch);
				mSpriteBatch.End();
			}

			if (mPlaneVisible[1])
			{
				// Camera für verschiebung einbinden.
				mSpriteBatch.Begin();
				foreach (GameObject go in mPlane1Objects)
					go.Draw(mSpriteBatch);
				mSpriteBatch.End();
			}

			if (mPlaneVisible[2])
			{
				// Camera für verschiebung einbinden.
				mSpriteBatch.Begin();
				foreach (GameObject go in mPlane2Objects)
					go.Draw(mSpriteBatch);
				mSpriteBatch.End();
			}

			if (mPlaneVisible[3])
			{
				// Camera für verschiebung einbinden.
				mSpriteBatch.Begin();
				foreach (GameObject go in mPlane3Objects)
					go.Draw(mSpriteBatch);
				mSpriteBatch.End();
			}

			if (mPlaneVisible[4])
			{
				// Camera für verschiebung einbinden.
				mSpriteBatch.Begin();
				foreach (GameObject go in mPlane4Objects)
					go.Draw(mSpriteBatch);
				mSpriteBatch.End();
			}
		}

		private void DrawMoveRectangle()
		{
			mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
			if(IsDrawingRectangle)
				mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), tmpRectangle, Color.White);
			foreach (GameObject go in mPlane1Objects)
				go.Draw(mSpriteBatch);
			foreach (Rectangle r in mlevelSceneData.MoveArea)
				mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), r, Color.BlueViolet);
			mSpriteBatch.End();
		}

        public void UpdateMouseInput()
        {
			//if (MouseHelper.Instance.IsClickedRight)
			//	mRightClickDropDown.OpenDropDownMenue(MouseHelper.Position);
            if (MouseHelper.Instance.IsPressedMiddle)
                MoveCameraByMouse();
            else
                mMouseCenter = Vector2.Zero;


			switch(GameLogic.EState)
			{ 
				case EditorState.Standart:
					if (MouseHelper.Instance.IsWheelUp)
						GameLogic.ParallaxLayerNow++;
					if (MouseHelper.Instance.IsWheelDown)
						GameLogic.ParallaxLayerNow--;
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
				}
			}
		}
        
        public void CreateNewScene(Rectangle pGameScreenSize)
        {
            mCamera.Position = new Vector2(0,20);
            mCamera.GameScreen = pGameScreenSize;
			mlevelSceneData.GamePlane = pGameScreenSize;
		}

		#endregion

		#region PlaneDropDownMenu
		private void SetAllPlanesVisible()
		{
			for (int i = 0; i < mPlaneVisible.Length; i++)
				mPlaneVisible[i] = true;
		}
		private void SetPlane0Visible()
		{
			mPlaneVisible[0] = !mPlaneVisible[0];
		}
		private void SetPlane1Visible()
		{
			mPlaneVisible[1] = !mPlaneVisible[1];
		}
		private void SetPlane2Visible()
		{
			mPlaneVisible[2] = !mPlaneVisible[2];
		}
		private void SetPlane3Visible()
		{
			mPlaneVisible[3] = !mPlaneVisible[3];
		}
		private void SetPlane4Visible()
		{
			mPlaneVisible[4] = !mPlaneVisible[4];
		}
		private void SetMoveRectangleState()
		{
			GameLogic.EState = EditorState.PlaceMoveArea;
		}
		#endregion

		#endregion
	}
}

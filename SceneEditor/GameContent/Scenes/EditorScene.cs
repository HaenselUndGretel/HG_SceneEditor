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
using KryptonEngine.HG_Data.Objects.Lights;
using Xml_Editor;

namespace MenuEditor.GameContent.Scenes
{
	enum LightState
	{
		Center,
		Range
	}

	enum RenderState
	{
		Diffuse,
		Normal,
		Depth,
		AO,
		Light,
		Final
	}

    class EditorScene : Scene
    {
        #region Properties
		protected Sprite mGround;

        private List<InterfaceObject> mInterfaceObjects = new List<InterfaceObject>();

		private bool[] mPlaneVisible;
        private SpriteFont mDebugFont;

        private FormNewGame mFormNewGame;
		private Form1 mXmlForm;

        private ImageBox mObjectBox;

		private Lighter mLighter;
		private AmbientLight mAmbientLight;

		private RenderTarget2D mRenderTargetFinalGame;

		private bool somethingUpdated;

		#region MoveArea
		private bool IsDrawingRectangle;
		private Vector2 mRectangleSelectPos1;
		private Vector2 mRectangleSelectPos2;
		private Rectangle tmpRectangle;
		#endregion

		#region LightSettings
		LightState mLightState;
		#endregion

		RenderState mRenderState;

		#region KeyboardInput

		protected KeyboardState ksCurrent;
        protected KeyboardState ksLast;
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
			mCamera.MoveCamera(new Vector2(0, 20));
			GameLogic.LevelSceneData = new SceneData();
			mXmlForm = new Form1();
        }
        #endregion

        #region Override Methods

        public override void Initialize()
        {
			GameLogic.LevelSceneData.Initialize();
			GameLogic.LevelSceneData.GamePlane = mCamera.GameScreen;
			mClearColor = Color.Red;
			GameLogic.ZDepth = GameLogic.LevelSceneData.GamePlane.Height;

            mMouseCenter = Vector2.Zero;
			GameLogic.EState = EditorState.Standard;

			mPlaneVisible = new bool[5] { true, true, true, true, true };
        }

        public override void LoadContent()
        {
            /// Relevante Sachen für HUD 

            mObjectBox = new ImageBox(new Vector2(EngineSettings.VirtualResWidth - 1440 - 1, 1), new Rectangle(0, 0, 1440, EngineSettings.VirtualResHeight - 2));
            DropDownMenu Main = new DropDownMenu(Vector2.Zero, 
				new List<string>() { "Neu...", "Laden", "Speichern unter...", "DebugMode", "Xml-Editor" }, 
				new List<Action>() { ShowFormNewGame, LoadScene, SaveScene, SetDebugMode, ShowXmlEditor });
			DropDownMenu Ebenen = new DropDownMenu(Vector2.Zero, 
				new List<string>() { "Alle Ebenen anzeigen", "Wegsetzding(PathStuff)" }, 
				new List<Action>() { SetAllPlanesVisible, SetMoveRectangleState });
			DropDownMenu RenderTargets = new DropDownMenu(Vector2.Zero,
				new List<String>() { "Diffuse Map", "Normal Map", "Depth Map", "AO Map", "Light Map", "Final Map" },
				new List<Action>() { SetDiffuseRenderTarget, SetNormalRenderTarget, SetDepthRenderTarget, SetAORenderTarget, SetLightRenderTarget, SetFinalRenderTarget });

			MenuBar mMenuBarTop = new MenuBar(Vector2.Zero);
            mMenuBarTop.AddMenueItem("Datei", Main);
            mMenuBarTop.AddMenueItem("Ebenen", Ebenen);
			mMenuBarTop.AddMenueItem("Render Targets", RenderTargets);
            mMenuBarTop.OrganizeButtons();

            mFormNewGame = new FormNewGame();

			mDebugFont = FontManager.Instance.GetElementByString("font");

            // Addet alle InterfaceObjekte in die InterfaceObject Liste
            mInterfaceObjects.Add(mMenuBarTop);
            mInterfaceObjects.Add(mObjectBox);

			mAmbientLight = new AmbientLight();
			mLighter = new Lighter(mAmbientLight);

			mRenderState = RenderState.Final;
			mRenderTargetFinalGame = new RenderTarget2D(EngineSettings.Graphics.GraphicsDevice, EngineSettings.VirtualResWidth, EngineSettings.VirtualResHeight);

            GameLogic.Initialize();
        }

        public override void Update()
        {
            ksLast = ksCurrent;
            ksCurrent = Keyboard.GetState();

			somethingUpdated = false;

			if (GameLogic.XmlFormFocus) return;

            if (GameLogic.SelectedEntity != null
                && ksLast.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Delete)
                && ksCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Delete))
					RemoveObjectFromPlane();

			if (ksLast.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemMinus)
				&& ksCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.OemMinus))
				mCamera.ZoomOut(2);

			if (ksLast.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemPlus)
				&& ksCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.OemPlus))
					mCamera.ResetScale();

            foreach (InterfaceObject io in mInterfaceObjects)
                io.Update();

            UpdateMouseInput();

			if (GameLogic.IsXmlFormShow && somethingUpdated)
				mXmlForm.UpdateInfo();
        }

        public override void Draw()
        {
            EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mRenderTargetDiffuse);
			EngineSettings.Graphics.GraphicsDevice.Clear(Color.ForestGreen);

            DrawBackground();

            mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null , mCamera.GetTranslationMatrix());
            mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), GameLogic.LevelSceneData.GamePlane, Color.ForestGreen);
            mSpriteBatch.End();

			DrawPlanes();
			DrawMoveRectangle();

			DrawCollectables();
			DrawItems();
			DrawLights();

			DrawNormalPlane();

			DrawDepthPlane();

			mLighter.GenerateLightMap(mRenderTargetLight, mRenderTargetNormal, mRenderTargetDepthObject, mRenderTargetAO, GameLogic.LevelSceneData.Lights);

			mLighter.CombineMaps(mRenderTargetFinalGame, mRenderTargetLight, mRenderTargetDiffuse);

			#region DrawFinalRendertargets
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mRenderTargetFinal);
			mSpriteBatch.Begin();
			switch(mRenderState)
			{
				case RenderState.Diffuse: mSpriteBatch.Draw(mRenderTargetDiffuse, Vector2.Zero, Color.White);
					break;
				case RenderState.Depth: mSpriteBatch.Draw(mRenderTargetDepthObject, Vector2.Zero, Color.White);
					break;
				case RenderState.AO: mSpriteBatch.Draw(mRenderTargetAO, Vector2.Zero, Color.White);
					break;
				case RenderState.Final: mSpriteBatch.Draw(mRenderTargetFinalGame, Vector2.Zero, Color.White);
					break;
				case RenderState.Light: mSpriteBatch.Draw(mRenderTargetLight, Vector2.Zero, Color.White);
					break;
				case RenderState.Normal: mSpriteBatch.Draw(mRenderTargetNormal, Vector2.Zero, Color.White);
					break;
			}
			mSpriteBatch.End();
			
			DrawGhostEntity();

			switch (GameLogic.EState)
			{
				case EditorState.PlaceWayPoint:
					DrawWayPoints();
					break;

				case EditorState.PlaceEventArea:
					DrawEventAreas();
					break;
			}

			DrawSelectedRectangle();

			DrawMenueItems();

			#endregion
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
			if (GameLogic.SelectedEntity != null)
				mSpriteBatch.DrawString(mDebugFont, "Object ID: " + GameLogic.SelectedEntity.ObjectId, new Vector2(0, 690), Color.White);
			mSpriteBatch.DrawString(mDebugFont, "Mouseposition: " + CalculatePlacePositionOrigin(Vector2.Zero), new Vector2(0, 710), Color.White);
            
            // MouseMoveIcon
            if(mMouseCenter != Vector2.Zero)
              mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), mMouseCenter, new Rectangle(-2, -2, 5, 5), Color.Black);

            mSpriteBatch.End();
        }

		private void DrawWayPoints()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, mCamera.GetTranslationMatrix());
			foreach (Waypoint w in GameLogic.LevelSceneData.Waypoints)
				w.Draw(mSpriteBatch);
			mSpriteBatch.End();
		}

		private void DrawEventAreas()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, mCamera.GetTranslationMatrix());
			foreach (EventTrigger e in GameLogic.LevelSceneData.Events)
				e.Draw(mSpriteBatch);
			mSpriteBatch.End();
		}

		private void DrawGhostEntity()
		{
            if (GameLogic.GhostData.Name == "") return;

			if (GameLogic.GhostData.Texture != null)
			{
				mSpriteBatch.Begin();
				mSpriteBatch.Draw(GameLogic.GhostData.Texture.Texture, MouseHelper.Position - GameLogic.GhostData.Texture.Origin, Color.White);
				mSpriteBatch.End();
			}
		}

		public void DrawPlanes()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, mCamera.GetTranslationMatrix());
			if (mGround != null)
				mGround.Draw(mSpriteBatch);
			
			// Roter Kamera Rand
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(-200, -200, mCamera.GameScreen.Width + 600, 200), Color.Red);
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(-200, -200, 200, mCamera.GameScreen.Height + 400), Color.Red);
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(mCamera.GameScreen.Width, -200, 400, mCamera.GameScreen.Height + 400), Color.Red);
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(-200, mCamera.GameScreen.Height, mCamera.GameScreen.Width + 600, 200), Color.Red);

			foreach (GameObject go in GameLogic.LevelSceneData.BackgroundSprites)
				if(go.DrawZ < GameLogic.ZDepth)
					go.Draw(mSpriteBatch);

			foreach (InteractiveObject io in GameLogic.LevelSceneData.InteractiveObjects)
				if (io.DrawZ < GameLogic.ZDepth)
					io.Draw(mSpriteBatch);

			mSpriteBatch.End();

			DrawEnemy();

			// Draw-Z Linie
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mRenderTargetDiffuse);
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, mCamera.GetTranslationMatrix());
			mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), new Rectangle(0, GameLogic.ZDepth, GameLogic.LevelSceneData.GamePlane.Width, 1), Color.Blue);
            mSpriteBatch.End();
		}

		private void DrawNormalPlane()
		{
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mRenderTargetNormal);
			EngineSettings.Graphics.GraphicsDevice.Clear(new Color(129, 129, 255));

			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, mCamera.GetTranslationMatrix());
			foreach (GameObject go in GameLogic.LevelSceneData.BackgroundSprites)
				if (go.DrawZ < GameLogic.ZDepth)
					go.DrawNormal(mSpriteBatch);

			foreach (InteractiveObject io in GameLogic.LevelSceneData.InteractiveObjects)
				if (io.DrawZ < GameLogic.ZDepth)
					io.DrawNormal(mSpriteBatch);

			mSpriteBatch.End();
		}

		private void DrawDepthPlane()
		{
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(mRenderTargetDepthObject);
			EngineSettings.Graphics.GraphicsDevice.Clear(Color.White);

			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, mCamera.GetTranslationMatrix());
			foreach (GameObject go in GameLogic.LevelSceneData.BackgroundSprites)
				if (go.DrawZ < GameLogic.ZDepth)
					go.DrawDepth(mSpriteBatch);

			foreach (InteractiveObject io in GameLogic.LevelSceneData.InteractiveObjects)
				if (io.DrawZ < GameLogic.ZDepth)
					io.DrawDepth(mSpriteBatch);
			mSpriteBatch.End();
		}

		private void DrawMoveRectangle()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, mCamera.GetTranslationMatrix());
			if(IsDrawingRectangle)
				mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), tmpRectangle, Color.White);

			foreach (Rectangle r in GameLogic.LevelSceneData.MoveArea)
				mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), r, Color.DarkRed * 0.8f);
			mSpriteBatch.End();

			if (GameLogic.EState == EditorState.PlaceMoveArea)
				DrawWayPoints();
		}

        private void DrawSelectedRectangle()
        {
            if (GameLogic.SelectEntityRectangle.IsSelected)
            {
                mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, mCamera.GetTranslationMatrix());
				GameLogic.SelectEntityRectangle.Draw(mSpriteBatch);
				if(GameLogic.SelectedEntity != null)
					mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"),
						new Rectangle(GameLogic.SelectedEntity.CollisionBox.X,
							GameLogic.SelectedEntity.DrawZ,
							GameLogic.SelectedEntity.CollisionBox.Width,
							1),
							Color.Black);
                mSpriteBatch.End();
            }
        }

		private void DrawCollectables()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, mCamera.GetTranslationMatrix());
			foreach (Collectable c in GameLogic.LevelSceneData.Collectables)
				c.Draw(mSpriteBatch);
			mSpriteBatch.End();
		}

		private void DrawItems()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, mCamera.GetTranslationMatrix());
			foreach (Item i in GameLogic.LevelSceneData.Items)
				i.Draw(mSpriteBatch);
			mSpriteBatch.End();
		}

		private void DrawEnemy()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, null, null, null, mCamera.GetTranslationMatrix());
			foreach (Enemy e in GameLogic.LevelSceneData.Enemies)
				e.Draw(mSpriteBatch);
			mSpriteBatch.End();
		}

		private void DrawLights()
		{
			mSpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, mCamera.GetTranslationMatrix());
			foreach (Light l in GameLogic.LevelSceneData.Lights)
				l.Draw(mSpriteBatch);
			mSpriteBatch.End();
		}

		#endregion
		public void UpdateMouseInput()
        {
            if (MouseHelper.Instance.IsPressedMiddle)
                MoveCameraByMouse();
            else
                mMouseCenter = Vector2.Zero;

			if (mLightState == LightState.Range)
				SetLightRange();

			if(MouseHelper.Instance.IsClickedRight)
			{
				if (mLightState == LightState.Range)
					SetLightRange();
				mLightState = LightState.Center;

				switch(GameLogic.EState)
				{
					case EditorState.PlaceMoveArea:
							UpdateRectangleDelete(GameLogic.LevelSceneData.MoveArea);
							break;
				}
			}

			//Update aller Objekte für DropDownMenu
			UpdateAllDropDownEntities();

			// Rechtsklick für DropDownMenu muss noch abgefangen werden.
            if (MouseHelper.Instance.IsClickedRight)
            {
                GameLogic.EState = EditorState.Standard;
                GameLogic.GhostData.Name = "";
                GameLogic.SelectEntityRectangle.IsSelected = false;
                mObjectBox.SelectRectangleObject.IsSelected = false;
                mObjectBox.SelectedEntity.Name = "";
                GameLogic.SelectedEntity = null;
            }
			
			if (GameLogic.EState != EditorState.PlaceMoveArea)
			{
				if (GameLogic.SelectedEntity == null)
				{
					if (MouseHelper.Instance.IsWheelUp)
						GameLogic.ZDepth -= 25;
					if (MouseHelper.Instance.IsWheelDown)
						GameLogic.ZDepth += 25;
				}
				else
				{
					if (MouseHelper.Instance.IsWheelUp)
					{
						GameLogic.SelectedEntity.DrawZ--;
						somethingUpdated = true;
						SortBackgroundPlane();
					}
					if (MouseHelper.Instance.IsWheelDown)
					{
						GameLogic.SelectedEntity.DrawZ++;
						somethingUpdated = true;
						SortBackgroundPlane();
					}
				}
			}

			// Zum selektieren der Objekte auf der Scene
			if (MouseHelper.Instance.IsClickedLeft)
				UpdateMouseLeftClick();

			switch(GameLogic.EState)
			{
				case EditorState.PlaceMoveArea:
					UpdateRectInput();
					break;

				case EditorState.PlaceWayPoint:
					UpdateRectInput();
					break;

				case EditorState.PlaceEventArea:
					UpdateRectInput();
					break;
			}
        }

		private void UpdateMouseLeftClick()
		{
			switch (GameLogic.EState)
			{
				case EditorState.Standard:
					UpdateSelectedEntity();
					break;

				case EditorState.PlaceWayPoint:
					UpdateSelectedEntity();
					break;

				case EditorState.PlaceEventArea:
					UpdateSelectedEntity();
					break;

				case EditorState.PlaceInteractiveObject:
					PlaceInteractiveObject();
					break;

				case EditorState.PlaceSprites:
					PlaceSprite();
					break;

				case EditorState.PlaceCollectable:
					PlaceCollectable();
					break;

				case EditorState.PlaceItem:
					PlaceItem();
					break;

				case EditorState.PlaceEnemy:
					PlaceEnemy();
					break;

				case EditorState.PlaceLight:
					if (mLightState == LightState.Center)
						PlaceLight();
					else if (mLightState == LightState.Range)
						SetLightRange();
					break;

				case EditorState.PlaceGround:
					PlaceGround();
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
				if(tmpRectangle.Width > 0
					&& tmpRectangle.Height > 0)
					switch(GameLogic.EState)
					{
						case EditorState.PlaceMoveArea:
							GameLogic.LevelSceneData.MoveArea.Add(tmpRectangle);
							break;
						case EditorState.PlaceWayPoint:
							Waypoint w = new Waypoint();
							w.CollisionBox = tmpRectangle;
							w.Position = new Vector2(tmpRectangle.X, tmpRectangle.Y);
							GameLogic.LevelSceneData.Waypoints.Add(w);
							break;
						case EditorState.PlaceEventArea:
							EventTrigger e = new EventTrigger();
							e.CollisionBox = tmpRectangle;
							e.Position = new Vector2(tmpRectangle.X, tmpRectangle.Y);
							GameLogic.LevelSceneData.Events.Add(e);
							break;
					}
				IsDrawingRectangle = false;
				somethingUpdated = true;
			}
		}

		private void UpdateGameObjectDelete(List<GameObject> go)
		{
			int delRect = -1;
			for (int i = 0; i < go.Count; i++)
				if (go[i].CollisionBox.Contains(MouseHelper.PositionPoint))
					delRect = i;
			if (delRect != -1)
			{
				go.RemoveAt(delRect);
				somethingUpdated = true;
			}
		}

		private void UpdateRectangleDelete(List<Rectangle> r)
		{
			Point MousePosition = new Point(MouseHelper.PositionPoint.X - (int)mCamera.Position.X, MouseHelper.PositionPoint.Y - (int)mCamera.Position.Y);
			int delRect = -1;
			for (int i = 0; i < r.Count; i++)
				if (r[i].Contains(MousePosition))
					delRect = i;
			if (delRect != -1)
				r.RemoveAt(delRect);
			MouseHelper.ResetClick();
			somethingUpdated = true;
		}

		private void UpdateAllDropDownEntities()
		{
			foreach (InteractiveObject io in GameLogic.LevelSceneData.InteractiveObjects)
				io.Update();

			foreach (Waypoint w in GameLogic.LevelSceneData.Waypoints)
				w.Update();
		}

		#region Place Funktionen

		private Vector2 CalculatePlacePositionOrigin(Sprite s)
		{
			return new Vector2((int)(MouseHelper.Position.X - s.Texture.Width / 2 - mCamera.Position.X), (int)(MouseHelper.Position.Y - s.Texture.Height / 2 - mCamera.Position.Y));
		}

		private Vector2 CalculatePlacePositionOrigin(Vector2 origin)
		{
			return new Vector2((int)(MouseHelper.Position.X - origin.X - mCamera.Position.X), (int)(MouseHelper.Position.Y - origin.Y - mCamera.Position.Y));
		}

		private void PlaceInteractiveObject()
		{
			InteractiveObject ioDefault = new InteractiveObject();
			InteractiveObject ioNew = new InteractiveObject();
			ioDefault = InteractiveObjectDataManager.Instance.GetElementByString(GameLogic.GhostData.Name);

			MouseState ms = Mouse.GetState();

			ioNew.Texture = ioDefault.Texture;
			ioNew.TextureName = ioDefault.TextureName;
			ioNew.Position = new Vector2((int)(MouseHelper.Position.X - mCamera.Position.X), (int)(MouseHelper.Position.Y - mCamera.Position.Y));
            ioNew.CollisionBox = new Rectangle((int)ioNew.Position.X,(int) ioNew.Position.Y, ioNew.Texture.Width, ioNew.Texture.Height);

			for (int i = 0; i < ioDefault.ActionRectList.Count; i++)
			{
				ioNew.ActionRectList.Add(new Rectangle
					(ioDefault.ActionRectList[i].X + ioNew.PositionX,
					 ioDefault.ActionRectList[i].Y + ioNew.PositionY,
					 ioDefault.ActionRectList[i].Width,
					 ioDefault.ActionRectList[i].Height));
			}

			for (int i = 0; i < ioDefault.CollisionRectList.Count; i++)
			{
				ioNew.CollisionRectList.Add(new Rectangle
					(ioDefault.CollisionRectList[i].X + ioNew.PositionX,
					 ioDefault.CollisionRectList[i].Y + ioNew.PositionY,
					 ioDefault.CollisionRectList[i].Width,
					 ioDefault.CollisionRectList[i].Height));
			}

			if (ioDefault.ActionPosition1 != Vector2.Zero)
				ioNew.ActionPosition1 = ioDefault.ActionPosition1 + ioNew.Position;
			if (ioDefault.ActionPosition2 != Vector2.Zero)
				ioNew.ActionPosition2 = ioDefault.ActionPosition2 + ioNew.Position;

			if (ioDefault.DrawZ != 0)
				ioNew.DrawZ = ioDefault.DrawZ + ioNew.PositionY - ioNew.Texture.Height / 2;
			else
				ioNew.DrawZ = (int)ioNew.PositionY + ioNew.Texture.Height / 2;

            ioNew.Position = MouseHelper.Position - new Vector2(ioNew.Texture.Width / 2, ioNew.Texture.Height / 2) - mCamera.Position;

			GameLogic.LevelSceneData.InteractiveObjects.Add(ioNew);

			GameLogic.SelectedEntity = ioNew;
			GameLogic.SelectEntityRectangle = new SelectRectangle(ioNew.Position, ioNew.CollisionBox);
			somethingUpdated = true;			
		}

		private void PlaceSprite()
		{
			Sprite s = new Sprite(Vector2.Zero, GameLogic.GhostData.Name);
			s.Position = CalculatePlacePositionOrigin(s);
			s.DrawZ = s.PositionY + s.Texture.Height;
			PlaceElementInPlane(s);
			somethingUpdated = true;
		}

		private void PlaceGround()
		{
			mGround = new Sprite(Vector2.Zero, GameLogic.GhostData.Name);
			mGround.Position = MouseHelper.Position - new Vector2(mGround.Texture.Width / 2, mGround.Texture.Height / 2) - mCamera.Position;
		}

		private void PlaceCollectable()
		{
			Collectable c = new Collectable(Vector2.Zero, GameLogic.GhostData.Name);
			c.Position = CalculatePlacePositionOrigin(c);
			string test = GameLogic.GhostData.Name.Substring(GameLogic.GhostData.Name.Length - 1);
			c.CollectableId = Convert.ToInt32(test);
			c.ShowTextureName = "ShowCollectable" + c.CollectableId;
			c.DrawZ = c.PositionY + c.Texture.Height;
			GameLogic.LevelSceneData.Collectables.Add(c);
			somethingUpdated = true;
		}

		private void PlaceItem()
		{
			if (GameLogic.GhostData.Name.Contains("Branch"))
			{
				Branch a = new Branch(Vector2.Zero, GameLogic.GhostData.Name);
				a.Position = CalculatePlacePositionOrigin(a);
				a.DrawZ = a.PositionY + a.Texture.Height;
				GameLogic.LevelSceneData.Items.Add(a);
			}
			else if (GameLogic.GhostData.Name.Contains("Knife"))
			{
				Knife d = new Knife(Vector2.Zero, GameLogic.GhostData.Name);
				d.Position = CalculatePlacePositionOrigin(d);
				d.DrawZ = d.PositionY + d.Texture.Height;
				GameLogic.LevelSceneData.Items.Add(d);
			}
			else if (GameLogic.GhostData.Name.Contains("Key"))
			{
				Key t = new Key(Vector2.Zero, GameLogic.GhostData.Name);
				t.Position = CalculatePlacePositionOrigin(t);
				t.DrawZ = t.PositionY + t.Texture.Height;
				GameLogic.LevelSceneData.Items.Add(t);
			}
			else if (GameLogic.GhostData.Name.Contains("Lantern"))
			{
				Lantern a = new Lantern(Vector2.Zero, GameLogic.GhostData.Name);
				a.Position = CalculatePlacePositionOrigin(a);
				a.DrawZ = a.PositionY + a.Texture.Height;
				GameLogic.LevelSceneData.Items.Add(a);
			}
			else if (GameLogic.GhostData.Name.Contains("Matches"))
			{
				Matches d = new Matches(Vector2.Zero, GameLogic.GhostData.Name);
				d.Position = CalculatePlacePositionOrigin(d);
				d.DrawZ = d.PositionY + d.Texture.Height;
				GameLogic.LevelSceneData.Items.Add(d);
			}

			somethingUpdated = true;
		}

		private void PlaceEnemy()
		{
			if (GameLogic.GhostData.Name.Contains("Spider"))
			{
				Spider s = new Spider();
				s.Position = CalculatePlacePositionOrigin(new Vector2(32, 32));
				GameLogic.LevelSceneData.Enemies.Add(s);
				GameLogic.SelectedEntity = s;
			}
			else if (GameLogic.GhostData.Name.Contains("Witch"))
			{
				Witch w = new Witch();
				w.Position = CalculatePlacePositionOrigin(new Vector2(32, 32));
				GameLogic.LevelSceneData.Enemies.Add(w);
				GameLogic.SelectedEntity = w;
			}
			else if (GameLogic.GhostData.Name.Contains("Wolf"))
			{
				Wolf w = new Wolf();
				w.Position = CalculatePlacePositionOrigin(new Vector2(32, 32));
				GameLogic.LevelSceneData.Enemies.Add(w);
				GameLogic.SelectedEntity = w;
			}

			somethingUpdated = true;
		}

		private void PlaceLight()
		{
			float depth = 1 / GameLogic.ZDepth + 1;
			// 32x32 da Texture 64x64 Groß ist und um den Origin verschoben wird.
			if (GameLogic.GhostData.Name.Contains("Direction"))
			{
				DirectionLight d = new DirectionLight();
				d.Position = CalculatePlacePositionOrigin(new Vector2(32, 32));
				d.Depth = depth;
				GameLogic.LevelSceneData.Lights.Add(d);
				GameLogic.SelectedEntity = d;
			}
			else if (GameLogic.GhostData.Name.Contains("Point"))
			{
				PointLight p = new PointLight();
				p.Position = CalculatePlacePositionOrigin(new Vector2(32, 32));
				p.Depth = depth;
				GameLogic.LevelSceneData.Lights.Add(p);
				GameLogic.SelectedEntity = p;
				mLightState = LightState.Range;
			}
			else if (GameLogic.GhostData.Name.Contains("Spot"))
			{
				SpotLight s = new SpotLight();
				s.Position = CalculatePlacePositionOrigin(new Vector2(32, 32));
				s.Depth = depth;
				GameLogic.LevelSceneData.Lights.Add(s);
				GameLogic.SelectedEntity = s;
			}

			somethingUpdated = true;
			// Später benutzen wenn Lichter komplett implementiert
			//mLightState = LightState.Range;
		}

		private void SetLightRange()
		{
			int foundBy = -1;
			for (int i = 0; i < GameLogic.LevelSceneData.Lights.Count; i++)
				if (GameLogic.LevelSceneData.Lights[i] == GameLogic.SelectedEntity)
					foundBy = i;

			if (foundBy < 0) return;

			PointLight l = (PointLight)GameLogic.LevelSceneData.Lights[foundBy];
			l.Radius = Vector2.Distance(l.Position, (new Vector2(MouseHelper.Position.X - (int)mCamera.Position.X, MouseHelper.Position.Y - (int)mCamera.Position.Y) - new Vector2(32, 32)));
			l.SetDrawCircle();

			if (MouseHelper.Instance.IsClickedLeft)
			{
				l.CollisionBox = new Rectangle(l.PositionX, l.PositionY, 64, 64);
				mLightState = LightState.Center;
				MouseHelper.ResetClick();
			}
		}
		#endregion

		private void UpdateSelectedEntity()
		{
            bool changeEntity = false;

			Point MousePosition = new Point(MouseHelper.PositionPoint.X - (int)mCamera.Position.X, MouseHelper.PositionPoint.Y - (int)mCamera.Position.Y);

			switch(GameLogic.EState)
			{
                case EditorState.Standard:
                    foreach(GameObject go in GameLogic.LevelSceneData.BackgroundSprites)
                        if (go.CollisionBox.Contains(MousePosition) && (GameLogic.ZDepth > go.DrawZ))
                        {
							changeEntity = ChangeSelectedEntity(go);
                            return;
                        }

					foreach(InteractiveObject io in GameLogic.LevelSceneData.InteractiveObjects)
						if (io.CollisionBox.Contains(MousePosition) && (GameLogic.ZDepth > io.DrawZ))
						{
							changeEntity = ChangeSelectedEntity(io);
						}

					foreach(Collectable c in GameLogic.LevelSceneData.Collectables)
						if (c.CollisionBox.Contains(MousePosition) && (GameLogic.ZDepth > c.DrawZ))
						{
							changeEntity = ChangeSelectedEntity(c);
							return;
						}

					foreach(Item i in GameLogic.LevelSceneData.Items)
						if (i.CollisionBox.Contains(MousePosition) && (GameLogic.ZDepth > i.DrawZ))
						{
							changeEntity = ChangeSelectedEntity(i);
							return;
						}

					foreach (Light l in GameLogic.LevelSceneData.Lights)
						if (l.CollisionBox.Contains(MousePosition) && (GameLogic.ZDepth > l.DrawZ))
						{
							changeEntity = ChangeSelectedEntity(l);
							return;
						}

					foreach(Enemy e in GameLogic.LevelSceneData.Enemies)
						if (e.CollisionBox.Contains(MousePosition) && (GameLogic.ZDepth > e.DrawZ))
						{
							changeEntity = ChangeSelectedEntity(e);
							return;
						}
					break;

				case EditorState.PlaceWayPoint:
					foreach (Waypoint w in GameLogic.LevelSceneData.Waypoints)
                        if (w.CollisionBox.Contains(MousePosition))
						{
							changeEntity = ChangeSelectedEntity(w);
							return;
						}
					break;

				case EditorState.PlaceEventArea:
					foreach (EventTrigger e in GameLogic.LevelSceneData.Events)
                        if (e.CollisionBox.Contains(MousePosition))
						{
							changeEntity = ChangeSelectedEntity(e);
							return;
						}
					break;

			}

            if(!changeEntity)
            {
                GameLogic.SelectedEntity = null;
                GameLogic.SelectEntityRectangle.IsSelected = false;
            }
		}

		private bool ChangeSelectedEntity(GameObject go)
		{
			GameLogic.SelectedEntity = go;
			GameLogic.SelectEntityRectangle = new SelectRectangle(new Vector2(go.CollisionBox.X, go.CollisionBox.Y), go.CollisionBox);
			MouseHelper.ResetClick();
			return true;
		}

		private void SortBackgroundPlane()
		{
			for (int j = GameLogic.LevelSceneData.BackgroundSprites.Count - 1; j > 0 ; j--)
				for (int i = 0; i < GameLogic.LevelSceneData.BackgroundSprites.Count - 1; i++)
				{
					if (GameLogic.LevelSceneData.BackgroundSprites[i].DrawZ > GameLogic.LevelSceneData.BackgroundSprites[i + 1].DrawZ)
					{
						GameObject temp = GameLogic.LevelSceneData.BackgroundSprites[i];
						GameLogic.LevelSceneData.BackgroundSprites[i] = GameLogic.LevelSceneData.BackgroundSprites[i + 1];
						GameLogic.LevelSceneData.BackgroundSprites[i + 1] = temp;
					}
				}

		}

        private void RemoveObjectFromPlane()
        {
			if (GameLogic.SelectedEntity.GetType() == typeof(Waypoint))
				GameLogic.LevelSceneData.Waypoints.Remove((Waypoint)GameLogic.SelectedEntity);

			else if (GameLogic.SelectedEntity.GetType() == typeof(DirectionLight)
				|| GameLogic.SelectedEntity.GetType() == typeof(PointLight)
				|| GameLogic.SelectedEntity.GetType() == typeof(SpotLight))
				GameLogic.LevelSceneData.Lights.Remove((Light)GameLogic.SelectedEntity);

			else if (GameLogic.SelectedEntity.GetType() == typeof(InteractiveObject))
				GameLogic.LevelSceneData.InteractiveObjects.Remove((InteractiveObject)GameLogic.SelectedEntity);

			else if (GameLogic.SelectedEntity.GetType() == typeof(Collectable))
				GameLogic.LevelSceneData.Collectables.Remove((Collectable)GameLogic.SelectedEntity);

			else if (GameLogic.SelectedEntity.GetType() == typeof(Branch)
				|| GameLogic.SelectedEntity.GetType() == typeof(Key)
				|| GameLogic.SelectedEntity.GetType() == typeof(Knife)
				|| GameLogic.SelectedEntity.GetType() == typeof(Lantern)
				|| GameLogic.SelectedEntity.GetType() == typeof(Matches))
				GameLogic.LevelSceneData.Items.Remove((Item)GameLogic.SelectedEntity);

			else
				GameLogic.LevelSceneData.BackgroundSprites.Remove(GameLogic.SelectedEntity);
            
			GameLogic.SelectEntityRectangle.IsSelected = false;
            GameLogic.SelectedEntity = null;
            GameLogic.GhostData.Name = "";

			if(GameLogic.EState != EditorState.PlaceWayPoint)
				GameLogic.EState = EditorState.Standard;
        }

		/// <summary>
		/// Setzt das Gameobject auf die richtige Plane
		/// </summary>
		/// <param name="go"></param>
		private void PlaceElementInPlane(GameObject go)
		{
			GameLogic.LevelSceneData.BackgroundSprites.Add(go);
            GameLogic.SelectedEntity = go;
            GameLogic.SelectEntityRectangle = new SelectRectangle(go.Position, go.CollisionBox);
			SortBackgroundPlane();
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
			// Damit beim Klicken nicht ausversehn ein Rectangle gezogen wird.
			GameLogic.EState = EditorState.Standard;

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
					xml.Serialize(writer, GameLogic.LevelSceneData);
					writer.Close();
					myStream.Close();

					SaveDiffuseBackground(saveFileDialog1.FileName);
					SaveNormalBackground(saveFileDialog1.FileName);
					SaveDepthBackground(saveFileDialog1.FileName);
				}
			}
			MouseHelper.ResetClick();
		}

		public void LoadScene()
		{
			// Damit beim Klicken nicht ausversehn ein Rectangle gezogen wird.
			GameLogic.EState = EditorState.Standard;

			GameLogic.LevelSceneData.ResetLevel();
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
					GameLogic.LevelSceneData = (SceneData)xml.Deserialize(myStream);
					reader.Close();
					myStream.Close();
				}
			LoadAllTexturesInPlanes();
			LoadTextures();
			GetHighestId();
			mCamera.GameScreen = GameLogic.LevelSceneData.GamePlane;

			mXmlForm.ResetStats();
			}
			MouseHelper.ResetClick();
			
		}

		private void ShowXmlEditor()
		{
			if (!GameLogic.IsXmlFormShow)
			{
				mXmlForm = new Form1();
				mXmlForm.Show();
				GameLogic.IsXmlFormShow = true;
			}

		}
        
        public void CreateNewScene(Rectangle pGameScreenSize)
        {
            mCamera.Position = new Vector2(0,20);
            mCamera.GameScreen = pGameScreenSize;
			GameLogic.LevelSceneData.ResetLevel();
			GameLogic.LevelSceneData.GamePlane = pGameScreenSize;
			GameLogic.ZDepth = GameLogic.LevelSceneData.GamePlane.Height;
			BaseObject.mIdAll = 0;
		}

		protected void SetDebugMode()
		{
			EngineSettings.IsDebug = !EngineSettings.IsDebug;
			MouseHelper.ResetClick();
		}

		private void GetHighestId()
		{
			int pHighestId = 0;

			foreach (GameObject go in GameLogic.LevelSceneData.Collectables)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			foreach (GameObject go in GameLogic.LevelSceneData.Items)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			foreach (GameObject go in GameLogic.LevelSceneData.Waypoints)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			foreach (GameObject go in GameLogic.LevelSceneData.Enemies)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			for (int i = 0; i < GameLogic.LevelSceneData.BackgroundSprites.Count; i++)
				foreach (GameObject go in GameLogic.LevelSceneData.BackgroundSprites)
					if (go.ObjectId > pHighestId)
						pHighestId = go.ObjectId;

			foreach(GameObject go in GameLogic.LevelSceneData.Lights)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			foreach(GameObject go in GameLogic.LevelSceneData.InteractiveObjects)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			BaseObject.mIdAll = pHighestId;
		}

		// Muss Ingame auch getätigt werden.
		private void LoadAllTexturesInPlanes()
		{
			foreach (GameObject go in GameLogic.LevelSceneData.BackgroundSprites)
			{
				if (go.GetType() == typeof(Sprite))
				{
					Sprite s = (Sprite)go;
					s.LoadTextures();
				}
			}

			foreach(InteractiveObject io in GameLogic.LevelSceneData.InteractiveObjects)
			{
				io.Texture = TextureManager.Instance.GetElementByString(io.TextureName);
			}
		}

		private void LoadTextures()
		{
			foreach (Collectable c in GameLogic.LevelSceneData.Collectables)
				c.Texture = TextureManager.Instance.GetElementByString(c.TextureName);

			foreach (Item i in GameLogic.LevelSceneData.Items)
				i.Texture = TextureManager.Instance.GetElementByString(i.TextureName);
		}

		#endregion

		#region PlaneDropDownMenu
		private void SetAllPlanesVisible()
		{
			GameLogic.ZDepth = GameLogic.LevelSceneData.GamePlane.Height;
			MouseHelper.ResetClick();
		}

		private void SetMoveRectangleState()
		{
			GameLogic.EState = EditorState.PlaceMoveArea;
			MouseHelper.ResetClick();
		}
		#endregion

		#region RenderDropDown

		private void SaveDiffuseBackground(String pPath)
		{
			pPath = pPath.Replace("\\hug\\", "\\backgrounds\\");
			pPath = pPath.Replace(".hug", "Diffuse.png");
			Stream myStream = File.Create(pPath);
			RenderTarget2D rt = new RenderTarget2D(EngineSettings.Graphics.GraphicsDevice, mCamera.GameScreen.Width, mCamera.GameScreen.Height);
			
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(rt);
			
			mSpriteBatch.Begin();
			EngineSettings.Graphics.GraphicsDevice.Clear(Color.ForestGreen);
			if(mGround != null)
				mGround.Draw(mSpriteBatch);
			foreach (GameObject go in GameLogic.LevelSceneData.BackgroundSprites)
				go.Draw(mSpriteBatch);
			mSpriteBatch.End();
			
			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);
			
			rt.SaveAsPng(myStream, rt.Width, rt.Height);
			myStream.Close();
		}

		private void SaveNormalBackground(String pPath)
		{
			pPath = pPath.Replace("\\hug\\", "\\backgrounds\\");
			pPath = pPath.Replace(".hug", "Normal.png");
			Stream myStream = File.Create(pPath);
			RenderTarget2D rt = new RenderTarget2D(EngineSettings.Graphics.GraphicsDevice, mCamera.GameScreen.Width, mCamera.GameScreen.Height);

			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(rt);

			mSpriteBatch.Begin();
			EngineSettings.Graphics.GraphicsDevice.Clear(new Color(129,129,255));
			foreach (GameObject go in GameLogic.LevelSceneData.BackgroundSprites)
				go.DrawNormal(mSpriteBatch);
			mSpriteBatch.End();

			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);

			rt.SaveAsPng(myStream, rt.Width, rt.Height);
			myStream.Close();
		}

		private void SaveDepthBackground(String pPath)
		{
			pPath = pPath.Replace("\\hug\\", "\\backgrounds\\");
			pPath = pPath.Replace(".hug", "Depth.png");
			Stream myStream = File.Create(pPath);
			RenderTarget2D rt = new RenderTarget2D(EngineSettings.Graphics.GraphicsDevice, mCamera.GameScreen.Width, mCamera.GameScreen.Height);

			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(rt);

			mSpriteBatch.Begin();
			EngineSettings.Graphics.GraphicsDevice.Clear(Color.White);
			foreach (GameObject go in GameLogic.LevelSceneData.BackgroundSprites)
				go.DrawDepth(mSpriteBatch);
			mSpriteBatch.End();

			EngineSettings.Graphics.GraphicsDevice.SetRenderTarget(null);

			rt.SaveAsPng(myStream, rt.Width, rt.Height);
			myStream.Close();
		}

		#region Rendertarget DropDown

		private void SetDiffuseRenderTarget()
		{
			mRenderState = RenderState.Diffuse;
		}

		private void SetNormalRenderTarget()
		{
			mRenderState = RenderState.Normal;
		}

		private void SetDepthRenderTarget()
		{
			mRenderState = RenderState.Depth;
		}

		private void SetAORenderTarget()
		{
			mRenderState = RenderState.AO;
		}

		private void SetLightRenderTarget()
		{
			mRenderState = RenderState.Light;
		}

		private void SetFinalRenderTarget()
		{
			mRenderState = RenderState.Final;
		}
		#endregion
		#endregion

		#endregion
	}
}

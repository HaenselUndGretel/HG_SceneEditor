﻿using System;
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
using HG_Data.Objects.Lights;

namespace MenuEditor.GameContent.Scenes
{
	enum LightState
	{
		Center,
		Range
	}

    class EditorScene : Scene
    {
        #region Properties
		protected SceneData mlevelSceneData;

        /// <summary>
        /// Hier sind Objekte für die Scene
        /// </summary>

        private List<InterfaceObject> mInterfaceObjects = new List<InterfaceObject>();
        private DropDownMenu mRightClickDropDown;

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

		#region LightSettings
		LightState mLightState;
		#endregion

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
			mRightClickDropDown = new DropDownMenu(Vector2.Zero, new List<string>() { "" }, new List<Action>() { });

            /// Relevante Sachen für HUD 

            mObjectBox = new ImageBox(new Vector2(EngineSettings.VirtualResWidth - 1440 - 1, 1), new Rectangle(0, 0, 1440 - 440, EngineSettings.VirtualResHeight - 2));
            mObjectInfoBox = new InfoBox(new Vector2(EngineSettings.VirtualResWidth - 440 - 1, 1), new Rectangle(0, 0, 440, (EngineSettings.VirtualResHeight / 2) - 2));
            mObjectInfoBox.BackgroundColor = Color.DarkGray;
            mIconBox = new Box(new Vector2(EngineSettings.VirtualResWidth - 440, EngineSettings.VirtualResHeight / 2 + 1), new Rectangle(0, 0, 440, (EngineSettings.VirtualResHeight / 2) - 2));
            DropDownMenu Main = new DropDownMenu(Vector2.Zero, 
				new List<string>() { "Neu...", "Laden", "Speichern unter...", "DebugMode" }, 
				new List<Action>() { ShowFormNewGame, LoadScene, SaveScene, SetDebugMode });
			DropDownMenu Ebenen = new DropDownMenu(Vector2.Zero, 
				new List<string>() { "Alle Ebenen anzeigen", "Ebene 1", "Ebene 2", "Ebene 3", "Ebene 4", "Ebene 5", "Wegsetzding(PathStuff)" }, 
				new List<Action>() { SetAllPlanesVisible, SetPlane0Visible, SetPlane1Visible, SetPlane2Visible, SetPlane3Visible, SetPlane4Visible, SetMoveRectangleState });

			MenuBar mMenuBarTop = new MenuBar(Vector2.Zero);
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

            GameLogic.Initialize();
        }

        public override void Update()
        {
            ksLast = ksCurrent;
            ksCurrent = Keyboard.GetState();

            if (GameLogic.SelectedEntity != null
                && ksLast.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Delete)
                && ksCurrent.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.Delete))
					RemoveObjectFromPlane();
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
			DrawCollectables();
			DrawItems();
			DrawLights();

			switch(GameLogic.EState)
			{
				case EditorState.PlaceWayPoint:
					DrawWayPoints();
					break;
			}

            DrawSelectedRectangle();

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

			//mSpriteBatch.Begin();
			//foreach (Waypoint w in mlevelSceneData.Waypoints)
			//	w.DropDown.Draw(mSpriteBatch);
			//mSpriteBatch.End();
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

			for (int i = 4; i >= 0; i-- )
			{
				if(mPlaneVisible[i])
				{
					mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
                    foreach (GameObject go in mlevelSceneData.ParallaxPlanes[i].Tiles)
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

        private void DrawSelectedRectangle()
        {
            if (GameLogic.SelectEntityRectangle.IsSelected)
            {
                mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
                GameLogic.SelectEntityRectangle.Draw(mSpriteBatch);
                mSpriteBatch.End();
            }
        }

		private void DrawCollectables()
		{
			mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
			foreach (Collectable c in mlevelSceneData.Collectables)
				c.Draw(mSpriteBatch);
			mSpriteBatch.End();
		}

		private void DrawItems()
		{
			mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
			foreach (Item i in mlevelSceneData.Items)
				i.Draw(mSpriteBatch);
			mSpriteBatch.End();
		}

		private void DrawLights()
		{
			mSpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, mCamera.GetTranslationMatrix());
			foreach (Light l in mlevelSceneData.Lights)
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
				switch(GameLogic.EState)
				{
					case EditorState.PlaceWayPoint:
						foreach(Waypoint w in mlevelSceneData.Waypoints)
						{
							if(w.CollisionBox.Contains(MouseHelper.PositionPoint.X - (int)mCamera.Position.X, MouseHelper.PositionPoint.Y - (int)mCamera.Position.Y))
								w.ShowDropDown(MouseHelper.Position);
						}
						break;
					case EditorState.PlaceMoveArea:
							UpdateRectangleDelete(mlevelSceneData.MoveArea);
							break;
				}
			}

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
				if (MouseHelper.Instance.IsWheelUp)
					GameLogic.ParallaxLayerNow++;
				if (MouseHelper.Instance.IsWheelDown)
					GameLogic.ParallaxLayerNow--;
			}


			//Update aller Objekte für DropDownMenu
			UpdateAllEntities();

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
			}
        }

		private void UpdateMouseLeftClick()
		{
			switch (GameLogic.EState)
			{
				case EditorState.Standard:
					UpdateSelectedEntity();
					break;

				case EditorState.PlaceMoveArea:
					UpdateRectInput();
					break;

				case EditorState.PlaceWayPoint:
					UpdateSelectedEntity();
					UpdateRectInput();
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

				case EditorState.PlaceLight:
					if (mLightState == LightState.Center)
						PlaceLight();
					else if (mLightState == LightState.Range)
						SetLightRange();
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
							mlevelSceneData.MoveArea.Add(tmpRectangle);
							break;
						case EditorState.PlaceWayPoint:
							Waypoint w = new Waypoint();
							w.CollisionBox = tmpRectangle;
							w.Texture = GameLogic.GhostData.Texture.Texture;
							w.Position = new Vector2(tmpRectangle.X, tmpRectangle.Y);
							mlevelSceneData.Waypoints.Add(w);
							break;
					}
				IsDrawingRectangle = false;
			}
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
		}

		private void UpdateAllEntities()
		{
			foreach (Waypoint w in mlevelSceneData.Waypoints)
				w.Update();
		}

		#region Place Funktionen
		private void PlaceInteractiveObject()
		{
			InteractiveObject ioDefault = new InteractiveObject();
			InteractiveObject ioNew = new InteractiveObject();
			ioDefault = InteractiveObjectDataManager.Instance.GetElementByString(GameLogic.GhostData.Name);

			MouseState ms = Mouse.GetState();

			ioNew.Texture = ioDefault.Texture;
			ioNew.TextureName = ioDefault.TextureName;
            ioNew.Position = MouseHelper.Position - mCamera.Position;
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

            ioNew.Position = MouseHelper.Position - new Vector2(ioNew.Texture.Width / 2, ioNew.Texture.Height / 2) - mCamera.Position;

			PlaceElementInPlane(ioNew);
		}

		private void PlaceSprite()
		{
			Sprite s = new Sprite(Vector2.Zero, GameLogic.GhostData.Name);
			s.Position = MouseHelper.Position - new Vector2(s.Texture.Width / 2, s.Texture.Height / 2) - mCamera.Position;
			PlaceElementInPlane(s);
		}

		private void PlaceCollectable()
		{
			if (GameLogic.GhostData.Name.Contains("Artefact"))
			{
				Artefact a = new Artefact(Vector2.Zero, GameLogic.GhostData.Name);
				a.Position = MouseHelper.Position - new Vector2(a.Texture.Width / 2, a.Texture.Height / 2) - mCamera.Position;
				mlevelSceneData.Collectables.Add(a);
			}
			else if (GameLogic.GhostData.Name.Contains("Diary"))
			{
				DiaryEntry d = new DiaryEntry(Vector2.Zero, GameLogic.GhostData.Name);
				d.Position = MouseHelper.Position - new Vector2(d.Texture.Width / 2, d.Texture.Height / 2) - mCamera.Position;
				mlevelSceneData.Collectables.Add(d);
			}
			else if (GameLogic.GhostData.Name.Contains("Toy"))
			{
				Toy t = new Toy(Vector2.Zero, GameLogic.GhostData.Name);
				t.Position = MouseHelper.Position - new Vector2(t.Texture.Width / 2, t.Texture.Height / 2) - mCamera.Position;
				mlevelSceneData.Collectables.Add(t);
			}
		}

		private void PlaceItem()
		{
			if (GameLogic.GhostData.Name.Contains("Branch"))
			{
				Branch a = new Branch(Vector2.Zero, GameLogic.GhostData.Name);
				a.Position = MouseHelper.Position - new Vector2(a.Texture.Width / 2, a.Texture.Height / 2) - mCamera.Position;
				mlevelSceneData.Items.Add(a);
			}
			else if (GameLogic.GhostData.Name.Contains("Knife"))
			{
				Knife d = new Knife(Vector2.Zero, GameLogic.GhostData.Name);
				d.Position = MouseHelper.Position - new Vector2(d.Texture.Width / 2, d.Texture.Height / 2) - mCamera.Position;
				mlevelSceneData.Items.Add(d);
			}
			else if (GameLogic.GhostData.Name.Contains("Key"))
			{
				Key t = new Key(Vector2.Zero, GameLogic.GhostData.Name);
				t.Position = MouseHelper.Position - new Vector2(t.Texture.Width / 2, t.Texture.Height / 2) - mCamera.Position;
				mlevelSceneData.Items.Add(t);
			}
			else if (GameLogic.GhostData.Name.Contains("Lantern"))
			{
				Lantern a = new Lantern(Vector2.Zero, GameLogic.GhostData.Name);
				a.Position = MouseHelper.Position - new Vector2(a.Texture.Width / 2, a.Texture.Height / 2) - mCamera.Position;
				mlevelSceneData.Items.Add(a);
			}
			else if (GameLogic.GhostData.Name.Contains("Matches"))
			{
				Matches d = new Matches(Vector2.Zero, GameLogic.GhostData.Name);
				d.Position = MouseHelper.Position - new Vector2(d.Texture.Width / 2, d.Texture.Height / 2) - mCamera.Position;
				mlevelSceneData.Items.Add(d);
			}
		}

		private void PlaceLight()
		{
			float depth = 1 / GameLogic.ParallaxLayerNow + 1;
			// 32x32 da Texture 64x64 Groß ist und um den Origin verschoben wird.
			Vector2 mousePosition = MouseHelper.Position - new Vector2(32,32) - mCamera.Position;


			if (GameLogic.GhostData.Name.Contains("Direction"))
			{
				DirectionLight d = new DirectionLight(mousePosition);
				d.Depth = depth;
				mlevelSceneData.Lights.Add(d);
				GameLogic.SelectedEntity = d;
			}
			else if (GameLogic.GhostData.Name.Contains("Point"))
			{
				PointLight p = new PointLight(mousePosition);
				p.Depth = depth;
				mlevelSceneData.Lights.Add(p);
				GameLogic.SelectedEntity = p;
			}
			else if (GameLogic.GhostData.Name.Contains("Spot"))
			{
				SpotLight s = new SpotLight(mousePosition);
				s.Depth = depth;
				mlevelSceneData.Lights.Add(s);
				GameLogic.SelectedEntity = s;
			}

			mLightState = LightState.Range;
		}

		private void SetLightRange()
		{
			int foundBy = -1;
			for (int i = 0; i < mlevelSceneData.Lights.Count; i++)
				if (mlevelSceneData.Lights[i] == GameLogic.SelectedEntity)
					foundBy = i;

			if (foundBy < 0) return;

			Light l = mlevelSceneData.Lights[foundBy];
			l.Range = Vector2.Distance(l.Position,(new Vector2(MouseHelper.Position.X - (int)mCamera.Position.X, MouseHelper.Position.Y - (int)mCamera.Position.Y) - new Vector2(32,32)));
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
                    foreach(GameObject go in mlevelSceneData.ParallaxPlanes[GameLogic.ParallaxLayerNow].Tiles)
                        if (go.CollisionBox.Contains(MousePosition))
                        {
							changeEntity = ChangeSelectedEntity(go);
                            return;
                        }
					foreach(Collectable c in mlevelSceneData.Collectables)
						if(c.CollisionBox.Contains(MousePosition))
						{
							changeEntity = ChangeSelectedEntity(c);
							return;
						}

					foreach(Item i in mlevelSceneData.Items)
						if(i.CollisionBox.Contains(MousePosition))
						{
							changeEntity = ChangeSelectedEntity(i);
							return;
						}

					foreach (Light l in mlevelSceneData.Lights)
						if (l.CollisionBox.Contains(MousePosition))
						{
							changeEntity = ChangeSelectedEntity(l);
							return;
						}
					break;

				case EditorState.PlaceWayPoint:
					foreach (Waypoint w in mlevelSceneData.Waypoints)
                        if (w.CollisionBox.Contains(MousePosition))
						{
							changeEntity = ChangeSelectedEntity(w);
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

        private void RemoveObjectFromPlane()
        {
			if (GameLogic.SelectedEntity.GetType() == typeof(Waypoint))
				mlevelSceneData.Waypoints.Remove((Waypoint)GameLogic.SelectedEntity);
			else if (GameLogic.SelectedEntity.GetType() == typeof(DirectionLight)
				|| GameLogic.SelectedEntity.GetType() == typeof(PointLight)
				|| GameLogic.SelectedEntity.GetType() == typeof(SpotLight))
				mlevelSceneData.Lights.Remove((Light)GameLogic.SelectedEntity);
			else
				mlevelSceneData.ParallaxPlanes[GameLogic.ParallaxLayerNow].Tiles.Remove(GameLogic.SelectedEntity);
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
			mlevelSceneData.ParallaxPlanes[GameLogic.ParallaxLayerNow].Tiles.Add(go);
            GameLogic.SelectedEntity = go;
            GameLogic.SelectEntityRectangle = new SelectRectangle(go.Position, go.CollisionBox);
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

                    //// Ersten 5 Listen löschen wichtig!!!
                    //for(int i = 0; i < 5; i++)
                    //{
                    //    mlevelSceneData.ParallaxPlanes.RemoveAt(0);
                    //}
				}
			LoadAllTexturesInPlanes();
			LoadTextures();
			GetHighestId();
			}
			MouseHelper.ResetClick();
			
		}
        
        public void CreateNewScene(Rectangle pGameScreenSize)
        {
            mCamera.Position = new Vector2(0,20);
            mCamera.GameScreen = pGameScreenSize;
			mlevelSceneData.ResetLevel();
			mlevelSceneData.GamePlane = pGameScreenSize;
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

			foreach (GameObject go in mlevelSceneData.Collectables)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			foreach (GameObject go in mlevelSceneData.Items)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			foreach (GameObject go in mlevelSceneData.Waypoints)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			foreach (GameObject go in mlevelSceneData.Enemies)
				if (go.ObjectId > pHighestId)
					pHighestId = go.ObjectId;

			for (int i = 0; i < mlevelSceneData.ParallaxPlanes.Length; i++)
				foreach (GameObject go in mlevelSceneData.ParallaxPlanes[i].Tiles)
					if (go.ObjectId > pHighestId)
						pHighestId = go.ObjectId;

			BaseObject.mIdAll = pHighestId;
		}

		// Muss Ingame auch getätigt werden.
		private void LoadAllTexturesInPlanes()
		{
			for (int i = 4; i >= 0; i-- )
			{
				foreach (GameObject go in mlevelSceneData.ParallaxPlanes[i].Tiles)
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

		private void LoadTextures()
		{
			foreach (Collectable c in mlevelSceneData.Collectables)
				c.Texture = TextureManager.Instance.GetElementByString(c.TextureName);

			foreach (Item i in mlevelSceneData.Items)
				i.Texture = TextureManager.Instance.GetElementByString(i.TextureName);
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

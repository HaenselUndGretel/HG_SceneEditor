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

namespace MenuEditor.GameContent.Scenes
{
    class EditorScene : Scene
    {
        #region Properties

        private MenueBar mMenuBarTop;
        //private Button mButtonMenu;
        //private Button mButtonEdit;

        //private Sprite mTestSprite;
        private ImageWindow mTestWindow;
        private SaveLoadWindow mLoadWindow;
        private Window mTestWindow2;

        private Rectangle GameScreen = new Rectangle(0, 0, 2480, 1440);

        /// <summary>
        /// Hier sind Objekte für die Scene
        /// </summary>

        private List<InterfaceObject> mInterfaceObjects = new List<InterfaceObject>();
        private DropDownMenue mRightClickDropDown;

        private SpriteFont mDebugFont;

        private FormNewGame mFormNewGame;

        private InfoBox mObjectInfoBox;
        private ImageBox mObjectBox;
        private Box mIconBox;

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
            mCamera.GameScreen = GameScreen;
        }
        #endregion

        #region Override Methods

        public override void Initialize()
        {
            mClearColor = Color.Red;
            mMouseCenter = Vector2.Zero;
        }

        public override void LoadContent()
        {
            // Images

            //TextureManager.Instance.Add("Haensel", @"gfx\Haensel");

            /// Testobjekte
            mTestWindow = new ImageWindow(new Vector2(500, 500), "Image Selecter", new Vector2(200, 200));
            mTestWindow2 = new ImageWindow(new Vector2(100, 250), "Test Window", new Vector2(600, 200));
            mLoadWindow = new SaveLoadWindow(new Vector2(300, 500), "Save und Load Window", new Vector2(800, 400));
            mRightClickDropDown = new DropDownMenue(Vector2.Zero, new List<string>() { "Change Background Color", "Open Test Window", "Close Test Window" }, new List<Action>() { SwitchBackgroundColor, mTestWindow.OpenWindow, mTestWindow.CloseWindow });

            mMenuBarTop = new MenueBar(Vector2.Zero);
            //mButtonMenu = new Button(Vector2.Zero, "ButtonTestSprite", "ButtonTestSprite", 100, 20);
            //mButtonEdit = new Button(Vector2.Zero, "ButtonTestSprite", "ButtonTestSprite", 100, 20);
            //mButtonEdit.CurrentTile = 1;
            //mButtonEdit.OnButtonPressed = SetButtonToTile1;


            //mTestSprite = new Sprite(new Vector2(100,100), "ButtonTestSprite", "ButtonTestSprite");

            /// Relevante Sachen für HUD 

            mObjectBox = new ImageBox(new Vector2(EngineSettings.VirtualResWidth - 1440 - 1, 1), new Rectangle(0, 0, 1440 - 440, EngineSettings.VirtualResHeight - 2));
            mObjectInfoBox = new InfoBox(new Vector2(EngineSettings.VirtualResWidth - 440 - 1, 1), new Rectangle(0, 0, 440, (EngineSettings.VirtualResHeight / 2) - 2));
            mObjectInfoBox.BackgroundColor = Color.DarkGray;
            mIconBox = new Box(new Vector2(EngineSettings.VirtualResWidth - 440, EngineSettings.VirtualResHeight / 2 + 1), new Rectangle(0, 0, 440, (EngineSettings.VirtualResHeight / 2) - 2));
            DropDownMenue Main = new DropDownMenue(Vector2.Zero, new List<string>() { "Neu...", "Laden", "Speichern unter...", "Schliessen" }, new List<Action>() { ShowFormNewGame });
            DropDownMenue Ebenen = new DropDownMenue(Vector2.Zero, new List<string>() { "Alle Ebenen anzeigen", "Ebene 1", "Ebene 2", "Ebene 3", "Ebene 4", "Ebene 5", "Wegsetzding(PathStuff)" }, null);

            mMenuBarTop.AddMenueItem("Datei", Main);
            mMenuBarTop.AddMenueItem("Ebenen", Ebenen);
            mMenuBarTop.OrganizeButtons();

            mFormNewGame = new FormNewGame();
            
            mDebugFont = EngineSettings.Content.Load<SpriteFont>(@"font\font");

            // Addet alle InterfaceObjekte in die InterfaceObject Liste
            mInterfaceObjects.Add(mMenuBarTop);
            mInterfaceObjects.Add(mRightClickDropDown);
            mInterfaceObjects.Add(mIconBox);
            mInterfaceObjects.Add(mObjectBox);

            mInterfaceObjects.Add(mTestWindow);
            mInterfaceObjects.Add(mTestWindow2);
            mInterfaceObjects.Add(mLoadWindow);

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
            mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), GameScreen, Color.SpringGreen);
            //mTestSprite.Draw(mSpriteBatch);
            mSpriteBatch.End();

            DrawMenueItems();

            DrawOnScene();
        }

        #endregion

        #region Methods
        public void DrawMenueItems()
        {
            mSpriteBatch.Begin();
            mMenuBarTop.Draw(mSpriteBatch);
            mTestWindow.Draw(mSpriteBatch);
            mTestWindow2.Draw(mSpriteBatch);
            mLoadWindow.Draw(mSpriteBatch);

            // Objectbox + Iconbox + Infobox
            mObjectInfoBox.Draw(mSpriteBatch);
            mObjectBox.Draw(mSpriteBatch);
            mIconBox.Draw(mSpriteBatch);

            Vector2 msPos = MouseHelper.Position - mCamera.Position;
            mSpriteBatch.DrawString(mDebugFont, "MousePosition: " + msPos, new Vector2(0, 20), Color.White);
            mRightClickDropDown.Draw(mSpriteBatch);
            
            // MouseMoveIcon
            if(mMouseCenter != Vector2.Zero)
              mSpriteBatch.Draw(TextureManager.Instance.GetElementByString("pixel"), mMouseCenter, new Rectangle(-2, -2, 5, 5), Color.Black);

            mSpriteBatch.End();
        }

        public void UpdateMouseInput()
        {
            if (MouseHelper.Instance.IsClickedRight)
                mRightClickDropDown.OpenDropDownMenue(MouseHelper.Position);
            if (MouseHelper.Instance.IsPressedMiddle)
                MoveCameraByMouse();
            else
                mMouseCenter = Vector2.Zero;

            if (MouseHelper.Instance.IsWheelUp)
                GameLogic.ParallaxLayerNow++;
            if (MouseHelper.Instance.IsWheelDown)
                GameLogic.ParallaxLayerNow--;
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

        private void SwitchBackgroundColor()
        {
            mClearColor = (mClearColor == Color.White) ? Color.Black : Color.White;
        }

        private void ShowFormNewGame()
        {
            if (!mFormNewGame.Visible)
                mFormNewGame.ShowDialog();
        }
        
        public void CreateNewScene(Rectangle pGameScreenSize)
        {
            GameScreen = pGameScreenSize;
            mCamera.Position = new Vector2(0,20);
            mCamera.GameScreen = pGameScreenSize;
        }
        #endregion
    }
}

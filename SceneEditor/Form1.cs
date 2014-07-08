using HanselAndGretel.Data;
using KryptonEngine;
using KryptonEngine.Controls;
using KryptonEngine.Entities;
using KryptonEngine.Manager;
using KryptonEngine.FModAudio;
using MenuEditor.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using MenuEditor.GameContent.Interface;
using KryptonEngine.HG_Data;

namespace Xml_Editor
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			ResetStats();
			comboBox1.SelectedIndex = 0;
			comboBox2.SelectedIndex = 0;
			comboBox3.SelectedIndex = 0;
			SetGroupboxVisibility(-1);
			SetGroupboxLocation();
			FillSoundCombobox();
		}

		private void FillSoundCombobox()
		{
			if(!Directory.Exists(Environment.CurrentDirectory + @"\Content\sfx\"))
				Directory.CreateDirectory(Environment.CurrentDirectory + @"\Content\sfx\");

			DirectoryInfo environmentPath = new DirectoryInfo(Environment.CurrentDirectory + @"\Content\sfx\");
			foreach (FileInfo f in environmentPath.GetFiles())
			{
				if (f.Name.Contains(".mp3"))
				{
					string fileName = f.Name.Substring(0, f.Name.Length - 4);
					comboBox4.Items.Add(fileName);
				}
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			listBox2.Items.Clear();
			UpdateListbox1();

			MouseHelper.ResetClick();
		}

		public void UpdateInfo()
		{
			int tmpSelectedIndex = listBox2.SelectedIndex;
			UpdateListbox2();
			listBox2.Items.Clear();
			UpdateListbox1();
			listBox2.SelectedIndex = tmpSelectedIndex;
		}

		public void ResetStats()
		{
			label1.Text = "Game Plane: " + GameLogic.LevelSceneData.GamePlane.Width + "; " + GameLogic.LevelSceneData.GamePlane.Height;
			comboBox5.SelectedIndex = (int)GameLogic.LevelSceneData.BackgroundSoundSetting;

			listBox1.SelectedIndex = -1;
			listBox2.Items.Clear();
		}

		private void ResetLabels()
		{
			#region GameObject
			textBox4.Text = "";
			textBox5.Text = "";
			textBox6.Text = "";
			#endregion

			#region Waypoints
			comboBox1.SelectedIndex = 0;
			textBox2.Text = "";
			textBox3.Text = "";
			checkBox1.Checked = false;
			#endregion

			#region InteractiveObject
			comboBox2.SelectedIndex = 0;
			#endregion

			#region Lights
			textBox1.Text = "";
			textBox7.Text = "";
			textBox8.Text = "";
			#endregion
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			GameLogic.IsXmlFormShow = false;
		}

		private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateListbox2();
		}

		private void UpdateListbox2()
		{
			GameLogic.SelectedEntity = null;
			ResetLabels();
			if (listBox1.SelectedIndex < 0 || listBox2.SelectedIndex == -1) return;

			switch (listBox1.SelectedIndex)
			{
				case 1: SetSelectedEntity(GameLogic.LevelSceneData.Waypoints[listBox2.SelectedIndex]);
					SetWaypointInfo();
					break;

				case 2: SetSelectedEntity(GameLogic.LevelSceneData.BackgroundSprites[listBox2.SelectedIndex]);
					break;

				case 3: SetSelectedEntity(GameLogic.LevelSceneData.InteractiveObjects[listBox2.SelectedIndex]);
					SetInteractiveObjectInfo();
					break;

				case 4: SetSelectedEntity(GameLogic.LevelSceneData.Collectables[listBox2.SelectedIndex]);
					SetCollectableInfo();
					break;

				case 5: SetSelectedEntity(GameLogic.LevelSceneData.Items[listBox2.SelectedIndex]);
					SetItemInfo();
					break;

				case 6: ChooseLightInfo();
					break;

				case 7: SetSelectedEntity(GameLogic.LevelSceneData.Events[listBox2.SelectedIndex]);
					SetEventInfo();
					break;

				case 8: SetSelectedEntity(GameLogic.LevelSceneData.Enemies[listBox2.SelectedIndex]);
					break;
			}

			if (listBox1.SelectedIndex == 0)
				SetRectangleInfo(GameLogic.LevelSceneData.MoveArea[listBox2.SelectedIndex]);
			else
			{
				SetGameObjectInfo();
				SetSelectedRectangle();
			}

			SetGroupboxVisibility(listBox1.SelectedIndex);
		}

		private void UpdateListbox1()
		{
			GameLogic.SelectedEntity = null;
			ResetLabels();
			switch (listBox1.SelectedIndex)
			{
				case 0:
					for (int i = 0; i < GameLogic.LevelSceneData.MoveArea.Count; i++)
						listBox2.Items.Add("MoveArea " + i);
					GameLogic.EState = SceneEditor.GameContent.EditorState.Standard;
					break;
				case 1:
					for (int i = 0; i < GameLogic.LevelSceneData.Waypoints.Count; i++)
						listBox2.Items.Add("Waypoint " + i);
					GameLogic.EState = SceneEditor.GameContent.EditorState.PlaceWayPoint;
					break;
				case 2:
					for (int i = 0; i < GameLogic.LevelSceneData.BackgroundSprites.Count; i++)
						listBox2.Items.Add("BackgroundSprite" + i);
					GameLogic.EState = SceneEditor.GameContent.EditorState.Standard;
					break;
				case 3:
					for (int i = 0; i < GameLogic.LevelSceneData.InteractiveObjects.Count; i++)
						listBox2.Items.Add("Interactive " + i);
					GameLogic.EState = SceneEditor.GameContent.EditorState.Standard;
					break;
				case 4:
					for (int i = 0; i < GameLogic.LevelSceneData.Collectables.Count; i++)
						listBox2.Items.Add("Collectable " + i);
					GameLogic.EState = SceneEditor.GameContent.EditorState.Standard;
					break;
				case 5:
					for (int i = 0; i < GameLogic.LevelSceneData.Items.Count; i++)
						listBox2.Items.Add("Item " + i);
					GameLogic.EState = SceneEditor.GameContent.EditorState.Standard;
					break;
				case 6:
					listBox2.Items.Add("AmbientLight");
					for (int i = 0; i < GameLogic.LevelSceneData.Lights.Count; i++)
						listBox2.Items.Add("Light " + i);
					listBox2.Items.Add("DirectionLight");
					GameLogic.EState = SceneEditor.GameContent.EditorState.Standard;
					break;
				case 7:
					for (int i = 0; i < GameLogic.LevelSceneData.Events.Count; i++)
						listBox2.Items.Add("Event" + i);
					GameLogic.EState = SceneEditor.GameContent.EditorState.PlaceEventArea;
					break;
				case 8:
					for (int i = 0; i < GameLogic.LevelSceneData.Enemies.Count; i++)
						listBox2.Items.Add("Enemies" + i);
					GameLogic.EState = SceneEditor.GameContent.EditorState.Standard;
					break;
				//Emitter
				//case 9:
				//	for (int i = 0; i < GameLogic.LevelSceneData.e.Count; i++ )
				//		listBox2.Items.Add("Emitter " + i);
				//	break;
			}
			SetGroupboxVisibility(-1);
		}

		private void SetSelectedRectangle()
		{
			if (GameLogic.SelectedEntity == null) return;
			GameLogic.SelectEntityRectangle = new SelectRectangle(new Vector2(GameLogic.SelectedEntity.CollisionBox.X, GameLogic.SelectedEntity.CollisionBox.Y), GameLogic.SelectedEntity.CollisionBox);
		}

		private void SetGameObjectInfo()
		{
			textBox4.Text = GameLogic.SelectedEntity.Position.ToString();
			textBox5.Text = GameLogic.SelectedEntity.CollisionBox.ToString();
			textBox6.Text = GameLogic.SelectedEntity.DrawZ.ToString();

			label8.Text = "Object ID: " + GameLogic.SelectedEntity.ObjectId;

			SetSelectedRectangle();
		}

		private void Form1_Activated(object sender, EventArgs e)
		{
			GameLogic.XmlFormFocus = true;
		}

		private void Form1_Deactivate(object sender, EventArgs e)
		{
			GameLogic.XmlFormFocus = false;
		}

		#region SetInfo

		private void SetRectangleInfo(Microsoft.Xna.Framework.Rectangle r)
		{
			textBox4.Text = r.Location.ToString() ;
			textBox5.Text = r.ToString();
		}

		private void SetWaypointInfo()
		{
			Waypoint w = (Waypoint)GameLogic.SelectedEntity;

			if (w.MovementOnEnter.Y == -1)
				comboBox1.SelectedIndex = 0;
			else if (w.MovementOnEnter.X == 1)
				comboBox1.SelectedIndex = 1;
			else if (w.MovementOnEnter.Y == -1)
				comboBox1.SelectedIndex = 2;
			else if (w.MovementOnEnter.X == -1)
				comboBox1.SelectedIndex = 3;

			textBox2.Text = w.DestinationScene.ToString();
			textBox3.Text = w.DestinationWaypoint.ToString();
			checkBox1.Checked = w.OneWay;
		}

		private void SetGroupboxVisibility(int index)
		{
			groupBox1.Visible = false;
			groupBox2.Visible = false;
			groupBox3.Visible = false;
			groupBox4.Visible = false;
			groupBox5.Visible = false;
			groupBox6.Visible = false;
			groupBox7.Visible = false;
			if(index > -1)
				groupBox2.Visible = true;

			switch (index)
			{
				case 1: groupBox1.Visible = true;
					break;
				case 3: groupBox3.Visible = true;
					break;
				case 4: groupBox7.Visible = true;
					break;
				case 5: groupBox6.Visible = true;
					break;
				case 6: groupBox4.Visible = true;
					break;
				case 7: groupBox5.Visible = true;
					break;
			}

		}

		private void SetGroupboxLocation()
		{
			groupBox3.Location = groupBox1.Location;
			groupBox4.Location = groupBox1.Location;
			groupBox5.Location = groupBox1.Location;
			groupBox6.Location = groupBox1.Location;
			groupBox7.Location = groupBox1.Location;
		}

		private void SetInteractiveObjectInfo()
		{
			InteractiveObject io = (InteractiveObject)GameLogic.SelectedEntity;
			//int tmp = io.ActionId;
			//if (tmp > 4)
			//	comboBox2.SelectedIndex = tmp - 2;
			//else if (tmp > 2)
			//	comboBox2.SelectedIndex = tmp - 1;
			//else
			comboBox2.SelectedIndex = io.ActionId;

		}

		private void SetLightInfo()
		{
			Light l = (Light)GameLogic.SelectedEntity;
			Vector3 color = l.LightColor * 255;
			textBox1.Text = color.ToString();
			panel2.BackColor = System.Drawing.Color.FromArgb((int)color.X, (int)color.Y, (int)color.Z);

			if (l.GetType() == typeof(PointLight))
			{
				PointLight pl = (PointLight)l;
				int radius = (int)pl.Radius;
				textBox7.Text = radius.ToString();
				textBox8.Text = "";
				textBox11.Text = pl.Intensity.ToString();
			}
			else if(l.GetType() == typeof(DirectionLight))
			{
				textBox7.Text = "";
				textBox8.Text = ConvertVector3NormalizedToValue(GameLogic.LevelSceneData.SceneDirectionLight.Direction, 10).ToString();
			}

			textBox11.Text = l.Intensity.ToString();
		}
		
		private void SetEventInfo()
		{
			EventTrigger et = (EventTrigger)GameLogic.SelectedEntity;
			
			switch(et.Event)
			{
				case EventTrigger.EEvent.SpawnWitch: 
					comboBox3.SelectedIndex = 0;
					textBox9.Text = "";
					textBox10.Text = et.WitchSpawnPosition.ToString();
					comboBox4.SelectedIndex = -1;
					break;
				case EventTrigger.EEvent.TreeFalling:
					comboBox3.SelectedIndex = 1;
					textBox9.Text = et.Target.ToString();
					textBox10.Text = "";
					comboBox4.SelectedIndex = -1;
					break;
				case EventTrigger.EEvent.PlaySound:
					comboBox3.SelectedIndex = 2;
					textBox9.Text = "";
					textBox10.Text = "";
					for (int i = 0; i < comboBox4.Items.Count; i++ )
					{
						if(et.SoundName.Equals(comboBox4.Items[i]))
						{
							comboBox4.SelectedIndex = i;
							break;
						}
						else
							comboBox4.SelectedIndex = -1;
					}
					break;
				case EventTrigger.EEvent.ActivateTrigger:
					comboBox3.SelectedIndex = 3;
					break;
			}
		}

		private void SetItemInfo()
		{
			Item i = (Item)GameLogic.SelectedEntity;
			checkBox2.Checked = i.IsHidden;
			String tmp = GameLogic.SelectedEntity.GetType().ToString();

			label17.Text = GameLogic.SelectedEntity.GetType().ToString().Substring(GameLogic.SelectedEntity.GetType().ToString().LastIndexOf('.') + 1);
		}

		private void SetCollectableInfo()
		{
			Collectable c = (Collectable)GameLogic.SelectedEntity;
			checkBox3.Checked = c.IsHidden;
		}
		#endregion

		#region ApplyInteractiveObjectInfo
		private int ApplyInteractiveObjectActionId()
		{
			//if (comboBox2.SelectedIndex > 4)
			//	return comboBox2.SelectedIndex + 2;
			//else if (comboBox2.SelectedIndex > 2)
			//	return comboBox2.SelectedIndex + 1;
			//else
				return comboBox2.SelectedIndex;
		}
		#endregion

		#region ApplyLight

		private void ChooseLightInfo()
		{
			if (listBox2.SelectedIndex == 0)
				SetSelectedEntity(GameLogic.LevelSceneData.SceneAmbientLight);
			else if (listBox2.SelectedIndex + 1 == listBox2.Items.Count)
				SetSelectedEntity(GameLogic.LevelSceneData.SceneDirectionLight);
			else
				SetSelectedEntity(GameLogic.LevelSceneData.Lights[listBox2.SelectedIndex - 1]);
			SetLightInfo();
		}

		private void ApplyLightInfo(Light l)
		{
			l.LightColor = ConvertTextboxToVector3Normalized(textBox1, 255);
			if(l.GetType() == typeof(PointLight))
			{
				PointLight pl = (PointLight)GameLogic.LevelSceneData.Lights[listBox2.SelectedIndex - 1];
				pl.Radius = ApplyLightRange();
				pl.SetDrawCircle();
			}
			if(l.GetType() == typeof(DirectionLight))
			{
				GameLogic.LevelSceneData.SceneDirectionLight.Direction = ConvertTextboxToVector3Normalized(textBox8, 10);
			}
		}

		private int ApplyLightRange()
		{
			return Convert.ToInt32(textBox7.Text);
		}

		#endregion

		#region Convert

		private Vector2 ConvertTextboxToVector2(TextBox tb)
		{
			Vector2 nVector;
			String workString = tb.Text;
			int startpos = workString.IndexOf("X:") + 2;
			int length = workString.IndexOf("Y:") - startpos - 1;
			if (length == 0)
				nVector.X = 0;
			else
				nVector.X = Convert.ToInt32(workString.Substring(startpos, length));

			startpos = workString.IndexOf("Y:") + 2;
			length = workString.IndexOf("}") - startpos;
			if (length == 0)
				nVector.Y = 0;
			else
				nVector.Y = Convert.ToInt32(workString.Substring(startpos, length));

			return nVector;
		}

		private Vector3 ConvertTextboxToVector3(TextBox tb)
		{
			Vector3 nVector3 = Vector3.Zero;
			String workString = tb.Text;
			int startpos = workString.IndexOf("X:") + 2;
			int length = workString.IndexOf("Y:") - startpos - 1;
			if (length == 0)
				nVector3.X = 0;
			else
				nVector3.X = Convert.ToInt32(workString.Substring(startpos, length));

			startpos = workString.IndexOf("Y:") + 2;
			length = workString.IndexOf("Z:") - startpos - 1;
			if (length == 0)
				nVector3.Y = 0;
			else
				nVector3.Y = Convert.ToInt32(workString.Substring(startpos, length));

			startpos = workString.IndexOf("Z:") + 2;
			length = workString.IndexOf("}") - startpos;
			if (length == 0)
				nVector3.Z = 0;
			else
				nVector3.Z = Convert.ToInt32(workString.Substring(startpos, length));

			return nVector3;
		}

		private Vector3 ConvertTextboxToVector3Normalized(TextBox tb, int NormalizedValue)
		{
			Vector3 nVector = ConvertTextboxToVector3(tb);

			nVector.X = (nVector.X > NormalizedValue) ? NormalizedValue : nVector.X;
			nVector.Y = (nVector.Y > NormalizedValue) ? NormalizedValue : nVector.Y;
			nVector.Z = (nVector.Z > NormalizedValue) ? NormalizedValue : nVector.Z;

			nVector.X = (nVector.X > 0) ? nVector.X / NormalizedValue : 0;
			nVector.Y = (nVector.Y > 0) ? nVector.Y / NormalizedValue : 0;
			nVector.Z = (nVector.Z > 0) ? nVector.Z / NormalizedValue : 0;

			return nVector;
		}

		private Vector3 ConvertVector3NormalizedToValue(Vector3 Vector, int NormalizedValue)
		{
			return Vector * NormalizedValue;
		}

		private Microsoft.Xna.Framework.Rectangle ConvertTextboxToRectangle(TextBox tb)
		{
			Vector2 v = ConvertTextboxToVector2(textBox4);

			return ConvertTextboxToRectangle(tb, v);
		}

		private Microsoft.Xna.Framework.Rectangle ConvertTextboxToRectangle(TextBox tb, Vector2 pPosition)
		{
			Microsoft.Xna.Framework.Rectangle newRectangle = new Microsoft.Xna.Framework.Rectangle();
			String workString = tb.Text;

			newRectangle.X = (int)pPosition.X;
			newRectangle.Y = (int)pPosition.Y;

			int startpos = workString.IndexOf("Width:") + 6;
			int length = workString.IndexOf("Height:") - startpos - 1;
			if (length == 0)
				newRectangle.Width = 0;
			else
				newRectangle.Width = Convert.ToInt32(workString.Substring(startpos, length));

			startpos = workString.IndexOf("Height:") + 7;
			length = workString.IndexOf("}") - startpos;
			if (length == 0)
				newRectangle.Height = 0;
			else
				newRectangle.Height = Convert.ToInt32(workString.Substring(startpos, length));

			return newRectangle;
		}
		#endregion

		private void SetSelectedEntity(GameObject go)
		{
			GameLogic.SelectedEntity = go;
		}

		private bool IsRightType<T>()
		{
			if (GameLogic.SelectedEntity == null) return false;

			if (GameLogic.SelectedEntity.GetType() == typeof(T))
				return true;

			return false;
		}

		#region TextboxChanged

		//erste Textbox in der Light Group
		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (textBox1.Text == "") return;
			if (!IsRightType<PointLight>() && !IsRightType<AmbientLight>() && !IsRightType<DirectionLight>()) return;
			
			Light l = (Light)GameLogic.SelectedEntity;
			l.LightColor = ConvertTextboxToVector3Normalized(textBox1, 255);
			SetLightInfo();			
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			if (!IsRightType<Waypoint>()) return;

			Waypoint w = (Waypoint)GameLogic.SelectedEntity;
			if (textBox2.Text == "")
				w.DestinationScene = 0;
			else
				w.DestinationScene = Convert.ToInt32(textBox2.Text);
			SetWaypointInfo();
		}

		private void textBox3_TextChanged(object sender, EventArgs e)
		{
			if (!IsRightType<Waypoint>()) return;
			Waypoint w = (Waypoint)GameLogic.SelectedEntity;
			if (textBox3.Text == "")
				w.DestinationWaypoint = 0;
			else
				w.DestinationWaypoint = Convert.ToInt32(textBox3.Text);
			SetWaypointInfo();
		}

		private void textBox4_TextChanged(object sender, EventArgs e)
		{
			if (textBox4.Text == "" || listBox1.SelectedIndex == 0) return;

			GameLogic.SelectedEntity.Position = ConvertTextboxToVector2(textBox4);

			SetGameObjectInfo();
		}

		private void textBox5_TextChanged(object sender, EventArgs e)
		{
			if (textBox5.Text == "") return;

			if (listBox1.SelectedIndex == 0)
				GameLogic.LevelSceneData.MoveArea[listBox2.SelectedIndex] = ConvertTextboxToRectangle(textBox5);
			else
			{
				GameLogic.SelectedEntity.CollisionBox = ConvertTextboxToRectangle(textBox5);
				SetGameObjectInfo();
			}
		}

		private void textBox6_TextChanged(object sender, EventArgs e)
		{
			if (textBox6.Text == "") return;

			GameLogic.SelectedEntity.DrawZ = Convert.ToInt32(textBox6.Text);
			SetGameObjectInfo();
		}

		private void textBox7_TextChanged(object sender, EventArgs e)
		{
			if (textBox7.Text == "") return;
			if (!IsRightType<PointLight>()) return;

			PointLight pl = (PointLight)GameLogic.SelectedEntity;
			pl.Radius = Convert.ToInt32(textBox7.Text);
			SetLightInfo();
		}

		private void textBox8_TextChanged(object sender, EventArgs e)
		{
			if (textBox8.Text == "") return;
			if (!IsRightType<DirectionLight>()) return;

			DirectionLight dl = (DirectionLight)GameLogic.SelectedEntity;
			dl.Direction = ConvertTextboxToVector3Normalized(textBox8, 10);
			SetLightInfo();
		}

		private void textBox9_TextChanged(object sender, EventArgs e)
		{
			if (!IsRightType<EventTrigger>()) return;
			if (textBox9.Text == "") return;

			EventTrigger et = (EventTrigger)GameLogic.SelectedEntity;
			if (et.Event != EventTrigger.EEvent.TreeFalling) return;

			et.Target = Convert.ToInt32(textBox9.Text);
			SetEventInfo();
		}

		private void textBox10_TextChanged(object sender, EventArgs e)
		{
			if (!IsRightType<EventTrigger>()) return;
			if (textBox10.Text == "") return;

			EventTrigger et = (EventTrigger)GameLogic.SelectedEntity;
			if (et.Event != EventTrigger.EEvent.SpawnWitch) return;

			et.WitchSpawnPosition = ConvertTextboxToVector2(textBox10);
			SetEventInfo();
		}


		private void textBox11_TextChanged(object sender, EventArgs e)
		{
			if (textBox11.Text == "") return;
			if (!IsRightType<PointLight>()) return;

			PointLight pl = (PointLight)GameLogic.SelectedEntity;
			pl.Intensity = (float)Convert.ToDouble(textBox11.Text);
			SetLightInfo();
		}
		#endregion

		#region ComboboxChanged
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!IsRightType<Waypoint>()) return;

			Waypoint w = (Waypoint)GameLogic.SelectedEntity;
			if (w == null) return;

			switch (comboBox1.SelectedIndex)
			{
				case 0: w.MovementOnEnter = new Vector2(0, -1);
					break;
				case 1: w.MovementOnEnter = new Vector2(1, 0);
					break;
				case 2: w.MovementOnEnter = new Vector2(0, 1);
					break;
				case 3: w.MovementOnEnter = new Vector2(-1, 0);
					break;
			}
			SetWaypointInfo();
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!IsRightType<InteractiveObject>()) return;

			InteractiveObject io = (InteractiveObject)GameLogic.SelectedEntity;
			io.ActionId = ApplyInteractiveObjectActionId();
			SetInteractiveObjectInfo();
		}

		private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!IsRightType<EventTrigger>()) return;

			EventTrigger et = (EventTrigger)GameLogic.SelectedEntity;
			switch(comboBox3.SelectedIndex)
			{
				case 0: et.Event = EventTrigger.EEvent.SpawnWitch;
					break;
				case 1: et.Event = EventTrigger.EEvent.TreeFalling;
					break;
				case 2: et.Event = EventTrigger.EEvent.PlaySound;
					break;
				case 3: et.Event = EventTrigger.EEvent.ActivateTrigger;
					break;
			}
			SetEventInfo();
		}

		private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!IsRightType<EventTrigger>() || comboBox4.SelectedIndex == -1) return;

			EventTrigger et = (EventTrigger)GameLogic.SelectedEntity;

			et.SoundName = comboBox4.Items[comboBox4.SelectedIndex].ToString();
			FmodMediaPlayer.Instance.AddSong(et.SoundName);
		}

		private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
		{
			GameLogic.LevelSceneData.BackgroundSoundSetting = (SoundSetting)comboBox5.SelectedIndex;
		}
		#endregion

		#region CheckboxChanged
		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if (!IsRightType<Waypoint>()) return;

			Waypoint w = (Waypoint)GameLogic.SelectedEntity;
			w.OneWay = checkBox1.Checked;
			SetWaypointInfo();
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			if (!IsRightType<Branch>() && !IsRightType<Item>() && !IsRightType<Key>() && !IsRightType<Knife>() && !IsRightType<Matches>() && IsRightType<Lantern>()) return;

			Item i = (Item)GameLogic.SelectedEntity;
			i.IsHidden = checkBox2.Checked;
			SetItemInfo();
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			if (!IsRightType<Collectable>()) return;

			Collectable c = (Collectable)GameLogic.SelectedEntity;
			c.IsHidden = checkBox3.Checked;
			SetCollectableInfo();
		}

		#endregion
	}
}

namespace Xml_Editor
{
	partial class Form1
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.GroupLists = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label1 = new System.Windows.Forms.Label();
			this.listBox2 = new System.Windows.Forms.ListBox();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textBox6 = new System.Windows.Forms.TextBox();
			this.textBox5 = new System.Windows.Forms.TextBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textBox8 = new System.Windows.Forms.TextBox();
			this.textBox7 = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.comboBox4 = new System.Windows.Forms.ComboBox();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textBox9 = new System.Windows.Forms.TextBox();
			this.textBox10 = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.label17 = new System.Windows.Forms.Label();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.checkBox3 = new System.Windows.Forms.CheckBox();
			this.groupBox8 = new System.Windows.Forms.GroupBox();
			this.label18 = new System.Windows.Forms.Label();
			this.comboBox5 = new System.Windows.Forms.ComboBox();
			this.textBox11 = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.GroupLists.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox7.SuspendLayout();
			this.groupBox8.SuspendLayout();
			this.SuspendLayout();
			// 
			// GroupLists
			// 
			this.GroupLists.BackColor = System.Drawing.SystemColors.Info;
			this.GroupLists.Controls.Add(this.panel1);
			this.GroupLists.Controls.Add(this.listBox2);
			this.GroupLists.Controls.Add(this.listBox1);
			this.GroupLists.Location = new System.Drawing.Point(12, 13);
			this.GroupLists.Name = "GroupLists";
			this.GroupLists.Padding = new System.Windows.Forms.Padding(5);
			this.GroupLists.Size = new System.Drawing.Size(153, 709);
			this.GroupLists.TabIndex = 1;
			this.GroupLists.TabStop = false;
			this.GroupLists.Text = "Objekte";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.DimGray;
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(0, 170);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(153, 27);
			this.panel1.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Game Plane:";
			// 
			// listBox2
			// 
			this.listBox2.FormattingEnabled = true;
			this.listBox2.ItemHeight = 14;
			this.listBox2.Location = new System.Drawing.Point(8, 204);
			this.listBox2.Name = "listBox2";
			this.listBox2.Size = new System.Drawing.Size(135, 494);
			this.listBox2.TabIndex = 2;
			this.listBox2.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
			// 
			// listBox1
			// 
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 14;
			this.listBox1.Items.AddRange(new object[] {
            "MoveArea",
            "Waypoints",
            "Background Sprites",
            "InteractiveObjects",
            "Collectables",
            "Items",
            "Lights",
            "EventTrigger",
            "Enemies",
            "PartikelEmitter"});
			this.listBox1.Location = new System.Drawing.Point(8, 19);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(135, 144);
			this.listBox1.TabIndex = 0;
			this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.Transparent;
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.comboBox1);
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Controls.Add(this.textBox3);
			this.groupBox1.Controls.Add(this.textBox2);
			this.groupBox1.Location = new System.Drawing.Point(171, 141);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(401, 131);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Waypoints";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 76);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(78, 14);
			this.label7.TabIndex = 5;
			this.label7.Text = "Ziel Waypoint:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 48);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(59, 14);
			this.label6.TabIndex = 5;
			this.label6.Text = "Ziel Scene:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 21);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(54, 14);
			this.label5.TabIndex = 5;
			this.label5.Text = "Verlassen:";
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            "Norden",
            "Osten",
            "Süden",
            "Westen"});
			this.comboBox1.Location = new System.Drawing.Point(143, 16);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(252, 22);
			this.comboBox1.TabIndex = 4;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(9, 107);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(65, 18);
			this.checkBox1.TabIndex = 4;
			this.checkBox1.Text = "Oneway";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(143, 76);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(252, 21);
			this.textBox3.TabIndex = 0;
			this.textBox3.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(143, 48);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(252, 21);
			this.textBox2.TabIndex = 0;
			this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.Transparent;
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.textBox6);
			this.groupBox2.Controls.Add(this.textBox5);
			this.groupBox2.Controls.Add(this.textBox4);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(171, 13);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(401, 122);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "GameObject Info";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 93);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(58, 14);
			this.label8.TabIndex = 6;
			this.label8.Text = "Object ID: ";
			// 
			// textBox6
			// 
			this.textBox6.Location = new System.Drawing.Point(143, 60);
			this.textBox6.Name = "textBox6";
			this.textBox6.Size = new System.Drawing.Size(252, 21);
			this.textBox6.TabIndex = 4;
			this.textBox6.TextChanged += new System.EventHandler(this.textBox6_TextChanged);
			// 
			// textBox5
			// 
			this.textBox5.Location = new System.Drawing.Point(143, 36);
			this.textBox5.Name = "textBox5";
			this.textBox5.Size = new System.Drawing.Size(252, 21);
			this.textBox5.TabIndex = 4;
			this.textBox5.TextChanged += new System.EventHandler(this.textBox5_TextChanged);
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(143, 12);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(252, 21);
			this.textBox4.TabIndex = 4;
			this.textBox4.TextChanged += new System.EventHandler(this.textBox4_TextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 67);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(45, 14);
			this.label4.TabIndex = 4;
			this.label4.Text = "Draw Z:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 42);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 14);
			this.label3.TabIndex = 4;
			this.label3.Text = "Collisionbox:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(47, 14);
			this.label2.TabIndex = 4;
			this.label2.Text = "Position:";
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.Color.Transparent;
			this.groupBox3.Controls.Add(this.label9);
			this.groupBox3.Controls.Add(this.comboBox2);
			this.groupBox3.Location = new System.Drawing.Point(171, 279);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(401, 55);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Interactive Objects";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(6, 23);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(45, 14);
			this.label9.TabIndex = 2;
			this.label9.Text = "Activity:";
			// 
			// comboBox2
			// 
			this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Items.AddRange(new object[] {
            "None",
            "CaughtInCobweb",
            "FreeFromCobweb",
            "CaughtInSwamp",
            "FreeFromSwamp",
            "KnockOverTree",
            "BalanceOverTree",
            "PushRock",
            "SlipThroughRock",
            "JumpOverGap",
            "LegUp",
            "LegUpGrab",
            "UseKey",
            "PushDoor",
            "PullDoor",
            "UseChalk",
            "UseWell",
            "UseItem",
            "SwitchItem",
            "BalanceOverBrokenTree"});
			this.comboBox2.Location = new System.Drawing.Point(143, 20);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(252, 22);
			this.comboBox2.TabIndex = 0;
			this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
			// 
			// groupBox4
			// 
			this.groupBox4.BackColor = System.Drawing.Color.Transparent;
			this.groupBox4.Controls.Add(this.panel2);
			this.groupBox4.Controls.Add(this.label19);
			this.groupBox4.Controls.Add(this.label12);
			this.groupBox4.Controls.Add(this.label11);
			this.groupBox4.Controls.Add(this.textBox11);
			this.groupBox4.Controls.Add(this.textBox8);
			this.groupBox4.Controls.Add(this.textBox7);
			this.groupBox4.Controls.Add(this.textBox1);
			this.groupBox4.Controls.Add(this.label10);
			this.groupBox4.Location = new System.Drawing.Point(173, 340);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(399, 173);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Light";
			// 
			// panel2
			// 
			this.panel2.Location = new System.Drawing.Point(9, 126);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(388, 21);
			this.panel2.TabIndex = 8;
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(4, 81);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(54, 14);
			this.label12.TabIndex = 7;
			this.label12.Text = "Direction:";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(6, 54);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(40, 14);
			this.label11.TabIndex = 7;
			this.label11.Text = "Radius:";
			// 
			// textBox8
			// 
			this.textBox8.Location = new System.Drawing.Point(141, 78);
			this.textBox8.Name = "textBox8";
			this.textBox8.Size = new System.Drawing.Size(252, 21);
			this.textBox8.TabIndex = 6;
			this.textBox8.TextChanged += new System.EventHandler(this.textBox8_TextChanged);
			// 
			// textBox7
			// 
			this.textBox7.Location = new System.Drawing.Point(141, 51);
			this.textBox7.Name = "textBox7";
			this.textBox7.Size = new System.Drawing.Size(252, 21);
			this.textBox7.TabIndex = 6;
			this.textBox7.TextChanged += new System.EventHandler(this.textBox7_TextChanged);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(141, 24);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(252, 21);
			this.textBox1.TabIndex = 6;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(6, 27);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(35, 14);
			this.label10.TabIndex = 0;
			this.label10.Text = "Color:";
			// 
			// groupBox5
			// 
			this.groupBox5.BackColor = System.Drawing.Color.Transparent;
			this.groupBox5.Controls.Add(this.label13);
			this.groupBox5.Controls.Add(this.label16);
			this.groupBox5.Controls.Add(this.comboBox4);
			this.groupBox5.Controls.Add(this.comboBox3);
			this.groupBox5.Controls.Add(this.label15);
			this.groupBox5.Controls.Add(this.textBox9);
			this.groupBox5.Controls.Add(this.textBox10);
			this.groupBox5.Controls.Add(this.label14);
			this.groupBox5.Location = new System.Drawing.Point(173, 493);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(399, 139);
			this.groupBox5.TabIndex = 7;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Events";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.Location = new System.Drawing.Point(6, 23);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(36, 14);
			this.label13.TabIndex = 1;
			this.label13.Text = "Event:";
			// 
			// label16
			// 
			this.label16.AutoSize = true;
			this.label16.Location = new System.Drawing.Point(4, 105);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(58, 14);
			this.label16.TabIndex = 7;
			this.label16.Text = "Sound File:";
			// 
			// comboBox4
			// 
			this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.comboBox4.FormattingEnabled = true;
			this.comboBox4.Location = new System.Drawing.Point(141, 102);
			this.comboBox4.Name = "comboBox4";
			this.comboBox4.Size = new System.Drawing.Size(253, 22);
			this.comboBox4.TabIndex = 0;
			this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
			// 
			// comboBox3
			// 
			this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Items.AddRange(new object[] {
            "SpawnWitch",
            "TreeFalling",
            "PlaySound",
            "ActivateTrigger"});
			this.comboBox3.Location = new System.Drawing.Point(141, 20);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(253, 22);
			this.comboBox3.TabIndex = 0;
			this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
			// 
			// label15
			// 
			this.label15.AutoSize = true;
			this.label15.Location = new System.Drawing.Point(6, 78);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(113, 14);
			this.label15.TabIndex = 7;
			this.label15.Text = "Witch Spawn Position:";
			// 
			// textBox9
			// 
			this.textBox9.Location = new System.Drawing.Point(141, 48);
			this.textBox9.Name = "textBox9";
			this.textBox9.Size = new System.Drawing.Size(252, 21);
			this.textBox9.TabIndex = 6;
			this.textBox9.TextChanged += new System.EventHandler(this.textBox9_TextChanged);
			// 
			// textBox10
			// 
			this.textBox10.Location = new System.Drawing.Point(141, 75);
			this.textBox10.Name = "textBox10";
			this.textBox10.Size = new System.Drawing.Size(252, 21);
			this.textBox10.TabIndex = 6;
			this.textBox10.TextChanged += new System.EventHandler(this.textBox10_TextChanged);
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.Location = new System.Drawing.Point(6, 51);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(50, 14);
			this.label14.TabIndex = 0;
			this.label14.Text = "Baum ID:";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.label17);
			this.groupBox6.Controls.Add(this.checkBox2);
			this.groupBox6.Location = new System.Drawing.Point(171, 638);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(401, 49);
			this.groupBox6.TabIndex = 8;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Items";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(328, 21);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(43, 14);
			this.label17.TabIndex = 1;
			this.label17.Text = "label17";
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(9, 20);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(68, 18);
			this.checkBox2.TabIndex = 0;
			this.checkBox2.Text = "IsHidden";
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.checkBox3);
			this.groupBox7.Location = new System.Drawing.Point(173, 693);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(401, 49);
			this.groupBox7.TabIndex = 8;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Collectable";
			// 
			// checkBox3
			// 
			this.checkBox3.AutoSize = true;
			this.checkBox3.Location = new System.Drawing.Point(9, 20);
			this.checkBox3.Name = "checkBox3";
			this.checkBox3.Size = new System.Drawing.Size(68, 18);
			this.checkBox3.TabIndex = 0;
			this.checkBox3.Text = "IsHidden";
			this.checkBox3.UseVisualStyleBackColor = true;
			this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
			// 
			// groupBox8
			// 
			this.groupBox8.Controls.Add(this.label18);
			this.groupBox8.Controls.Add(this.comboBox5);
			this.groupBox8.Location = new System.Drawing.Point(578, 13);
			this.groupBox8.Name = "groupBox8";
			this.groupBox8.Size = new System.Drawing.Size(294, 81);
			this.groupBox8.TabIndex = 9;
			this.groupBox8.TabStop = false;
			this.groupBox8.Text = "Scene Settings";
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(6, 18);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(73, 14);
			this.label18.TabIndex = 1;
			this.label18.Text = "Sound Setting";
			// 
			// comboBox5
			// 
			this.comboBox5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.comboBox5.FormattingEnabled = true;
			this.comboBox5.Items.AddRange(new object[] {
            "Forest",
            "Mountain",
            "Swamp",
            "Inside"});
			this.comboBox5.Location = new System.Drawing.Point(6, 39);
			this.comboBox5.Name = "comboBox5";
			this.comboBox5.Size = new System.Drawing.Size(282, 22);
			this.comboBox5.TabIndex = 0;
			this.comboBox5.SelectedIndexChanged += new System.EventHandler(this.comboBox5_SelectedIndexChanged);
			// 
			// textBox11
			// 
			this.textBox11.Location = new System.Drawing.Point(141, 105);
			this.textBox11.Name = "textBox11";
			this.textBox11.Size = new System.Drawing.Size(252, 21);
			this.textBox11.TabIndex = 6;
			this.textBox11.TextChanged += new System.EventHandler(this.textBox11_TextChanged);
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(4, 108);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(50, 14);
			this.label19.TabIndex = 7;
			this.label19.Text = "Intensity:";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.LightCyan;
			this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.ClientSize = new System.Drawing.Size(884, 734);
			this.Controls.Add(this.groupBox8);
			this.Controls.Add(this.groupBox7);
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.GroupLists);
			this.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.Font = new System.Drawing.Font("Miramonte", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(900, 772);
			this.MinimumSize = new System.Drawing.Size(900, 772);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Xml - Editor";
			this.Activated += new System.EventHandler(this.Form1_Activated);
			this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.GroupLists.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.groupBox6.ResumeLayout(false);
			this.groupBox6.PerformLayout();
			this.groupBox7.ResumeLayout(false);
			this.groupBox7.PerformLayout();
			this.groupBox8.ResumeLayout(false);
			this.groupBox8.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox GroupLists;
		private System.Windows.Forms.ListBox listBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ListBox listBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox6;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textBox7;
		private System.Windows.Forms.TextBox textBox8;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.ComboBox comboBox3;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.ComboBox comboBox4;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textBox9;
		private System.Windows.Forms.TextBox textBox10;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.GroupBox groupBox7;
		private System.Windows.Forms.CheckBox checkBox3;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.GroupBox groupBox8;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.ComboBox comboBox5;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox textBox11;
	}
}


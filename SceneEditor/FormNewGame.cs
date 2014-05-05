using KryptonEngine.SceneManagement;
using MenuEditor.GameContent.Scenes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MenuEditor
{
    public partial class FormNewGame : Form
    {
        const char WILDCARD = 'x';

        public FormNewGame()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EditorScene es = (EditorScene)SceneManager.Instance.GetScene("Editor");

            String gameScreenResolution = comboBox1.Items[comboBox1.SelectedIndex].ToString();

            int posWildCard = gameScreenResolution.IndexOf(WILDCARD);

            int gameScreenResolutionWidth = Convert.ToInt16(gameScreenResolution.Substring(0, posWildCard));
            int gameScreenResolutionHeight = Convert.ToInt16(gameScreenResolution.Substring(posWildCard + 1, gameScreenResolution.Length - (posWildCard + 1)));

            es.CreateNewScene(new Microsoft.Xna.Framework.Rectangle(0, 0, gameScreenResolutionWidth, gameScreenResolutionHeight));

            this.Close();
        }
    }
}

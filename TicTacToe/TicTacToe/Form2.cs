using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToe
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void play_Click(object sender, EventArgs e)
        {
            Form1.setPlayerNames(p1.Text, p2.Text); //When user clicks play, it runs the method where the names is set.

            if (p2.Text == "") //If there is no name for p2, then activate computer and name it computer on the field.
            {
                Form1.computer = true;
                Form1.player2 = "Computer";
            }

            if (p1.Text == "") //If there is not a name for p1, then form1 won't appear. 
                {
                    MessageBox.Show("Please enter a name for Player 1");
                }
            else
            {
                this.Hide(); //Hides the namebox after "play" has been clicked.
            }

        }
        
        public void p2_KeyPress(object sender, KeyPressEventArgs e) //After writing player 2's name, you are now able to press enter instead of pressing the "play" button.
        {
            if (e.KeyChar.ToString() == "\r") //If the clicked button (enter) is equal to a return, then click the play button.
                play.PerformClick();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing) //If the reason for closing is because the user pressed exit, then close the application.
            {
                Environment.Exit(0);
            }
        }

    }
}


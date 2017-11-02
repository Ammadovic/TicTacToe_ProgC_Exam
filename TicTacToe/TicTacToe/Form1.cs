using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TicTacToe
{
    public partial class Form1 : Form
    {
        bool turn = true; //To regulate whose turn it is. When true = X turn, when false = O turn.
        public static bool computer = false; //To check if user i playing against computer or not. By default, it's not vs computer.
        int turn_count = 0; //Counts up every time a user makes a turn. By the 9th turn, there either has to be found a winner, otherwise it's a draw.
        public static String player1, player2; //Player names, being used to update their names to the point score.

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form2 f2 = new Form2(); //Instantiating an object with Form2.

            //When Form1 is loading, the program has to open Form2 first.
            f2.ShowDialog(); //Makes sure that the Form1 does not occur, before the player has closed Form2 (by pressing Play etc.)

            //Changes the score labels to the players names. Only gets executed, when the dialog box is closed.
            label1.Text = player1;
            label2.Text = player2;

        }


        public static void setPlayerNames(String n1, String n2) //Updates the typed player names.
        {
            player1 = n1;
            player2 = n2;
        }

        

        private void button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender; //Converts the sender object to a button, and store it in b. Makes me able to call the properties for the buttons.
            if (turn) //If the boolean variable "turn" is true, then it is X's move, and therefore the box should show an X.
                b.Text = "X";
            else //Otherwise it is O's turn, and then the box should show an O.
                b.Text = "O";

            turn = !turn; //After the turn, it has to be the others turn.
            b.Enabled = false; //Makes sure that you cannot click an already clicked button.
            turn_count++; //At the end of each turn, it adds +1 to the count.

            checkForWinner(); //At the end of each turn, it checks for a winner.

            //Check if playing is against computer, and O's turn.
            if ((!turn) && (computer))
            {
                computer_makes_move();
            }
        }

        private void checkForWinner()
        {
            bool winnerFound = false; //Keeps a track on whether a winner has been found or not.

            //Horizontal checks on row A, B and C.
            //When the first button is pressed, all the other are empty, and therefore equal. That is why A1, B1 and C1 has to be disabled, because that requires the rest of the buttons in the same row to be disabled as well.
            if ((A1.Text == A2.Text) && (A2.Text == A3.Text) && (!A1.Enabled))
                winnerFound = true;
            else if ((B1.Text == B2.Text) && (B2.Text == B3.Text) && (!B1.Enabled))
                winnerFound = true;
            else if ((C1.Text == C2.Text) && (C2.Text == C3.Text) && (!C1.Enabled))
                winnerFound = true;

            //Vertical checks on column 1, 2 and 3.
            //When the first button is pressed, all the other are empty, and therefore equal. That is why A1, A2 and A3 has to be disabled, and that requires the rest of the buttons in the same row to be disabled as well.
            else if ((A1.Text == B1.Text) && (B1.Text == C1.Text) && (!A1.Enabled))
                winnerFound = true;
            else if ((A2.Text == B2.Text) && (B2.Text == C2.Text) && (!A2.Enabled))
                winnerFound = true;
            else if ((A3.Text == B3.Text) && (B3.Text == C3.Text) && (!A3.Enabled))
                winnerFound = true;

            //Diagonal checks.
            //When the first button is pressed, all the other are empty, and therefore equal. That is why A1 and C1 has to be disabled, and that requires the rest of the buttons in the same row to be disabled as well.
            if ((A1.Text == B2.Text) && (B2.Text == C3.Text) && (!A1.Enabled))
                winnerFound = true;
            else if ((C1.Text == B2.Text) && (B2.Text == A3.Text) && (!C1.Enabled))
                winnerFound = true;

            if (winnerFound)
            {
                disableButtons();

                String winner = "";
                if (turn) //If it is Player 2's turn while the winner is found, then Player 2 wins.
                {
                    winner = label2.Text;
                    o_win_count.Text = (Int32.Parse(o_win_count.Text) + 1).ToString(); //Makes the label, which is a string, into a number - adds +1 - and makes it a string again.
                }
                else //Otherwise Player 1 wins.
                {
                    winner = label1.Text;
                    x_win_count.Text = (Int32.Parse(x_win_count.Text) + 1).ToString(); //Makes the label, which is a string, into a number - adds +1 - and makes it a string again.
                }

                if (MessageBox.Show(winner + " wins!\n Would you like to play again?", "Congratulations!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) //Restarts game, if the player wishes.
                {
                    restartGame();
                }
            } //End if winnerFound

            else //If there is no winner, it is a draw.
            {
                if (turn_count == 9)
                {
                    draw_count.Text = (Int32.Parse(draw_count.Text) + 1).ToString(); //Makes the label, which is a string, into a number - adds +1 - and makes it a string again.
                    if (MessageBox.Show("It's a draw!\n Would you like to play again?", "Draw!", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes) //Restarts game, if the player wishes.
                    {
                        restartGame();
                    }
                }
            }

        }//End checkForWinner.

        private void computer_makes_move()
        {
            //First priority is to get tic tac toe.
            //Second priority is to block opponents tic tac toe.
            //Third priority is to go for the middle space.
            //Fourth priority is to go for an open space.

            Button move = null;

            move = priority1and2("O"); //Look for tic tac toe
            if (move == null)
            {
                move = priority1and2("X"); //Look for block
                if (move == null)
                {
                    move = priority3(); //Look for the middle space.
                    if (move == null)
                    {
                        move = priority4(); //Look for open space.
                    } //End if
                } //End if
            } //End if

            try //To ignore any possible bugs there might be.
            {
                move.PerformClick(); //Performs the click, depends on the priority.
            }
            catch { }
        }

        private Button priority1and2(string mark) //mark is either X or O, and therefore either causes O to get tic tac toe or block X chance to win.
        {
            //Horizontal checks
            if ((A1.Text == mark) && (A2.Text == mark) && (A3.Text == ""))
                return A3;
            if ((A2.Text == mark) && (A3.Text == mark) && (A1.Text == ""))
                return A1;
            if ((A1.Text == mark) && (A3.Text == mark) && (A2.Text == ""))
                return A2;

            if ((B1.Text == mark) && (B2.Text == mark) && (B3.Text == ""))
                return B3;
            if ((B2.Text == mark) && (B3.Text == mark) && (B1.Text == ""))
                return B1;
            if ((B1.Text == mark) && (B3.Text == mark) && (B2.Text == ""))
                return B2;

            if ((C1.Text == mark) && (C2.Text == mark) && (C3.Text == ""))
                return C3;
            if ((C2.Text == mark) && (C3.Text == mark) && (C1.Text == ""))
                return C1;
            if ((C1.Text == mark) && (C3.Text == mark) && (C2.Text == ""))
                return C2;

            //Vertical checks
            if ((A1.Text == mark) && (B1.Text == mark) && (C1.Text == ""))
                return C1;
            if ((B1.Text == mark) && (C1.Text == mark) && (A1.Text == ""))
                return A1;
            if ((A1.Text == mark) && (C1.Text == mark) && (B1.Text == ""))
                return B1;

            if ((A2.Text == mark) && (B2.Text == mark) && (C2.Text == ""))
                return C2;
            if ((B2.Text == mark) && (C2.Text == mark) && (A2.Text == ""))
                return A2;
            if ((A2.Text == mark) && (C2.Text == mark) && (B2.Text == ""))
                return B2;

            if ((A3.Text == mark) && (B3.Text == mark) && (C3.Text == ""))
                return C3;
            if ((B3.Text == mark) && (C3.Text == mark) && (A3.Text == ""))
                return A3;
            if ((A3.Text == mark) && (C3.Text == mark) && (B3.Text == ""))
                return B3;

            //Diagonal checks
            if ((A1.Text == mark) && (B2.Text == mark) && (C3.Text == ""))
                return C3;
            if ((B2.Text == mark) && (C3.Text == mark) && (A1.Text == ""))
                return A1;
            if ((A1.Text == mark) && (C3.Text == mark) && (B2.Text == ""))
                return B2;

            if ((A3.Text == mark) && (B2.Text == mark) && (C1.Text == ""))
                return C1;
            if ((B2.Text == mark) && (C1.Text == mark) && (A3.Text == ""))
                return A3;
            if ((A3.Text == mark) && (C1.Text == mark) && (B2.Text == ""))
                return B2;

            return null;
        }

        private Button priority3 ()
        {
            if (B2.Text == "")
            {
                return B2;
            }

            return null;
        }

        private Button priority4()
        {
            Button b = null; //Declaring button
            foreach (Control c in Controls) //Looping through each control on the form.
            {
                b = c as Button; //Casts each control as a button, and stores it in b. If it cannot store a control in a button, it will return null.
                if (b != null)
                {
                    if (b.Text == "") //If there is an empty space, then go for it.
                        return b; //Returns the button.
                }//end if
            }//end foreach loop

            return null;
        }

        

        private void disableButtons() //Method that disables all the buttons on the field.
        {
                foreach (Control c in Controls)
                {
                    try //Needed because disabling all the components as buttons means that even the menustrip is considered as a button, which causes som errors.
                    {
                    Button b = (Button)c; //Converts the c component to a button, and stores it in b.
                    b.Enabled = false;
                    } //End try
                    catch { } //Ignores the warning, considered as empty.
                } //End foreach loop
        }

        private void restartGame() //Method that restarts the game.
        {
            turn = true; //Because X always starts.
            turn_count = 0;
            movefocus.Focus(); //Invisible label that gets all focus and makes sure that no button gets highlighted.

            foreach (Control c in Controls)
            {
                try //Needed because disabling all the components as buttons means that every component is considered as a button, which causes som errors.
                {
                    Button b = (Button)c; //Converts the c component to a button, and store it in b.
                    b.Enabled = true;
                    b.Text = "";
                }//End try
                catch { } //Ignores the warning, considered as empty.
            }//End foreach loop
                 
        }

        private void nytSpilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restartGame();
        }

        private void button_enter(object sender, EventArgs e) //Method that shows whose turn it is, when mouse enters.
        {
            Button b = (Button)sender; //Converts the sender object to a button, and store it in b.
            if (b.Enabled)
            {
                if (turn)
                    b.Text = "X";
                else
                    b.Text = "O";
            }//End if
        }

        private void button_leave(object sender, EventArgs e) //Method that removes the signs, when the mouse leaves the button.
        {
            Button b = (Button)sender; //Converts the sender object to a button, and store it in b.
            if (b.Enabled)
            {
                b.Text = "";
            }//End if
        }
     
        private void nulstilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            x_win_count.Text = "0";
            draw_count.Text = "0";
            o_win_count.Text = "0";
        }

        private void gemSpilToolStripMenuItem_Click(object sender, EventArgs e) //Udprinter spilresultaterne, og gemmer som tekstfil.
        {
            StreamWriter Save = new StreamWriter(@"save.txt"); //Saves to a file named save.txt.
            Save.Write(label1.Text + "\r\n"
                + label2.Text + "\r\n"
                + x_win_count.Text + "\r\n"
                + o_win_count.Text + "\r\n"
                + draw_count.Text);
            Save.Close();
        }

        private void andetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This game is developed by Ammad Hameed\rProgrammering C - Eksamensprojekt 2016\rH.C. Ørsted Gymnasiet Ballerup, 2.C", "About");
        }

        private void reglerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The rules are simple. The player who succeeds in placing three of their marks in a horizontal, vertical, or diagonal row, in a 3x3 grid, wins the game. \r\rNow show what you got!", "Rules");
        }

        public void Load_Game(Object sender, EventArgs e)
        {
            int counter = 0; //To count which line it is at.
            string line; //To copy the text from the line, and paste it in a label etc.
            restartGame();

            try
            {
               StreamReader load = new StreamReader(@"C:\Users\Ammad\Desktop\Eksamensprojekt - Programmering C\TicTacToe\TicTacToe\bin\Debug\save.txt"); //Loads the save file.
        
                   while ((line = load.ReadLine()) != null) //Reads one line at a time.
                   {
                       if (counter == 0) //First player name
                       {
                           label1.Text = line;
                       }
                       else if (counter == 1) //Second player name
                       {
                           label2.Text = line;
                       }
                       else if (counter == 2) //First player score
                       {
                           x_win_count.Text = line;
                       }
                       else if (counter == 3) //Second player score
                       {
                           o_win_count.Text = line;
                       }
                       else if (counter == 4) //Draw score
                       {
                           draw_count.Text = line;
                       }

                       counter++; //Counts +1

                     } //End while loop
            }//End try
               catch { MessageBox.Show("No saved game found."); }

                if (label2.Text == "Computer") //To make sure, that you play against the computer.
                {
                    computer = true;
                }
                else //Otherwise, you play against another player.
                {
                    computer = false;
                }

        }

        private void afslutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}

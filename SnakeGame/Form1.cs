using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class frmSnakes : Form
    {
        Random rand;
        enum GameBoardFields
        {
            Free,
            Snake,
            Bonus
        };

        enum Directions
        {
            Up,
            Down,
            Left,
            Right
        };

        struct SnakeCoordinates
        {
            public int x;
            public int y;
        }

        GameBoardFields[,] gameBoardField;
        SnakeCoordinates[] snakeXY;
        int snakeLength;
        Directions direction;
        Graphics g;

        public frmSnakes()
        {
            InitializeComponent();
            gameBoardField = new GameBoardFields[11, 11]; //starts from 0 so 12, 12 // two dimension array
            snakeXY = new SnakeCoordinates[100];// single dimension array // 100 fields
            rand = new Random();
        }

        private void frmSnakes_Load(object sender, EventArgs e)
        {
            picGameBoard.Image = new Bitmap(420, 420); // 35px for single row and column, 12*35=420
            g = Graphics.FromImage(picGameBoard.Image);
            g.Clear(Color.White);

            for (int i = 1; i <= 10; i++) // X-axis
            {
                //top and bottom walls
                g.DrawImage(imgList.Images[5], i * 35, 0);
                g.DrawImage(imgList.Images[5], i * 35, 385); //Starts from 1 to 10 which is 350px and 35px more sore 350+35=385px
            }

            for (int i = 0; i <= 11; i++) // Y-axis // Also counts corner walls
            {
                //left and right walls
                g.DrawImage(imgList.Images[5], 0, i * 35); 
                g.DrawImage(imgList.Images[5], 385, i * 35);
            }

            //initial snake body and head
            snakeXY[0].x = 5; // head
            snakeXY[0].y = 5; 
            snakeXY[1].x = 5; // first body part
            snakeXY[1].y = 6;
            snakeXY[2].x = 5; // second body part
            snakeXY[2].y = 7;

            g.DrawImage(imgList.Images[4], 5 * 35, 5 * 35); //head
            g.DrawImage(imgList.Images[3], 5 * 35, 6 * 35); // first body part
            g.DrawImage(imgList.Images[3], 5 * 35, 7 * 35); // second body part

            gameBoardField[5, 5] = GameBoardFields.Snake;
            gameBoardField[5, 6] = GameBoardFields.Snake;
            gameBoardField[5, 7] = GameBoardFields.Snake;

            direction = Directions.Up;
            snakeLength = 3;

            for (int i = 0; i < 4; i++)
            {
                Bonus();
            }
        }

        private void Bonus()
        {
            int x, y;
            var imgIndex = rand.Next(0, 3); // 0 to 2

            do
            {
                x = rand.Next(1, 10); // 0 to 9
                y = rand.Next(1, 10); // 0 to 9
            }
            while (gameBoardField[x,y] != GameBoardFields.Free); // checks until coordinates doesn't collide with wall and snake.

            gameBoardField[x, y] = GameBoardFields.Bonus; //set this field to be bonus

            g.DrawImage(imgList.Images[imgIndex], x * 35, y * 35);
        }

        private void frmSnake_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    direction = Directions.Up;
                    break;
                case Keys.Down:
                    direction = Directions.Down;
                    break;
                case Keys.Left:
                    direction = Directions.Left;
                    break;
                case Keys.Right:
                    direction = Directions.Right;
                    break;
                
            }
        }

        private void GameOver()
        {
            timer.Enabled = false;
            MessageBox.Show("GAME OVER");
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            g.FillRectangle(Brushes.White, snakeXY[snakeLength - 1].x * 35, 
                            snakeXY[snakeLength - 1].y * 35, 35, 35);
            gameBoardField[snakeXY[snakeLength - 1].x, snakeXY[snakeLength - 1].y] = GameBoardFields.Free;

            //move snake body on the position of previous one
            // array starts from 0th index
            for (int i = snakeLength; i >= 1; i--) // 0th index is head, so move until 1 index // -- for moving up the index
            {
                snakeXY[i].x = snakeXY[i - 1].x;
                snakeXY[i].y = snakeXY[i - 1].y;
            }

            //Body part of is index 3 and placing it to the index of head which is 0 and multiplying it by 35px.
            g.DrawImage(imgList.Images[3], snakeXY[0].x * 35, snakeXY[0].y * 35);

            //change direction of the head
            switch (direction)
            {
                case Directions.Up:
                    snakeXY[0].y = snakeXY[0].y - 1;
                    break;
                case Directions.Down:
                    snakeXY[0].y = snakeXY[0].y + 1;
                    break;
                case Directions.Left:
                    snakeXY[0].x = snakeXY[0].x - 1;
                    break;
                case Directions.Right:
                    snakeXY[0].x = snakeXY[0].x + 1;
                    break;
                
            }

            //check if snake hit the wall
            if (snakeXY[0].x < 1 || snakeXY[0].x > 10 || snakeXY[0].y < 1 || snakeXY[0].y > 10)
            {
                GameOver();
                picGameBoard.Refresh();
                return;
            }

            //check if snake hit its body
            if (gameBoardField[snakeXY[0].x,snakeXY[0].y] == GameBoardFields.Snake)
            {
                GameOver();
                picGameBoard.Refresh();
                return;
            }

            //check if snake hit the bonus
            if (gameBoardField[snakeXY[0].x, snakeXY[0].y] == GameBoardFields.Bonus)
            {
                g.DrawImage(imgList.Images[3], snakeXY[snakeLength].x * 35,
                    snakeXY[snakeLength].y * 35);  // adding body at the end so snakeLength and index of imgList is body.
                gameBoardField[snakeXY[snakeLength].x, snakeXY[snakeLength].y] = GameBoardFields.Snake;
                snakeLength++;

                if (snakeLength < 96) // 96 because playfield is 100 minus 2 body and head
                    Bonus();

                this.Text = "Snake - score: " + snakeLength;
            }

            //draw the head
            g.DrawImage(imgList.Images[4], snakeXY[0].x * 35, snakeXY[0].y * 35);
            gameBoardField[snakeXY[0].x, snakeXY[0].y] = GameBoardFields.Snake;

            picGameBoard.Refresh();
        }
    }
}

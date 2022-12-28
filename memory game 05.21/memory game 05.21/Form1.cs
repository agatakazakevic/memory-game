using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace memory_game_05._21
{

    public partial class Form1 : Form
    {
        int moves = 0;
        string filepath = Application.StartupPath + @"\highscore.txt";//kad nepagalvotu kad tai new lines



        private void showhighscores()
        {
            StreamReader reader = new StreamReader(filepath);
            MessageBox.Show(reader.ReadToEnd()); 
            reader.Close();

            reader = new StreamReader(filepath);
            List<string> allscores = new List<string>();
            while (!reader.EndOfStream)
            {
                allscores.Add(reader.ReadLine());
            }
            allscores.Sort();
            string result = "";
            int length = 3;
            if (allscores.Count<3)
            {
                length = allscores.Count;
            }
            for (int i = 0; i < length; i++)
            {
                result += allscores[i] + "\n";
            }
            MessageBox.Show("result");
            reader.Close();
        }

        private void writehighscore()
        {
            FileStream file = new FileStream(filepath, FileMode.Append, FileAccess.Write);
           
            var m =Encoding.UTF8.GetBytes( moves.ToString()+" ");
          file.Write(m,0,m.Length);
            file.Close();

        }
        struct imageforgeneration
        {
            public Image Image;
            public bool Placed;
        
            public imageforgeneration(Image image, bool placed = false)
            {
                Image = image;
                Placed = placed;
            }  
        }
        Button[,] buttons;
        imageforgeneration[,] images;
        int tablesize = 4;
        List<imageforgeneration> allimages = new List<imageforgeneration>();
        Random rnd = new Random();
        int totalopened = 0;
        int imagesopened = 0;
        int image1x = -1;
        int image1y = -1;
        int image2x = -1;
        int image2y = -1;
        int revealtime = 1;

        public Form1()
        {
            InitializeComponent();
            generateimages();
            generatetable();
        }

        private void generatetable()
        {
            Rectangle screenrectangle = RectangleToScreen(ClientRectangle);
            int height = screenrectangle.Top - Top-menuStrip1.Top;
            Size = new Size(400+16, 400+height+50);
            buttons = new Button[tablesize, tablesize];
            for (int i = 0; i < tablesize; i++)
            {
                for (int y = 0; y < tablesize; y++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(100, 100),
                        Location = new Point(i * 100, (y * 100)+menuStrip1.Height),
                        Text = ""
                    };
                    buttons[i, y] = btn;
                    btn.Click += new EventHandler(Button_Click);
                    Controls.Add(btn);
                }
            }

        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            for (int i = 0; i < tablesize; i++)
            {
                for (int j = 0; j < tablesize; j++)
                {
                    if (buttons[i, j].Equals(btn))
                    {
                        imagesopened++;
                        switch (imagesopened)
                        {
                            case 1:
                                closeopened();
                                image1x = i;
                                image1y = j;
                                break;
                            case 2:
                                image2x = i;
                                image2y = j;
                                break;

                        }
                        buttons[i, j].Click -= Button_Click;
                        buttons[i, j].Image = images[i,j].Image;

                    }
                }
            }
            if (imagesopened==2)
            {
                moves++;
                lblmoves.Text = moves.ToString();
                if(images[image1x, image1y].Image== images[image2x, image2y].Image)
                {
                    totalopened+= 2;
                    //check for win
                    if (totalopened==tablesize*tablesize)
                    {
                        MessageBox.Show("YOU WIN");
                        //check if it hegh score
                        writehighscore();
                        moves = 0;
                        for (int i = 0; i < this.Controls.Count; i++)
                        {
                            if (this.Controls[i].GetType()==typeof(Button))
                            {
                                this.Controls.Remove(this.Controls[i]);
                                i--;
                            }
                        }
                        generateimages();
                        generatetable();
                       

                    }
                }
                else
                {
                    revealtime = 3;
                    timer.Start();

                }
                imagesopened = 0;
            }
        }

        private void generateimages()
        {
            var imagesfromrec = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, true, true);
            foreach (DictionaryEntry entry in imagesfromrec)
            {
                allimages.Add(new imageforgeneration((Image)entry.Value));
            }
            images = new imageforgeneration[tablesize, tablesize];
            foreach (imageforgeneration ifg in allimages.OrderBy(x=>rnd.Next()).Take(8))
            {
                placetime(ifg);
                placetime(ifg);
            }
        }
        private void placetime(imageforgeneration imagetoset)
        {
            int x;
            int y;
            do
            {
                x = rnd.Next(0, tablesize);
                y = rnd.Next(0, tablesize);


            } while (images[x, y].Placed == true);
            images[x, y].Image = imagetoset.Image;
            images[x, y].Placed = true;

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            revealtime--;
            if (revealtime <= 0)
            {
                closeopened();
            }
           
        }

        private void closeopened()
        {
            if (image1x>=0 && image1y>=0 && image2x>=0 && image2y>=0)
            {
                if (images[image1x, image1y].Image == images[image2x, image2y].Image)
                    return;
                else
                {

                    buttons[image1x, image1y] = null;
                    buttons[image2x, image2y] = null;
                    buttons[image1x, image1y].Click += Button_Click;
                    buttons[image2x, image2y].Click += Button_Click;
                
                    image1x = -1;
                    image1y = -1;
                    image2x = -1;
                    image2y = -1;
                    timer.Stop();

                } 
            }
        }

        private void highScoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showhighscores();
        }
    }
}

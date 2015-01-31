using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using Uploader;

namespace SecurityCamCross
{
    public partial class Form1 : Form
    {
        private Process Record = null;//Create a class Process variable
        private static string DirectorySet = "";//Create a class path variable
        private string SpeedSet = "1";//Default recording frequency variable
        private static int ImageNumber = 0;//Initial image number for recording
        private static int FileNo = 0;
        private static bool Platform;
        private static string Filer;
        private static FTPClient Hauler;
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "/home/simon/Public/";//Initialise entrybox text to default
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            Platform = true;
            Filer = "Image";

            OperatingSystem os = Environment.OSVersion;
            PlatformID PId = os.Platform;
            if (PId == PlatformID.Win32Windows || PId == PlatformID.Win32NT)
            {
                Platform = false;
                textBox1.Text = "C:";
                Filer = "\\Image";

            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string ActPath = AppDomain.CurrentDomain.BaseDirectory;//Obtain local path
                if (!Platform) ActPath = ActPath + "\\";
                string FTPAddress = textBox3.Text;
                string FTPUser = textBox4.Text;
                string FTPPassword = textBox5.Text;
                if (checkBox1.Checked == true && FTPAddress != string.Empty)
                    Hauler = new FTPClient("Uploader1", DirectorySet, FTPAddress, FTPUser, FTPPassword);
                //Watcher = new VisionThread("Watcher1", DirectorySet, SpeedSet, ActPath);
                //VideoOn( DirectorySet, SpeedSet, ActPath );
                Record = new Process();
                Record.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Record.StartInfo.Arguments = "\"" + DirectorySet + "\" " + "\"" + SpeedSet + "\" " + "\"" + ActPath + "\"";
                Record.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "Faces";
                Record.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (Hauler != null)
                    Hauler.Client.Abort();

                if (Record != null)
                    Record.Kill();//Close down C++ utility executable process
                //VideoOff();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem.ToString() == "Fast")//Translate combo box selection to update frequency
                SpeedSet = "1";
            else if (comboBox1.SelectedItem.ToString() == "Medium")
                SpeedSet = "2";
            else SpeedSet = "3";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog filechooser1 = new OpenFileDialog();
            DialogResult result= filechooser1.ShowDialog();
            if (result == DialogResult.OK)
            {
            
                if (Platform)
                    DirectorySet = filechooser1.FileName + "/";//Select destination
                else
                    DirectorySet = filechooser1.FileName;
                textBox1.Text = DirectorySet;//Fill in edit box with selection
                textBox2.Text = DirectorySet;
                ImageNumber = 0;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (ImageNumber + 1 <= FileNo)
                {
                    ++ImageNumber;//Increment image number and assign image to image area
                    pictureBox1.ImageLocation = DirectorySet + Filer + ImageNumber.ToString() + ".jpg";

                    label2.Text = "Image Number " + ImageNumber.ToString();
                    if (ImageNumber > 1)
                        button4.Enabled = true;
                    if (ImageNumber > 5)
                        button3.Enabled = true;


                }
                else button5.Enabled = false;
                if (ImageNumber == FileNo)
                    button5.Enabled = false;
                if (ImageNumber + 5 > FileNo)
                    button6.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (ImageNumber > 1)
                {
                    --ImageNumber;
                    pictureBox1.ImageLocation = DirectorySet + Filer + ImageNumber.ToString() + ".jpg";
                    label2.Text = "Image Number " + ImageNumber.ToString();
                    if (ImageNumber == 1)
                        button4.Enabled = false;
                    if (ImageNumber <= 5)
                        button3.Enabled = false;
                    button5.Enabled = true;
                    if (ImageNumber <= FileNo - 5)
                        button6.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (ImageNumber > 5)
                {
                    ImageNumber -= 5;
                    pictureBox1.ImageLocation = DirectorySet + Filer + ImageNumber.ToString() + ".jpg";
                    label2.Text = "Image Number " + ImageNumber.ToString();
                    if (ImageNumber <= 5)
                        button3.Enabled = false;
                    if (ImageNumber == 1)
                        button4.Enabled = false;
                    button5.Enabled = true;
                    if (ImageNumber <= FileNo - 5)
                        button6.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (ImageNumber + 5 <= FileNo)
                {
                    ImageNumber += 5;
                    pictureBox1.ImageLocation = DirectorySet + Filer + ImageNumber.ToString() + ".jpg";
                    label2.Text = "Image Number " + ImageNumber.ToString();
                    if (ImageNumber > 1)
                        button4.Enabled = true;
                    if (ImageNumber > 5)
                        button3.Enabled = true;
                    if (ImageNumber + 5 > FileNo)
                        button6.Enabled = false;

                }
                else button6.Enabled = false;
                if (ImageNumber == FileNo)
                    button6.Enabled = false;
                if (ImageNumber + 1 > FileNo)
                    button5.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DirectorySet = textBox1.Text;
        }
        private void ReadDirectory()
        {
            DirectoryInfo Dirt = new DirectoryInfo(DirectorySet);
            if (Dirt.Exists)
            {
                FileNo = Dirt.GetFiles().Length;
                label3.Text = "Number of files is " + FileNo.ToString() + "  ( " + DirectorySet + " )";
            }

        }

        private void tabPage1_Leave(object sender, EventArgs e)
        {
            ReadDirectory();
            if (FileNo >= 1)
                button5.Enabled = true;
            if (FileNo >= 5)
                button6.Enabled = true;
		
        }

       
       
    }
}

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
using System.Configuration;
using System.Text.RegularExpressions;
using static System.Windows.Forms.LinkLabel;

namespace BASE64
{
    public partial class frmBase64Conversion : Form
    {
        OpenFileDialog fileDialog;
        MyUserSettings mus;
        public frmBase64Conversion()
        {
            InitializeComponent();
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace( txtConvert.Text ))
            {
                string fpath = Path.GetDirectoryName(txtConvert.Text), fName = Path.GetFileName(txtConvert.Text);

                string[] files = Directory.GetFiles(fpath, fName, SearchOption.TopDirectoryOnly);
                foreach (var cfile in files)
                {
                    byte[] bytes = File.ReadAllBytes(Path.Combine(fpath,cfile));
                    string file = Convert.ToBase64String(bytes);
                    File.WriteAllText(Path.Combine(fpath,cfile) + ".base64.txt", file);
                }

            }
        }

        private void btnReverse_Click(object sender, EventArgs e)
        {
            string fpath = txtReverse.Text;
            string pattern = @"^(.*)(\r?\n\1)+$"; //^(.*)(\r?\n\1)+$
            var file = Regex.Replace( File.ReadAllText(fpath), pattern,@"$1",RegexOptions.Multiline );
                file = Regex.Replace(file, @"^\d+/\d+\$", @"", RegexOptions.Multiline);
            StringReader strReader = new StringReader(file);
            StringBuilder sb = new StringBuilder(1024 * 100);
            var counter = 1;
            var noOfLines = 0;
            while (true)
            {
                var aLine = strReader.ReadLine();

                if (aLine != null)
                {
                    if (counter == 1)
                    {
                        var splt = new char[] { '/' };
                        noOfLines = Convert.ToInt32(aLine.Split(splt)[1])+2;
                    }
                    else
                    {
                      sb.Append(
                            counter == 2 ? aLine.Replace("data:text/plain;base64,",""):aLine.Replace(Environment.NewLine,"")
                            );
                    }
                    counter++;
                    if (counter > noOfLines)
                    {
                        break;
                    }
                }
                else
                {
                   // aParagraph = aParagraph + "\n";
                    break;
                }
            }
            var bytes = Convert.FromBase64String(sb.ToString());
            fpath = fpath.Replace(".base64","").Replace(".txt", "");
            File.WriteAllBytes(fpath + ".rev", bytes);
        }
        
        

        private void btnShowFileDialogToBase64_Click(object sender, EventArgs e)
        {
            txtConvert.Text = GetFile() ?? txtConvert.Text;


          //  fileDialog.Filter = "Database files (*.mdb, *.accdb)|*.mdb;*.accdb";
          //fileDialog.FilterIndex = 0;

        }
        private string GetFile()
        {
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return fileDialog.FileName;
        }

        private void frmBase64Conversion_Load(object sender, EventArgs e)
        {
            fileDialog = new OpenFileDialog();
            MyUserSettings mus = new MyUserSettings();
          //  fileDialog.
           // fileDialog.InitialDirectory = Environment.CurrentDirectory;
            txtConvert_TextChanged(null, null);
            txtReverse_TextChanged(null, null);
            txtConvert.Text = mus.txtConvert;
            txtReverse.Text = mus.txtReverse;
        }

        private void btnShowFileDialogFromBase64_Click(object sender, EventArgs e)
        {
            txtReverse.Text = GetFile() ?? txtConvert.Text;
        }

        private void txtConvert_TextChanged(object sender, EventArgs e)
        {
            btnConvert.Enabled = txtConvert.Text.Trim() != "";// && File.Exists(txtConvert.Text);
        }

        private void txtReverse_TextChanged(object sender, EventArgs e)
        {
            //if (btnReverse.Text.Trim() == "") return;
            btnReverse.Enabled = txtReverse.Text.Trim() != "" && File.Exists(txtReverse.Text);
        }

        private void frmBase64Conversion_FormClosing(object sender, FormClosingEventArgs e)
        {
             mus.txtConvert= txtConvert.Text;
            mus.txtReverse =txtReverse.Text;
            mus.Save();

        }
    }

}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SiebelSearchExprCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //this.Visible = false;
            //this.Hide();
        }

        string[,] ParseTable(string str)
        {
            string[] rows = null;
            int columnsCount;
            int rowsCount = 0;
            int pars = 0;
            rows = Regex.Split(str, "\r\n");
            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = rows[i].Trim();
                if (string.IsNullOrEmpty(rows[i])) continue;
                rowsCount++;
                if (rows[i].IndexOf("\t") > -1) pars++;
            }
            if (rowsCount > pars) columnsCount = 1;
            else columnsCount = 2;
            string[,] res = new string[rowsCount, /*columnsCount*/2];
            rowsCount = 0;
            for (int i = 0; i < rows.Length; i++)
            {
                if (string.IsNullOrEmpty(rows[i])) continue;
                if (columnsCount == 1) res[rowsCount, 0] = rows[i];
                else
                {
                    res[rowsCount, 1] = rows[i].Substring(0, rows[i].IndexOf("\t"));
                    res[rowsCount, 0] = rows[i].Substring(rows[i].IndexOf("\t") + 1);
                }
                rowsCount++;

            }
            return res;
        }
        string ProcessValue(string val)
        {

            if (val.IndexOf('"') > -1 && val.IndexOf("'") == -1) return "'" + val + "'";
            if (val.IndexOf('"') > 0) return "LIKE \"" + val + '"';
            return "\"" + val + "\"";

        }
        string GetDataFromCB()
        {
            IDataObject cbd = Clipboard.GetDataObject();
            
            if (!cbd.GetDataPresent(DataFormats.Text)) return "";
            if(cbd.GetDataPresent(DataFormats.UnicodeText)) return (string)cbd.GetData(DataFormats.UnicodeText);
            if (cbd.GetDataPresent(DataFormats.Text)) return (string)cbd.GetData(DataFormats.Text);
            return "";

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string s;
            StringBuilder searchExp;
            s = GetDataFromCB();
            if (string.IsNullOrEmpty(s)) return;
            searchExp = new StringBuilder();
            string[,] tbl;
            tbl = ParseTable(s);
            
            for (int i = 0; i < tbl.GetLength(0) - 0; i++)
            {
                if (string.IsNullOrEmpty(tbl[i, 1]))
                {
                    if (searchExp.Length > 0) searchExp.Append(" or ");
                    searchExp.Append(ProcessValue(tbl[i, 0]));
                }
                else
                {
                    if (searchExp.Length > 0) searchExp.Append(" and ");
                    searchExp.Append(String.Format("[{0}] = {1}", tbl[i, 1], ProcessValue(tbl[i, 0])));
                }

            }
            if (searchExp.Length != 0) s = searchExp.ToString();
            Clipboard.SetText(s, System.Windows.Forms.TextDataFormat.UnicodeText);
            //Clipboard.SetText(s);
            //notifyIcon1.Text = s.Substring(0, 63);
            notifyIcon1.ShowBalloonTip(5, "Text In Clipboard", Clipboard.GetText(), ToolTipIcon.None);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) return;
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
                notifyIcon1.ShowBalloonTip(5, "Text In Clipboard", string.IsNullOrEmpty(Clipboard.GetText()) ? "No text seted to Clipboard": Clipboard.GetText() , ToolTipIcon.None);
            else
                notifyIcon1.ShowBalloonTip(5, "Text In Clipboard", "No present text in clipboard", ToolTipIcon.Info);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ShowDialog();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (
            MessageBox.Show("Do you want close " + this.Text + "?", "Good buy?", MessageBoxButtons.OKCancel,MessageBoxIcon.Question) != System.Windows.Forms.DialogResult.OK)
                return;
            Application.Exit();
        }
    }
}

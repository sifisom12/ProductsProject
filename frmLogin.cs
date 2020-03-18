using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace Products
{
    public partial class frmLogin : frmInheritance
    {
        bool boolUserExists = false;

        string strAccessConnectionString = "Driver= {Microsoft Access Driver (*.mdb)}; Dbq=Products.mdb;Uid=Admin;Pwd=;";

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (btnLogin.Text == "Login")
            {
                if (txtUserName.Text == "")
                {
                    MessageBox.Show("Username field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtPasscode.Text == "")
                {
                    MessageBox.Show("Passcode field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                checkIfUserExists();
                if (boolUserExists == false)
                {
                    ClearTextBoxes();
                }
                else if (boolUserExists == true)
                {
                    string UserName = txtUserName.Text;
                    string Passcode = txtPasscode.Text;
                    string query = "SELECT Username, Passcode FROM Users WHERE Username=@Username and Passcode=@Passcode";
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = query;
                    cmd.Parameters.AddWithValue("@Username", UserName);
                    cmd.Parameters.AddWithValue("@Passcode", Passcode);
                    if (UserName == "Admin" && Passcode == "000000")
                    {
                        frmMain frmMain = new frmMain();
                        frmMain.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Username Or Passcode are Incorrect", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                }
            }


            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void ClearTextBoxes()
        {
            txtUserName.Text = "";
            txtPasscode.Text = "";
        }

        private void checkIfUserExists()
        {
            string query = "SELECT * FROM Users WHERE Username='" + txtUserName.Text + "'";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;
            OdbcConnection.Open();
            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();
            //OdbcDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                boolUserExists = true;
            }
            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }
    }
}

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
    public partial class frmUsers : frmInheritance
    {
        string strUsername;
        string strPasscode;
        string strFirstName;
        string strLastName;
        bool boolUserExists = false;
        int intUserID = 0;

        string strAccessConnectionString = "Driver={Microsoft Access Driver (*.mdb)}; Dbq=Products.mdb; Uid=Admin; Pwd=;";

        public frmUsers()
        {
            InitializeComponent();
        }

        private void frmUsers_Load(object sender, EventArgs e)
        {
            controlsLoad();
            loadUsers();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            frmMain frmMain = new frmMain();
            frmMain.Show();
            this.Hide();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (btnAdd.Text == "Save")
            {
                if (txtFirstName.Text == "")
                {
                    MessageBox.Show("First Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtLastName.Text == "")
                {
                    MessageBox.Show("Last Name field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtPasscode.Text == "")
                {
                    MessageBox.Show("Passcode field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (txtUsername.Text == "")
                {
                    MessageBox.Show("Username field cannot be left empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                checkIfUserExists();
                if (boolUserExists == false)
                {
                    createUser();
                    controlsLoad();
                    clearTextBoxes();
                    loadUsers();
                }
                else if (boolUserExists == true)
                {
                    MessageBox.Show("User already Exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else if (btnAdd.Text == "Add")
            {
                controlsAdd();
            }
            
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            editUser();
            controlsEdit();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            updateUser();
            controlsLoad();
            clearTextBoxes();
            loadUsers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            deleteUser();
            controlsLoad();
            clearTextBoxes();
            loadUsers();

        }

        private void controlsLoad()
        {
            txtFirstName.Enabled = false;
            txtLastName.Enabled = false;
            txtPasscode.Enabled = false;
            txtUsername.Enabled = false;

            cboUsers.Enabled = true;

            btnAdd.Enabled = true;
            btnEdit.Enabled = true;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnReturn.Enabled = true;

            btnAdd.Text = "Add";
           
        }

        private void controlsAdd()
        {

            txtFirstName.Enabled = true;
            txtLastName.Enabled = true;
            txtPasscode.Enabled = true;
            txtUsername.Enabled = true;

            cboUsers.Enabled = false;

            btnAdd.Enabled = true;
            btnEdit.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
            btnReturn.Enabled = false;

            btnAdd.Text = "Save";
        }

        private void controlsEdit()
        {
            txtFirstName.Enabled = true;
            txtLastName.Enabled = true;
            txtPasscode.Enabled = true;
            txtUsername.Enabled = true;

            cboUsers.Enabled = false;

            btnAdd.Enabled = false;
            btnEdit.Enabled = false;
            btnUpdate.Enabled = true;
            btnDelete.Enabled = true;
            btnReturn.Enabled = false;
        }

        private void clearTextBoxes()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtPasscode.Text = "";
            txtUsername.Text = "";
        }

        private void loadUsers()
        {
            cboUsers.DataSource = null;
            cboUsers.Items.Clear();

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcConnection.ConnectionString = strAccessConnectionString;

            string query = "select Username from Users";

            OdbcCommand cmd = new OdbcCommand(query, OdbcConnection);

            OdbcConnection.Open();
            OdbcDataReader dr = cmd.ExecuteReader();
            AutoCompleteStringCollection UserCollection = new AutoCompleteStringCollection();

            while (dr.Read())
            {
                UserCollection.Add(dr.GetString(0));
            }
            cboUsers.DataSource = UserCollection;
            OdbcConnection.Close();
        }

        private void createUser()
        {
            string query = "Select * from Users where ID=0";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcDataAdapter da = new OdbcDataAdapter(query, OdbcConnection);

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            DataRow dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            da.Fill(ds, "Users");
            dt = ds.Tables["Users"];

            try
            {
                dr = dt.NewRow();
                dr["Username"] = txtUsername.Text;
                dr["Passcode"] = txtPasscode.Text;
                dr["FirstName"] = txtFirstName.Text;
                dr["LastName"] = txtLastName.Text;

                dt.Rows.Add(dr);
                OdbcCommandBuilder cmd = new OdbcCommandBuilder(da);

                da.Update(ds, "Users");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString());
            }
            finally
            {
                OdbcConnection.Close();
                OdbcConnection.Dispose();
            }
        }

        private void checkIfUserExists()
        {
            string query = "select * from Users where Username='"+txtUsername.Text +"'";
            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();

            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                boolUserExists = true;
            }
            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();  
            
        }

        private void editUser()
        {
            string query = "select * from Users where Username='" + cboUsers.Text + "'";

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();

            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                intUserID = dr.GetInt32(0);
                txtUsername.Text = dr.GetString(1);
                txtPasscode.Text = dr.GetString(2);
                txtFirstName.Text = dr.GetString(3);
                txtLastName.Text = dr.GetString(4);
            }
            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }

        private void updateUser()
        {
            string query = "select * from Users where id=" + intUserID;

            OdbcConnection OdbcConnection = new OdbcConnection();

            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcDataAdapter da = new OdbcDataAdapter(query, OdbcConnection);
            DataSet ds = new DataSet("Users");

            da.FillSchema(ds, SchemaType.Source, "Users");
            da.Fill(ds, "Users");
            DataTable dt;

            dt = ds.Tables["Users"];
            DataRow dr;
            dr = dt.NewRow();

            try
            {
                dr = dt.Rows.Find(intUserID);
                dr.BeginEdit();

                dr["Username"] = txtUsername.Text;
                dr["Passcode"] = txtPasscode.Text;
                dr["FirstName"] = txtFirstName.Text;
                dr["LastName"] = txtLastName.Text; ;

                dr.EndEdit();

                OdbcCommandBuilder cmd = new OdbcCommandBuilder(da);
                da.Update(ds, "Users");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message.ToString());
            }
            finally
            {
                OdbcConnection.Close();
                OdbcConnection.Dispose();
            }
        }

        private void deleteUser()
        {
            string query = "delete from Users where id=" + intUserID;

            OdbcConnection OdbcConnection = new OdbcConnection();
            OdbcCommand cmd;
            OdbcDataReader dr;
            OdbcConnection.ConnectionString = strAccessConnectionString;

            OdbcConnection.Open();
            cmd = new OdbcCommand(query, OdbcConnection);
            dr = cmd.ExecuteReader();

            if (dr.Read())
            {
            }
            dr.Close();
            OdbcConnection.Close();
            dr.Dispose();
            OdbcConnection.Dispose();
        }
            
    }
}

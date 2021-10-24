using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UpdatedM2Application
{
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            if(txtEmail.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Fill in missing data");
            }
            else
            {
                if(radioManager.Checked)
                {
                    string managerUsername = "Admin";
                    string password = "adminAdmin";
                    if(txtEmail.Text.Trim() != managerUsername)
                    {
                        MessageBox.Show("Username incorrect");
                        return;
                    }else
                    {
                        if(password != txtPassword.Text)
                        {
                            MessageBox.Show("Incorrect Password");
                            return;
                        }else
                        {
                            MessageBox.Show("Successful Log In");
                            Manager f = new Manager();
                            txtPassword.Clear();
                            txtEmail.Clear();
                            this.Hide();
                            f.Show();
                        }
                    }
                }
                else if(radioStaff.Checked)
                {
                    int count;
                    try { count = Convert.ToInt16(this.employeeTableAdapter1.doesEmailExist(txtEmail.Text)); }
                    catch(Exception ex) { MessageBox.Show(ex.Message); return; }
                    if(count == 0) { MessageBox.Show("This Email Does Not Exist"); return; }
                    else
                    {
                        string password;
                        try
                        {
                            password = this.employeeTableAdapter1.GetPasword(txtEmail.Text.Trim()).ToString();
                        }catch(Exception ex) { MessageBox.Show(ex.Message); return; }
                        if (password != txtPassword.Text.Trim()) { MessageBox.Show("Incorrect Password"); return; }
                        else
                        {
                            MessageBox.Show("Successful Log In");
                            Staff f = new Staff();
                            txtPassword.Clear();
                            txtEmail.Clear();
                            this.Hide();
                            f.Show();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Select one radiobox");
                }
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(txtPassword.PasswordChar == '*')
            {
                txtPassword.PasswordChar = '\0';
            }
            else
            {
                txtPassword.PasswordChar = '*';
            }
        }
    }
}

using System;
using System.Windows.Forms;

namespace LibraryManagementSystems
{
    public partial class AdminLogin : Form
    {
        public AdminLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Hardcoded credentials
            string username = "admin";
            string password = "12345";

            // Check if the username or password fields are empty
            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Please enter both Username and Password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate login credentials
            if (txtUsername.Text == username && txtPassword.Text == password)
            {
                MessageBox.Show("Login Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open the main form (Form1)
                Form1 form1 = new Form1();
                form1.Show();

                // Hide the login form
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password.", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Optional: Handle form closing to exit the application if needed
        private void AdminLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit(); // Ensure the application exits when the login form is closed
        }
    }
}

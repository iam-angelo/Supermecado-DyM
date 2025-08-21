using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Supermecado_DyM
{
    public partial class frmLogin : Form
    {
        string connectionString = Properties.Settings.Default.SeguridadDB;

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text.Trim();

            if (ValidarUsuario(usuario, contrasena))  
            {
                this.Hide();
                frmPrincipal principal = new frmPrincipal();
                principal.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidarUsuario(string usuario, string contrasena)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM TUSR_USUARIOS WHERE TC_Usuario=@usuario AND TC_Contrasena=@contrasena";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@contrasena", contrasena);

                conn.Open();
                int result = (int)cmd.ExecuteScalar();

                return result > 0;
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
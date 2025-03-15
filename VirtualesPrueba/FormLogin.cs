using System;
using System.Windows.Forms;

namespace VirtualesPrueba
{
    public partial class FormLogin : Form
    {
        private const string ClaveCorrecta = "betbet123";

        public FormLogin()
        {
            InitializeComponent();
            
            this.AcceptButton = null;
            this.CancelButton = null;
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            
            this.ShowInTaskbar = false;
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (txtClave.Text == ClaveCorrecta)
            {
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Clave incorrecta. Intenta nuevamente.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtClave.Clear();
                txtClave.Focus();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void FormLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                btnCancelar_Click(null, null);
            }
        }

    }
}

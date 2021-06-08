using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Proyecto_cliente
{

    public partial class Form2 : Form
    {
    Form1 F1 = new Form1();
   
        public Form2()
        {
            InitializeComponent();
        }

        private void TUsername_TextChanged(object sender, EventArgs e)
        {

        }

        private void IniciarSesion_Click(object sender, EventArgs e)
        {
            string tusername = TUsername.Text;
            string tpassword = Password.Text;
            if ((string.IsNullOrEmpty(TUsername.Text) || (string.IsNullOrEmpty(Password.Text))))
            {
                MessageBox.Show("Asegurese de rellenar todas las casillas");
            }
            else{
                int er = F1.Connectar();
                if (er == 0)
                {
                    int c;
                    c = F1.Iniciar_Sesion(tusername, tpassword);
                    if (c == 1)
                    {
                        //F1.Benvinguda();
                        MessageBox.Show("Iniciado correctamente");
                        this.Hide();
                        F1.ShowDialog();
                        this.Show();

                    }
                    else
                        MessageBox.Show("Usuario o contraseña incorrecta");
                }
                else
                    MessageBox.Show("No se ha podido establecer conexión");
            }
        }

        private void Registro_Click(object sender, EventArgs e)
        {
            Form1 F1 = new Form1();
            string tusername = TUsername.Text;
            string tpassword = Password.Text;
            if ((string.IsNullOrEmpty(TUsername.Text) || (string.IsNullOrEmpty(Password.Text))))
            {
                MessageBox.Show("Asegurese de rellenar todas las casillas");
            }
            else
            {
                F1.Connectar();
                int c;
                c = F1.Registrarse(tusername, tpassword);
                if (c == 1)
                {
                    MessageBox.Show("Registro completado");
                }
                else
                    MessageBox.Show("Usuario existente. Elija otro");
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}

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
    delegate void DelegadoParaPonerTexto(string texto);
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
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                this.Invoke(del, new Object[] { "Asegurese de rellenar todas las casillas" });
                //MessageBox.Show("Asegurese de rellenar todas las casillas");
            }
            else{
                int er = F1.Connectar();
                if (er == 0)
                {
                    int c;
                    c = F1.Iniciar_Sesion(tusername, tpassword);
                    if (c == 1)
                    {
                        //MessageBox.Show("Iniciado correctamente");
                        this.Hide();
                        F1.ShowDialog();
                    }
                    else if (c == 2)
                    {
                        DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                        this.Invoke(del, new Object[] { "Este usuario ya esta en linea" });
                        //MessageBox.Show("Este usuario ya esta en linea");
                    }
                    else
                    {
                        DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                        this.Invoke(del, new Object[] { "Usuario o contraseña incorrecta" });
                        //MessageBox.Show("Usuario o contraseña incorrecta");
                    }
                }
                else
                {
                    DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                    this.Invoke(del, new Object[] { "No se ha podido establecer conexión" });
                    //MessageBox.Show("No se ha podido establecer conexión");
                }
            }
        }

        private void Registro_Click(object sender, EventArgs e)
        {
            Form1 F1 = new Form1();
            string tusername = TUsername.Text;
            string tpassword = Password.Text;
            if ((string.IsNullOrEmpty(TUsername.Text) || (string.IsNullOrEmpty(Password.Text))))
            {
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                this.Invoke(del, new Object[] { "Asegurese de rellenar todas las casillas" });
                //MessageBox.Show("Asegurese de rellenar todas las casillas");
            }
            else
            {
                F1.Connectar();
                int c;
                c = F1.Registrarse(tusername, tpassword);
                if (c == 1)
                {
                    DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                    this.Invoke(del, new Object[] { "Registrado correctamente" });
                    //MessageBox.Show("Registro completado");
                }
                else
                {
                    DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                    this.Invoke(del, new Object[] { "Usuario existente. Elija otro" });
                    //MessageBox.Show("Usuario existente. Elija otro");
                }
            }
        }

        public void Invitacio(string respuesta)
        {
            this.label4.Text = respuesta;//Notificacion de invitacion
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}

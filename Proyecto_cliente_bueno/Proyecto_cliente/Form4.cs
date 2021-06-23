using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace Proyecto_cliente
{
    public partial class Form4 : Form
    {
        string ids;
        Socket server;
        string jugador;
        public Form4(Socket server, string ids, string jugador)
        {
            InitializeComponent();
            this.server = server;
            this.ids = ids;
            this.jugador = jugador;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //int id = EncontrarID(ids);
            string mensaje = "10/" + textBox1.Text + "/" + jugador;
            MessageBox.Show("Envio el mensaje");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            //this.Hide();
            //Form3 f = new Form3(server, textBox1.Text);
            //f.thread();
            //cargarlista();
            //f.Show();
            Close();

        }

        public void nombrepartida(ref string npartida)
        {
            npartida = textBox1.Text;
        }

        public int EncontrarID(string ids)
        {
            int contid = 0;
            string[] ID = ids.Split('/');
            while (Convert.ToInt32(ID[contid]) != -1)
            {
                contid++;
            }
            ID[contid] = Convert.ToString(contid);
            int n = 0;
            string mensaje1;
            if (contid == 0)
                return contid;
            else
                mensaje1 = ID[n] + "/";
            n++;
            string mensaje;
            string mensaje2;
            while (ID[n] != null)
                {
                mensaje2 = mensaje1 + ID[n] + "/";
                mensaje = mensaje2;
                if(ID[n+1] != null)
                    {
                    mensaje1 = mensaje2 + ID[n+1] + "/";
                    }
                n = n + 2;
                }
            return contid;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        public void cargarlista()
        {
            string mensaje = "13/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
    }
}

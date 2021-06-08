using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Proyecto_cliente
{

    public partial class Form3 : Form
    {
        delegate void DelegadoParaPonerTexto(string texto);
        //Form1 F2 = new Form1();
        Socket server;
        Thread actualitzar;
        public Form3(Socket server)
        {
            InitializeComponent();
            this.server = server;
        }
        public void thread()//Ponemos en marcha el thread de actualizaciones
        {
            ThreadStart ts = delegate { Actualitzacions(); };
            actualitzar = new Thread(ts);
            actualitzar.Start();
        }

        private void Actualitzacions()//actualiza la posicion de la ficha movida por el rival
        {
            while (true)
            {
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Notificacion);
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);

                switch (codigo)
                {
                    case 8:
                        MessageBox.Show("El numero es: " + trozos[1]);
                        TomaRespuesta1(trozos[1]);
                        break;

                }
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
        }

        public void Notificacion(string respuesta)//Imprime en pantalla informacion del juego como el turno o movimientos erroneos
        {
            this.label1.Text = respuesta;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void button2_Click(object sender, EventArgs e)
        {
            string mensaje = "9/";
            MessageBox.Show("Envio el mensaje");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        public void TomaRespuesta1(string mensaje)
        {
            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Notificacion);
            this.Invoke(del, new Object[] { mensaje });
        }
    }
}

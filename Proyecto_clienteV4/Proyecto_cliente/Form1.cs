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
    public partial class Form1 : Form
    {
        int invitacio = 0;//para saber si tenemos invitacion o no
        Socket server;
        Thread atender;
        delegate void DelegadoParaPonerTexto(string texto);
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public void Invitacio(string respuesta)
        {
            this.label4.Text = respuesta;//Notificacion de invitacion
        }
        private void Atenderservidor()
        {
            while (true)
            {
                string box = textBox1.Text;
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
                int codigo = Convert.ToInt32(trozos[0]);

                switch (codigo)
                {
                    
                    case 1:
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            MessageBox.Show("Registrat correctament");
                        }
                        else
                        {
                            MessageBox.Show("Error al registrar");
                        }
                        break;
                    case 2:
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            MessageBox.Show("Login correcto");
                        }
                        else
                            MessageBox.Show("Persona no registrado");
                        break;
                    case 3:
                        if (box == "")
                        {
                            MessageBox.Show("Escribe el nombre del usuario de quien quieres hacer la consulta");
                        }
                        else
                        {
                            if (Convert.ToInt32(trozos[1]) == 0)
                            {
                                MessageBox.Show(textBox1.Text + " tiene " + Convert.ToInt32(trozos[2]) + "€");
                            }
                            else
                            {
                                MessageBox.Show("Error al consultar");
                            } 
                        }
                        break;
                    case 4:
                        if (box == "")
                        {
                            MessageBox.Show("Escribe el nombre del usuario de quien quieres hacer la consulta");
                        }
                        else
                        {
                            if (Convert.ToInt32(trozos[1]) == -1)
                            {
                                MessageBox.Show("El jugador contrincante es: " + trozos[2]);
                            }
                            else
                            {
                                MessageBox.Show("Error al consultar");
                            }
                        }
                        break;
                    case 5:
                        int j;
                        if(Convert.ToInt32(trozos[1])== -1)
                        {
                            for(j=2;j<=10;j++){
                            MessageBox.Show("El ranking es: " + trozos[j]);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;
                    case 6:
                        if(Convert.ToInt32(trozos[1]) == 0)
                        {
                            int f = Convert.ToInt32(trozos[2]);
                            int i = 0;
                            dataGridView1.Rows.Clear();

                            while (i < f)
                            {
                                //MessageBox.Show(trozos[i+3]);
                                dataGridView1.Rows.Add(trozos[i + 3]);
                                i++;
                            }
                            //MessageBox.Show("soy LIBRE");
                            //int b = Convert.ToInt32(trozos[2]);
                            //int i = 0;
                            //if (dataGridView1.Rows.Count != 0)
                            //    dataGridView1.Rows.Clear();
                            //while(i<b)
                            //{
                            //    dataGridView1.Rows.Add();
                            //    dataGridView1.Rows[i].Cells[0].Value = trozos[i + 3];
                            //    i++;
                            //}
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;
                    case 7://INVITAR          ***********************  N U E V O Version4
                        if(Convert.ToInt32(trozos[1])==0)
                        {
                            MessageBox.Show("Invitación aceptada");//ACEPTADA
                            Form3 F3 = new Form3();
                            F3.ShowDialog();
                            
                        }
                        else if (Convert.ToInt32(trozos[1]) == 1)//RECHAZADO
                        {
                            //MessageBox.Show("Invitación rechazada");
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke(del, new Object[] { "Te han rechazado" });
                        }
                        else if (Convert.ToInt32(trozos[1]) == 2)//NOTIFICACION DE INVITACION
                        {
                            //MessageBox.Show("Te han invitado");
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke (del, new Object[]{ "Te han invitado a jugar"});
                            invitacio = 1;
                        
                        }
                        else if (Convert.ToInt32(trozos[1])==3)
                        {
                            MessageBox.Show("Jugador no encontrado");
                        }
                        break;

                }
            }
        }
        //string username;
        //string username2;
        //string password;
        string rusername, rpassword;
        //bool finalizado = false;
        //// int fila;
        //// string invitacion;
        //delegate void DelegadoParaEscribir(string text);
        //delegate void DelegadoParaActualizarLista(string[] nombres, int num);
        //delegate void DelegadoParaGroupBox();

        public void SetUsername(string usuario)
        {
            rusername = usuario;
        }
        public void SetPassword(string contraseña)
        {
            rpassword = contraseña;
        }


        private void Desconectar_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";
            dataGridView1.Rows.Clear();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            atender.Abort();
            server.Close();
        }

        private void Aceptar_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked) // CONSULTA 1. Jugador con mas victorias
            {
                // Quiere saber la longitud
                string mensaje = "3/" + textBox1.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje); // cojo el string y lo convierto a un vect de bytes
                server.Send(msg);

                //Recibimos la respuesta del servidor
                //byte[] msg2 = new byte[80];
                //server.Receive(msg2); // deja la respuesta
                //mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0]; //convierto el vect en un string quiero q se quede con el 1r trozo
                //MessageBox.Show("El jugador con mas victorias es: " + mensaje);
            }
            else if (JugadorContrincante.Checked) //CONSULTA 2. 
            {
                string mensaje = "4/" + textBox1.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                ////Recibimos la respuesta del servidor
                //byte[] msg2 = new byte[80];
                //server.Receive(msg2);
                //mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                //MessageBox.Show("La partida más larga ha durado: " + mensaje);
            
            }
            else // CONSULTA 3.
            {
                string mensaje = "5/";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                ////Recibimos la respuesta del servidor
                //byte[] msg2 = new byte[80];
                //server.Receive(msg2);
                //mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                //MessageBox.Show(textBox1.Text + " ha ganado " + mensaje + "partidas");
                

            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Bingo_Enter(object sender, EventArgs e)
        {

        }








        public int Connectar()
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9059);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado");
                
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return -1;
            }
            ThreadStart ts = delegate { Atenderservidor(); };
            atender = new Thread(ts);
            atender.Start();
            return 0;
        }

        public int Iniciar_Sesion(string tusername, string tpassword)
        {
            string mensaje = "2/" + tusername + "/" + tpassword;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            byte[] msg2 = new byte[500];
            server.Receive(msg2);
            string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
            if (Convert.ToInt32(trozos[1]) == 0)
            {
                SetUsername(tusername);
                return 1;
            }
            else
                return 0;
        }


        public int Registrarse(string tusername, string tpassword)
        {
            // Envia el nombre y la constraseña del registro con el código 1 y separado por /
            string mensaje = "1/" + tusername + "/" + tpassword;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            byte[] msg2 = new byte[80];
            server.Receive(msg2);
            string[] trozos = Encoding.ASCII.GetString(msg2).Split('/');
            if (Convert.ToInt32(trozos[1]) == 0)
            {
                MessageBox.Show("Registrado correctament");
                return 1;
            }
            else 
            { 
                MessageBox.Show("Error al registrarse");
                return 0;
            }
            }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string mensaje = "7/" + Convert.ToString(textBox2.Text);
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (invitacio==0)
                MessageBox.Show("No tienes ninguna invitación para jugar aún");
            else
            {
                string mensaje = "8/0";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                Form3 F3 = new Form3();//abrimos el form
                F3.ShowDialog();
                invitacio = 0;
        }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (invitacio == 0)
                MessageBox.Show("No tienes ninguna invitación para jugar aún");
            else
            {
                string mensaje = "8/1";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                invitacio = 0;
            }
        }
    }
}

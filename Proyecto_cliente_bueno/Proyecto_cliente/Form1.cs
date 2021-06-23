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
        Thread T;
        string partida;
        string nombre;
        string jugador;
        string mensnum;
        int[] numeros = new int[25];
        Form3 f;
        string listaID = "-1/";
        delegate void DelegadoParaPonerTexto(string texto);
        List<Form3> formularios = new List<Form3>();

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
        public void Atenderservidor()
        {
            while (true)                                                                                                                                                                    
            {
                string box = textBox1.Text;
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                string[] trozos = mensaje.Split('/');
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
                        if (textBox1.Text == "")
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
                        if (textBox1.Text == "")
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
                            for(j=3;j<=10;j++){
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
                            int f = dataGridView1.RowCount;
                            int m = Convert.ToInt32(trozos[3]);
                            int i = 4;
                            int b = 0;
                            string comp = trozos[2];
                            jugador = comp;
                            dataGridView1.Rows.Clear();
                            dataGridView2.Rows.Clear();
                            i = 4;
                            b = 0;
                            while (b < m)
                            {
                                dataGridView1.Rows.Add(trozos[i]);
                                i++;
                                b++;
                            }
                            
                            f = Convert.ToInt32(trozos[i]);
                            i++;
                            b = 0;
                            while (b < f)
                            {
                                dataGridView2.Rows.Add(trozos[i]);
                                i++;
                                b++;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        try
                        {
                            f.caso11(trozos);
                        }
                        catch { }
                        break;
                    case 7://INVITAR          ***********************  N U E V O Version4
                        if(Convert.ToInt32(trozos[1])==0)
                        {
                            MessageBox.Show("Invitación aceptada");//ACEPTADA 
                        }
                        else if (Convert.ToInt32(trozos[1]) == 1)//RECHAZADO
                        {
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke(del, new Object[] { "Te han rechazado" });
                            nombre = trozos[2];
                        }
                        else if (Convert.ToInt32(trozos[1]) == 2)//NOTIFICACION DE INVITACION
                        {
                            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                            this.Invoke (del, new Object[]{ "Te han invitado a jugar"});
                            nombre = trozos[2];
                            invitacio = 1;
                        
                        }
                        else if (Convert.ToInt32(trozos[1])==3)
                        {
                            MessageBox.Show("Jugador no encontrado");
                        }
                        break;
                 case 8:
                        string npartida = trozos[2];
                        if (npartida == nombre)
                        {
                            f.TomaRespuesta2(trozos[1]);
                            f.mensn(ref mensnum);
                            try
                            {
                                f.convertir(mensnum, ref numeros);
                            }
                            catch
                            {
                            }
                            f.ActualizarNumeros(ref numeros, trozos[1], ref mensnum);
                            f.BorrarArchivo(nombre);
                            f.CrearArchivo(mensnum, nombre);
                            f.LeerArchivo(nombre, ref numeros);
                            f.IngresarDatos(numeros);
                            f.Ganador(numeros, nombre);
                        }
                        break;
                 case 9:
                        int k = Convert.ToInt32(trozos[1]);
                        int l = 0;
                        dataGridView2.Rows.Clear();

                        while (l < k)
                        {
                            dataGridView2.Rows.Add(trozos[l + 3]);
                            l++;
                        }
                        break;
                 case 10:
                        f.AparecerEmpezar();
                        f.TomaRespuesta1("Ya puedes empezar la partida");
                        break;
                 case 11:
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            f.AparecerSiguiente();
                            f.TomaRespuesta1("La partida ha empezado");
                            f.DesaparecerEmpezar();
                        }
                        if (Convert.ToInt32(trozos[1]) == -1)
                        {
                            MessageBox.Show("La partida ya ha comenzado, no puedes unirte");
                        }
                        if (Convert.ToInt32(trozos[1]) == 1)
                        {
                            MessageBox.Show(trozos[2]);
                            MessageBox.Show(jugador);
                            MessageBox.Show(rusername);
                            algo(trozos[2]);
                            this.Show();
                        }
                        if (Convert.ToInt32(trozos[1]) == 2)
                        {
                            T.Abort();
                            f.Close();
                        }
                        break;
                 case 12:
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            f.TomaRespuesta1("Has ganado");
                        }
                        else
                            f.TomaRespuesta1("Has perdido");
                        f.escondersiguiente();
                        break;
                }
            }
        }
        string rusername, rpassword;

        public void SetUsername(string usuario)
        {
            rusername = usuario;
        }
        public void SetPassword(string contraseña)
        {
            rpassword = contraseña;
        }

        public void Agregarform3(ref Form3 fs, Socket serv, string nom)
        {
            fs = new Form3(serv, nom, jugador);
        }

        public void Guardarforms(Form3 fs)
        {
        
        }

        private void Desconectar_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            atender.Abort();
            server.Close();
            Application.Exit();
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
            }
            else
            {
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                this.Invoke(del, new Object[] { "Asegurate de poner el nombre en consultas" });
            }
                /*if (JugadorContrincante.Checked) //CONSULTA 2. 
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
                

            }*/
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

        public void siguiente()
        {
            string mensaje = "9/";
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }






        public int Connectar()
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9053);


            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return
                return -1;
            }
            ThreadStart ts = delegate { Atenderservidor(); };
            atender = new Thread(ts);
            atender.Start();
            return 0;
        }

        private void PonerenmarchaFormulario(string g)
        {
            f = new Form3(server, g, rusername);
            f.ShowDialog();
        }

        private void algo(string g)
        {
            ThreadStart ts = delegate { PonerenmarchaFormulario(g); };
            T = new Thread(ts);
            T.Start();
        }

        public int Iniciar_Sesion(string tusername, string tpassword)
        {
            string mensaje = "2/" + tusername + "/" + tpassword ;
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
            else if (Convert.ToInt32(trozos[1]) == 2)
            {
                return 2;
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
            string mensaje = "7/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (invitacio==0)
                MessageBox.Show("No tienes ninguna invitación para jugar aún");
            else
            {
                string mensaje = "11/" + nombre;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Invitacio);
                this.Invoke(del, new Object[] { "" });
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

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form4 F4 = new Form4(server, listaID, rusername);
            F4.ShowDialog();
            F4.nombrepartida(ref nombre);
            algo(nombre);
        }

        private void dataGridView2_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            
            string usergrid = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            usergrid = usergrid.TrimEnd('\0');
            partida = usergrid;

            string mensaje = "11/" + usergrid + "/" + rusername;
            // Enviamos al servidor el nombre tecleado
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            nombre = partida;
        }

        private void Bingo_Enter_1(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            try
            {
                server.Send(msg);

                // Nos desconectamos
                this.BackColor = Color.Gray;
                server.Shutdown(SocketShutdown.Both);
                atender.Abort();
                server.Close();
            }
            catch { }
            Application.Exit();
        }
    }
}

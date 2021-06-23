using System;
using System.IO;
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
        string nombre;
        string mensnum;
        string jugador;
        int[] numeros = new int[25];
        public Form3(Socket server, string nombre, string jugador)
        {
            InitializeComponent();
            this.server = server;
            this.nombre = nombre;
            this.jugador = jugador;
        }

        public void caso8(string[] trozos)
        {
            string npartida = trozos[2];
            //string[] partida = npartida.Split('/');
            //string nppartida = partida[0];
            if (npartida == nombre)
            {
                TomaRespuesta2(trozos[1]);

                //ActualizarNumeros(ref numeros, trozos[1]);
                BorrarArchivo(nombre);
                CrearArchivo(mensnum, nombre);
                LeerArchivo(nombre, ref numeros);
                IngresarDatos(numeros);
                Ganador(numeros, nombre);
            }
        }


        public void caso11(string[] trozos)
        {
            if (Convert.ToInt32(trozos[1]) == 0)
            {
                int f = Convert.ToInt32(trozos[3]);
                int i = 4;
                int b = 0;
                dataGridView3.Rows.Clear();

                while (b < f)
                {
                    dataGridView3.Rows.Add(trozos[i]);
                    i++;
                    b++;
                }
                f = Convert.ToInt32(trozos[i]);
                i++;
                b = 0;
                while (b < f)
                {
                    i++;
                    b++;
                }
                if (Convert.ToInt32(trozos[i]) == 1)
                {
                    Listo.Visible = false;
                    TomaRespuesta1("Necesitas al menos 2 jugadores");
                    //Empezar.Visible = true;
                }
            }
            else
            {
                MessageBox.Show("Error al consultar");
            }
        }

        public void escondersiguiente()
        {
            Siguiente.SendToBack();
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
                        string npartida = trozos[2];
                        //string[] partida = npartida.Split('/');
                        //string nppartida = partida[0];
                        if (npartida == nombre)
                        {
                            TomaRespuesta2(trozos[1]);

                            //ActualizarNumeros(ref numeros, trozos[1]);
                            BorrarArchivo(nombre);
                            CrearArchivo(mensnum, nombre);
                            LeerArchivo(nombre, ref numeros);
                            IngresarDatos(numeros);
                            Ganador(numeros, nombre);
                        }
                        break;
                    case 10:
                        //NumerosAleatorios(ref mensnum);
                        //CrearArchivo(mensnum, nombre);
                        //LeerArchivo(nombre, ref numeros);
                        //IngresarDatos(numeros);
                        //Siguiente.Visible = true;
                        break;
                    case 11:
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            int f = Convert.ToInt32(trozos[2]);
                            int i = 0;
                            dataGridView3.Rows.Clear();

                            while (i < f)
                            {
                                //MessageBox.Show(trozos[i+3]);
                                dataGridView3.Rows.Add(trozos[i + 3]);
                                i++;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error al consultar");
                        }
                        break;
                    case 12:
                        if (Convert.ToInt32(trozos[1]) == 0)
                        {
                            TomaRespuesta1("Has ganado");
                        }
                        else
                            TomaRespuesta1("Has perdido");
                        Siguiente.SendToBack();
                        break;
                }
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            try
            {
                LeerArchivo(nombre, ref numeros);
                IngresarDatos(numeros);
                convertir2(ref mensnum, numeros);
                Listo.Visible = false;
                Siguiente.BringToFront();
            }
            catch
            {
                string mensaje2 = "6/" + nombre + "/" + jugador;
                byte[] msg2 = System.Text.Encoding.ASCII.GetBytes(mensaje2);
                server.Send(msg2);
            }
        }

        public void Notificacion(string respuesta)//Imprime en pantalla informacion del juego como el turno o movimientos erroneos
        {
            this.label4.Text = respuesta;
        }

        public void Notificacion2(string respuesta)//Imprime en pantalla informacion del juego como el turno o movimientos erroneos
        {
            this.label1.Text = respuesta;
            int d = Convert.ToInt32(respuesta);
            int residuo;
            int quo;
            quo = d / 10;
            residuo = d % 10;
            pictureBox79.ImageLocation = quo + ".png";
            pictureBox78.ImageLocation = residuo + ".png";
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string mensaje = "15/" + nombre;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            
            
            mensaje = "6/" + nombre;
            msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            this.Close();
        }

        public void salir()
        {
            string mensaje = "6/" + nombre;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            Close();
        }

        private void Form3_FormClosing(Object sender, FormClosingEventArgs e)
        {

            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "CloseReason", e.CloseReason);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Cancel", e.Cancel);
            messageBoxCS.AppendLine();
            MessageBox.Show(messageBoxCS.ToString(), "FormClosing Event");
        }

       /* private void Form3_Closing(object sender, CancelEventArgs e)
        {
            string mensaje = "6/" + nombre;
            MessageBox.Show("Envio el mensaje");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            actualitzar_running = actualitzar.IsAlive;
            if (actualitzar_running)
            {
                actualitzar_running = false;
                if (!actualitzar.Join(1000)) // Give the thread 1 sec to stop
                {
                    actualitzar.Abort();
                }
            }
            Close();
        }*/

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void button2_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            int aleatorio = rnd.Next(1, 99);
            string mensaje = "9/" + nombre + "/" + aleatorio;
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

        public void TomaRespuesta2(string mensaje)
        {
            DelegadoParaPonerTexto del = new DelegadoParaPonerTexto(Notificacion2);
            this.Invoke(del, new Object[] { mensaje });
        }

        public void Ganador(int[] numeros, string nombre)
        {
            string fileName = nombre + ".txt";
            int n = 0;
            while (n < 25)
            {
                if (numeros[n] == -1)
                    n++;
                else
                    n = 100;
            }
            if (n == 25)
            {
                string mensaje = "14/" + nombre;
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
                File.Delete(fileName);
            }
        }

        public void ActualizarNumeros(ref int[] num, string numero, ref string me)
        {
            int m = 0;
            //File.Delete(nombre + ".txt");
            while (m < 25)
            {
                if (num[m] == Convert.ToInt32(numero))
                    num[m] = -1;
                if (m == 0)
                    me = (num[m] + "/");
                else
                    me = (me + num[m] + "/");
                    m++;
            }
            mensnum = me;
        }

        public void BorrarArchivo(string nombre)
        {
            /*StreamReader archivo = new StreamReader(nombre + ".txt");
            string numero;
            int i = 1;
            numero = numeros[0] + "/";
            while (i < 25)
            {
                numero = (numero + "/");
                i++;
            }
            */
            string fileName = nombre + ".txt";
            var tempFile = Path.GetTempFileName();
            try
            {
                var linesToKeep = File.ReadLines(fileName).Where(l => l != "removeme");
                File.WriteAllLines(tempFile, linesToKeep);

            File.Delete(fileName);
            File.Move(tempFile, fileName);
            }
            catch
            {
            }
            
        }

        public void convertir2(ref string men, int[] numer)
        {
            int i = 1;
            men = numer[0] + "/";
            while (i <= 24)
            {
                men = men + numer[i] + "/";
                i++;
            }
        }

        public void convertir(string men, ref int[] numer)
        {
            int i = 0;
            string[] trozos = men.Split('/');
            while (i <= 24)
            {
                numer[i] = Convert.ToInt32(trozos[i]);
                i++;
            }
        }

        public void NumerosAleatorios(ref string mensnum)
        {
            int i = 0;
            int k = 0;
            string numero;
            int num = 0;
            int j = 0;
            int [] repetidos = new int [99];
            int aleatorio;
            int nn = 0;
            Random rnd = new Random();
            aleatorio = rnd.Next(1, 99);
            numero=(aleatorio + "/");
            while (i < 25)
            {
                aleatorio = rnd.Next(1, 99);
                while (k < num)
                    {
                        if (repetidos[k] == aleatorio)
                        {
                            nn++;
                            k++;
                        }
                        else
                            k++;
                    }
                    k = 0;
                    if (nn == 0)
                    {
                        num++;
                        repetidos[j] = aleatorio;
                        j++;
                        numero = (numero + aleatorio + "/");
                        i++;
                    }
                    nn = 0;
            }
            mensnum = numero;
        }

        public void CrearArchivo(string numero, string nombre)
        {
            StreamWriter numeros = new StreamWriter(nombre + ".txt");
            numeros.WriteLine(numero);
            numeros.Close();
        }

        public void LeerArchivo(string nombre, ref int[] numeros)
        {
            string mensaje;
            int depaso;
            int i = 0;
            StreamReader archivo = new StreamReader(nombre + ".txt");
            mensaje = archivo.ReadLine();
            string[] num = mensaje.Split('/');
            while (i < 25)
            {
                depaso = Convert.ToInt32(num[i]);
                numeros[i] = depaso;
                i++;
            }
            archivo.Close();
        }

        public void PonerNumeros(int numeros, PictureBox picture, PictureBox picture2, PictureBox picture3)
        {
            int residuo;
            int quo;
            if (numeros != -1)
            {
                quo = numeros / 10; residuo = numeros % 10; picture.ImageLocation = quo + ".png"; picture2.ImageLocation = residuo + ".png"; picture3.Visible = false;
            }
            else
            {
                picture3.Visible = true; picture3.ImageLocation = "x.png"; picture.Visible = false; picture2.Visible = false;
            }
        }

        public void IngresarDatos(int [] numeros)
        {
            //int residuo;
            //int quo;
            PonerNumeros(numeros[0], pictureBox3, pictureBox4, pictureBox53);
            PonerNumeros(numeros[1], pictureBox6, pictureBox5, pictureBox54);
            PonerNumeros(numeros[2], pictureBox8, pictureBox7, pictureBox55);
            PonerNumeros(numeros[3], pictureBox10, pictureBox9, pictureBox56);
            PonerNumeros(numeros[4], pictureBox12, pictureBox11, pictureBox57);
            PonerNumeros(numeros[5], pictureBox14, pictureBox13, pictureBox58);
            PonerNumeros(numeros[6], pictureBox16, pictureBox15, pictureBox59);
            PonerNumeros(numeros[7], pictureBox18, pictureBox17, pictureBox60);
            PonerNumeros(numeros[8], pictureBox20, pictureBox19, pictureBox61);
            PonerNumeros(numeros[9], pictureBox22, pictureBox21, pictureBox62);
            PonerNumeros(numeros[10], pictureBox24, pictureBox23, pictureBox63);
            PonerNumeros(numeros[11], pictureBox26, pictureBox25, pictureBox64);
            PonerNumeros(numeros[12], pictureBox28, pictureBox27, pictureBox65);
            PonerNumeros(numeros[13], pictureBox30, pictureBox29, pictureBox66);
            PonerNumeros(numeros[14], pictureBox32, pictureBox31, pictureBox67);
            PonerNumeros(numeros[15], pictureBox34, pictureBox33, pictureBox68);
            PonerNumeros(numeros[16], pictureBox36, pictureBox35, pictureBox69);
            PonerNumeros(numeros[17], pictureBox38, pictureBox37, pictureBox70);
            PonerNumeros(numeros[18], pictureBox40, pictureBox39, pictureBox71);
            PonerNumeros(numeros[19], pictureBox42, pictureBox41, pictureBox72);
            PonerNumeros(numeros[20], pictureBox44, pictureBox43, pictureBox73);
            PonerNumeros(numeros[21], pictureBox46, pictureBox45, pictureBox74);
            PonerNumeros(numeros[22], pictureBox48, pictureBox47, pictureBox75);
            PonerNumeros(numeros[23], pictureBox50, pictureBox49, pictureBox76);
            PonerNumeros(numeros[24], pictureBox52, pictureBox51, pictureBox77);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string mensaje = "12/" + nombre;
            MessageBox.Show("Envio el mensaje");
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
            TomaRespuesta1("Esperando al resto de jugadores");

            Listo.Visible = false;

            NumerosAleatorios(ref mensnum);
            CrearArchivo(mensnum, nombre);
            LeerArchivo(nombre, ref numeros);
            IngresarDatos(numeros);
        }

        public void AparecerEmpezar()
        {
            Empezar.BringToFront();
        }

        public void AparecerSiguiente()
        {
            Siguiente.BringToFront();
            button2.SendToBack();
        }

        public void DesaparecerEmpezar()
        {
            Empezar.SendToBack();
        }

        public void mensn(ref string m)
        {
            m = mensnum;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //string usergrid = dataGridView3.CurrentRow.Cells[0].Value.ToString();
            string usergrid;
            //usergrid = usergrid.TrimEnd('\0');
            DataGridViewImageCell cell = (DataGridViewImageCell)
            dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex];
            usergrid = Convert.ToString(cell.Value);
            string mensaje = "7/" + usergrid + nombre;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView3_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            string mensaje = "7/" + textBox6.Text + "/" + nombre;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void Empezar_Click(object sender, EventArgs e)
        {
            string mensaje = "16/" + nombre;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            NumerosAleatorios(ref mensnum);
            CrearArchivo(mensnum, nombre);
            LeerArchivo(nombre, ref numeros);
            IngresarDatos(numeros);
        }
    }


}

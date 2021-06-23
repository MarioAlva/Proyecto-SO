#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <search.h>
#include <pthread.h>

int sockets[100];

typedef struct{
	char orden[20];
}Orden;

typedef struct{
	int numeros[25];
	int socket;
	char nombre[30];
}Jugador;
typedef struct{
	int num;
	int anfitrion;
	int totaljugadores;
	int empezado;
	int guardar;
	int listo;
	int salidos;
	int ganador;
	int socket[10];
	int repetidos [100];
	char nombre [80];
	Jugador jugador[10];
}Partida;

typedef struct{
	int num;
	Partida partida[100];
	int id[100];
}Partidas;

Partidas lista_partidas;

typedef struct{
	int num;
	Orden servicios[100];
}Servicios;

Servicios lista_servicios;

typedef struct{
	char jugador[20];
	int socket;
}Usuarios;
typedef struct{
	int num;
	Usuarios usuarios[100];
}Conectados;
Conectados lista_conectados;
int isocket[3];
char inombre[80];
pthread_mutex_t mutex;

int PonLista(Conectados *l, char jugador[20], int socket)
{
	if (l->num<100)
	{
		strcpy(l->usuarios[l->num].jugador,jugador);
		l->usuarios[l->num].socket=socket;
		l->num++;
		return 0;
	}
	else
		return -1;
}

int SaberRepetido(Conectados *l, char jugador[20])
{
	int i = 0;
	while (i < l->num)
	{
		if(strcmp(l->usuarios[i].jugador, jugador) == 0)
		{
		return -1;
		}
		i++;
	}
	return 0;
}

int PonJugadors(Partidas *s, int socket, char nombre[80], int j)
{
	if (s->num<100)
	{
		strcpy(s->partida[j].nombre,nombre);
		s->partida[j].socket[s->partida[j].num]=socket;
		s->partida[j].jugador[s->partida[j].num].socket=socket;
		s->partida[j].num++;
		s->num++;
		return socket;
	}
	else
		return -1;
}

int BuscarPartidaLibre(Partidas *s)
{
	int j;
	int i = 0;
	int encontrado = 0;
	printf("No he encontrado una partida libre\n");
	while(encontrado == 0)
	{
		printf("No he encontrado una partida libre\n");
		if ((s->partida[i].num == 0) && (s->partida[i].guardar == 0))
		{
			printf("He encontrado una partida libre\n");
			encontrado = 1;
			j = i;
			i++;
			return j;
		}
		else
			i++;
	}
}

int CompararNombres (Partidas *s, char nombre[80])
{
	int i = 0;
	int encontrado = 0;
	while ((i<100) && (encontrado == 0))
	{
		if (strcmp(s->partida[i].nombre, nombre) == 0)
		{
			encontrado = 1;
			return i;
		}
		else
			i++;
		printf("i es igual a %d\n", i);
	}
}

void EliminarPartida (Partidas *s, int y, int l)
{
	int o = y;
	if(s->partida[y].guardar == 0)
	{
		s->partida[y].listo = 0;
		s->partida[y].guardar = 0;
		s->partida[y].num = 0;
		if (s->partida[y].salidos == s->partida[y].totaljugadores-1)
			{
			if(s->partida[y].ganador == 1)
				{
				s->num--;
				s->partida[y].salidos = 0;
				lista_partidas.partida[y].ganador = 0;
				for(o=0;o < s->num; o++)
				{
					s->partida[o].ganador = s->partida[o+1].ganador;
					s->partida[o].guardar = s->partida[o+1].guardar;
					s->partida[o].listo = s->partida[o+1].listo;
					s->partida[o].num = s->partida[o+1].num;
					s->partida[o].salidos = s->partida[o+1].salidos;
					s->partida[o].totaljugadores = s->partida[o+1].totaljugadores;
					strcpy(s->partida[o].nombre, s->partida[o+1].nombre);
					int u;
					for(u=0;u<s->partida[o+1].num;u++)
					{
						s->partida[o].socket[u] = s->partida[o+1].socket[u];
					}
					for(u=0;u<=100;u++)
					{
						s->partida[o].repetidos[u] = s->partida[o+1].repetidos[u];
					}
					for(u=0;u< s->partida[o+1].totaljugadores ;u++)
					{
						s->partida[o].jugador[u].socket = s->partida[o+1].jugador[u].socket;
						strcpy(s->partida[o].jugador[u].nombre, s->partida[o+1].jugador[u].nombre);
					}
				}	
				}
			else
			   s->partida[y].salidos = 0;
			}
		
		else
			s->partida[y].salidos++;
	}
	else
    {
	   s->partida[y].num--;
	}
	printf("%d\n", s->partida[y].salidos);
	int i = 0;
	int j = -1;
	while(i<=3)
	{
		if(s->partida[y].socket[i] == l)
		{
			s->partida[y].socket[i] = 0;
			i = j;
		}
		i++;
	}
	if (j!=-1)
	{
		while(j<=2)
		{
			s->partida[y].socket[j] = s->partida[y].socket[j+1];
			j++;
		}
	}
}

int EliminarLista(Conectados *l, int socket)
{
	int encontrado=0;
	int i=0;
	while((i<l->num) && (encontrado==0))
	{
		if(l->usuarios[i].socket==socket)
			encontrado=1;
		else
			i++;
	}
	if (!encontrado)
		return -1;
	else
	{
		while(i<l->num-1)
		{
			l->usuarios[i]=l->usuarios[i+1];
			i++;
		}
		l->num=l->num-1;
		return 0;
	}
}
int UsuarioExistente(char Usuario[20])
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Bingo",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	sprintf(consulta, "SELECT Jugador.Username FROM Jugador WHERE Jugador.Username = '%s'\n", Usuario);
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (row == NULL){ //no hay usuarios con ese nombre
		return 0;
		printf("No existe el usuario\n");
	}
	
	else            //nombre ya existe
		return -1;
	
}
int Registro(char usuario[20], char password[20])
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Bingo",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	sprintf(consulta,"INSERT INTO Jugador VALUES ('%s', '%s', 'NuevoJugador', 0);",usuario, password);
	printf("%s\n",consulta);
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	
	mysql_close (conn);
	return 0;
}
int IniciarSesion(char username[20], char password[20])
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [100];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Bingo",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	sprintf(consulta, "SELECT Jugador.Username FROM Jugador WHERE Jugador.Username='%s' AND Jugador.Password='%s' ",username, password);
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL) 
		return 0;
	else            
		return -1;
	
}

int Fondos(char nombre[200], char respuesta[512])
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Bingo",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	strcpy(consulta, "SELECT Jugador.Fondos FROM Jugador WHERE Jugador.Username ='");
	strcat(consulta, nombre);
	strcat(consulta, "';");
	err=mysql_query (conn, consulta);
	
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	//al obtener la consulta
	
	if (row == NULL)
		sprintf (respuesta,"No se han obtenido datos en la consulta\n");
	else
		while (row !=NULL) {
			printf("%s\n", row[0]);
			sprintf (respuesta,"%s\n", row[0]);
			printf("%s\n", respuesta);
			row = mysql_fetch_row (resultado);
	}
	return 0;
	
	}

int JugadorContrincante(char nombre[20], char respuesta [200])
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado1;
	MYSQL_ROW row1;
	
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [300];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Bingo",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	sprintf(consulta, "SELECT Game.Identificador FROM (Jugador, Partida, Game) WHERE Game.Username = Jugador.Username AND Jugador.Username = '%s';", nombre);
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (-1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	sprintf(consulta, "SELECT Jugador.Username FROM (Jugador, Partida, Game) WHERE Game.Username = Jugador.Username AND Game.Identificador = '%s';", row[0]);
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (-1);
	}
	
	resultado1 = mysql_store_result (conn);
	row1 = mysql_fetch_row (resultado1);
	if (row == NULL)
		sprintf (respuesta,"No se han obtenido datos en la consulta\n");
	else
	{	while (row !=NULL) 
	{
		// la columna 0 contiene el nombre del jugador
		sprintf (respuesta,"%s\n", row[0]);
		// obtenemos la siguiente fila
		row = mysql_fetch_row (resultado1);
	}
	printf("%s\n",respuesta);
	return 0;
	}
}
int Ranking(char respuesta[200])
{
	MYSQL *conn;
	int err;
	//++++++++++++++++++++++++++++++++++Almacenar consulta
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	//++++++++++++++++++++++++++++++++++Establecemos conexion
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (-1);
	}
	//inicializar la conexion
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "Bingo",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n", 
				mysql_errno(conn), mysql_error(conn));
		return (-1);
	}
	//++++++++++++++++++++++++++++++++++++++++++CONSULTA
	strcpy(consulta, "SELECT * FROM Jugador ORDER BY Fondos DESC;");
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return (-1);
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	printf("%s %s %s\n", row[0], row[1], row[2]);
	row = mysql_fetch_row (resultado);
	printf("%s %s %s\n", row[0], row[1], row[2]);
	if (row == NULL)
	{	
		sprintf (respuesta,"No se han obtenido datos en la consulta\n");
		return (0);
	}
	else
	{	while (row !=NULL) 
	{
		// la columna 0 contiene el nombre del jugador
		sprintf (respuesta,"%s\n", row[0]);
		// obtenemos la siguiente fila
		row = mysql_fetch_row (resultado);
	}
	printf("%s\n",respuesta);
	int ganadas= atoi (respuesta);
	return ganadas;
	}
}

int Companys(Conectados *l, char invitacio[20])//Buscar el socket del invitado
{
	int i=0;
	int trobat=0;
	printf("%s\n", invitacio);
	while ((i<l->num)&&(trobat==0))
	{
		if (strcmp(l->usuarios[i].jugador, invitacio) == 0)
		{
			trobat=1;
		}
		else
					i++;
	}
	if (trobat==0)
	{
		return -1;//no encontrado
	}
	else
		return l->usuarios[i].socket;//devuelve el socket del invitado
}
int guardar(int socket, int i) //Guardar socket del que invita
{
	isocket[i]= socket;
	i++;
}

void guardar2(char nombre[80]) //Guardar socket del que invita
{
	strcpy(inombre, nombre);
}


void *AtenderCliente(void *socket)
{
	
	int sock_conn;
	int ret;
	int *s;
	s = (int*) socket;
	sock_conn = *s;
	char buff[512];
	char respuesta[512];
	
	char user[20];
	int terminar = 0;
	int formnum;
	while (terminar == 0)
	{
		// Recibimos peticion de un usuario
		ret=read(sock_conn,buff, sizeof(buff));
		printf ("Recibido\n");
		
		buff[ret]='\0'; //fin de string
		
		//Escribimos en consola quien se ha conectado
		printf ("El código de peticion es: %s\n",buff);
		
		//Servicios
		
		char *servicios = strtok(buff, "/");
		int codigo = atoi (servicios);
		int i=0;
		while ((servicios = strtok(NULL,"/")) != NULL)
		{
			//mensaje troceado
			strcpy(lista_servicios.servicios[i].orden,servicios);
			i++;
		}
		
		//situaciones
		if (codigo==0)
		{
			int socket;
			char npartidas[10];
			printf("Desconectar socket %d\n", sock_conn);
			socket=sock_conn;
			pthread_mutex_lock(&mutex);
			int e= EliminarLista (&lista_conectados,socket);
			pthread_mutex_unlock(&mutex);
			int j;
			j=0;
			strcpy(respuesta, "6/0/");
			char numero[10];
			strcat(respuesta, "algo/");
			sprintf(numero, "%d", lista_conectados.num);
			strcat(respuesta, numero);
			
			while ( j < lista_conectados.num)
			{
				strcat(respuesta, "/");
				strcat (respuesta, lista_conectados.usuarios[j].jugador);
				j = j + 1;
			}
			j=0;
			sprintf(npartidas, "/%d", lista_partidas.num);
			strcat(respuesta, npartidas);
			while ( j < lista_partidas.num)
			{
				strcat(respuesta, "/");
				strcat (respuesta, lista_partidas.partida[j].nombre);
				j = j + 1;
			}
			strcat(respuesta, "/");
			printf("%s\n", respuesta);
			//enviar por todos los sockets q tengo conectados en ese momento
			int k;
			for(k=0; k<lista_conectados.num; k++)
				write(lista_conectados.usuarios[k].socket,respuesta, strlen(respuesta));
			terminar=1;
		}
		
		if(codigo==1)
		{                       	//Registro
			
			char username[20];
			char password[20];
			
			strcpy (username, lista_servicios.servicios[0].orden);
			strcpy (password, lista_servicios.servicios[1].orden);
			int existe = UsuarioExistente(username);
			printf("%d\n",existe);
			if(existe == 0)
			{
				int r = Registro(username,password);
				strcpy(respuesta, "1/0");
				
			}
			else //usuario exitente
			{		
				strcpy(respuesta, "1/-1");
				printf("Error Registro");
			}
			write(sock_conn,respuesta,strlen(respuesta));
		}
		if (codigo == 2) //Inicio Sesion
		{
			
			char username [20];
			char password[20];
			strcpy (username, lista_servicios.servicios [0].orden);
			strcpy (password, lista_servicios.servicios[1].orden);
			int lerr= IniciarSesion(username, password);
			if (lerr==-1)
			{
				char jugador[20];
				strcpy(jugador, username);
				int g = SaberRepetido(&lista_conectados, jugador);
				if(g == 0)
				{
				strcpy ( respuesta, "2/0");

				pthread_mutex_lock(&mutex);
				write (sock_conn, respuesta, strlen(respuesta));
				pthread_mutex_unlock (&mutex); // Ya puedes interrumpirme
				printf("%d\n", sock_conn);

					int p= PonLista(&lista_conectados, jugador, sock_conn);
					// notificar a todos los clientes conectados
					int j;
					j=0;
					strcpy(respuesta, "6/0/");
					char numero[10];
					char npartidas[10];
					strcat(respuesta, username);
					strcat(respuesta, "/");
					sprintf(numero, "%d", lista_conectados.num);
					strcat(respuesta, numero);
				
					while ( j < lista_conectados.num)
					{
						strcat(respuesta, "/");
						strcat (respuesta, lista_conectados.usuarios[j].jugador);
					
						j = j + 1;
					}
					j=0;
					sprintf(npartidas, "/%d", lista_partidas.num);
					strcat(respuesta, npartidas);
					while ( j < lista_partidas.num)
					{
						strcat(respuesta, "/");
						strcat (respuesta, lista_partidas.partida[j].nombre);
						j = j + 1;
					}
					strcat(respuesta, "/");
					printf("%s\n", respuesta);
				
					//enviar por todos los sockets q tengo conectados en ese momento
					int k;
					pthread_mutex_lock(&mutex);
					for(k=0; k<lista_conectados.num; k++)
					{
						printf("%d\n", lista_conectados.usuarios[k].socket);
						write(lista_conectados.usuarios[k].socket,respuesta, strlen(respuesta));
					}
					pthread_mutex_unlock (&mutex);
				}
				else
				{
					strcpy(respuesta, "2/2/");
					write(sock_conn ,respuesta, strlen(respuesta));
					strcpy(respuesta, "6/2/");
					write(sock_conn ,respuesta, strlen(respuesta));
				}
			}
			else
				strcpy (respuesta, "2/-1");
			
			write (sock_conn, respuesta, strlen(respuesta));
			
		}
		
		if (codigo==3) //Fondos
		{
			char nombre[200];
			char prueba[20];
			strcpy (nombre, lista_servicios.servicios [0].orden);
			int Consulta = Fondos(nombre, prueba);
			
			
			if (Consulta==0)
			{
				sprintf (respuesta,"3/0/%s\n",prueba);
				printf("Hola: %s",respuesta);
				
			}
			else
			{
				strcpy(respuesta,"Error");
			}
			write (sock_conn,respuesta, strlen(respuesta));
		}
		if(codigo ==4) //Jugador contrincante
		{
			char nombre[20];
			char prueba[20];
			strcpy (nombre, lista_servicios.servicios[0].orden);
			int Consulta = JugadorContrincante(nombre, prueba);
			
			if (Consulta == 0)
			{
				sprintf (respuesta,"4/-1/%s\n", prueba);
				printf("%s\n", respuesta);
			}
			else
			{
				strcpy(respuesta,"Error");
			}
			write (sock_conn, respuesta, strlen(respuesta));
			
		}
		if (codigo == 5) // Ranking
		{
			char prueba[500];
			int Consulta = Ranking(prueba);
			printf("%s\n", prueba);
			if (Consulta >= 0)
			{
				sprintf (respuesta,"5/-1/%s\n", prueba);
				
			}
			else
			{
				strcpy(respuesta,"Error");
			}
			write (sock_conn,respuesta, strlen(respuesta));
			
		}
		if (codigo == 6)	//enviar lista de conectados
		{
			char nombre[80];
			char jugador[30];
			strcpy(nombre, lista_servicios.servicios[0].orden);
			strcpy(jugador, lista_servicios.servicios[1].orden);
			int y = CompararNombres(&lista_partidas, nombre);
			printf("%d\n", lista_partidas.partida[y].num);
				int j;
				j=0;
				strcpy(respuesta, "6/0/");
				char numero[10];
				char npartidas[10];
				strcat(respuesta, jugador);
				strcat(respuesta, "/");
				sprintf(numero, "%d", lista_conectados.num);
				strcat(respuesta, numero);
				
				while ( j < lista_conectados.num)
				{
					printf("Entro al primer bucle\n");
					strcat(respuesta, "/");
					strcat (respuesta, lista_conectados.usuarios[j].jugador);
					j = j + 1;
				}
				j=0;
				sprintf(npartidas, "/%d", lista_partidas.num);
				strcat(respuesta, npartidas);
				while ( j < lista_partidas.num)
				{
					printf("Entro al segundo bucle\n");
					strcat(respuesta, "/");
					strcat (respuesta, lista_partidas.partida[j].nombre);
					j = j + 1;
				}
				strcat(respuesta, "/");
				printf("Llego hasta aqui\n");
				printf("%s\n", respuesta);
				char respuesta2[512];
				strcpy(respuesta2, respuesta);
				strcat(respuesta, "1/");
				j = 0;
				printf("%s\n", respuesta);
				printf("%d\n", lista_partidas.partida[y].anfitrion);
				pthread_mutex_lock(&mutex);
				write(lista_partidas.partida[y].anfitrion, respuesta, strlen(respuesta));
				//enviar por todos los sockets q tengo conectados en ese momento
				int k;
				for(k=0; k<lista_conectados.num; k++)
				{
					printf("Envio el mensaje al cliente\n");
					printf("%d\n", lista_conectados.usuarios[k].socket);
					printf("%s\n", respuesta2);
					write(lista_conectados.usuarios[k].socket,respuesta2, strlen(respuesta2));
				}
				pthread_mutex_unlock (&mutex); // Ya puedes interrumpirme
		}
		if (codigo==7)//enllestit
		{
			char nom[80];
			char invitacio[20];//nombre del invitado
			strcpy(nom,lista_servicios.servicios[1].orden);
			guardar(sock_conn, 0);
			guardar2(nom);
			strcpy(invitacio,lista_servicios.servicios[0].orden);
			printf ("%d\n", sock_conn);
			int s= Companys(&lista_conectados, invitacio);//buscamos el socket del invitado
			if (s == -1)
			{
				strcpy(respuesta, "7/3/");//no encontrado
				strcat(respuesta, nom);
				write(sock_conn, respuesta, strlen(respuesta));
			}
			else
			{
				strcpy(respuesta, "7/2/");//he encontrado al que invito en las lista conectados
				strcat(respuesta, nom);
				write(s, respuesta, strlen(respuesta));
			}
			printf("%s\n",respuesta);
		}
		if (codigo==8)//Respuesta del invitado
		{
			char nombre[80];
			char nom[80];
			strcpy(nom,lista_servicios.servicios[1].orden);
			int r = atoi (lista_servicios.servicios[0].orden);
			if (r==0)
			{
				strcpy(respuesta, "7/0/");//ha aceptado la invitacion
				strcat(respuesta, nom);
				int y = BuscarPartidaLibre(&lista_partidas);
				PonJugadors(&lista_partidas, sock_conn, inombre, y);
				write( isocket[0]  , respuesta, strlen(respuesta));
				write( sock_conn  , respuesta, strlen(respuesta));
			}
			else
			{
				strcpy(respuesta, "7/1/");//ha rechazado la invitacion
				strcat(respuesta, nom);
				printf ("%d\n", isocket[0]);
				write(isocket[0] , respuesta,strlen(respuesta));
			}
		}
		if (codigo==9)//Respuesta del invitado
		{
			char npartida[80];
			int aleatorio = atoi(lista_servicios.servicios[1].orden);
			strcpy(npartida, lista_servicios.servicios[0].orden);
				int k = 0;
				strcpy(respuesta, "8/");//ha aceptado la invitacion

				int m = 0;
				int encontrado = 0;
				int u = -1;
				u = CompararNombres(&lista_partidas, npartida);
				while((m<99)&&(encontrado == 0))
				{
				if(lista_partidas.partida[u].repetidos[aleatorio-1] == aleatorio)
				{
					int random = rand () % 99 + 1;
					aleatorio = random;
				}
				else
				   {
				   lista_partidas.partida[u].repetidos[aleatorio-1] = aleatorio;
				   encontrado = 1;
					}
				m++;
				}
				sprintf(respuesta, "%s%d/%s/",respuesta, aleatorio, npartida);
				
				printf("%s\n",respuesta);
				printf("%d\n", lista_partidas.num);
				printf("%d\n", u);
				if(u != -1)
				{
					for(k=0; k<lista_partidas.partida[u].num; k++)
					{
						printf("%d\n", lista_partidas.partida[u].socket[k]);
						write(lista_partidas.partida[u].socket[k],respuesta, strlen(respuesta));
					}
				}
		}
			if (codigo==10)
			{
				printf("holiiiiiisadios\n");
				char partida[80];
				strcpy(partida, lista_servicios.servicios[0].orden);
				int y = BuscarPartidaLibre(&lista_partidas);
				printf("%d\n", y);
				PonJugadors(&lista_partidas, sock_conn, partida, y);
				lista_partidas.partida[y].anfitrion = sock_conn;
				printf("%d/%d/%s\n", lista_partidas.partida[0].socket[lista_partidas.partida[0].num-1], lista_partidas.partida[0].num, lista_partidas.partida[0].nombre);
				lista_partidas.partida[lista_partidas.num-1].jugador[lista_partidas.partida[lista_partidas.num-1].num-1].socket = sock_conn;
				strcpy(lista_partidas.partida[y].jugador[0].nombre, lista_servicios.servicios[1].orden);
				int j;
				j=0;
				strcpy(respuesta, "9/");
				char numero[10];
				sprintf(numero, "%d/", lista_partidas.num);
				strcat(respuesta, numero);
				sprintf(respuesta, "%s%s",respuesta, partida);
				printf("%s\n", respuesta);
				printf("%d\n", lista_partidas.partida[y].num);
				int l;
				l=0;
				while ( l < lista_partidas.num)
				{
					strcat(respuesta, "/");
					printf("holiiiiiis%d\n",l);
					strcat (respuesta, lista_partidas.partida[l].nombre);
					l = l + 1;
				}
				strcat(respuesta, "/");
				printf("%s\n", respuesta);
				int q;
				for(q=0; q<lista_conectados.num; q++)
					write(lista_conectados.usuarios[q].socket,respuesta, strlen(respuesta));
			}
			if (codigo==11)
			{
				char npartida[80];
				char iguales[80];
				int encontrado = 0;
				int i;
				strcpy(npartida, lista_servicios.servicios[0].orden);
				int y= CompararNombres(&lista_partidas, npartida);
				printf("%d\n", y);
				printf("%d/%s/%s\n",lista_partidas.num, lista_partidas.partida[y].nombre, npartida);
				printf("%d\n", lista_partidas.partida[y].empezado);
				printf("%s\n", lista_servicios.servicios[1].orden);
				if(lista_partidas.partida[y].empezado != 0)
				{
					printf("Entro al condicional");
					printf("%d\n", lista_partidas.partida[y].totaljugadores);
				for (i=0;i<lista_partidas.partida[y].totaljugadores;i++)
				{
					if(strcmp(lista_partidas.partida[y].jugador[i].nombre, lista_servicios.servicios[1].orden) == 0)
						encontrado = 1;
					printf("%s\n", lista_partidas.partida[y].jugador[i].nombre);
				}
				
				printf("%d\n", encontrado);
				
				}
				if((lista_partidas.partida[y].empezado == 0) || (encontrado == 1))
				{
					if(encontrado == 0)
						strcpy(lista_partidas.partida[y].jugador[lista_partidas.partida[y].num].nombre, lista_servicios.servicios[1].orden);	
				lista_partidas.partida[y].socket[lista_partidas.partida[y].num] = sock_conn;
				lista_partidas.partida[y].jugador[lista_partidas.partida[y].num].socket = sock_conn;
				printf("%d/%s\n", lista_partidas.partida[y].socket[lista_partidas.partida[y].num-1], lista_partidas.partida[y].nombre);
				lista_partidas.partida[y].num++;
				printf("%d\n", lista_partidas.partida[y].num);
				strcpy(respuesta, "11/1/");
				strcat(respuesta, npartida);
				strcat(respuesta, "/");
				printf("%s\n", respuesta);
				write(sock_conn,respuesta, strlen(respuesta));
				}
				else
				{
				   strcpy(respuesta, "11/-1/");
				   write(sock_conn,respuesta, strlen(respuesta));
				}
				printf("%d\n",sock_conn);
				printf("%d/%s\n", lista_partidas.partida[y].socket[lista_partidas.partida[y].num-1], lista_partidas.partida[y].nombre);
			}
			if(codigo==12)
			{
				strcpy(respuesta, "10/");//ha aceptado la invitacion
				char npartida[80];
				char iguales[80];
				int sock = 0;
				int cont;
				strcpy(npartida, lista_servicios.servicios[0].orden);
				int y = CompararNombres(&lista_partidas, npartida);
				int k;
				printf("%d/%s/%s\n",lista_partidas.num, lista_partidas.partida[0].nombre, npartida);

				lista_partidas.partida[y].listo++;
				cont = y;
				printf("Este es el socket actual: %d\n", sock_conn);
				printf("segundo: %d\n",y);
				printf("%d\n",lista_partidas.partida[y].listo);
				guardar(sock_conn, sock);
				write(lista_partidas.partida[cont].anfitrion, respuesta, strlen(respuesta));
			}
			if (codigo == 13)	//enviar lista de conectados
			{
				int j;
				int t = 0;
				j=0;
				strcpy(respuesta, "11/0/");
				char numero[10];
				sprintf(numero, "%d", lista_conectados.num);
				strcat(respuesta, numero);
				printf("%s\n", respuesta);
				while ( j < lista_conectados.num)
				{
					strcat(respuesta, "/");
					strcat (respuesta, lista_conectados.usuarios[j].jugador);
					j = j + 1;
				}
				strcat(respuesta, "/");
				j=0;
				while(t < lista_partidas.num)
				{
					while ( j < lista_partidas.partida[t].num)
					{
						printf("%d\n", lista_partidas.partida[t].socket[j]);
						write (lista_partidas.partida[t].socket[j], respuesta, strlen(respuesta));
						j = j + 1;
					}
					j=0;
					t++;
				}
				printf("%s\n", respuesta);
				write (sock_conn,respuesta, strlen(respuesta));
			}
			if(codigo==14)
			{
				char nombre[80];
				int j = 0;
				strcpy(nombre, lista_servicios.servicios[0].orden);
				int y = CompararNombres(&lista_partidas, nombre);
				while(j<lista_partidas.partida[y].num)
				{
					if(lista_partidas.partida[y].socket[j] != sock_conn)
					{
						strcpy(respuesta, "12/1/");
						printf("%d\n", lista_partidas.partida[y].socket[j]);
						printf("Has ganado creo\n");
						write(lista_partidas.partida[y].socket[j],respuesta, strlen(respuesta));
					}
					else
						{
						strcpy(respuesta, "12/0/");
						printf("%d\n", lista_partidas.partida[y].socket[j]);
						printf("No has ganado creo\n");
						write(lista_partidas.partida[y].socket[j],respuesta, strlen(respuesta));
						}
					lista_partidas.partida[y].ganador = 1;
					j++;
				}
			}
			if(codigo==15)
			{
				char nombre[80];
				strcpy(nombre, lista_servicios.servicios[0].orden);
				int y = CompararNombres(&lista_partidas, nombre);
				if (lista_partidas.partida[y].ganador == 1)
					{
					lista_partidas.partida[y].listo = 0;
					lista_partidas.partida[y].guardar = 0;
					EliminarPartida(&lista_partidas, y, sock_conn);
					printf("%d\n", lista_partidas.partida[y].num);
					}
				else
				{
					lista_partidas.partida[y].guardar = 1;
					EliminarPartida(&lista_partidas, y, sock_conn);
				}
				printf("%d\n", lista_partidas.partida[y].socket[0]);
				printf("%d\n", lista_partidas.partida[y].socket[1]);
				strcpy(respuesta, "11/2/");
				write(sock_conn ,respuesta, strlen(respuesta));
			}
			if(codigo==16)
			{
				char nombre[80];
				strcpy(nombre, lista_servicios.servicios[0].orden);
				int y = CompararNombres(&lista_partidas, nombre);
				lista_partidas.partida[y].totaljugadores = lista_partidas.partida[y].listo + 1;
				lista_partidas.partida[y].empezado = 1;
				strcpy(respuesta, "11/0/");
				int k;
				for(k=0; k<lista_partidas.partida[y].num; k++)
				{
					write(lista_partidas.partida[y].socket[k],respuesta, strlen(respuesta));
				}
			}
		}
}





int main(int argc, char *argv[])
{
	
	int sock_conn;
	int sock_listen;
	struct sockaddr_in serv_adr;
	pthread_t thread[100];
	int sockets[100];
	
	
	// INICIALITZACIONS
	// Abrimos socket
	/*if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)*/
	if ((sock_listen = socket(AF_INET, SOCK_STREAM,0)) < 0)
		printf("Error creant socket");
	// Fem el bind al port
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicializa a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	
	// escucharemos en el port 9051
	serv_adr.sin_port = htons(9053);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen");
	int i;
	int j=0;	
//	int e= BBDD	();
	pthread_mutex_init(&mutex, NULL);
	for(;;)
	{
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		//sock_conn es el socket que usaremos para este cliente
		sockets[j] = sock_conn;
		pthread_create ( &thread[j], NULL, AtenderCliente, &sockets[j]);
		j++;
	}
	for (i=0;i<100;i++){
		pthread_join(thread[i],NULL);
	}
	
	
	
}

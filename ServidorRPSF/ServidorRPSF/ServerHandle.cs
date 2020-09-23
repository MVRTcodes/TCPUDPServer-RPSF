using System;
using System.Collections.Generic;
using System.Text;

namespace ServidorRPSF
{
    class ServerHandle
    {
        public static void BienvenidaRecibida(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();
            int _PJ = _packet.ReadInt();

            Console.WriteLine($"{Servidor.clientes[_fromClient].tcp.socket.Client.RemoteEndPoint} se ha conectado correctamente y ahora es el jugador {_fromClient} con el personaje {_PJ}");
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($" Jugador \"{_username}\" (ID: {_fromClient} ha cogido un id de cliente equivocado ({_clientIdCheck})!");
            }
            // envia al jugador dentro del juego.
            Servidor.clientes[_fromClient].EnviaDentroJuego(_username,_PJ);
        }

        public static void ResultadosRonda(int _fromClient, Packet _packet)
        {
            bool[] _ataques = new bool[_packet.ReadInt()];
            for (int i = 0; i < _ataques.Length; i++)
            {
                _ataques[i] = _packet.ReadBool();
            }
            int[] _resultados = new int[_packet.ReadInt()];
            for (int i = 0; i < _ataques.Length; i++)
            {
                _resultados[i] = _packet.ReadInt();
            }
            Servidor.clientes[_fromClient].jugador.InstanciarResults(_ataques, _resultados);

        }
    }
}

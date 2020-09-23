using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ServidorRPSF
{
    class EnvioServidor
    {
        private static void EnviarDatosTCP(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
            Servidor.clientes[_toClient].tcp.EnviarDatos(_packet);
        }
        private static void EnviarDatosTCPaTodos(Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Servidor.MaxJugadores; i++)
            {
                    Servidor.clientes[i].tcp.EnviarDatos(_packet);
            }
        }

        private static void EnviarDatosTCPaTodos(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Servidor.MaxJugadores; i++)
            {
                if (i != _exceptClient)
                {
                    Servidor.clientes[i].tcp.EnviarDatos(_packet);
                }
            }
        }

        public static void Bienvenido(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                EnviarDatosTCP(_toClient, _packet);

            }
        }

        public static void SpawnJugador(int _toClient, Jugador _jugador)
        {
            using(Packet _packet = new Packet((int)ServerPackets.SpawnJugador))
            {
                _packet.Write(_jugador.id);
                _packet.Write(_jugador.username);
                _packet.Write(_jugador.PJ);

                EnviarDatosTCP(_toClient, _packet);
            }
        }

        public static void ResultadosRonda(Jugador _jugador)
        {
            using (Packet _packet = new Packet((int)ServerPackets.ResultadosRonda))
            {
                _packet.Write(_jugador.id);
                _packet.Write(_jugador.attacks[0]);
                _packet.Write(_jugador.attacks[1]);
                _packet.Write(_jugador.attacks[2]);
                _packet.Write(_jugador.results[0]);
                _packet.Write(_jugador.results[1]);
                _packet.Write(_jugador.results[2]);
                Console.WriteLine($"De camino a todos vosotros de parte del id {_jugador.id}");
                EnviarDatosTCPaTodos(_jugador.id, _packet);
            }
        }
    }
}

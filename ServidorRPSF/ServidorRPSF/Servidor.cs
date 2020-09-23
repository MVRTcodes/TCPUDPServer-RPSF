using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ServidorRPSF
{
    class Servidor
    {
        public static int MaxJugadores { get; private set; }
        public static int Puerto { get; private set; }
        public static Dictionary<int, Cliente> clientes = new Dictionary<int, Cliente>();
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener TCPListener;

        public static void Start(int _maxJugadores, int _puerto)
        {
            


            MaxJugadores = _maxJugadores;
            Puerto = _puerto;

            Console.WriteLine($"Iniciando servidor...");
            InicializarDatosServidor();

            TCPListener = new TcpListener(IPAddress.Any, Puerto);
            TCPListener.Start();
            TCPListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            
            Console.WriteLine($"Servidor iniciado en el puerto {Puerto}");
        }
        
        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _cliente = TCPListener.EndAcceptTcpClient(_result);
            TCPListener.BeginAcceptTcpClient(TCPConnectCallback, null);
            Console.WriteLine($"Conexión recibida desde {_cliente.Client.RemoteEndPoint}...");
            for (int i = 1; i <= MaxJugadores; i++)
            {
                if(clientes[i].tcp.socket == null)
                {
                    clientes[i].tcp.Connect(_cliente);
                    return;
                }
            }
            Console.WriteLine($"{_cliente.Client.RemoteEndPoint} no se ha podido conectar, el servidor esta lleno");

        }

        private static void InicializarDatosServidor()
        {
            for(int i = 1; i <= MaxJugadores; i++)
            {
                clientes.Add(i, new Cliente(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.BienvenidaRecibida },
                { (int)ClientPackets.ResultadosRonda, ServerHandle.ResultadosRonda }
            };
            Console.WriteLine("Packets inicializados.");
        }
    }
}

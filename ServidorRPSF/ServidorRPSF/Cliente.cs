using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ServidorRPSF
{
    class Cliente
    {
        public static int dataBufferSize = 4096;
        public int id;
        public Jugador jugador;
        public TCP tcp;
        
        public Cliente(int _clienteId)
        {
            id = _clienteId;
            tcp = new TCP(id);
        }
        public class TCP
        {
            public TcpClient socket;

            private readonly int id;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                id = _id;
            }
            
            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;
                
                stream = socket.GetStream();

                receivedData = new Packet();
                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                EnvioServidor.Bienvenido(id, "Bienvenido al Servidor");
            }

            public void EnviarDatos(Packet _packet)
            {
                try 
                { 
                    if(socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);

                    }
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {_ex}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        Servidor.clientes[id].Disconnect();
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    // handle data
                    receivedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception _ex)
                {
                    Console.WriteLine($"Error recibiendo los datos TCP: {_ex}");
                    Servidor.clientes[id].Disconnect(); 
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Servidor.packetHandlers[_packetId](id, _packet);
                        }

                    });

                    _packetLength = 0;

                    if (receivedData.UnreadLength() >= 4)
                    {
                        _packetLength = receivedData.ReadInt();
                        if (_packetLength <= 0)
                        {
                            return true;
                        }
                    }

                }

                if (_packetLength <= 1)
                {
                    return true;
                }

                return false;
            }
        
            public void Disconnect()
            {
                socket.Close();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }
    
        public void EnviaDentroJuego(string _jugadorNombre,int _PJ) {
            jugador = new Jugador(id, _jugadorNombre, _PJ);

            foreach (Cliente _cliente in Servidor.clientes.Values)
            {
                if(_cliente.jugador != null)
                {
                    if(_cliente.id != id)
                    {
                        EnvioServidor.SpawnJugador(id, _cliente.jugador);
                    }
                }
            }

            foreach (Cliente _cliente in Servidor.clientes.Values)
            {
                if (_cliente.jugador != null)
                {
                    EnvioServidor.SpawnJugador(_cliente.id, jugador);
                }
            }
        }
    
        private void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} se ha desconectado.");

            jugador = null;
            tcp.Disconnect();
        }
        
    }
}

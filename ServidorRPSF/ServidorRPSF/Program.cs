using System;
using System.Threading;

namespace ServidorRPSF
{
    class Program
    {
        private static bool isRunning = false;
        static void Main(string[] args)
        {
            Console.Title = "Servidor RPSF";
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            Servidor.Start(2, 24855);

            
        }

        private static void MainThread()
        {
            Console.WriteLine($"El MainThread ha comenzado. Corriendo en {Constantes.TICKS_POR_SEC} ticks por segundo.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while(_nextLoop< DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constantes.MS_POR_TICK);

                    if(_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}

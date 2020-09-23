using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServidorRPSF
{
    class GameLogic
    {
        public static void Update()
        {
            ThreadManager.UpdateMain();
        }
    }
}

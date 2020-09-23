using System;
using System.Collections.Generic;
using System.Text;

namespace ServidorRPSF
{
    class Jugador
    {
        public int id;
        public string username;

        public int PJ;
        public bool[] attacks;
        public int[] results;

        public Jugador(int _id, string _username, int _PJ) {
            id = _id;
            username = _username;
            PJ = _PJ;
            attacks = new bool[3];
            results = new int[3];
        }


        public void InstanciarResults(bool[] _attacks, int[] _results)
        {
            this.attacks = _attacks;
            this.results = _results;
            EnvioServidor.ResultadosRonda(this);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPFinal_Kiosco.Clases
{
    class Generador
    {
        private Random rnd;

        public Generador()
        {
            rnd = new Random();
        }


        public double generadorUniforme()
        {
            double random = Math.Round(rnd.NextDouble(),2);

            if (random == 0)
            {
                random = 0.01f;
                Math.Round(random, 2);
            }

            if(random == 1)
            {
                random = 0.99f;
                Math.Round(random, 2);
            }

            return Math.Round(random, 2);
        }


        public double generadorUniforme(double A, double B, double random)
        {
            return Math.Round(A + random * (B - A), 2);
        }

        public double generadorExpNeg(double media, double random)
        {
            return Math.Round(-media * Math.Log(1 - random), 2);
        }
    }
}

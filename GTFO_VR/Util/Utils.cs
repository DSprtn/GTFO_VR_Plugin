using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTFO_VR.Util
{
    class Utils
    {

        public static int LargestDivisor(int n)
        {
            if (n % 2 == 0)
            {
                return n / 2;
            }
            int sqrtn = (int)Math.Sqrt(n);
            for (int i = 3; i <= sqrtn; i += 2)
            {
                if (n % i == 0)
                {
                    return n / i;
                }
            }
            return 1;
        }
    }
}

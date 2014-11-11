using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pudvox {
    class Utils {
        public static T[][][] InitializeArray3D<T>(int d1, int d2, int d3, Func<T> initializer = null)
        {
            var arr = new T[d1][][];
            if (initializer == null)
            {
                for (int i = 0; i < d1; i++)
                {
                    arr[i] = new T[d2][];
                    for (int j = 0; j < d2; j++)
                    {
                        arr[i][j] = new T[d3];
                    }
                }
            }
            else
            {
                for (int i = 0; i < d1; i++) {
                    arr[i] = new T[d2][];
                    for (int j = 0; j < d2; j++) {
                        arr[i][j] = new T[d3];
                        for (int k = 0; k < d3; k++)
                        {
                            arr[i][j][k] = initializer();
                        }
                    }
                }
            }
            return arr;
        }
    }
}

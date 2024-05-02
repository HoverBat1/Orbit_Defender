using System.Collections.Generic;
using UnityEngine;








// ============================================================================================================
// ---------------------------------------------------------------------
namespace Other
{
    public static class Various
    {
        // Fisher-Yates shuffle algorithm
        public static void Shuffle<T>(List<T> _list)
        {
            int n = _list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n+1);
                T _VALUE = _list[k];
                _list[k] = _list[n];
                _list[n] = _VALUE;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxInterventionK_Subsets
{
    internal interface IAlgorithm
    {
        Solution Run();
        String PrintSolution();
        String PrintSummarySolution();
    }
}

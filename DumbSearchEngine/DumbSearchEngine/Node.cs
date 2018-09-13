using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbSearchEngine
{
    public class Node
    {
        public DocumentRef Document { get; set; }

        public double Score { get; set; }
    }
}

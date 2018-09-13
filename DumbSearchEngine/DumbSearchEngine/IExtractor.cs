using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbSearchEngine
{
    public interface IExtractor
    {
        Document Extract(string path);
    }
}

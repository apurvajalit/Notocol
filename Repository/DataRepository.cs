using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public abstract class DataRepository
    {

        //public abstract IConfigurationRepository GetConfigurationRepository(string connString, bool createContext = true);

        public abstract SourceRepository GetSourceRepository(bool createContext = true);
        public abstract TagRepository GetTagRepository(bool createContext = true);

    }
}

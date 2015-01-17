using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace Repository
{
    public class SourceRepository : BaseRepository
    {
        public Source SaveSource(Source objSource)
        {
            // Save Source in Database
            return objSource;
        }
    }
}

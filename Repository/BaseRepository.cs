using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Configuration;

namespace Repository
{
   
    public abstract  class BaseRepository
    {
        protected NotColEntities context;
        protected string connectionString;

        public void CreateDataContext(bool createContext = true)
        {
            if (createContext)
            {
                
            }
            this.connectionString = ConfigurationManager.ConnectionStrings["NotColEntities"].ToString();
        }

        public void SetDataContext(NotColEntities localContext)
        {
            this.context = localContext;
        }

        public NotColEntities GetDataContext()
        {
            this.context = new NotColEntities(connectionString);
            this.context.Configuration.LazyLoadingEnabled = false;
            this.context.Configuration.ProxyCreationEnabled = false;

            return this.context;

        }

        public void DisposeContext()
        {
            if (this.context != null)
            {

                this.context.Dispose();
                this.context = null;
            }
        }


    }
}

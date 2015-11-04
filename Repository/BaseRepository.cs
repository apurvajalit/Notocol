using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using System.Configuration;
using System.Data;
using System.ComponentModel;

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


        public static DataTable ConvertToDataTable<T>(IList<T> list)
        {
            PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                Type propType = propertyDescriptor.PropertyType;
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    table.Columns.Add(propertyDescriptor.Name, Nullable.GetUnderlyingType(propType));
                }
                else
                {
                    table.Columns.Add(propertyDescriptor.Name, propType);
                }
            }
            object[] values = new object[propertyDescriptorCollection.Count];
            foreach (T listItem in list)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = propertyDescriptorCollection[i].GetValue(listItem);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supermecado_DyM
{
    public class Conexion
    {
        public SqlConnection LeerCadena()
        {
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString);

            if (Conn.State == ConnectionState.Open)
            {
                Conn.Close();

            }else
            {
                Conn.Open();
            }

            return Conn;
        }

    }
}

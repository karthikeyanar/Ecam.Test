using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace Ecam.Views {
    public class MySqlConfiguration:DbConfiguration {
        public MySqlConfiguration() {
            SetHistoryContext("MySql.Data.MySqlClient",(conn,schema) => new MySqlHistoryContext(conn,schema));
        }
    }
}
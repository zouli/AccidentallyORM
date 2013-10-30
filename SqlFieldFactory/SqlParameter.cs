using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using AccidentallyORM.DBHelper;

namespace AccidentallyORM.SqlFieldFactory
{
    public class SqlParameter : List<DbParameter>
    {
        private static int _serialNumber;

        public void Add(string field, DbType dbtype, object value)
        {
            Add(DbService.CreateParameter(field, dbtype, value));
        }

        public static string GetParameterName(string field)
        {
            return "@" + field + (_serialNumber == int.MaxValue ? 0 : _serialNumber++);
        }
    }
}

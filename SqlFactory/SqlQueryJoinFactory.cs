using System.Text;
using AccidentallyORM.Entity;
using AccidentallyORM.SqlFieldFactory;

namespace AccidentallyORM.SqlFactory
{
    public partial class SqlQueryFactory<T>
    {
        private readonly StringBuilder _sqlJoinOn = new StringBuilder();

        public SqlQueryFactory<T> LeftJoin<TJoin>(SqlQueryFactory<TJoin> queryJoin) where TJoin : EntityBase, new()
        {
            var joinTableName = queryJoin.TableName;

            _sqlJoin.Append(" LEFT JOIN ");
            _sqlJoin.Append(joinTableName);


            _sqlFields.Append("," + string.Join(",", EntityHelper.GetFieldNames(queryJoin.SqlFields).ToArray()));
            return this;
        }

        public SqlQueryFactory<T> On<TOn>(SqlField<TOn> sqlField) where TOn : EntityBase, new()
        {
            _sqlJoinOn.Append(_sqlJoinOn.Length > 0 ? " AND " : " ON ");
            _sqlJoinOn.Append(sqlField.ToString());

            return this;
        }
    }
}

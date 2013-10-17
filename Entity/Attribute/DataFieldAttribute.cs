using System.Data;

namespace AccidentallyORM.Entity.Attribute
{
    public class DataFieldAttribute : System.Attribute
    {
        public DataFieldAttribute()
        {
            FieldName = string.Empty;
            ColumnType = DbType.Object;
            PrimaryKey = false;
            Identity = false;
            CanNull = false;
            Default = string.Empty;
            ConvertFormat = string.Empty;
        }

        public string FieldName { get; set; }

        public DbType ColumnType { get; set; }

        public bool PrimaryKey { get; set; }

        public bool Identity { get; set; }

        public bool CanNull { get; set; }

        public string Default { get; set; }

        public string ConvertFormat { get; set; }
    }
}
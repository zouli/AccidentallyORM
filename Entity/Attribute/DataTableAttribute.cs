namespace AccidentallyORM.Entity.Attribute
{
    public class DataTableAttribute : System.Attribute
    {
        public DataTableAttribute()
        {
            TableName = string.Empty;
            FieldPrefix = string.Empty;
        }

        public string TableName { get; set; }

        public string FieldPrefix { get; set; }
    }
}
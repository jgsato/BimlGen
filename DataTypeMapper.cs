using System;
using System.Data;
using Microsoft.SqlServer.Management.Smo;

namespace BimlGen
{
	/// <summary>Maps SMO data types to the corresponding BIML data type</summary>
	public static class DataTypeMapper
	{
		public static string GetClrType( this DataType sqlType )
		{
			var typeString = sqlType.ToString().ToLower();
			switch( typeString )
			{
				case "bigint":
					return typeof( long ).ToString();

				case "binary":
				case "image":
				case "timestamp":
				case "varbinary":
					return "Binary";

				case "bit":
					return typeof( bool ).ToString();

				case "char":
				case "varchar":
					return "AnsiString";
				case "nchar":
				case "ntext":
				case "nvarchar":
				case "text":
				case "xml":
				case "sysname":
					return typeof( string ).ToString();

				case "datetime":
				case "smalldatetime":
				case "date":
				case "time":
				case "datetime2":
					return typeof( DateTime ).ToString();

				case "decimal":
				case "money":
				case "smallmoney":
					return typeof( decimal ).ToString();

				case "float":
					return typeof( double ).ToString();

				case "int":
					return typeof( int ).ToString();

				case "real":
					return typeof( float ).ToString();

				case "uniqueidentifier":
					return typeof( Guid ).ToString();

				case "smallint":
					return typeof( short ).ToString();

				case "tinyint":
					return typeof( byte ).ToString();

				case "variant":
				case "udt":
					return typeof( object ).ToString();

				case "structured":
					return typeof( DataTable ).ToString();

				case "datetimeoffset":
					return typeof( DateTimeOffset ).ToString();

				default:
					return typeof( object ).ToString();
			}
		}
	}
}

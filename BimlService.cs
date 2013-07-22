using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.Smo;

namespace BimlGen
{
	/// <summary>Methods to retrieve SMO data and transform it into a POCO representation of BIML</summary>
	public class BimlService
	{
		public Connections GetConnections( Server server, Microsoft.SqlServer.Management.Smo.Database database )
		{
			var sqlConnection = new Connection
				{
					Name = "DbConnection",
					ConnectionString =
						string.Format( "Server={0};Initial Catalog={1};Integrated Security=SSPI;", server.Name, database.Name )
				};

			return new Connections
				{
					Connection = sqlConnection
				};
		}

		public List<Database> GetDatabases( Microsoft.SqlServer.Management.Smo.Database database )
		{
			return new List<Database>
				{
					new Database
						{
							ConnectionName = "DbConnection",
							Name = database.Name,
						}
				};
		}

		public List<Schema> GetSchemas( Microsoft.SqlServer.Management.Smo.Database database )
		{
			var schemas = new List<Schema>();
			foreach (Microsoft.SqlServer.Management.Smo.Schema schema in database.Schemas)
			{
				var bimlSchema = new Schema
					{
						DatabaseName = database.Name,
						Name = schema.Name,
						Owner = schema.Owner,
					};

				var annotations = GetAnnotations( schema.ExtendedProperties );
				if (annotations.Any())
					bimlSchema.Annotations = annotations;

				schemas.Add( bimlSchema );
			}
			return schemas;
		}

		public List<Table> GetTables( Microsoft.SqlServer.Management.Smo.Database database, bool splitFactsAndDimensions )
		{
			var tables = new List<Table>();
			foreach (Microsoft.SqlServer.Management.Smo.Table table in database.Tables)
			{
				if (splitFactsAndDimensions)
				{
					var tableName = table.Name.ToLower();
					if (tableName.StartsWith( "dim" ) || tableName.StartsWith( "fact" ))
						continue;
				}

				var bimlTable = new Table
					{
						Name = table.Name,
						SchemaName = database.Name + "." + table.Schema,
						Columns = GetColumns( table ),
						Indexes = GetIndexes( table ),
						Keys = GetKeys( table ),
					};

				var annotations = GetAnnotations( table.ExtendedProperties );
				if (annotations.Any())
					bimlTable.Annotations = annotations;

				tables.Add( bimlTable );
			}
			return tables;
		}

		public List<Fact> GetFacts( Microsoft.SqlServer.Management.Smo.Database database )
		{
			var facts = new List<Fact>();
			foreach( Microsoft.SqlServer.Management.Smo.Table table in database.Tables )
			{
				var tableName = table.Name.ToLower();
				if( !tableName.StartsWith( "fact" ) )
					continue;

				var bimlTable = new Fact
				{
					Name = table.Name,
					SchemaName = database.Name + "." + table.Schema,
					Columns = GetColumns( table ),
					Indexes = GetIndexes( table ),
					Keys = GetKeys( table ),
				};

				var annotations = GetAnnotations( table.ExtendedProperties );
				if( annotations.Any() )
					bimlTable.Annotations = annotations;

				facts.Add( bimlTable );
			}
			return facts;
		}

		public List<Dimension> GetDimensions( Microsoft.SqlServer.Management.Smo.Database database )
		{
			var dimensions = new List<Dimension>();
			foreach( Microsoft.SqlServer.Management.Smo.Table table in database.Tables )
			{
				var tableName = table.Name.ToLower();
				if( !tableName.StartsWith( "dim" ) )
					continue;

				var bimlTable = new Dimension
				{
					Name = table.Name,
					SchemaName = database.Name + "." + table.Schema,
					Columns = GetColumns( table ),
					Indexes = GetIndexes( table ),
					Keys = GetKeys( table ),
				};

				var annotations = GetAnnotations( table.ExtendedProperties );
				if( annotations.Any() )
					bimlTable.Annotations = annotations;

				dimensions.Add( bimlTable );
			}
			return dimensions;
		}

		public List<Column> GetColumns( Microsoft.SqlServer.Management.Smo.Table table )
		{
			var columns = new List<Column>();
			foreach (Microsoft.SqlServer.Management.Smo.Column column in table.Columns)
			{
				var dataType = column.DataType.GetClrType().Replace( "System.", "" );
				var bimlColumn = new Column
					{
						Name = column.Name,
						DataType = dataType,
					};

				if (column.Nullable)
					bimlColumn.IsNullable = true.ToString().ToLower();

				bool dataTypeHasLength = ( dataType.Equals( "string", StringComparison.InvariantCultureIgnoreCase )
				                           || dataType.Equals( "ansistring", StringComparison.InvariantCultureIgnoreCase )
				                           || dataType.Equals( "binary", StringComparison.CurrentCultureIgnoreCase ) );
				if (dataTypeHasLength)
					bimlColumn.Length = column.DataType.MaximumLength.ToString();

				var annotations = GetAnnotations( column.ExtendedProperties );
				if (annotations.Any())
					bimlColumn.Annotations = annotations;

				columns.Add( bimlColumn );
			}
			return columns;
		}

		public List<Annotation> GetAnnotations( ExtendedPropertyCollection props )
		{
			var list = new List<Annotation>();
			foreach (ExtendedProperty prop in props)
			{
				list.Add( new Annotation
					{
						AnnotationType = "Tag",
						Tag = prop.Name,
						Value = prop.Value.ToString()
					} );
			}
			return list;
		}

		public List<Index> GetIndexes( Microsoft.SqlServer.Management.Smo.Table table )
		{
			var indexes = new List<Index>();
			foreach (Microsoft.SqlServer.Management.Smo.Index index in table.Indexes)
			{
				var indexType = index.IndexKeyType;

				bool isRegularIndex = indexType != IndexKeyType.DriPrimaryKey && indexType != IndexKeyType.DriUniqueKey;
				if (isRegularIndex)
				{
					var columns = index.IndexedColumns.Cast<IndexedColumn>()
					                   .Select( indexedColumn => new Column {ColumnName = indexedColumn.Name} )
					                   .ToList();

					var bimlIndex = new Index
						{
							Name = index.Name,
							Columns = columns,
						};

					indexes.Add( bimlIndex );
				}
			}
			return indexes;
		}

		public Keys GetKeys( Microsoft.SqlServer.Management.Smo.Table table )
		{
			var keys = new Keys();
			foreach (Microsoft.SqlServer.Management.Smo.Index index in table.Indexes)
			{
				var indexType = index.IndexKeyType;

				bool isRegularIndex = indexType != IndexKeyType.DriPrimaryKey && indexType != IndexKeyType.DriUniqueKey;
				if (isRegularIndex)
					continue;

				var columns = index.IndexedColumns.Cast<IndexedColumn>()
				                   .Select( indexedColumn => new Column {ColumnName = indexedColumn.Name} )
				                   .ToList();

				if (indexType == IndexKeyType.DriPrimaryKey)
				{
					keys.PrimaryKey = new PrimaryKey
						{
							Name = index.Name,
							Columns = columns,
						};
					continue;
				}

				if (indexType == IndexKeyType.DriUniqueKey)
				{
					keys.UniqueKey = new UniqueKey
						{
							Name = index.Name,
							Columns = columns,
						};
				}
			}
			return keys;
		}
	}
}

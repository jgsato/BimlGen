using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BimlGen
{
	/// <summary>Hand-crafted POCO representation of BIML</summary>
	[XmlRoot( ElementName = "Biml", Namespace = "http://schemas.varigence.com/biml.xsd" )]
	[Serializable]
	public class Biml
	{
		[XmlElement]
		public Connections Connections { get; set; }

		[XmlArray]
		public List<Database> Databases { get; set; } 

		[XmlArray]
		public List<Schema> Schemas { get; set; } 

		[XmlArray]
		public List<Table> Tables { get; set; }

		[XmlArray]
		public List<Fact> Facts { get; set; }

		[XmlArray]
		public List<Dimension> Dimensions { get; set; }
	}

	[Serializable]
	public class Table
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string SchemaName { get; set; }

		[XmlArray]
		public List<Column> Columns { get; set; }

		[XmlArray]
		public List<Annotation> Annotations { get; set; }

		[XmlArray]
		public List<Index> Indexes { get; set; }

		[XmlElement]
		public Keys Keys { get; set; }
	}

	[Serializable]
	public class Fact
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string SchemaName { get; set; }

		[XmlArray]
		public List<Column> Columns { get; set; }

		[XmlArray]
		public List<Annotation> Annotations { get; set; }

		[XmlArray]
		public List<Index> Indexes { get; set; }

		[XmlElement]
		public Keys Keys { get; set; }
	}

	[Serializable]
	public class Dimension
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string SchemaName { get; set; }

		[XmlArray]
		public List<Column> Columns { get; set; }

		[XmlArray]
		public List<Annotation> Annotations { get; set; }

		[XmlArray]
		public List<Index> Indexes { get; set; }

		[XmlElement]
		public Keys Keys { get; set; }
	}

	[Serializable]
	public class Column
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string ColumnName { get; set; }

		[XmlAttribute]
		public string DataType { get; set; }

		[XmlAttribute]
		public string Length { get; set; }

		[XmlAttribute]
		public string IsNullable { get; set; }

		[XmlArray]
		public List<Annotation> Annotations { get; set; }
	}

	[Serializable]
	public class Annotation
	{
		[XmlAttribute]
		public string AnnotationType { get; set; }

		[XmlAttribute]
		public string Tag { get; set; }

		[XmlText]
		public string Value { get; set; }
	}

	[Serializable]
	public class Index
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlArray]
		public List<Column> Columns { get; set; }

		[XmlArray]
		public List<Annotation> Annotations { get; set; } 
	}

	[Serializable]
	public class Keys
	{
		[XmlElement]
		public PrimaryKey PrimaryKey { get; set; }
		
		[XmlElement]
		public UniqueKey UniqueKey { get; set; }
	}

	[Serializable]
	public class PrimaryKey
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlArray]
		public List<Column> Columns { get; set; }

		[XmlArray]
		public List<Annotation> Annotations { get; set; }
	}

	[Serializable]
	public class UniqueKey
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlArray]
		public List<Column> Columns { get; set; }

		[XmlArray]
		public List<Annotation> Annotations { get; set; }
	}

	[Serializable]
	public class Schema
	{
		[XmlAttribute]
		public string DatabaseName { get; set; }

		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string Owner { get; set; }

		[XmlArray]
		public List<Annotation> Annotations { get; set; } 
	}

	[Serializable]
	public class Database
	{
		[XmlAttribute]
		public string ConnectionName { get; set; }

		[XmlAttribute]
		public string Name { get; set; }
	}

	[Serializable]
	public class Connections
	{
		[XmlElement]
		public Connection Connection { get; set; }
	}

	[Serializable]
	public class Connection
	{
		[XmlAttribute]
		public string Name { get; set; }

		[XmlAttribute]
		public string ConnectionString { get; set; }
	}
}

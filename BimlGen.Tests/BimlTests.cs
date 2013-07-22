using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using FluentAssertions;
using NUnit.Framework;

namespace BimlGen.Tests
{
	[TestFixture]
	public class BimlTests
	{
		[Test]
		public void PocoCanBeSerialized()
		{
			// Given a class representation of BIML
			var biml = new Biml();
			biml.Tables = new List<Table>
				{
					new Table
						{
							Name = "Table1",
							Columns = new List<Column>
								{
									new Column
										{
											Name = "Column1",
											DataType = "string",
											Length = "11",
											Annotations =
												new List<Annotation>
													{
														new Annotation
															{
																AnnotationType = "Tag",
																Tag = "col1",
																Value = "abc"
															},
														new Annotation
															{
																AnnotationType = "Tag",
																Tag = "col2",
																Value = "xyz"
															}
													}
										},
									new Column
										{
											Name = "Column2",
											DataType = "string",
											Length = "12"
										}
								},
							Annotations = new List<Annotation>
								{
									new Annotation {AnnotationType = "Description", Tag = "tag1", Value = "TableAnnotation1"},
									new Annotation {AnnotationType = "Tag", Tag = "tag2", Value = "TableAnnotation2"},
								},
							Indexes = new List<Index>
								{
									new Index {Name = "IX_Table1"}
								},
							Keys = new Keys
								{
									PrimaryKey = new PrimaryKey
										{
											Name = "PK_Table1"
										},
									UniqueKey = new UniqueKey
										{
											Name = "UK_Table1_2",
											Columns = new List<Column>
												{
													new Column {ColumnName = "Column1"}
												}
										}
								}
						}
				};

			// When the class is serialized
			string output = "";
			var serializer = new XmlSerializer( biml.GetType() );
			using( var writer = new StringWriter() )
			{
				serializer.Serialize( writer, biml );
				output = writer.ToString();
			}

			// Then valid XML is generated
			output.Should().NotBeNullOrEmpty();
			output.Should().Contain( "Biml" );
		}

		[Test]
		[Explicit]
		public void SmoRetrievesData()
		{
			// Given a database connection
			var request = new BimlRequest
				{
					ServerName = "SERVERNAME",
					DatabaseName = "AdventureWorks2008R2",
					HasConnections = true
				};

			// When the BIML is generated
			var biml = BimlGenerator.GetBiml( request );

			// Then valid XML is generated
			biml.Should().NotBeNullOrEmpty();
		}
	}
}

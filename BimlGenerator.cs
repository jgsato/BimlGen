using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.SqlServer.Management.Smo;

namespace BimlGen
{
	public class BimlGenerator
	{
		public static string GetBiml( string serverName, string databaseName )
		{
			var biml = BuildBiml( serverName, databaseName );

			var serializer = new XmlSerializer( biml.GetType() );
			using (var writer = new StringWriter())
			{
				serializer.Serialize( writer, biml );
				return writer.ToString();
			}
		}

		public static Biml BuildBiml( string serverName, string databaseName )
		{
			// Configure SQL SMO
			var server = new Server( serverName );
			var scriptingOptions = new ScriptingOptions { Encoding = Encoding.UTF8 };
			server.Script( scriptingOptions );
			var database = new Microsoft.SqlServer.Management.Smo.Database( server, databaseName );
			database.Refresh();

			var bimlService = new BimlService();
			return new Biml
			{
				Connections = bimlService.GetConnections( server, database ),
				Databases = bimlService.GetDatabases( database ),
				Schemas = bimlService.GetSchemas( database ),
				Tables = bimlService.GetTables( database ),
			};
		}

		public static bool IsValidBiml( string xml )
		{
			// Set validation rules
			var settings = new XmlReaderSettings
				{
					CloseInput = true,
					ValidationType = ValidationType.Schema,
					ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings |
					                  XmlSchemaValidationFlags.ProcessIdentityConstraints |
					                  XmlSchemaValidationFlags.ProcessInlineSchema |
					                  XmlSchemaValidationFlags.ProcessSchemaLocation
				};

			// Get xsd as embedded resource
			Assembly myAssembly = Assembly.GetExecutingAssembly();
			using (Stream schemaStream = myAssembly.GetManifestResourceStream( "BimlGen.Resources.Biml.xsd" ))
			{
				using (XmlReader schemaReader = XmlReader.Create( schemaStream ))
				{
					settings.Schemas.Add( null, schemaReader );
				}
			}
			
			// Read through document to validate
			bool isValid = true;
			var stringReader = new StringReader( xml );
			using (XmlReader validatingReader = XmlReader.Create( stringReader, settings ))
			{
				try
				{
					while (validatingReader.Read()) {}
				}
				catch
				{
					isValid = false;
				}
			}
			return isValid;
		}
	}
}

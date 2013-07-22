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
		public static string GetBiml( BimlRequest request )
		{
			var biml = BuildBiml( request );

			var serializer = new XmlSerializer( biml.GetType() );
			using (var writer = new StringWriter())
			{
				serializer.Serialize( writer, biml );
				return writer.ToString();
			}
		}

		public static Biml BuildBiml( BimlRequest request )
		{
			// Configure SQL SMO
			var server = new Server( request.ServerName );
			var scriptingOptions = new ScriptingOptions { Encoding = Encoding.UTF8 };
			server.Script( scriptingOptions );
			var database = new Microsoft.SqlServer.Management.Smo.Database( server, request.DatabaseName );
			database.Refresh();

			var bimlService = new BimlService();
			var output = new Biml();
			
			// Selectively build sections
			if ( request.HasConnections )
				output.Connections = bimlService.GetConnections( server, database );
			if ( request.HasDatabases )
				output.Databases = bimlService.GetDatabases( database );
			if( request.HasSchemas )
				output.Schemas = bimlService.GetSchemas( database );
			if( request.HasTables )
				output.Tables = bimlService.GetTables( database );

			return output;
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

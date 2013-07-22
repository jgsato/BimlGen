namespace BimlGen
{
	public class BimlRequest
	{
		public string ServerName { get; set; }
		public string DatabaseName { get; set; }
		public string OutputFolder { get; set; }
		public bool HasConnections { get; set; }
		public bool HasDatabases { get; set; }
		public bool HasSchemas { get; set; }
		public bool HasTables { get; set; }
	}
}
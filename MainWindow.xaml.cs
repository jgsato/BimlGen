using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BimlGen
{
	/// <summary>Interaction logic for MainWindow.xaml</summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();

			OutputFolder.Text = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
		}

		private void Submit_Click( object sender, RoutedEventArgs e )
		{
			ToggleSubmit( Submit );

			string serverName = ServerName.Text;
			string databaseName = DatabaseName.Text;
			string outputFolder = OutputFolder.Text;

			if (string.IsNullOrWhiteSpace( serverName )
				|| string.IsNullOrWhiteSpace( databaseName )
				|| string.IsNullOrWhiteSpace( outputFolder ))
			{
				throw new Exception( "Please complete all form fields" );
			}

			var result = Task.Run( () => GetBiml( serverName, databaseName, outputFolder ) );
			result.ContinueWith( t => OpenExplorer( outputFolder ) );
			result.ContinueWith( t => ToggleSubmit( Submit ),
			                     CancellationToken.None,
			                     TaskContinuationOptions.None,
			                     TaskScheduler.FromCurrentSynchronizationContext() );
		}

		private void GetBiml(string serverName, string databaseName, string outputFolder)
		{
			var biml = BimlGenerator.GetBiml( serverName, databaseName );
			if( !string.IsNullOrEmpty( biml ) )
			{
				var now = DateTime.Now;
				var timestamp = "" + now.Hour + now.Minute + now.Second;
				File.WriteAllText( string.Format("{0}\\{1}{2}.biml", outputFolder, databaseName, timestamp), biml );
			}
		}

		private void OpenExplorer( string outputFolder )
		{
			Process.Start( outputFolder );
		}

		private void ToggleSubmit( Button button )
		{
			button.IsEnabled = !button.IsEnabled;
			button.Content = button.IsEnabled ? "Generate" : "Processing...";
		}
	}
}

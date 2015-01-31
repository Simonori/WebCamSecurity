using System;
using System.Net;
using System.IO;
using System.Threading;

namespace Uploader
{


	public class FTPClient
	{
		private string Site;
		private string User;
		private string Pwd;
		public Thread Client;

		public FTPClient (string Name, string SourceAddress, string FTPSite, string FTPUser, string FTPPwd)
		{
			Site = FTPSite;
			User = FTPUser;
			Pwd = FTPPwd;
			Client = new Thread(new ParameterizedThreadStart(Runup));
			Client.Name = Name;
			Client.Start(SourceAddress);
		}
		private void Runup( object Address )
		{
			try
			{
				bool m_Tell = true;
				string DirectorySet = Address.ToString();
				FileSystemWatcher DUI = new FileSystemWatcher(DirectorySet);
				DirectoryInfo WUI = new DirectoryInfo(DirectorySet);
				
				Uri FTPURI = new Uri(Site);//"ftp://ftp.simonoriordan.com");
				string m_StrFTPUser = User;//"spy@simonoriordan.com";
				string m_StrFTPPassword = Pwd;//"recorded1";
				FtpWebRequest ftpwr = null;
				
				while(m_Tell == true)
				{
					DUI.WaitForChanged(WatcherChangeTypes.Created);
					
					ftpwr = (FtpWebRequest)WebRequest.Create(FTPURI + "Image" + WUI.GetFiles().Length.ToString() + ".jpg");
					ftpwr.Credentials = new NetworkCredential(m_StrFTPUser, m_StrFTPPassword);
					ftpwr.Method = WebRequestMethods.Ftp.UploadFile;
					ftpwr.UseBinary = true;
					//ftpwr.UsePassive = true;
					//ftpwr.KeepAlive = true;
					
					BinaryWriter writer = new BinaryWriter(ftpwr.GetRequestStream());	
		            writer.Write(File.ReadAllBytes(DirectorySet + "/Image" + WUI.GetFiles().Length.ToString() + ".jpg"));
					writer.Flush();
					writer.Close();
					writer = null;
					
					/*FileStream stream = new FileStream(DirectorySet + "/Image" + WUI.GetFiles().Length.ToString() + ".jpg", FileMode.Open);
					Byte[] buffer = new Byte[stream.Length];
					stream.Read(buffer, 0, buffer.Length);
					stream.Close();
					Stream reqStream = ftpwr.GetRequestStream();
					reqStream.Write(buffer, 0, buffer.Length);
					reqStream.Close();
					*/
					
					
					
						
					
				}
				
			
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
	
		}
	}
}

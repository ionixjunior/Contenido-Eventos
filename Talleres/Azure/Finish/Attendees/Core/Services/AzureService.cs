using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Core.Models;
using Core.Plugins;

namespace Core.Services
{
	public class AzureService
	{
		private static AzureService _instance;
		public static AzureService Instance => _instance ?? (_instance = new AzureService());

		private AzureService() {}

		public async Task<IList<AttendeeModel>> GetAttendees()
		{
			return await AzureMobilePlugin.Instance.GetAttendees();
		}

		public async Task Save(AttendeeModel attendeeModel)
		{
			await AzureMobilePlugin.Instance.Save(attendeeModel);
		}

		public async Task Delete(AttendeeModel attendeeModel)
		{
			await AzureMobilePlugin.Instance.Delete(attendeeModel);
		}

		public async Task SavePhoto(Stream stream, string name)
		{
			await AzureStoragePlugin.Instance.UploadFile(stream, name);
		}

		public async Task<byte[]> LoadPhoto(string name)
		{
			return await AzureStoragePlugin.Instance.DownloadFile(name);
		}

		public async Task DeletePhoto(string name)
		{
			await AzureStoragePlugin.Instance.DeleteFile(name);
		}
	}
}

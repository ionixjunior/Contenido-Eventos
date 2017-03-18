using System;
using System.Collections.Generic;
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
	}
}

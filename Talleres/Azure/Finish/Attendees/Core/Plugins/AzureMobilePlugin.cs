using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

namespace Core.Plugins
{
	public class AzureMobilePlugin
	{
		private static AzureMobilePlugin _instance;
		public static AzureMobilePlugin Instance => _instance ?? (_instance = new AzureMobilePlugin());

		private MobileServiceClient _client;
		private IMobileServiceSyncTable<AttendeeModel> _attendee;

		private AzureMobilePlugin()
		{
			var store = new MobileServiceSQLiteStore(AppConfig.DatabaseName);
			store.DefineTable<AttendeeModel>();

			_client = new MobileServiceClient(AppConfig.MobileAppUri);
			_client.SyncContext.InitializeAsync(store);

			_attendee = _client.GetSyncTable<AttendeeModel>();
		}

		private async Task Synchronize()
		{
			await _client.SyncContext.PushAsync();
			await _attendee.PullAsync("attendees", _attendee.CreateQuery());
		}

		public async Task<IList<AttendeeModel>> GetAttendees()
		{
			await Synchronize();
			return await _attendee.ToListAsync();
		}

		public async Task Save(AttendeeModel attendeeModel)
		{
			if (string.IsNullOrEmpty(attendeeModel.Id))
				await _attendee.InsertAsync(attendeeModel);
			else
				await _attendee.UpdateAsync(attendeeModel);

			await _client.SyncContext.PushAsync();
		}

		public async Task Delete(AttendeeModel attendeeModel)
		{
			await _attendee.DeleteAsync(attendeeModel);
			await _client.SyncContext.PushAsync();
		}
	}
}

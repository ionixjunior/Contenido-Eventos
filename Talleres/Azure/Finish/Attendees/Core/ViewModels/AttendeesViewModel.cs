using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Helpers;
using Core.Models;
using Core.Services;
using Xamarin.Forms;

namespace Core.ViewModels
{
	public class AttendeesViewModel : BaseViewModel
	{
		private ObservableCollection<AttendeeModel> _attendees;
		public ObservableCollection<AttendeeModel> Attendees
		{
			get { return _attendees; }
			set
			{
				_attendees = value;
				OnPropertyChanged();
			}
		}

		public ICommand LoadAttendeesCommand 
			=> new Command(async () => await LoadAttendees());

		public ICommand OpenProfileCommand 
			=> new Command<AttendeeModel>(async (attendeeModel) => await OpenProfile(attendeeModel));

		public ICommand AddProfileCommand
			=> new Command(async () => await AddProfile());

		public async Task OnAppearing()
		{
			await LoadAttendees();
		}

		public async Task LoadAttendees()
		{
			if (IsBusy)
				return;
			
			try
			{
				IsBusy = true;
				Attendees = new ObservableCollection<AttendeeModel>(
					await AzureService.Instance.GetAttendees()
				);
			}
			catch (Exception e)
			{
				LogHelper.Instance.AddLog(e);
			}
			finally
			{
				IsBusy = false;
			}
		}

		private async Task OpenProfile(AttendeeModel attendeeModel)
		{
			await NavigationHelper.Instance.GotoProfile(attendeeModel);
		}

		private async Task AddProfile()
		{
			await NavigationHelper.Instance.GotoProfile(new AttendeeModel());
		}
	}
}

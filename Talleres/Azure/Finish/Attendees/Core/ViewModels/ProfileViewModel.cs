using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Helpers;
using Core.Models;
using Core.Services;
using Xamarin.Forms;

namespace Core.ViewModels
{
	public class ProfileViewModel : BaseViewModel
	{
		private AttendeeModel _attendee;
		public AttendeeModel Attendee
		{
			get { return _attendee; }
			set
			{
				_attendee = value;
				OnPropertyChanged();
			}
		}

		private bool _canDelete;
		public bool CanDelete
		{
			get { return _canDelete; }
			set
			{
				_canDelete = value;
				OnPropertyChanged();
			}
		}

		public ICommand SaveCommand 
			=> new Command(async () => await Save());

		public ICommand DeleteCommand => new Command(async () => await Delete());

		public ProfileViewModel(AttendeeModel attendeeModel)
		{
			Attendee = attendeeModel;

			if (string.IsNullOrEmpty(Attendee.Id))
				CanDelete = false;
			else
				CanDelete = true;
		}

		private async Task Save()
		{
			if (IsBusy)
				return;

			Exception exception = null;

			try
			{
				IsBusy = true;

				if (string.IsNullOrEmpty(Attendee.Name))
					throw new Exception("O nome deve ser preenchido");

				if (string.IsNullOrEmpty(Attendee.Email))
					throw new Exception("O e-mail deve ser preenchido");

				await AzureService.Instance.Save(Attendee);
			}
			catch (Exception e)
			{
				exception = e;
				LogHelper.Instance.AddLog(e);
			}
			finally
			{
				IsBusy = false;
			}

			if (exception != null)
			{
				await MessageHelper.Instance.ShowMessage(
					"Ocorreu um erro",
					exception.Message,
					"Ok"
				);
				return;
			}

			await NavigationHelper.Instance.GoBack();
		}

		private async Task Delete()
		{
			if (IsBusy)
				return;

			var delete = await MessageHelper.Instance.ShowAsk(
				"Apagar perfil",
				"Tem certeza que quer apagar o perfil?",
				"Sim",
				"Não"
			);

			if (delete == false)
				return;

			Exception exception = null;

			try
			{
				IsBusy = true;
				await AzureService.Instance.Delete(Attendee);
			}
			catch (Exception e)
			{
				exception = e;
				LogHelper.Instance.AddLog(e);
			}
			finally
			{
				IsBusy = false;
			}

			if (exception != null)
			{
				await MessageHelper.Instance.ShowMessage(
					"Ocorreu um erro",
					exception.Message,
					"Ok"
				);
				return;
			}

			await NavigationHelper.Instance.GoBack();
		}
	}
}

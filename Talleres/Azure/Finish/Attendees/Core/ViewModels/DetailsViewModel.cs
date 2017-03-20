using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Helpers;
using Core.Models;
using Core.Plugins;
using Core.Services;
using Xamarin.Forms;

namespace Core.ViewModels
{
	public class DetailsViewModel : BaseViewModel
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

		private object _photo;
		public object Photo
		{
			get { return _photo; }
			set
			{
				_photo = value;
				OnPropertyChanged();
			}
		}

		private bool _isLoadingPhoto;
		public bool IsLoadingPhoto
		{
			get { return _isLoadingPhoto; }
			set
			{
				_isLoadingPhoto = value;
				OnPropertyChanged();
			}
		}

		private Stream _photoStream;

		public ICommand SaveCommand 
			=> new Command(async () => await Save());

		public ICommand DeleteCommand 
			=> new Command(async () => await Delete());

		public ICommand ChangePhotoCommand 
			=> new Command(async () => await ChangePhoto());

		public ICommand LoadPhotoCommand
			=> new Command(async () => await LoadPhoto());

		public DetailsViewModel(AttendeeModel attendeeModel)
		{
			Attendee = attendeeModel;
			LoadDefaultPhoto();

			if (string.IsNullOrEmpty(Attendee.Id))
				CanDelete = false;
			else
				CanDelete = true;

			LoadPhotoCommand.Execute(null);
		}

		public void LoadDefaultPhoto()
		{
			Photo = "profile.png";
			_photoStream = null;
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
				
				if (_photoStream != null)
				{
					if (string.IsNullOrEmpty(Attendee.PhotoName))
						Attendee.PhotoName = Guid.NewGuid().ToString();
					
					await ServerService.Instance.SavePhoto(_photoStream, Attendee.PhotoName);
				}

				await ServerService.Instance.Save(Attendee);
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

				if (string.IsNullOrEmpty(Attendee.PhotoName) == false)
					await ServerService.Instance.DeletePhoto(Attendee.PhotoName);

				await ServerService.Instance.Delete(Attendee);
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

		private async Task ChangePhoto()
		{
			var textoTirarFoto = "Tirar foto";
			var textoAbrirGaleria = "Abrir galeria";
			var textoCancelar = "Cancelar";
			var textoApagar = "Apagar";

			var actions = new string[] { textoTirarFoto, textoAbrirGaleria };

			var response = await MessageHelper.Instance.ShowOptions(
				"O que deseja fazer com sua imagem?",
				textoCancelar,
				textoApagar,
				actions
			);

			if (response == textoCancelar)
				return;

			Exception exception = null;

			try
			{
				if (response == textoAbrirGaleria)
				{
					if (await MediaPlugin.Instance.IsPickPhotoSupported() == false)
						throw new Exception("Não foi possível abrir a galeria de imagens");

					var file = await MediaPlugin.Instance.PickPhotoAsync();
					if (file != null)
					{
						Photo = ImageSource.FromFile(file.Path);
						_photoStream = file.GetStream();
					}
				}

				if (response == textoTirarFoto)
				{
					if (await MediaPlugin.Instance.IsCameraAvailable() == false)
						throw new Exception("Parece que seu dispositivo não possui câmera ou não foi possível acessá-la.");

					var file = await MediaPlugin.Instance.TakePhotoAsync();
					if (file != null)
					{
						Photo = ImageSource.FromFile(file.Path);
						_photoStream = file.GetStream();
					}
				}

				if (response == textoApagar)
				{
					await ServerService.Instance.DeletePhoto(Attendee.PhotoName);
					Attendee.PhotoName = null;
					LoadDefaultPhoto();
				}
			}
			catch (Exception e)
			{
				exception = e;
				LogHelper.Instance.AddLog(e);
			}

			if (exception != null)
			{
				await MessageHelper.Instance.ShowMessage(
					"Algo está errado",
					exception.Message,
					"Ok"
				);
				return;
			}
		}

		private async Task LoadPhoto()
		{
			if (IsLoadingPhoto)
				return;

			if (string.IsNullOrEmpty(Attendee.PhotoName))
				return;
			
			try
			{
				IsLoadingPhoto = true;
				var bytes = await ServerService.Instance.LoadPhoto(Attendee.PhotoName);
				Photo = ImageSource.FromStream(() =>
				{
					return new MemoryStream(bytes);
				});
			}
			catch (Exception e)
			{
				LogHelper.Instance.AddLog(e);
			}
			finally
			{
				IsLoadingPhoto = false;
			}
		}
	}
}

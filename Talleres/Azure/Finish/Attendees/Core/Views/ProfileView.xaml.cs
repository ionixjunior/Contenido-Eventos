using Core.Models;
using Core.ViewModels;
using Xamarin.Forms;

namespace Core.Views
{
	public partial class ProfileView : ContentPage
	{
		public ProfileView(AttendeeModel attendeeModel)
		{
			InitializeComponent();
			BindingContext = new ProfileViewModel(attendeeModel);
		}
	}
}

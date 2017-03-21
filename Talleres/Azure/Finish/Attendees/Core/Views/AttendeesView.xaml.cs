using Core.ViewModels;
using Xamarin.Forms;

namespace Core.Views
{
	public partial class AttendeesView : ContentPage
	{
		private AttendeesViewModel _viewModel = new AttendeesViewModel();

		public AttendeesView()
		{
			InitializeComponent();
			BindingContext = _viewModel;
		}

		void OnItemTapped(object sender, ItemTappedEventArgs e)
		{
			(sender as ListView).SelectedItem = null;
			_viewModel.OpenProfileCommand.Execute(e.Item);
		}
	}
}

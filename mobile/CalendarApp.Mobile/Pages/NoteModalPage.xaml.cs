using CalendarApp.Mobile.Models;
using CalendarApp.Mobile.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace CalendarApp.Mobile.Views
{
    public partial class NoteModalPage : ContentPage
    {
        private readonly ApiService _apiService;
        private readonly Note _note;

        public bool IsExistingNote => _note.Id != 0;

        public NoteModalPage(ApiService apiService, Note note)
        {
            InitializeComponent();
            _apiService = apiService;
            _note = note;

            BindingContext = this;

            // Инициализация полей
            TitleEntry.Text = _note.Title;
            DescriptionEditor.Text = _note.Description;
            DatePickerControl.Date = _note.Date;
            ImageEntry.Text = _note.ImageUrl;
            PreviewImage.Source = _note.ImageUrl;
        }

        private void OnColorClicked(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                _note.Color = btn.BackgroundColor.ToHex();
                // Можно визуально подсветить выбранный цвет, например, рамкой

                // Сбрасываем стиль у всех кнопок
                foreach (var child in ((HorizontalStackLayout)btn.Parent).Children.OfType<Button>())
                    child.BorderWidth = 0;

                // Подсвечиваем выбранный
                btn.BorderWidth = 3;
                btn.BorderColor = Colors.Black;
            }
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            _note.Title = TitleEntry.Text ?? "";
            _note.Description = DescriptionEditor.Text ?? "";
            _note.Date = DatePickerControl.Date;
            _note.ImageUrl = ImageEntry.Text ?? "";

            if (_note.Id == 0)
            {
                await _apiService.CreateNoteAsync(_note);
            }
            else
            {
                await _apiService.UpdateNoteAsync(_note); // <- только один аргумент
            }

            await Navigation.PopModalAsync();

            //if (AppState.CurrentUser == null) return;

            //var note = new Note
            //{
            //    Title = titleEntry.Text,
            //    Description = descriptionEditor.Text,
            //    Date = datePicker.Date,
            //    Color = colorPicker.SelectedItem.ToString(),
            //    UserId = AppState.CurrentUser.Id
            //};

            //var (created, error) = await _api.CreateNoteAsync(note);
            //if (created != null)
            //{
            //    await Navigation.PopAsync();
            //}
            //else
            //{
            //    await DisplayAlert("Ошибка", error ?? "Не удалось сохранить", "OK");
            //}
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Удалить заметку?", "Вы уверены?", "Да", "Нет");
            if (!confirm) return;

            await _apiService.DeleteNoteAsync(_note.Id);
            await Navigation.PopModalAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}

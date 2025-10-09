// Pages/CalendarPage.xaml.cs
using CalendarApp.Mobile.Models;
using CalendarApp.Mobile.Services;
//using CalendarApp.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;

namespace CalendarApp.Mobile.Pages
{
    public partial class CalendarPage : ContentPage
    {
        private readonly ApiService _apiService;
        private DateTime _currentMonth;
        private List<Note> _notes = new();

        private DateTime _selectedDate; // выбранная дата
        private int _userId => AppState.CurrentUser!.Id; // Здесь подставь реальный userId после логина

        private Button _selectedDayButton;

        public CalendarPage(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            _currentMonth = DateTime.Today;
            _selectedDate = DateTime.Today;

            // Загружаем заметки асинхронно
            _ = LoadNotesAsync(); // fire-and-forget - метод вернёт Task

            //LoadNotes();
            //UpdateCalendar();
        }

        // Заменяем существующий LoadNotes на асинхронный Task
        private async Task LoadNotesAsync()
        {
            if (!await CheckUserAsync()) return; // проверка пользователя при старте

            _notes = await _apiService.GetNotesForUserAsync(_userId);
            UpdateCalendar();
            UpdateNotesForSelectedDate(_selectedDate);
            //try
            //{
            //    _notes = await _apiService.GetNotesForUserAsync(_userId);
            //    // после загрузки перерисовываем календарь для текущего месяца
            //    UpdateCalendar(); // UpdateCalendar должен использовать _notes
            //                      // и показываем заметки для выбранной даты
            //    UpdateNotesForSelectedDate(_selectedDate);
            //}
            //catch (Exception ex)
            //{
            //    // логирование/ошибка
            //    await DisplayAlert("Ошибка", "Не удалось загрузить заметки: " + ex.Message, "OK");
            //}
        }

        private async void LoadNotes()
        {
            _notes = await _apiService.GetNotesForUserAsync(_userId);
            UpdateNotesForSelectedDate(DateTime.Today);
        }

        private void UpdateCalendar()
        {
            monthLabel.Text = _currentMonth.ToString("MMMM yyyy");

            daysGrid.Children.Clear();
            daysGrid.RowDefinitions.Clear();

            int daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);
            DateTime firstDay = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            int startDay = ((int)firstDay.DayOfWeek + 6) % 7; // Пн=0

            int totalRows = (int)Math.Ceiling((daysInMonth + startDay) / 7.0);
            for (int i = 0; i < totalRows; i++)
                daysGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            int dayCounter = 1;
            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    if (row == 0 && col < startDay) continue;
                    if (dayCounter > daysInMonth) break;

                    var noteForDay = _notes.FirstOrDefault(n => n.Date.Date == new DateTime(_currentMonth.Year, _currentMonth.Month, dayCounter));

                    var btn = new Button
                    {
                        //Text = dayCounter.ToString(),
                        //BackgroundColor = HasNote(new DateTime(_currentMonth.Year, _currentMonth.Month, dayCounter))
                        //? Colors.LightYellow
                        //: Colors.Transparent,
                        //CornerRadius = 0,
                        //TextColor = Colors.BlueViolet,
                        //FontAttributes = FontAttributes.Bold

                        Text = dayCounter.ToString(),
                        BackgroundColor = Colors.Transparent, // фон прозрачный
                        CornerRadius = 0,
                        TextColor = noteForDay != null ? Color.FromArgb(noteForDay.Color) : Colors.White, // цвет текста
                        FontAttributes = FontAttributes.Bold
                    };

                    int capturedDay = dayCounter;
                    //btn.Clicked += (s, e) =>
                    //{
                    //    UpdateNotesForSelectedDate(new DateTime(_currentMonth.Year, _currentMonth.Month, capturedDay));
                    //};
                    btn.Clicked += (s, e) =>
                    {
                        // Убираем выделение с предыдущей кнопки
                        if (_selectedDayButton != null)
                        {
                            _selectedDayButton.BorderColor = Colors.Transparent;
                            _selectedDayButton.BorderWidth = 0;
                        }

                        // Запоминаем новую выбранную кнопку
                        _selectedDayButton = btn;

                        // Добавляем рамку (можно заменить цвет)
                        _selectedDayButton.BorderColor = Colors.DeepSkyBlue;
                        _selectedDayButton.BorderWidth = 3;

                        // Обновляем заметки
                        UpdateNotesForSelectedDate(new DateTime(_currentMonth.Year, _currentMonth.Month, capturedDay));
                    };

                    daysGrid.Add(btn, col, row);
                    dayCounter++;
                }
            }
        }

        private bool HasNote(DateTime date)
        {
            return _notes.Any(n => n.Date.Date == date.Date);
        }

        private void UpdateNotesForSelectedDate(DateTime date)
        {
            _selectedDate = date;

            notesStack.Children.Clear();

            var notes = _notes.Where(n => n.Date.Date == date.Date).ToList();


            if (!notes.Any())
            {
                notesStack.Children.Add(new Label
                {
                    Text = "Нет заметок на этот день",
                    TextColor = Colors.Gray,
                    HorizontalOptions = LayoutOptions.Center
                });
                return;
            }


            foreach (var note in notes)
            {
                var frame = new Frame
                {
                    BackgroundColor = Color.FromArgb(note.Color),
                    //Content = new Label { Text = note.Title },
                    Content = new Label
                    {
                        Text = note.Title,
                        TextColor = Colors.Black,
                        FontAttributes = FontAttributes.Bold
                    },
                    Padding = 10
                };

                var tapGesture = new TapGestureRecognizer();
                //tapGesture.Tapped += async (s, e) =>
                //{
                //    var modal = new NoteModalPage(_apiService, note);
                //    await Navigation.PushModalAsync(modal);
                //    modal.Disappearing += (sender, args) => LoadNotes();
                //};
                // Открытие для редактирования (в UpdateNotesForSelectedDate при тапе по заметке)
                tapGesture.Tapped += async (s, e) =>
                {
                    var modal = new NoteModalPage(_apiService, note, async () => await LoadNotesAsync());
                    await Navigation.PushModalAsync(modal);
                    //await Shell.Current.GoToAsync("//NoteModalPage");
                };
                frame.GestureRecognizers.Add(tapGesture);
                notesStack.Children.Add(frame);
            }
        }


        private async void OnAddNoteClicked(object sender, EventArgs e)
        {
            //var modal = new NoteModalPage(_apiService, new Note { Date = DateTime.Today, UserId = _userId });
            //await Navigation.PushModalAsync(modal);
            ////modal.Disappearing += (s, args) => LoadNotes();
            //modal.Disappearing += (s, e) =>
            //{
            //    LoadNotes();
            //};

            var newNote = new Note { Date = _selectedDate, UserId = _userId };
            var modal = new NoteModalPage(_apiService, newNote, async () => await LoadNotesAsync());
            await Navigation.PushModalAsync(modal);
            //await Shell.Current.GoToAsync("//NoteModalPage");
        }



        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Выход", "Вы уверены, что хотите выйти?", "Да", "Нет");
            if (!confirm) return;

            // Очистка текущего пользователя
            AppState.CurrentUser = null;

            // Переход на страницу входа
            await Shell.Current.GoToAsync("//LoginPage");
        }

        // создаём асинхронную проверку перед обновлением календаря
        private async Task<bool> CheckUserAsync()
        {
            if (AppState.CurrentUser == null) return false;

            bool exists = await _apiService.IsUserExistsAsync(AppState.CurrentUser.Id);
            if (!exists)
            {
                AppState.CurrentUser = null;
                await DisplayAlert("Профиль удалён",
                    "Ваш профиль был удалён администратором.", "OK");
                await Shell.Current.GoToAsync("//LoginPage");
                return false;
            }
            return true;
        }

        private async void OnPrevMonthClicked(object sender, EventArgs e)
        {
            if (!await CheckUserAsync()) return; // проверяем, существует ли пользователь

            _currentMonth = _currentMonth.AddMonths(-1);
            UpdateCalendar();
        }

        private async void OnNextMonthClicked(object sender, EventArgs e)
        {
            if (!await CheckUserAsync()) return; // проверяем, существует ли пользователь

            _currentMonth = _currentMonth.AddMonths(1);
            UpdateCalendar();
        }

    }
}

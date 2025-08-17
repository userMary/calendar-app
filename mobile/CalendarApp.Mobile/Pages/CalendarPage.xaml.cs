// Pages/CalendarPage.xaml.cs
using CalendarApp.Mobile.Models;
using CalendarApp.Mobile.Services;
using CalendarApp.Mobile.Views;
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
        private int _userId => AppState.CurrentUser!.Id; // «десь подставь реальный userId после логина

        public CalendarPage(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            _currentMonth = DateTime.Today;
            LoadNotes();
            UpdateCalendar();
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
            int startDay = ((int)firstDay.DayOfWeek + 6) % 7; // ѕн=0

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
                    btn.Clicked += (s, e) =>
                    {
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
            notesStack.Children.Clear();
            var notes = _notes.Where(n => n.Date.Date == date.Date).ToList();
            foreach (var note in notes)
            {
                var frame = new Frame
                {
                    BackgroundColor = Color.FromArgb(note.Color),
                    Content = new Label { Text = note.Title },
                    Padding = 10
                };

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += async (s, e) =>
                {
                    var modal = new NoteModalPage(_apiService, note);
                    await Navigation.PushModalAsync(modal);
                    modal.Disappearing += (sender, args) => LoadNotes();
                };
                frame.GestureRecognizers.Add(tapGesture);
                notesStack.Children.Add(frame);
            }
        }

        private void OnPrevMonthClicked(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            UpdateCalendar();
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            UpdateCalendar();
        }

        private async void OnAddNoteClicked(object sender, EventArgs e)
        {
            var modal = new NoteModalPage(_apiService, new Note { Date = DateTime.Today, UserId = _userId });
            await Navigation.PushModalAsync(modal);
            modal.Disappearing += (s, args) => LoadNotes();
        }

    }
}

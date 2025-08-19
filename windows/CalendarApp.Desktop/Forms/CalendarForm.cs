using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CalendarApp.Desktop.Models;
using CalendarApp.Desktop.Services;
using Microsoft.VisualBasic.ApplicationServices;

namespace CalendarApp.Desktop.Forms
{
    public partial class CalendarForm : Form
    {

        private readonly ApiService api;
        private DateTime currentDate;
        private readonly int _userId;

        public CalendarForm(ApiService apiService)
        {
            InitializeComponent();
            api = apiService;
            currentDate = DateTime.Now;

            InitCalendarTable();
            LoadCalendar(currentDate);
        }

        private void InitCalendarTable()
        {
            calendarTable.ColumnCount = 7;
            calendarTable.RowCount = 6;
            calendarTable.Dock = DockStyle.Fill;
            calendarTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

            for (int i = 0; i < 7; i++)
                calendarTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28f));

            for (int i = 0; i < 6; i++)
                calendarTable.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6f));
        }

        private async void LoadCalendar(DateTime date)
        {
            calendarTable.Controls.Clear();

            DateTime firstDay = new DateTime(date.Year, date.Month, 1);
            int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            // Загружаем заметки из API
            //var notes = await api.GetNotesAsync();
            var notes = await api.GetNotesForUserAsync(_userId);

            int row = 0;
            int col = (int)firstDay.DayOfWeek;
            if (col == 0) col = 7; // чтобы воскресенье было в конце
            col--;

            for (int day = 1; day <= daysInMonth; day++)
            {
                Panel dayPanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoScroll = true
                };

                Label lblDay = new Label
                {
                    Text = day.ToString(),
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.TopLeft,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                dayPanel.Controls.Add(lblDay);

                var dayNotes = notes.Where(n => n.Date.Date == new DateTime(date.Year, date.Month, day).Date);

                foreach (var note in dayNotes)
                {
                    Label noteLbl = new Label
                    {
                        Text = note.Title,
                        AutoSize = true,
                        BackColor = GetColor(note.Color),
                        Margin = new Padding(2)
                    };
                    dayPanel.Controls.Add(noteLbl);
                }

                calendarTable.Controls.Add(dayPanel, col, row);

                col++;
                if (col > 6)
                {
                    col = 0;
                    row++;
                }
            }

            lblMonth.Text = date.ToString("MMMM yyyy");
        }

        private Color GetColor(string colorName)
        {
            return colorName?.ToLower() switch
            {
                "red" => Color.LightCoral,
                "blue" => Color.LightBlue,
                "green" => Color.LightGreen,
                "yellow" => Color.LightYellow,
                "purple" => Color.Plum,
                _ => Color.White
            };
        }

        //private readonly ApiService _api;
        //private DateTime currentDate;
        //private readonly int _userId;
        //private int _currentYear;
        //private int _currentMonth;

        //private TableLayoutPanel calendarTable = null!;
        ////public CalendarForm(ApiService api, int userId)
        //public CalendarForm(ApiService apiService)
        //{
        //    //_api = api ?? throw new ArgumentNullException(nameof(api));
        //    //_userId = userId;
        //    //_currentYear = DateTime.Now.Year;
        //    //_currentMonth = DateTime.Now.Month;

        //    //InitializeComponent();
        //    //BuildUi();
        //    //_ = LoadNotesAsync();
        //    InitializeComponent();
        //    _api = apiService;
        //    currentDate = DateTime.Now;
        //    LoadCalendar(currentDate);
        //}

        //private async void LoadCalendar(DateTime date)
        //{
        //    calendarTable.Controls.Clear();

        //    // 1. Узнаем первый день месяца и количество дней
        //    DateTime firstDay = new DateTime(date.Year, date.Month, 1);
        //    int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

        //    // 2. Загружаем заметки из API
        //    //var notes = await _api.GetNotesAsync();
        //    var notes = await _api.GetNotesForUserAsync(_userId);

        //    // 3. Рисуем ячейки
        //    int row = 0, col = (int)firstDay.DayOfWeek; // смещение
        //    for (int day = 1; day <= daysInMonth; day++)
        //    {
        //        Panel dayPanel = new Panel { Dock = DockStyle.Fill, BorderStyle = BorderStyle.FixedSingle };
        //        Label lbl = new Label { Text = day.ToString(), Dock = DockStyle.Top };
        //        dayPanel.Controls.Add(lbl);

        //        // показываем заметки в этот день
        //        var dayNotes = notes.Where(n => n.Date.Date == new DateTime(date.Year, date.Month, day).Date);
        //        foreach (var note in dayNotes)
        //        {
        //            Label noteLbl = new Label
        //            {
        //                Text = note.Title,
        //                AutoSize = true,
        //                BackColor = Color.LightYellow
        //            };
        //            dayPanel.Controls.Add(noteLbl);
        //        }

        //        calendarTable.Controls.Add(dayPanel, col, row);

        //        col++;
        //        if (col > 6) { col = 0; row++; }
        //    }
        //}

        //private void BuildUi()
        //{
        //    this.Text = "Календарь заметок";
        //    this.Size = new Size(900, 700);

        //    var header = new FlowLayoutPanel
        //    {
        //        Dock = DockStyle.Top,
        //        Height = 40
        //    };

        //    var prevBtn = new Button { Text = "◀" };
        //    prevBtn.Click += (s, e) =>
        //    {
        //        if (_currentMonth == 1) { _currentMonth = 12; _currentYear--; }
        //        else _currentMonth--;
        //        _ = LoadNotesAsync();
        //    };

        //    var nextBtn = new Button { Text = "▶" };
        //    nextBtn.Click += (s, e) =>
        //    {
        //        if (_currentMonth == 12) { _currentMonth = 1; _currentYear++; }
        //        else _currentMonth++;
        //        _ = LoadNotesAsync();
        //    };

        //    var monthLabel = new Label
        //    {
        //        Name = "MonthLabel",
        //        Text = $"{GetMonthName(_currentMonth)} {_currentYear}",
        //        AutoSize = true,
        //        Font = new Font("Segoe UI", 14, FontStyle.Bold),
        //        Margin = new Padding(20, 5, 20, 5)
        //    };

        //    header.Controls.Add(prevBtn);
        //    header.Controls.Add(monthLabel);
        //    header.Controls.Add(nextBtn);

        //    calendarTable = new TableLayoutPanel
        //    {
        //        Dock = DockStyle.Fill,
        //        ColumnCount = 7,
        //        RowCount = 7,
        //        CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
        //    };

        //    for (int i = 0; i < 7; i++)
        //        calendarTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 7));

        //    this.Controls.Add(calendarTable);
        //    this.Controls.Add(header);
        //}

        //private async Task LoadNotesAsync()
        //{
        //    var notes = await _api.GetNotesForUserAsync(_userId);

        //    calendarTable.Controls.Clear();

        //    // Заголовки дней недели
        //    string[] days = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
        //    for (int i = 0; i < 7; i++)
        //    {
        //        var lbl = new Label
        //        {
        //            Text = days[i],
        //            Dock = DockStyle.Fill,
        //            TextAlign = ContentAlignment.MiddleCenter,
        //            Font = new Font("Segoe UI", 10, FontStyle.Bold)
        //        };
        //        calendarTable.Controls.Add(lbl, i, 0);
        //    }

        //    var firstDay = new DateTime(_currentYear, _currentMonth, 1);
        //    int startDay = ((int)firstDay.DayOfWeek + 6) % 7; // понедельник = 0
        //    int totalDays = DateTime.DaysInMonth(_currentYear, _currentMonth);

        //    int row = 1, col = startDay;

        //    for (int day = 1; day <= totalDays; day++)
        //    {
        //        int dayLocal = day; // чтобы корректно захватило в лямбдах
        //        var panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        //        var lblDay = new Label
        //        {
        //            Text = day.ToString(),
        //            Dock = DockStyle.Top,
        //            TextAlign = ContentAlignment.TopRight
        //        };
        //        panel.Controls.Add(lblDay);


        //        //Ещё надёжнее — сравнивать даты по дате(без времени):
        //        //string dateStr = new DateTime(_currentYear, _currentMonth, day).ToString("yyyy-MM-dd");
        //        var dayDate = new DateTime(_currentYear, _currentMonth, day).Date;

        //        // Добавляем заметки для этого дня
        //        foreach (var note in notes)
        //        {
        //            // Было (неправильно):
        //            //var noteDate = DateTime.Parse(note.Date).ToString("yyyy-MM-dd");

        //            // Стало (правильно):
        //            var noteDate = note.Date.ToString("yyyy-MM-dd"); // если NoteDto.Date = DateTime
        //            if (note.Date.Date == dayDate.Date)
        //            //if (noteDate == dateStr)
        //            {
        //                var lblNote = new Label
        //                {
        //                    Text = note.Title,
        //                    BackColor = ColorTranslator.FromHtml(note.Color ?? "#FFFFFF"),
        //                    AutoSize = false,
        //                    Height = 20,
        //                    Dock = DockStyle.Top,
        //                    TextAlign = ContentAlignment.MiddleLeft,
        //                    Cursor = Cursors.Hand
        //                };
        //                lblNote.Click += (s, e) =>
        //                {
        //                    var noteForm = new NoteForm(_api, note, _userId);
        //                    if (noteForm.ShowDialog() == DialogResult.OK)
        //                        _ = LoadNotesAsync();
        //                };
        //                panel.Controls.Add(lblNote);
        //                panel.Controls.SetChildIndex(lblNote, 0); // чтобы заметка была над датой
        //            }
        //        }

        //        // Клик по пустому месту ячейки — создание новой заметки
        //        panel.Click += (s, e) =>
        //        {
        //            var newNote = new NoteDto
        //            {
        //                Date = new DateTime(_currentYear, _currentMonth, day),
        //                UserId = _userId
        //            };
        //            var noteForm = new NoteForm(_api, newNote, _userId);
        //            if (noteForm.ShowDialog() == DialogResult.OK)
        //                _ = LoadNotesAsync();
        //        };

        //        calendarTable.Controls.Add(panel, col, row);

        //        col++;
        //        if (col == 7) { col = 0; row++; }
        //    }

        //    // Обновить заголовок месяца
        //    //var monthLabel = this.Controls.Find("MonthLabel", true)[0] as Label;
        //    //if (monthLabel != null)
        //    //    monthLabel.Text = $"{GetMonthName(_currentMonth)} {_currentYear}";
        //    if (Controls.Find("MonthLabel", true) is { Length: > 0 } arr && arr[0] is Label monthLabel)
        //        monthLabel.Text = $"{GetMonthName(_currentMonth)} {_currentYear}";

        //    calendarTable.ResumeLayout();
        //}

        //private string GetMonthName(int month)
        //{
        //    string[] months = { "Январь","Февраль","Март","Апрель","Май","Июнь",
        //                        "Июль","Август","Сентябрь","Октябрь","Ноябрь","Декабрь" };
        //    return months[month - 1];
        //}

        //private static Color SafeColor(string? color)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(color)) return Color.White;
        //        return ColorTranslator.FromHtml(color); // поддерживает и "#A5D6A7", и "red"
        //    }
        //    catch { return Color.White; }
        //}

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void prevBtn_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddMonths(-1);
            LoadCalendar(currentDate);
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            currentDate = currentDate.AddMonths(1);
            LoadCalendar(currentDate);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalendarApp.Desktop.Models;
using CalendarApp.Desktop.Services;

namespace CalendarApp.Desktop.Forms
{
    public partial class CalendarForm2 : Form
    {
        private readonly ApiService _api;
        private readonly int _userId;
        private DateTime _currentDate;
        private List<NoteDto> _notes = new();

        // Header
        private Panel headerPanel;
        private TableLayoutPanel headerTable;
        private Button prevBtn;
        private Button nextBtn;
        private Label lblMonth;

        // Week days row
        private TableLayoutPanel weekDaysPanel;

        // Calendar grid
        private TableLayoutPanel calendarTable;

        public CalendarForm2(ApiService apiService, int userId)
        {
            _api = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _userId = userId;
            _currentDate = DateTime.Now.Date;

            InitializeComponentCustom();
            // загрузка календаря
            LoadCalendar(_currentDate);

            this.FormClosed += (s, e) => Application.Exit();
        }

        private void InitializeComponentCustom()
        {
            // --- Основные свойства формы ---
            this.Text = "Календарь";
            this.Size = new Size(1000, 720);
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- Header Panel (Dock Top) ---
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(6),
                BackColor = Color.FromArgb(245, 245, 245)
            };
            //this.Controls.Add(headerPanel);

            // Header layout: 3 колонки (кнопка | заголовок | кнопка)
            headerTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            headerTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            headerTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            headerTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20));
            headerPanel.Controls.Add(headerTable);

            prevBtn = new Button
            {
                Text = "< Предыдущий",
                Dock = DockStyle.Fill,
                Margin = new Padding(6)
            };
            prevBtn.Click += PrevBtn_Click;

            nextBtn = new Button
            {
                Text = "Следующий >",
                Dock = DockStyle.Fill,
                Margin = new Padding(6)
            };
            nextBtn.Click += NextBtn_Click;

            lblMonth = new Label
            {
                Text = "",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoEllipsis = true
            };

            headerTable.Controls.Add(prevBtn, 0, 0);
            headerTable.Controls.Add(lblMonth, 1, 0);
            headerTable.Controls.Add(nextBtn, 2, 0);

            // --- Week day labels (Пн..Вс) ---
            weekDaysPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 28,
                ColumnCount = 7,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            for (int i = 0; i < 7; i++)
                weekDaysPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 7f));

            string[] days = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            for (int i = 0; i < 7; i++)
            {
                var lbl = new Label
                {
                    Text = days[i],
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.DarkSlateGray
                };
                weekDaysPanel.Controls.Add(lbl, i, 0);
            }
            //this.Controls.Add(weekDaysPanel);

            // --- Calendar grid (7 x 6) ---
            calendarTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 7,
                RowCount = 6,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                BackColor = Color.White
            };
            for (int i = 0; i < 7; i++)
                calendarTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 7f));
            for (int r = 0; r < 6; r++)
                calendarTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 6f));


            this.Controls.Add(calendarTable);
            this.Controls.Add(weekDaysPanel);
            this.Controls.Add(headerPanel);

            // Убедимся, что порядок Controls: header, weekDays, calendar (Add -> last added is at bottom),
            // но мы добавляли header, weekDays, then calendarTable — всё ок.
        }

        private async void CalendarForm2_Load(object sender, EventArgs e)
        {
            await LoadNotesAsync();
            LoadCalendar(_currentDate);
        }

        private async Task LoadNotesAsync()
        {
            _notes = await _api.GetNotesForUserAsync(_userId);
        }


        // Заполнение календаря для указанной даты (месяц/год)
        private async void LoadCalendar(DateTime date)
        {
            // Проверяем, существует ли пользователь
            var user = await _api.GetUserByIdAsync(_userId);
            if (user == null)
            {
                MessageBox.Show("Ваш аккаунт был удалён.", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Возврат к форме логина
                this.Hide();
                var loginForm = new LoginForm(); // если у тебя она принимает api
                loginForm.Show();
                this.Close(); // закрываем текущий календарь
                return;
            }

            try
            {
                // блокируем перерисовку
                calendarTable.SuspendLayout();
                calendarTable.Controls.Clear();

                lblMonth.Text = date.ToString("MMMM yyyy");

                // Загружаем заметки из API
                var notes = await _api.GetNotesForUserAsync(_userId);

                // Начало месяца и параметры
                DateTime firstDay = new DateTime(date.Year, date.Month, 1);
                int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

                // Вычисляем стартовую колонку так, чтобы понедельник был 0
                // DayOfWeek: Sunday=0, Monday=1 ... Saturday=6
                int startCol = ((int)firstDay.DayOfWeek + 6) % 7; // преобразование: Mon->0 ... Sun->6

                int row = 0;
                int col = startCol;

                for (int day = 1; day <= daysInMonth; day++)
                {
                    DateTime cellDate = new DateTime(date.Year, date.Month, day);

                    // Панель дня — FlowLayoutPanel удобнее для списка заметок
                    var dayPanel = new FlowLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        FlowDirection = FlowDirection.TopDown,
                        WrapContents = false,
                        AutoScroll = true,
                        Margin = new Padding(0),
                        Padding = new Padding(4),
                        BackColor = Color.White
                    };

                    // Номер дня — вверху
                    var lblDay = new Label
                    {
                        Text = day.ToString(),
                        AutoSize = false,
                        Height = 22,
                        Width = 40,
                        TextAlign = ContentAlignment.MiddleLeft,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold)
                    };
                    // клик по номеру дня открывает форму создания
                    lblDay.Click += (s, e) => OpenNoteForm(null, cellDate);

                    dayPanel.Controls.Add(lblDay);

                    // Все заметки на эту дату
                    var dayNotes = notes.Where(n => n.Date.Date == cellDate.Date).ToList();

                    foreach (var note in dayNotes)
                    {
                        var noteLink = new LinkLabel
                        {
                            Text = note.Title,
                            AutoSize = true,
                            Margin = new Padding(2),
                            Tag = note,
                            LinkColor = Color.Black,
                            ActiveLinkColor = Color.Black
                        };

                        // устанавливаем фон в зависимости от note.Color (мягкий)
                        var bg = GetColor(note.Color);
                        noteLink.BackColor = ControlPaint.LightLight(bg);
                        noteLink.Padding = new Padding(3);

                        noteLink.LinkClicked += (s, e) =>
                        {
                            var nd = (NoteDto)noteLink.Tag;
                            OpenNoteForm(nd, cellDate);
                        };

                        dayPanel.Controls.Add(noteLink);
                    }

                    // клик по пустой панели (вне дочерних элементов) — также открыть NoteForm
                    dayPanel.Click += (s, e) => OpenNoteForm(null, cellDate);

                    // Добавляем панель в таблицу
                    calendarTable.Controls.Add(dayPanel, col, row);

                    col++;
                    if (col > 6)
                    {
                        col = 0;
                        row++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке календаря: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                calendarTable.ResumeLayout();
            }
        }

        // Открывает NoteForm для создания (note==null) или редактирования (note != null)
        private void OpenNoteForm(NoteDto? note, DateTime date)
        {
            var nf = new NoteForm(_api, _userId, date, note);
            var dr = nf.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                // Обновляем календарь после закрытия NoteForm
                LoadCalendar(_currentDate);
            }
        }

        // Навигация
        private void PrevBtn_Click(object sender, EventArgs e)
        {
            _currentDate = _currentDate.AddMonths(-1);
            LoadCalendar(_currentDate);
        }

        private void NextBtn_Click(object sender, EventArgs e)
        {
            _currentDate = _currentDate.AddMonths(1);
            LoadCalendar(_currentDate);
        }

        // Небольшая утилита для цвета (возвращает Color по имени)
        private Color GetColor(string colorName)
        {
            if (string.IsNullOrWhiteSpace(colorName))
                return Color.White;

            colorName = colorName.Trim().ToLower();

            if (colorName.StartsWith("#"))
            {
                try
                {
                    return ColorTranslator.FromHtml(colorName);
                }
                catch
                {
                    return Color.White;
                }
            }

            return colorName switch
            {
                "white" => Color.White,
                "red" => ColorTranslator.FromHtml("#EF9A9A"),
                "blue" => ColorTranslator.FromHtml("#90CAF9"),
                "green" => ColorTranslator.FromHtml("#A5D6A7"),
                "yellow" => ColorTranslator.FromHtml("#FFF9C4"),
                "purple" => ColorTranslator.FromHtml("#CE93D8"),
                _ => Color.White
            };

            //return colorName?.ToLower() switch
            //{
            //    "red" => Color.LightCoral,
            //    "blue" => Color.LightBlue,
            //    "green" => Color.LightGreen,
            //    "yellow" => Color.LightYellow,
            //    "purple" => Color.Plum,
            //    _ => Color.White
            //};
        }
        //public CalendarForm2()
        //{
        //    InitializeComponent();
        //}
    }
}

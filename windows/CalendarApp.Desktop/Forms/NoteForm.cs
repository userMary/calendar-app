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
    public partial class NoteForm : Form
    {
        private readonly ApiService _api;
        private readonly int _userId;
        private readonly DateTime _date;
        private readonly NoteDto? _noteToEdit;

        // Элементы формы
        private TextBox txtTitle;
        private TextBox txtDescription;
        private ComboBox cmbColor;
        private Button btnSave;
        private Button btnCancel;

        public NoteForm(ApiService api, int userId, DateTime date, NoteDto? note = null)
        {
            _api = api;
            _userId = userId;
            _date = date;
            _noteToEdit = note;

            InitializeComponents();
            LoadNote();
        }

        private void InitializeComponents()
        {
            this.Text = $"Заметка на {_date:dd.MM.yyyy}";
            this.Size = new Size(400, 350);
            this.StartPosition = FormStartPosition.CenterParent;

            // ---------- Заголовок ----------
            Label lblTitle = new Label
            {
                Text = "Название",
                Location = new Point(10, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            txtTitle = new TextBox
            {
                Location = new Point(10, 35),
                Width = 360
            };
            this.Controls.Add(txtTitle);

            // ---------- Описание ----------
            Label lblDescription = new Label
            {
                Text = "Описание",
                Location = new Point(10, 70),
                AutoSize = true
            };
            this.Controls.Add(lblDescription);

            txtDescription = new TextBox
            {
                Location = new Point(10, 90),
                Width = 360,
                Height = 120,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.Add(txtDescription);

            // ---------- Цвет заметки ----------
            Label lblColor = new Label
            {
                Text = "Цвет",
                Location = new Point(10, 220),
                AutoSize = true
            };
            this.Controls.Add(lblColor);

            cmbColor = new ComboBox
            {
                Location = new Point(60, 215),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbColor.Items.AddRange(new string[] { "white", "red", "blue", "green", "yellow", "purple" });
            cmbColor.SelectedIndex = 0;
            this.Controls.Add(cmbColor);

            // ---------- Кнопки ----------
            btnSave = new Button
            {
                Text = "Сохранить",
                Location = new Point(200, 260),
                Width = 80
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(290, 260),
                Width = 80
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            // ---------- Кнопка "Удалить" только для редактирования ----------
            if (_noteToEdit != null)
            {
                btnDelete = new Button
                {
                    Text = "Удалить",
                    BackColor = Color.MistyRose,
                    Location = new Point(10, 260),
                    Size = new Size(80, 23)
                };
                btnDelete.Click += BtnDelete_Click;
                this.Controls.Add(btnDelete);
            }
        }

        private void LoadNote()
        {
            if (_noteToEdit != null)
            {
                txtTitle.Text = _noteToEdit.Title;
                txtDescription.Text = _noteToEdit.Description;
                cmbColor.SelectedItem = _noteToEdit.Color;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Введите название заметки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            NoteDto note = new NoteDto
            {
                Title = txtTitle.Text,
                Description = txtDescription.Text,
                Color = cmbColor.SelectedItem.ToString() ?? "white",
                Date = _date,
                UserId = _userId
            };

            if (_noteToEdit != null)
            {
                note.Id = _noteToEdit.Id;
                bool updated = await _api.UpdateNoteAsync(note.Id, note);
                if (!updated)
                {
                    MessageBox.Show("Ошибка при обновлении заметки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                var createdNote = await _api.CreateNoteAsync(note);
                if (createdNote == null)
                {
                    MessageBox.Show("Ошибка при создании заметки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void NoteForm_Load(object sender, EventArgs e)
        {

        }




        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_noteToEdit == null)
                return;

            var confirm = MessageBox.Show($"Удалить заметку \"{_noteToEdit.Title}\"?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            bool deleted = await _api.DeleteNoteAsync(_noteToEdit.Id);
            if (deleted)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при удалении заметки", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private readonly ApiService _api;
        //private readonly NoteDto _note;
        //private readonly int _userId;
        //public NoteForm(ApiService api, NoteDto note, int userId)
        //{
        //    _api = api;
        //    _note = note;
        //    _userId = userId;
        //    InitializeComponent();
        //    BuildUi();
        //}
        //private void BuildUi()
        //{
        //    this.Text = _note.Id == 0 ? "Новая заметка" : "Редактирование";
        //    this.Size = new System.Drawing.Size(400, 300);

        //    var titleBox = new TextBox { Text = _note.Title, Dock = DockStyle.Top };
        //    var descBox = new TextBox { Text = _note.Description, Multiline = true, Height = 100, Dock = DockStyle.Top };
        //    var colorBox = new ComboBox { Dock = DockStyle.Top };
        //    colorBox.Items.AddRange(new object[] { "white", "#FFF9C4", "#A5D6A7", "#EF9A9A", "#CE93D8", "#90CAF9" });
        //    colorBox.Text = _note.Color ?? "white";

        //    var saveBtn = new Button { Text = "Сохранить", Dock = DockStyle.Bottom };
        //    saveBtn.Click += async (s, e) =>
        //    {
        //        _note.Title = titleBox.Text;
        //        _note.Description = descBox.Text;
        //        _note.Color = colorBox.Text;
        //        _note.UserId = _userId;

        //        if (_note.Id == 0)
        //            await _api.CreateNoteAsync(_note);
        //        else
        //            await _api.UpdateNoteAsync(_note.Id, _note);

        //        this.DialogResult = DialogResult.OK;
        //        this.Close();
        //    };

        //    var deleteBtn = new Button { Text = "Удалить", Dock = DockStyle.Bottom };
        //    deleteBtn.Click += async (s, e) =>
        //    {
        //        if (_note.Id != 0)
        //            await _api.DeleteNoteAsync(_note.Id);

        //        this.DialogResult = DialogResult.OK;
        //        this.Close();
        //    };

        //    this.Controls.Add(deleteBtn);
        //    this.Controls.Add(saveBtn);
        //    this.Controls.Add(colorBox);
        //    this.Controls.Add(descBox);
        //    this.Controls.Add(titleBox);
        //}
    }
}

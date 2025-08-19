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
        private readonly NoteDto _note;
        private readonly int _userId;
        public NoteForm(ApiService api, NoteDto note, int userId)
        {
            _api = api;
            _note = note;
            _userId = userId;
            InitializeComponent();
            BuildUi();
        }
        private void BuildUi()
        {
            this.Text = _note.Id == 0 ? "Новая заметка" : "Редактирование";
            this.Size = new System.Drawing.Size(400, 300);

            var titleBox = new TextBox { Text = _note.Title, Dock = DockStyle.Top };
            var descBox = new TextBox { Text = _note.Description, Multiline = true, Height = 100, Dock = DockStyle.Top };
            var colorBox = new ComboBox { Dock = DockStyle.Top };
            colorBox.Items.AddRange(new object[] { "white", "#FFF9C4", "#A5D6A7", "#EF9A9A", "#CE93D8", "#90CAF9" });
            colorBox.Text = _note.Color ?? "white";

            var saveBtn = new Button { Text = "Сохранить", Dock = DockStyle.Bottom };
            saveBtn.Click += async (s, e) =>
            {
                _note.Title = titleBox.Text;
                _note.Description = descBox.Text;
                _note.Color = colorBox.Text;
                _note.UserId = _userId;

                if (_note.Id == 0)
                    await _api.CreateNoteAsync(_note);
                else
                    await _api.UpdateNoteAsync(_note.Id, _note);

                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            var deleteBtn = new Button { Text = "Удалить", Dock = DockStyle.Bottom };
            deleteBtn.Click += async (s, e) =>
            {
                if (_note.Id != 0)
                    await _api.DeleteNoteAsync(_note.Id);

                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(deleteBtn);
            this.Controls.Add(saveBtn);
            this.Controls.Add(colorBox);
            this.Controls.Add(descBox);
            this.Controls.Add(titleBox);
        }
    }
}

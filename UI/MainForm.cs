using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Core.Models;
using Core.Refactorings;

namespace UI
{
    public partial class MainForm : Form
    {
        private RichTextBox inputTextBox = null!;
        private RichTextBox outputTextBox = null!;
        private ComboBox refactoringComboBox = null!;
        private TextBox oldNameTextBox = null!;
        private TextBox newNameTextBox = null!;
        private Button applyButton = null!;
        private Button loadButton = null!;

        private List<IRefactoring> availableRefactorings = new();

        public MainForm()
        {
            // Цей метод визначений у файлі .Designer.cs
            InitializeComponent();
            // Ваші методи налаштування
            SetupUI();
            LoadRefactorings();
        }

        private void SetupUI()
        {
            this.Text = "C++ Code Refactoring Tool";
            this.Size = new Size(1000, 600);
            this.MinimumSize = new Size(800, 400);

            Panel topPanel = new Panel { Dock = DockStyle.Top, Height = 60 };
            this.Controls.Add(topPanel);

            loadButton = new Button { Text = "Load C++ File", Location = new Point(10, 15), Width = 110 };
            loadButton.Click += LoadButton_Click;
            topPanel.Controls.Add(loadButton);

            Label refLabel = new Label { Text = "Method:", Location = new Point(130, 20), Width = 55 };
            topPanel.Controls.Add(refLabel);

            refactoringComboBox = new ComboBox { Location = new Point(185, 15), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            topPanel.Controls.Add(refactoringComboBox);

            Label oldNameLabel = new Label { Text = "Old Name:", Location = new Point(380, 20), Width = 65 };
            topPanel.Controls.Add(oldNameLabel);

            oldNameTextBox = new TextBox { Location = new Point(445, 15), Width = 100 };
            topPanel.Controls.Add(oldNameTextBox);

            Label newNameLabel = new Label { Text = "New Name:", Location = new Point(560, 20), Width = 65 };
            topPanel.Controls.Add(newNameLabel);

            newNameTextBox = new TextBox { Location = new Point(625, 15), Width = 100 };
            topPanel.Controls.Add(newNameTextBox);

            applyButton = new Button { Text = "Apply Refactoring", Location = new Point(740, 15), Width = 120 };
            applyButton.Click += ApplyButton_Click;
            topPanel.Controls.Add(applyButton);

            SplitContainer splitContainer = new SplitContainer { Dock = DockStyle.Fill };
            this.Controls.Add(splitContainer);
            splitContainer.BringToFront();

            GroupBox inputGroup = new GroupBox { Text = "Original Code", Dock = DockStyle.Fill };
            inputTextBox = new RichTextBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 11) };
            inputGroup.Controls.Add(inputTextBox);
            splitContainer.Panel1.Controls.Add(inputGroup);

            GroupBox outputGroup = new GroupBox { Text = "Refactored Code", Dock = DockStyle.Fill };
            outputTextBox = new RichTextBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 11), ReadOnly = true, BackColor = Color.WhiteSmoke };
            outputGroup.Controls.Add(outputTextBox);
            splitContainer.Panel2.Controls.Add(outputGroup);
        }

        private void LoadRefactorings()
        {
            availableRefactorings = new List<IRefactoring> { new RenameMethodRefactoring() };
            foreach (var r in availableRefactorings)
            {
                refactoringComboBox.Items.Add(r.Name);
            }
            if (refactoringComboBox.Items.Count > 0)
                refactoringComboBox.SelectedIndex = 0;
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "C++ files (*.cpp;*.h)|*.cpp;*.h|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                inputTextBox.Text = File.ReadAllText(openFileDialog.FileName);
            }
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (refactoringComboBox.SelectedIndex < 0) return;

            var selectedRefactoring = availableRefactorings[refactoringComboBox.SelectedIndex];
            var parameters = new RefactoringParameters();
            parameters.Parameters["oldName"] = oldNameTextBox.Text;
            parameters.Parameters["newName"] = newNameTextBox.Text;

            try
            {
                outputTextBox.Text = selectedRefactoring.Apply(inputTextBox.Text, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
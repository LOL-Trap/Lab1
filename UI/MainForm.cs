using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Core.Models;
using Core.Refactorings;
using Core.Interfaces;

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

        private Label oldNameLabel = null!;
        private Label newNameLabel = null!;

        private List<IRefactoring> availableRefactorings = new();

        public MainForm()
        {
            InitializeComponent();
            SetupUI();
            LoadRefactorings();
        }

        private void SetupUI()
        {
            Text = "C++ Code Refactoring Tool";
            Size = new Size(1100, 650);
            MinimumSize = new Size(900, 500);
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.Dpi;

            TableLayoutPanel rootLayout = new TableLayoutPanel();
            rootLayout.Dock = DockStyle.Fill;
            rootLayout.RowCount = 2;
            rootLayout.ColumnCount = 1;
            rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Controls.Add(rootLayout);

            TableLayoutPanel topLayout = new TableLayoutPanel();
            topLayout.Dock = DockStyle.Top;
            topLayout.AutoSize = true;
            topLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            topLayout.Padding = new Padding(8);
            topLayout.ColumnCount = 8;
            topLayout.RowCount = 2;

            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            topLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            topLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            rootLayout.Controls.Add(topLayout, 0, 0);

            loadButton = new Button();
            loadButton.Text = "Load C++ File";
            loadButton.AutoSize = true;
            loadButton.Anchor = AnchorStyles.Left;
            loadButton.Click += LoadButton_Click;
            topLayout.Controls.Add(loadButton, 0, 0);

            Label methodLabel = new Label();
            methodLabel.Text = "Method:";
            methodLabel.AutoSize = true;
            methodLabel.Anchor = AnchorStyles.Left;
            topLayout.Controls.Add(methodLabel, 1, 0);

            refactoringComboBox = new ComboBox();
            refactoringComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            refactoringComboBox.Dock = DockStyle.Fill;
            refactoringComboBox.SelectedIndexChanged += RefactoringComboBox_SelectedIndexChanged;
            topLayout.Controls.Add(refactoringComboBox, 2, 0);

            oldNameLabel = new Label();
            oldNameLabel.Text = "Old Name:";
            oldNameLabel.AutoSize = true;
            oldNameLabel.Anchor = AnchorStyles.Left;
            topLayout.Controls.Add(oldNameLabel, 3, 0);

            oldNameTextBox = new TextBox();
            oldNameTextBox.Dock = DockStyle.Fill;
            topLayout.Controls.Add(oldNameTextBox, 4, 0);

            newNameLabel = new Label();
            newNameLabel.Text = "New Name:";
            newNameLabel.AutoSize = true;
            newNameLabel.Anchor = AnchorStyles.Left;
            topLayout.Controls.Add(newNameLabel, 5, 0);

            newNameTextBox = new TextBox();
            newNameTextBox.Dock = DockStyle.Fill;
            topLayout.Controls.Add(newNameTextBox, 6, 0);

            applyButton = new Button();
            applyButton.Text = "Apply Refactoring";
            applyButton.AutoSize = true;
            applyButton.Anchor = AnchorStyles.Left;
            applyButton.Click += ApplyButton_Click;
            topLayout.Controls.Add(applyButton, 7, 0);

            SplitContainer splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.SplitterDistance = Width / 2;
            rootLayout.Controls.Add(splitContainer, 0, 1);

            GroupBox inputGroup = new GroupBox();
            inputGroup.Text = "Original Code";
            inputGroup.Dock = DockStyle.Fill;

            inputTextBox = new RichTextBox();
            inputTextBox.Dock = DockStyle.Fill;
            inputTextBox.Font = new Font("Consolas", 11);
            inputGroup.Controls.Add(inputTextBox);

            splitContainer.Panel1.Controls.Add(inputGroup);

            GroupBox outputGroup = new GroupBox();
            outputGroup.Text = "Refactored Code";
            outputGroup.Dock = DockStyle.Fill;

            outputTextBox = new RichTextBox();
            outputTextBox.Dock = DockStyle.Fill;
            outputTextBox.Font = new Font("Consolas", 11);
            outputTextBox.ReadOnly = true;
            outputTextBox.BackColor = Color.WhiteSmoke;
            outputGroup.Controls.Add(outputTextBox);

            splitContainer.Panel2.Controls.Add(outputGroup);
        }

        private void LoadRefactorings()
        {
            availableRefactorings = new List<IRefactoring>
            {
                new RenameVariableRefactoring(),
                new RenameMethodRefactoring(),
                new AddParameterRefactoring(),
                new RemoveParameterRefactoring()
                new RemoveParameterRefactoring(),
                new BlockFormatterRefactoring()
            };

            refactoringComboBox.Items.Clear();

            foreach (IRefactoring refactoring in availableRefactorings)
            {
                refactoringComboBox.Items.Add(refactoring.Name);
            }

            if (refactoringComboBox.Items.Count > 0)
            {
                refactoringComboBox.SelectedIndex = 0;
            }
        }

        private void RefactoringComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (refactoringComboBox.SelectedIndex < 0)
            {
                return;
            }

            string selectedName = refactoringComboBox.SelectedItem!.ToString()!;

            oldNameTextBox.Clear();
            newNameTextBox.Clear();

            if (selectedName == "Rename Variable")
            {
                oldNameLabel.Text = "Old Name:";
                newNameLabel.Text = "New Name:";
                oldNameLabel.Visible = true;
                oldNameTextBox.Visible = true;
                newNameLabel.Visible = true;
                newNameTextBox.Visible = true;
            }
            else if (selectedName == "Rename Method")
            {
                oldNameLabel.Text = "Old Name:";
                newNameLabel.Text = "New Name:";
                oldNameLabel.Visible = true;
                oldNameTextBox.Visible = true;
                newNameLabel.Visible = true;
                newNameTextBox.Visible = true;
            }
            else if (selectedName == "Add Parameter")
            {
                oldNameLabel.Text = "Method Name:";
                newNameLabel.Text = "Type and Name:";
                oldNameLabel.Visible = true;
                oldNameTextBox.Visible = true;
                newNameLabel.Visible = true;
                newNameTextBox.Visible = true;
            }
            else if (selectedName == "Remove Parameter")
            {
                oldNameLabel.Text = "Method Name:";
                newNameLabel.Text = "Parameter Name:";
                oldNameLabel.Visible = true;
                oldNameTextBox.Visible = true;
                newNameLabel.Visible = true;
                newNameTextBox.Visible = true;
            }
            else if (selectedName == "Block Formatter")
            {
                oldNameLabel.Text = "Brace Style:";
                newNameLabel.Text = "Indent Size:";
                oldNameLabel.Visible = true;
                oldNameTextBox.Visible = true;
                newNameLabel.Visible = true;
                newNameTextBox.Visible = true;

                oldNameTextBox.Text = "Allman";
                newNameTextBox.Text = "4";
            }
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
            if (refactoringComboBox.SelectedIndex < 0)
            {
                return;
            }

            IRefactoring selectedRefactoring = availableRefactorings[refactoringComboBox.SelectedIndex];
            RefactoringParameters parameters = new RefactoringParameters();

            try
            {
                string selectedName = selectedRefactoring.Name;

                if (selectedName == "Rename Variable")
                {
                    parameters.Parameters["oldName"] = oldNameTextBox.Text;
                    parameters.Parameters["newName"] = newNameTextBox.Text;
                }
                else if (selectedName == "Rename Method")
                {
                    parameters.Parameters["oldName"] = oldNameTextBox.Text;
                    parameters.Parameters["newName"] = newNameTextBox.Text;
                }
                else if (selectedName == "Add Parameter")
                {
                    parameters.Parameters["methodName"] = oldNameTextBox.Text;

                    string[] parts = newNameTextBox.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length < 2)
                    {
                        MessageBox.Show(
                            "Для Add Parameter введіть у друге поле значення у форматі: int b",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    parameters.Parameters["parameterType"] = parts[0];
                    parameters.Parameters["parameterName"] = parts[1];
                }
                else if (selectedName == "Remove Parameter")
                {
                    parameters.Parameters["methodName"] = oldNameTextBox.Text;
                    parameters.Parameters["parameterName"] = newNameTextBox.Text;
                }
                else if (selectedName == "Block Formatter")
                {
                    parameters.Parameters["braceStyle"] = oldNameTextBox.Text;

                    if (!int.TryParse(newNameTextBox.Text, out int indentSize))
                    {
                        indentSize = 4;
                    }

                    parameters.Parameters["indentSize"] = indentSize;
                }

                outputTextBox.Text = selectedRefactoring.Apply(inputTextBox.Text, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
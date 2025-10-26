using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace cs.HoLMod.ES3EditorAPP
{
    public class MainForm : Form
    {
        private string currentFilePath = string.Empty;
        private byte[] fileContent = null;
        private LanguageManager languageManager = LanguageManager.Instance;

        public MainForm()
        {
            InitializeUI();
        }

        // 移除InitializeComponent方法调用，因为我们直接在InitializeUI中创建所有控件

        private void InitializeUI()
        {
            // 设置窗口标题
            this.Text = languageManager.GetTranslation("Title");
            this.Size = new Size(600, 400);

            // 创建密码标签和文本框
            Label passwordLabel = new Label
            {
                Text = languageManager.GetTranslation("Password"),
                Location = new Point(20, 20),
                AutoSize = true
            };
            TextBox passwordTextBox = new TextBox
            {
                Location = new Point(100, 17),
                Size = new Size(200, 20),
                PasswordChar = '*'
            };
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);

            // 创建打开文件按钮
            Button openFileButton = new Button
            {
                Text = languageManager.GetTranslation("OpenFileButton"),
                Location = new Point(20, 50),
                Size = new Size(120, 30)
            };
            openFileButton.Click += (sender, e) =>
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "All files (*.*)|*.*|EasySave3 files (*.es3)|*.es3";
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            currentFilePath = openFileDialog.FileName;
                            fileContent = File.ReadAllBytes(currentFilePath);
                            statusLabel.Text = languageManager.GetTranslation("FileOpenedSuccess");
                        }
                        catch (Exception ex)
                        {
                            statusLabel.Text = languageManager.GetTranslation("FileNotFound") + ": " + ex.Message;
                        }
                    }
                }
            };
            this.Controls.Add(openFileButton);

            // 创建解密/解压按钮
            Button decryptButton = new Button
            {
                Text = languageManager.GetTranslation("DecryptButton"),
                Location = new Point(150, 50),
                Size = new Size(120, 30)
            };
            decryptButton.Click += (sender, e) =>
            {
                if (fileContent == null)
                {
                    statusLabel.Text = languageManager.GetTranslation("FileNotFound");
                    return;
                }

                try
                {
                    string password = passwordTextBox.Text;
                    byte[] processedContent = ProcessFile(fileContent, password);
                    fileContent = processedContent;
                    statusLabel.Text = languageManager.GetTranslation("DecryptionSuccess");
                }
                catch (Exception ex)
                {
                    statusLabel.Text = languageManager.GetTranslation("DecryptionFailed") + ": " + ex.Message;
                }
            };
            this.Controls.Add(decryptButton);

            // 创建保存文件按钮
            Button saveFileButton = new Button
            {
                Text = languageManager.GetTranslation("SaveFileButton"),
                Location = new Point(280, 50),
                Size = new Size(120, 30)
            };
            saveFileButton.Click += (sender, e) =>
            {
                if (fileContent == null)
                {
                    statusLabel.Text = languageManager.GetTranslation("FileNotFound");
                    return;
                }

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "All files (*.*)|*.*|EasySave3 files (*.es3)|*.es3";
                    saveFileDialog.RestoreDirectory = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            File.WriteAllBytes(saveFileDialog.FileName, fileContent);
                            statusLabel.Text = languageManager.GetTranslation("FileSavedSuccess");
                        }
                        catch (Exception ex)
                        {
                            statusLabel.Text = languageManager.GetTranslation("FileNotFound") + ": " + ex.Message;
                        }
                    }
                }
            };
            this.Controls.Add(saveFileButton);

            // 创建语言选择下拉框
            Label languageLabel = new Label
            {
                Text = languageManager.GetTranslation("Language"),
                Location = new Point(420, 20),
                AutoSize = true
            };
            ComboBox languageComboBox = new ComboBox
            {
                Location = new Point(490, 17),
                Size = new Size(80, 20)
            };
            foreach (string langCode in languageManager.GetSupportedLanguages())
            {
                languageComboBox.Items.Add(languageManager.GetLanguageDisplayName(langCode));
            }
            languageComboBox.SelectedIndex = 0;
            languageComboBox.SelectedIndexChanged += (sender, e) =>
            {
                string selectedLangName = languageComboBox.SelectedItem.ToString();
                string langCode = GetLanguageCodeByName(selectedLangName);
                if (!string.IsNullOrEmpty(langCode))
                {
                    languageManager.SetLanguage(langCode);
                    UpdateUIText();
                }
            };
            this.Controls.Add(languageLabel);
            this.Controls.Add(languageComboBox);

            // 创建状态标签
            statusLabel = new Label
            {
                Text = languageManager.GetTranslation("Status"),
                Location = new Point(20, 320),
                AutoSize = true
            };
            this.Controls.Add(statusLabel);

            // 创建说明文本框
            TextBox infoTextBox = new TextBox
            {
                Location = new Point(20, 100),
                Size = new Size(550, 200),
                Multiline = true,
                ReadOnly = true,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.None
            };
            infoTextBox.Text = languageManager.GetTranslation("NoPasswordMessage") + Environment.NewLine + 
                              languageManager.GetTranslation("DecryptionNote") + Environment.NewLine + 
                              "\n" + languageManager.GetTranslation("EncryptionType") + ": " + languageManager.GetTranslation("GZip");
            this.Controls.Add(infoTextBox);
        }

        private void UpdateUIText()
        {
            this.Text = languageManager.GetTranslation("Title");
            foreach (Control control in this.Controls)
            {
                if (control is Label)
                {
                    Label label = (Label)control;
                    if (label.Text == "密码") label.Text = languageManager.GetTranslation("Password");
                    else if (label.Text == "语言") label.Text = languageManager.GetTranslation("Language");
                    else if (label.Text.StartsWith("状态: ")) label.Text = languageManager.GetTranslation("Status");
                }
                else if (control is Button)
                {
                    Button button = (Button)control;
                    if (button.Text == "打开文件") button.Text = languageManager.GetTranslation("OpenFileButton");
                    else if (button.Text == "解密/解压") button.Text = languageManager.GetTranslation("DecryptButton");
                    else if (button.Text == "保存文件") button.Text = languageManager.GetTranslation("SaveFileButton");
                }
                else if (control is TextBox && ((TextBox)control).Multiline)
                {
                    TextBox textBox = (TextBox)control;
                    textBox.Text = languageManager.GetTranslation("NoPasswordMessage") + Environment.NewLine + 
                                  languageManager.GetTranslation("DecryptionNote") + Environment.NewLine + 
                                  "\n" + languageManager.GetTranslation("EncryptionType") + ": " + languageManager.GetTranslation("GZip");
                }
            }
        }

        private string GetLanguageCodeByName(string languageName)
        {
            if (languageName == "中文简体") return "zh-CN";
            else if (languageName == "English") return "en-US";
            return "zh-CN";
        }

        private byte[] ProcessFile(byte[] fileData, string password)
        {
            // 尝试作为GZip文件解压
            try
            {
                using (MemoryStream ms = new MemoryStream(fileData))
                {
                    using (GZipStream gs = new GZipStream(ms, CompressionMode.Decompress))
                    {
                        using (MemoryStream outputMs = new MemoryStream())
                        {
                            gs.CopyTo(outputMs);
                            return outputMs.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 如果不是GZip文件，尝试其他处理方式
                // 这里可以根据需要添加其他解密逻辑
                throw new Exception(languageManager.GetTranslation("ProcessingFailed") + ": " + ex.Message);
            }
        }

        private Label statusLabel;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
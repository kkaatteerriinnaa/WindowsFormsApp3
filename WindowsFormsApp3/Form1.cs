using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        private readonly PersonPresenter _presenter;

        public Form1()
        {
            InitializeComponent();
            _presenter = new PersonPresenter(this, listView1); 
        }
        public class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        public interface IPersonView
        {
            string Name { get; set; }
            int Age { get; set; }

            event EventHandler AddPerson;
            event EventHandler SearchPerson;
        }
        public class PersonPresenter
        {
            private readonly IPersonView _view;
            private readonly List<Person> _people;
            private readonly System.Windows.Forms.ListView _listView; // Добавляем поле для хранения ссылки на listView1
            private Form1 form1;
            private System.Windows.Forms.ListView listView1;

            public PersonPresenter(IPersonView view, System.Windows.Forms.ListView listView) // Добавляем параметр для передачи ссылки на listView1
            {
                _view = view;
                _people = new List<Person>();
                _listView = listView; // Сохраняем ссылку на listView1

                _view.AddPerson += AddPerson;
                _view.SearchPerson += SearchPerson;
            }

            public PersonPresenter(Form1 form1, System.Windows.Forms.ListView listView1)
            {
                this.form1 = form1;
                this.listView1 = listView1;
            }

            private void AddPerson(object sender, EventArgs e)
            {
                var person = new Person
                {
                    Name = _view.Name,
                    Age = _view.Age
                };

                _people.Add(person);
            }
            private void SearchPerson(object sender, EventArgs e)
            {
                string searchName = _view.Name;
                var result = _people.FirstOrDefault(person => person.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase));

                _listView.Items.Clear();
                if (result != null) // Проверяем, что result не равен null
                {
                    _listView.Items.Add(result);
                }
            }
            private void SavePeopleToFile(List<Person> people, string filePath)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) // Указываем кодировку UTF-8
                    {
                        foreach (Person person in people)
                        {
                            writer.WriteLine($"{person.Name},{person.Age}");
                        }
                    }

                    MessageBox.Show("Данные успешно сохранены в файл.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}");
                }
            }

            private void Save_Click(object sender, EventArgs e)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    SavePeopleToFile(_people, filePath);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog(); // Создаем объект класса OpenFileDialog
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName; // Получаем путь к выбранному файлу

                    listView1.Items.Clear();
                    using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8)) // Указываем кодировку UTF-8
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split(',');
                            if (parts.Length == 2)
                            {
                                string name = parts[0].Trim();
                                int age;
                                if (int.TryParse(parts[1].Trim(), out age))
                                {
                                    ListViewItem item = new ListViewItem(name);
                                    item.SubItems.Add(age.ToString());
                                    listView1.Items.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) // Обрабатываем исключения
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных: {ex.Message}");
            }
        }
    }
}
﻿using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telhai.CS.APIServer.Models;

namespace Telhai.CS.FinalProject
{
    public class ByteArrayConverter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            short[] sByteArray = JsonSerializer.Deserialize<short[]>(ref reader);
            byte[] value = new byte[sByteArray.Length];
            for (int i = 0; i < sByteArray.Length; i++)
            {
                value[i] = (byte)sByteArray[i];
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var val in value)
            {
                writer.WriteNumberValue(val);
            }

            writer.WriteEndArray();
        }
    }
    /// <summary>
    /// Interaction logic for TeacherWindow.xaml
    /// </summary>
    public partial class TeacherWindow : Window
    {
        private const float MAX_TEST_DURATION = 3f;
        private const float MIN_TEST_DURATION = 1f;
        static int id = 0;
        static int Qid = 0;
      //  bool isNew = false;
        Exam exam;
        private ObservableCollection<Exam> exams = new ObservableCollection<Exam>();
        private ObservableCollection<Question> questionsList = new ObservableCollection<Question>();
        private ObservableCollection<String> answersList = new ObservableCollection<String>();
        public TeacherWindow()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded_1;
            examsList.ItemsSource = exams;
//            this.QuestionsLB.ItemsSource = questionsList;
            exame_Datepicker.SelectedDate = DateTime.Now;
            time_begining.Text = DateTime.Now.ToString("HH:mm");
        }
        //****************************************************************************************************

        private async void btn_AddExam_Click(object sender, RoutedEventArgs e)
        {
            id++;
            string idCounter = id.ToString();
            exam = new Exam() {examName = "Name_" + idCounter };
            await HttpExamRepository.Instance.AddExamAsync(exam);
            //Reload
          //  List<Exam> list = await 
            
        //    isNew = true;
            examsList.Items.Refresh();
        }
        //****************************************************************************************************

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            for (float i = MIN_TEST_DURATION; i <= MAX_TEST_DURATION; i += 0.5f)
            {
                time_duration.Items.Add(i.ToString());
            }
            time_duration.Items.Insert(0, "Choose exam duration");
            time_duration.SelectedIndex = 0;
            examsList.ItemsSource =  HttpExamRepository.Instance.examList;
        }

        //****************************************************************************************************

        private void AddAnswer_btn_Click(object sender, RoutedEventArgs e)
        {
            string newAnswer = Interaction.InputBox("Enter new answer:", "New Answer", "", 0, 0);
            if (newAnswer != "")
            {
                /*Question question = questionsLB.SelectedItem as Question;
                question.AllA.Add(input);
                answersSP.Items.Add(input);
                answersSP.SelectedItem = input;*/

                Question question = QuestionsLB.SelectedItem as Question;
                question.answers.Add(newAnswer);
                AnswersListBox.Items.Add(newAnswer);
                AnswersListBox.SelectedItem = newAnswer;
            }

        }
        //****************************************************************************************************


        public /* List<Exam> */ void allExams()
        {
            //  throw new NotImplementedException();
        }
        //****************************************************************************************************

        private void btnLoadPhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Select a picture";
            openFileDialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
           "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
           "Portable Network Graphic (*.png)|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    if(this.QuestionsLB.SelectedItem is Question q) 
                    {
                        q.QImage = File.ReadAllBytes(openFileDialog.FileName);
                    }
                    
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                //this.QuestionImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }
        //****************************************************************************************************

        public bool IsTimeValid(string timeStr)
        {
            DateTime time;
            return DateTime.TryParseExact(timeStr, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
        }

        //****************************************************************************************************

        public void setTime(ref DateTime dateTime, string timeString)
        {
            TimeSpan timeSpan = TimeSpan.ParseExact(timeString, "g", CultureInfo.InvariantCulture);
            // parse the time string as a TimeSpan object using the "g" format specifier

            // set the time component of the existing dateTime object to the new time
            dateTime = dateTime.Date + timeSpan;
        }
        //****************************************************************************************************

        private void examsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.examsList.SelectedItem is Exam ex)
            {
                QuestionsLB.Items.Clear();
                this.txtExame.Text = ex.examName;
                this.txtID.Text = ex.examId;
                this.txtTeacher.Text = ex.TeacherName;
                this.exame_Datepicker.SelectedDate = ex.date;
                this.time_begining.Text = ex.BeginTime.ToString("HH:mm");
             //   this.isNew = false;

                foreach (var question in ex.questions)
                {
                    QuestionsLB.Items.Add(question);
                }
            }
        }
        //****************************************************************************************************

        private void btn_RemoveExam_Click(object sender, RoutedEventArgs e)
        {
            if (examsList.SelectedItem != null)
            {
                Exam exam = examsList.SelectedItem as Exam;
                //Delete the exam from the detabase

                var response = HttpExamRepository.Instance.RemoveExam(exam).Result;
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("The Exam: " + exam.examName + "was deleted successfuly");
                    exams.Remove(exam);
                    //SeachBTN_Click(sender, e);
                }
                else
                    MessageBox.Show("Error");
            }
        }

        private Exam GetExam()
        {
            return exam;
        }

        //****************************************************************************************************

        private async void submit_Exam_Click(object sender, RoutedEventArgs e)
        {
            // List<Question> questions = new List<Question>();
            foreach (var question in QuestionsLB.Items)
            {
                Question q = question as Question;
                exam.questions.Add(q);
            }
            exam.date = (DateTime)exame_Datepicker.SelectedDate;
            exam.TeacherName = this.txtTeacher.Text;
            exam.examName = this.txtExame.Text;
            DateTime tempExamBeginTime = exame_Datepicker.SelectedDate.Value;
            setTime(ref tempExamBeginTime, this.time_begining.Text);

            exam.BeginTime = DateTime.Parse(time_begining.Text);
            exam.duration = this.time_duration.SelectedIndex;
            exam.isRandom = this.IsRandomCB.IsChecked.Value;
            exam.examId = this.txtID.Text;

            
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://localhost:8080/");
                StringContent reqBody = new StringContent(JsonSerializer.Serialize(exam),Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync("api/exams/add", reqBody);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    MessageBox.Show($" Exam - {exam.examName} has been Created successfuly");
                    Close();
                }
                else
                {
                    string returnValue = response.Content.ReadAsStringAsync().Result;
                    MessageBox.Show($"Failed to POST data: ({response.StatusCode}): {returnValue}");
                }
            
            /*else
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://localhost:7252/");
                StringContent reqBody = new StringContent(JsonSerializer.Serialize(exam), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync("api/exams/" +exam.examId, reqBody);

                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    MessageBox.Show($" Exam - {exam.examName} has been Created successfuly");
                    Close();
                }
                else
                {
                    string returnValue = response.Content.ReadAsStringAsync().Result;
                    MessageBox.Show($"Failed to POST data: ({response.StatusCode}): {returnValue}");
                }
            }*/
        }
        //****************************************************************************************************

        private void time_begining_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsTimeValid(time_begining.Text))
            {
                MessageBox.Show("Exam's begining time needs to be in HH:MM format and valid");
                return;
            }
            DateTime time = exame_Datepicker.SelectedDate.Value;
            setTime(ref time, exame_Datepicker.Text);
        }
        //****************************************************************************************************

        private void QuestionsLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QuestionsLB.SelectedItem != null)
            {
                Question selectedQ = QuestionsLB.SelectedItem as Question;

                textQuestion.Text = selectedQ.content;
                if (selectedQ.QImage != null)
                {
                    this.QuestionImage.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(selectedQ.QImage);
                }

                AnswersListBox.Items.Clear();
                //int idxAns = 0;

                foreach (var answer in selectedQ.answers)
                {
                    AnswersListBox.Items.Add(answer);
                    /*if (answer == selectedQ.A)
                    {
                        AnswersListBox.SelectedIndex = idxAns;
                    }
                    idxAns++;*/
                }
                this.Correct_answer.SelectedValue = selectedQ.correct;

            }
        }

        private void time_begining_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void btn_AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            int numberOfQuestion = QuestionsLB.Items.Count + 1;
            Question question = new Question();
            QuestionsLB.Items.Add(question);
            QuestionsLB.SelectedIndex = numberOfQuestion;
        }

        private void time_duration_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //****************************************************************************************************

    }
}

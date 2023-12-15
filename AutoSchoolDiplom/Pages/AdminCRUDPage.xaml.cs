﻿using DBConnection;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoSchoolDiplom.Pages
{


    public partial class AdminCRUDPage : Page
    {
        public AdminCRUDPage()
        {
            InitializeComponent();

            BindingLvStudents();
            BindingLvCoursStudent();
            BindingLvGroupStudent();

            NameUser.Text = Connection.users.FirstName + " " + Connection.users.LastName + " " + Connection.users.Patronymic;
        }

        public void BindingLvStudents()
        {
            Binding binding = new Binding();
            binding.Source = Connection.infoStudents;
            lvStudents.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectInfoStudent();
        }
        public void BindingLvGroupStudent()
        {
            Binding binding = new Binding();
            binding.Source = Connection.classGroups;
            lvGroupStudent.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectGroup();
        }
        public void BindingLvCoursStudent()
        {
            Binding binding = new Binding();
            binding.Source = Connection.cours;
            lvCoursStudent.SetBinding(ItemsControl.ItemsSourceProperty, binding);
            Connection.SelectCoursStudent();
        }


        private void Filter()
        {
            string searchString = tbSearch.Text.Trim();

            var view = CollectionViewSource.GetDefaultView(lvStudents.ItemsSource);
            view.Filter = new Predicate<object>(o =>
            {
                FullInfoStudent student = o as FullInfoStudent;
                if (student == null) { return false; }

                bool isVisible = true;
                if (searchString.Length > 0)
                {
                    isVisible = student.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        student.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        student.Patronymic.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1 ||
                        student.NumberGroup.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) != -1;
                }
                return isVisible;
            });
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void btnTransitionInstructor_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDInctructor());
            Connection.infoStudents.Clear();
        }

        private void btnTransitionLecturer_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminCRUDLecturer());
            Connection.infoStudents.Clear();
        }

        private void btnTransitionStudent_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы уже находитесь на данной странице");
        }

        private void btnSignOut_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EntryPage());
            Connection.infoStudents.Clear();
        }

        public void ClearingInformationElements()
        {
            lvStudents.SelectedItem = null;
            tbLogin.Clear();
            tbPass.Clear();
            tbFirstName.Clear();
            tbLastName.Clear();
            tbPatronymic.Clear();
            tbPhone.Clear();
            tbEmail.Clear();
            dpDateBirth.Text = null;
            tbImage.Clear();
            lvCoursStudent.SelectedItem= null;
        }

        public void InsertStudentInfo(FullInfoStudent student)
        {
            int coursId = (lvCoursStudent.SelectedItem as CLassCours).Id;
            int groupId = (lvGroupStudent.SelectedItem as ClassGroup).Id;

            var login = tbLogin.Text.Trim();
            var password = tbPass.Text.Trim();
            var firstName = tbFirstName.Text.Trim();
            var lastName = tbLastName.Text.Trim();
            var patronymic = tbPatronymic.Text.Trim();
            var phone = tbPhone.Text.Trim();
            var email = tbEmail.Text.Trim();
            var birth = dpDateBirth.SelectedDate;
            var photo = tbImage.Text.Trim();
            string icon = "C:\\Users\\cloze\\source\\repos\\AutoSchoolDiplom\\AutoSchoolDiplom\\Image\\Icon.png";
            string role = "Студент";


            NpgsqlCommand cmd = Connection.GetCommand("insert into \"User\" (\"Login\", \"Password\", \"FirstName\", \"LastName\", \"Patronymic\", \"Phone\", \"Email\", \"DateBirth\", \"Role\")" +
                 "values (@login, @password, @firstName, @lastName, @patronymic, @phone, @email, @dateBirth, @role) returning \"Id\"");
            cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, login);
            cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, password);
            cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, firstName);
            cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, lastName);
            cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, patronymic);
            cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, phone);
            cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, email);
            cmd.Parameters.AddWithValue("@dateBirth", NpgsqlDbType.Date, birth);
            cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, role);
            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                int IdStudent = student.Id = (int)result;

                cmd = Connection.GetCommand("insert into \"Student\" (\"Id\", \"Photo\", \"Cours\")" +
                 "values (@id, @photo, @cours)");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, IdStudent);
                cmd.Parameters.AddWithValue("@photo", NpgsqlDbType.Varchar, icon);
                cmd.Parameters.AddWithValue("@cours", NpgsqlDbType.Integer, coursId);
                result = cmd.ExecuteNonQuery();
                if(result != null)
                {
                    cmd = Connection.GetCommand("insert into \"StudentGroup\" (\"Student\", \"Group\")" +
                                    "values (@student, @group)");
                    cmd.Parameters.AddWithValue("@student", NpgsqlDbType.Integer, IdStudent);
                    cmd.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, groupId);
                    result = cmd.ExecuteNonQuery();
                }
            }

            if (result == null)
            {
                MessageBox.Show("Данные не добавлены");
            }
            else
            {
                MessageBox.Show("Данные добавлены");
            }
        }

        private void AddLecturer_Click(object sender, RoutedEventArgs e)
        {
            FullInfoStudent student = new FullInfoStudent();
            InsertStudentInfo(student);
            Connection.infoStudents.Clear();
            BindingLvStudents();
        }

        private void DeleteLecturer_Click(object sender, RoutedEventArgs e)
        {
            FullInfoStudent student = lvStudents.SelectedItem as FullInfoStudent;

            NpgsqlCommand cmd = Connection.GetCommand("DELETE FROM \"StudentGroup\" WHERE \"Student\" = @id returning \"Id\"");
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, student.Id);
            cmd.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, );
            var result = cmd.ExecuteScalar();
            if (result != null)
            {
                //int IdStudent = student.Id = (int)result;

                //cmd = Connection.GetCommand("DELETE FROM \"User\" WHERE \"Id\" = @id");
                //cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, IdStudent);
                //cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, student.Login);
                //cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, student.Password);
                //cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, student.FirstName);
                //cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, student.LastName);
                //cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, student.Patronymic);
                //cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, student.Phone);
                //cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, student.Email);
                //cmd.Parameters.AddWithValue("@dateBirth", NpgsqlDbType.Date, student.DateBirth);
                //cmd.Parameters.AddWithValue("@role", NpgsqlDbType.Varchar, student.Role);
                //result = cmd.ExecuteNonQuery();
                //if (result != null)
                //{
                //    Connection.infoStudents.Remove(lvStudents.SelectedItem as FullInfoStudent);
                //    Connection.infoStudents.Clear();
                //    ClearingInformationElements();
                //    BindingLvStudents();
                //}
                //if (result == null)
                //{
                //    MessageBox.Show("Данные не удалены");
                //}
                //else
                //{
                //    MessageBox.Show("Данные удалены");
                //}
            }
        }

        private void UpdateLecturer_Click(object sender, RoutedEventArgs e)
        {
            FullInfoStudent student = lvStudents.SelectedItem as FullInfoStudent;

            int StudentId = (lvStudents.SelectedItem as FullInfoStudent).Id;
            if (lvCoursStudent.SelectedItem == null) { return; }
            int coursId = (lvCoursStudent.SelectedItem as CLassCours).Id;
            if (lvGroupStudent.SelectedItem == null) { return; }
            int groupId = (lvGroupStudent.SelectedItem as ClassGroup).Id;
            

            var login = tbLogin.Text.Trim();
            var password = tbPass.Text.Trim();
            var firstName = tbFirstName.Text.Trim();
            var lastName = tbLastName.Text.Trim();
            var patronymic = tbPatronymic.Text.Trim();
            var phone = tbPhone.Text.Trim();
            var email = tbEmail.Text.Trim();
            var birth = dpDateBirth.SelectedDate;
            var photo = tbImage.Text.Trim();

            NpgsqlCommand cmd = Connection.GetCommand("UPDATE \"StudentGroup\" SET \"Group\"= @group where \"Student\" = @id");
            cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, StudentId);
            cmd.Parameters.AddWithValue("@group", NpgsqlDbType.Integer, groupId);
            var result = cmd.ExecuteNonQuery();
            if (result != 0)
            {
                cmd = Connection.GetCommand("UPDATE \"Student\" SET \"Photo\"= @photo, \"Cours\" = @cours where \"Id\" = @id");
                cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, StudentId);
                cmd.Parameters.AddWithValue("@photo", NpgsqlDbType.Varchar, photo);
                cmd.Parameters.AddWithValue("@cours", NpgsqlDbType.Integer, coursId);
                result = cmd.ExecuteNonQuery();
                if(result != 0)
                {
                    cmd = Connection.GetCommand("UPDATE \"User\" SET \"Login\"= @login, \"Password\"= @password, \"FirstName\"= @firstName, \"LastName\"= @lastName," +
                   "\"Patronymic\"= @patronymic, \"Phone\"= @phone, \"Email\"= @email, \"DateBirth\"= @birth where \"Id\" = @id");
                    cmd.Parameters.AddWithValue("@id", NpgsqlDbType.Integer, StudentId);
                    cmd.Parameters.AddWithValue("@login", NpgsqlDbType.Varchar, login);
                    cmd.Parameters.AddWithValue("@password", NpgsqlDbType.Varchar, password);
                    cmd.Parameters.AddWithValue("@firstName", NpgsqlDbType.Varchar, firstName);
                    cmd.Parameters.AddWithValue("@lastName", NpgsqlDbType.Varchar, lastName);
                    cmd.Parameters.AddWithValue("@patronymic", NpgsqlDbType.Varchar, patronymic);
                    cmd.Parameters.AddWithValue("@phone", NpgsqlDbType.Varchar, phone);
                    cmd.Parameters.AddWithValue("@email", NpgsqlDbType.Varchar, email);
                    cmd.Parameters.AddWithValue("@birth", NpgsqlDbType.Date, birth);
                    result = cmd.ExecuteNonQuery();
                }
            }
            if (result != 0)
            {
                Connection.infoStudents.Clear();
                BindingLvStudents();
                MessageBox.Show("Данные обновлены");
            }
            else
            {
                MessageBox.Show("Данные не обновлены");
            }


        }

        private void DeselectSelection_Click(object sender, RoutedEventArgs e)
        {
            ClearingInformationElements();
        }

        private void lvStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvStudents.SelectedItem != null)
            {
                tbLogin.Text = (lvStudents.SelectedItem as FullInfoStudent).Login.ToString();
                tbPass.Text = (lvStudents.SelectedItem as FullInfoStudent).Password.ToString();
                tbFirstName.Text = (lvStudents.SelectedItem as FullInfoStudent).FirstName.ToString();
                tbLastName.Text = (lvStudents.SelectedItem as FullInfoStudent).LastName.ToString();
                tbPatronymic.Text = (lvStudents.SelectedItem as FullInfoStudent).Patronymic.ToString();
                tbPhone.Text = (lvStudents.SelectedItem as FullInfoStudent).Phone.ToString();
                tbEmail.Text = (lvStudents.SelectedItem as FullInfoStudent).Email.ToString();
                dpDateBirth.Text = (lvStudents.SelectedItem as FullInfoStudent).DateBirth.ToString();
                tbImage.Text = (lvStudents.SelectedItem as FullInfoStudent).Photo.ToString();
            }
        }
    }
}

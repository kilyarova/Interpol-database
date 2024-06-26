﻿using Interpol.Forms;
using Interpol.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interpol
{
    public partial class MainForm : Form
    {
        private Archive archive;

        private List<Criminal> criminalsList;

        public MainForm()
        {
            InitializeComponent();
            archive = Archive.LoadArchive();
            criminalsList = archive.Criminals;

            RefreshCriminalList();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            criminalsList = archive.Criminals;
            RefreshCriminalList();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.Show();
            Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFirstNameFilter.Text = string.Empty;
            txtLastNameFilter.Text = string.Empty;
            txtNicknameFilter.Text = string.Empty;
            cmbGenderFilter.Text = string.Empty;
            txtAgeFromFilter.Text = string.Empty;
            txtAgeToFilter.Text = string.Empty;
            txtCitizenshipFilter.Text = string.Empty;
            txtBirthPlaceFilter.Text = string.Empty;
            txtCrimePlaceFilter.Text = string.Empty;
            dtpCrimeDateFilter.Checked = false;
            cmbCrimeTypeFilter.Text = string.Empty;
            txtHeightFromFilter.Text = string.Empty;
            txtHeightToFilter.Text = string.Empty;
            cmbHairColorFilter.Text = string.Empty;
            cmbEyeColorFilter.Text = string.Empty;
        }

        private void btnAddCriminal_Click(object sender, EventArgs e)
        {
            AddCriminalForm addForm = new AddCriminalForm();
            addForm.ShowDialog();
            RefreshCriminalList();
        }

        private void RefreshCriminalList()
        {
            archive = Archive.LoadArchive();
            criminalsList = archive.Criminals;
            lstCriminals.Items.Clear();
            foreach (var criminal in criminalsList)
            {
                lstCriminals.Items.Add(criminal);
            }
        }

        private void lstCriminals_DoubleClick(object sender, EventArgs e)
        {
            if (lstCriminals.SelectedItem != null)
            {
                var selectedCriminal = (Criminal)lstCriminals.SelectedItem;
                CriminalDetailsForm detailsForm = new CriminalDetailsForm(
                    selectedCriminal, archive);
                detailsForm.ShowDialog();
                RefreshCriminalList();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var selectedCriminal = (Criminal)lstCriminals.SelectedItem;
            if (selectedCriminal == null)
            {
                MessageBox.Show("Злочинця не обрано", "Помилка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EditCriminalForm editForm = new EditCriminalForm(
                selectedCriminal, archive);
            editForm.ShowDialog();
            RefreshCriminalList();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedCriminal = (Criminal)lstCriminals.SelectedItem;
            if (selectedCriminal == null)
            {
                MessageBox.Show("Злочинця не обрано", "Помилка", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show("Ви впевнені, що хочете видалити " +
                "цього злочинця?", "Підтвердження", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                archive.Criminals.Remove(selectedCriminal);
                archive.SaveArchive();
                MessageBox.Show("Злочинця видалено.", "Успіх", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            RefreshCriminalList();
        }

        private void toolStripMenuItemLogout_Click(object sender, 
            EventArgs e)
        {
            var result = MessageBox.Show("Вийти з акаунту?", 
                "Підтвердження", MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var loginForm = new LoginForm();
                loginForm.Show();
                Hide();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вийти з програми?", 
                "Підтвердження", MessageBoxButtons.YesNo, 
                MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Автор програми - Кілярова Анна " +
                "Павлівна. Програма призначена для зберігання інформації " +
                "про зареєстрованих злочинців.", "Про програму", 
                MessageBoxButtons.OK);
        }

        private int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.DayOfYear < birthDate.DayOfYear)
                age--;

            return age;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstNameFilter.Text.Trim();
            string lastName = txtLastNameFilter.Text.Trim();
            string nickname = txtNicknameFilter.Text.Trim();
            string gender = cmbGenderFilter.SelectedItem?.ToString();
            string ageFromStr = txtAgeFromFilter.Text.Trim();
            string ageToStr = txtAgeToFilter.Text.Trim();
            string citizenship = txtCitizenshipFilter.Text.Trim();
            string birthPlace = txtBirthPlaceFilter.Text.Trim();
            string crimePlace = txtCrimePlaceFilter.Text.Trim();
            DateTime? crimeDate = dtpCrimeDateFilter.Checked ? 
                dtpCrimeDateFilter.Value.Date : (DateTime?)null;
            string crimeType = cmbCrimeTypeFilter.SelectedItem?.ToString();
            string heightFromStr = txtHeightFromFilter.Text.Trim();
            string heightToStr = txtHeightToFilter.Text.Trim();
            string hairColor = cmbHairColorFilter.SelectedItem?.ToString();
            string eyeColor = cmbEyeColorFilter.SelectedItem?.ToString();

            bool isAgeFromParsed = int.TryParse(ageFromStr, out int ageFrom);
            bool isAgeToParsed = int.TryParse(ageToStr, out int ageTo);
            bool isHeightFromParsed = int.TryParse(heightFromStr, 
                out int heightFrom);
            bool isHeightToParsed = int.TryParse(heightToStr, 
                out int heightTo);

            FilterCriminals(firstName, lastName, nickname, gender,
                            isAgeFromParsed ? ageFrom : (int?)null,
                            isAgeToParsed ? ageTo : (int?)null,
                            citizenship, birthPlace, crimePlace, crimeDate, 
                            crimeType, isHeightFromParsed ? heightFrom : 
                            (int?)null, isHeightToParsed ? heightTo : (int?)
                            null, hairColor, eyeColor);
        }

        public void FilterCriminals(string firstName, string lastName, 
            string nickname, string gender, int? ageFrom, int? ageTo, 
            string citizenship, string birthPlace, string crimePlace, 
            DateTime? crimeDate, string crimeType, int? heightFrom, 
            int? heightTo, string hairColor, string eyeColor)
        {
            var filters = new List<Func<Criminal, bool>>();

            if (!string.IsNullOrEmpty(firstName))
            {
                filters.Add(c => c.FirstName.IndexOf(firstName, 
                    StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                filters.Add(c => c.LastName.IndexOf(lastName, 
                    StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(nickname))
            {
                filters.Add(c => c.Nickname.IndexOf(nickname, 
                    StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(gender))
            {
                filters.Add(c => c.Gender.Equals(gender, 
                    StringComparison.OrdinalIgnoreCase));
            }

            if (ageFrom.HasValue)
            {
                filters.Add(c => CalculateAge(c.BirthDate) >= ageFrom);
            }

            if (ageTo.HasValue)
            {
                filters.Add(c => CalculateAge(c.BirthDate) <= ageTo);
            }

            if (!string.IsNullOrEmpty(citizenship))
            {
                filters.Add(c => c.Citizenship.IndexOf(citizenship, 
                    StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(birthPlace))
            {
                filters.Add(c => c.BirthPlace.IndexOf(birthPlace, 
                    StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(crimePlace))
            {
                filters.Add(c => c.CrimePlace.IndexOf(crimePlace, 
                    StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (crimeDate.HasValue)
            {
                filters.Add(c => c.CrimeDate.Date == crimeDate.Value);
            }

            if (!string.IsNullOrEmpty(crimeType))
            {
                filters.Add(c => c.CrimeType.Equals(crimeType, 
                    StringComparison.OrdinalIgnoreCase));
            }

            if (heightFrom.HasValue)
            {
                filters.Add(c => c.Height >= heightFrom);
            }

            if (heightTo.HasValue)
            {
                filters.Add(c => c.Height <= heightTo);
            }

            if (!string.IsNullOrEmpty(hairColor))
            {
                filters.Add(c => c.HairColor.Equals(hairColor, 
                    StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(eyeColor))
            {
                filters.Add(c => c.EyeColor.Equals(eyeColor, 
                    StringComparison.OrdinalIgnoreCase));
            }

            var filteredCriminals = criminalsList.Where(c => 
            filters.All(filter => filter(c))).ToList();

            lstCriminals.Items.Clear();

            foreach (var criminal in filteredCriminals)
            {
                lstCriminals.Items.Add(criminal);
            }
        }

        private void OnlyDigit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Delete)
            {
                e.Handled = true;
            }
        }

        private void OnlyLetter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Delete)
            {
                e.Handled = true;
            }
        }

        private void OnlyLetterOrHyphenOrSpace_KeyPress(object sender, 
            KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) && e.KeyChar != '-' && 
                e.KeyChar != ' ' && e.KeyChar != (char)Keys.Delete)
            {
                e.Handled = true;
            }
        }
    }
}
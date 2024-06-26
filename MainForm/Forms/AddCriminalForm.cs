﻿using Interpol.Models;
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

namespace Interpol.Forms
{
    public partial class AddCriminalForm : Form
    {
        private string PhotoFileName = "";

        public AddCriminalForm()
        {
            InitializeComponent();

            cmbGender.DropDownStyle = ComboBoxStyle.DropDown;
            cmbHairColor.DropDownStyle = ComboBoxStyle.DropDown;
            cmbEyeColor.DropDownStyle = ComboBoxStyle.DropDown;
            cmbCrimeType.DropDownStyle = ComboBoxStyle.DropDown;

            txtFirstName.TextChanged += new EventHandler(Field_TextChanged);
            txtLastName.TextChanged += new EventHandler(Field_TextChanged);
            dtpBirthDate.ValueChanged += new EventHandler(Field_TextChanged);
            cmbGender.TextChanged += new EventHandler(Field_TextChanged);
            cmbHairColor.TextChanged += new EventHandler(Field_TextChanged);
            cmbEyeColor.TextChanged += new EventHandler(Field_TextChanged);
            cmbCrimeType.TextChanged += new EventHandler(Field_TextChanged);
            txtHeight.TextChanged += new EventHandler(Field_TextChanged);
            pictureBoxPhoto.LoadCompleted += new AsyncCompletedEventHandler(
                Field_TextChanged);
            btnAdd.Enabled = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo file = new FileInfo(openFileDialog.FileName);
                    file.CopyTo(Guid.NewGuid().ToString() + "." + file.Extension);
                    PhotoFileName = file.FullName;
                    pictureBoxPhoto.Image = Image.FromFile(openFileDialog.FileName);
                    pictureBoxPhoto.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            ValidateForm();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!IsFormValid())
            {
                MessageBox.Show("Усі поля обов'язкові для заповнення.",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var newCriminal = new Criminal
            {
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Nickname = txtNickname.Text,
                Gender = cmbGender.Text,
                BirthDate = dtpBirthDate.Value,
                BirthPlace = txtBirthPlace.Text,
                LastResidence = txtLastResidence.Text,
                Citizenship = txtCitizenship.Text,
                Height = int.Parse(txtHeight.Text),
                HairColor = cmbHairColor.Text,
                EyeColor = cmbEyeColor.Text,
                SpecialFeatures = txtSpecialFeatures.Text,
                CrimeType = cmbCrimeType.Text,
                CrimeDate = dtpCrimeDate.Value,
                CrimePlace = txtCrimePlace.Text,
                CourtDecision = txtCourtDecision.Text,
                PhotoPath = PhotoFileName
            };

            Archive archive = Archive.LoadArchive();
            archive.Criminals.Add(newCriminal);
            archive.SaveArchive();

            MessageBox.Show("Злочинця додано до архіву.", "Успіх",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private bool IsFormValid()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                !dtpBirthDate.Checked ||
                string.IsNullOrWhiteSpace(cmbGender.Text) ||
                string.IsNullOrWhiteSpace(cmbHairColor.Text) ||
                string.IsNullOrWhiteSpace(cmbEyeColor.Text) ||
                string.IsNullOrWhiteSpace(cmbCrimeType.Text) ||
                pictureBoxPhoto.Image == null)
            {
                return false;
            }

            if (!int.TryParse(txtHeight.Text, out _))
            {
                return false;
            }

            return true;
        }

        private void Field_TextChanged(object sender, EventArgs e)
        {
            ValidateForm();
        }

        private void ValidateForm()
        {
            btnAdd.Enabled = IsFormValid();
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
            if (char.IsDigit(e.KeyChar) && e.KeyChar != '-' && e.KeyChar
                != ' ' && e.KeyChar != (char)Keys.Delete)
            {
                e.Handled = true;
            }
        }
    }
}
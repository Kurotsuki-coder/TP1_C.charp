using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Serilog;

namespace GestionEtudiantWinForms
{
    public class Etudiant
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
    }

    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private const string apiUrl = "https://localhost:7264/api/etudiants";

        public Form1()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Log.Information("Application WinForms démarrée");
            await ChargerEtudiants();
        }

        private async Task ChargerEtudiants()
        {
            try
            {
                Log.Information("Chargement de la liste des étudiants");
                var response = await client.GetStringAsync(apiUrl);
                var liste = JsonConvert.DeserializeObject<List<Etudiant>>(response);
                dataGridView1.DataSource = liste;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors du chargement des étudiants");
                MessageBox.Show(
                    "Erreur lors du chargement : " + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async void btnAjouter_Click(object sender, EventArgs e)
        {
            try
            {
                var etudiant = new Etudiant()
                {
                    Nom = txtNom.Text,
                    Prenom = txtPrenom.Text,
                    Email = txtEmail.Text
                };

                var json = JsonConvert.SerializeObject(etudiant);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                Log.Information("Étudiant ajouté avec succès : {Nom} {Prenom}", etudiant.Nom, etudiant.Prenom);
                MessageBox.Show("Étudiant ajouté avec succès");
                ViderChamps();
                await ChargerEtudiants();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de l'ajout de l'étudiant");
                MessageBox.Show(
                    "Erreur lors de l'ajout : " + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async void btnModifier_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Sélectionnez d'abord un étudiant dans la liste.");
                    return;
                }

                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);

                var etudiant = new Etudiant()
                {
                    Id = id,
                    Nom = txtNom.Text,
                    Prenom = txtPrenom.Text,
                    Email = txtEmail.Text
                };

                var json = JsonConvert.SerializeObject(etudiant);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PutAsync(apiUrl + "/" + id, content);
                response.EnsureSuccessStatusCode();

                Log.Information("Étudiant {Id} modifié avec succès", id);
                MessageBox.Show("Étudiant modifié avec succès");
                ViderChamps();
                await ChargerEtudiants();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la modification de l'étudiant");
                MessageBox.Show(
                    "Erreur lors de la modification : " + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private async void btnSupprimer_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.CurrentRow == null)
                {
                    MessageBox.Show("Sélectionnez d'abord un étudiant dans la liste.");
                    return;
                }

                int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["Id"].Value);

                var confirm = MessageBox.Show(
                    "Voulez-vous vraiment supprimer cet étudiant ?",
                    "Confirmation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm != DialogResult.Yes)
                    return;

                var response = await client.DeleteAsync(apiUrl + "/" + id);
                response.EnsureSuccessStatusCode();

                Log.Information("Étudiant {Id} supprimé avec succès", id);
                MessageBox.Show("Étudiant supprimé avec succès");
                ViderChamps();
                await ChargerEtudiants();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la suppression de l'étudiant");
                MessageBox.Show(
                    "Erreur lors de la suppression : " + ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;
            if (e.RowIndex < 0) return;

            txtNom.Text = dataGridView1.CurrentRow.Cells["Nom"].Value?.ToString();
            txtPrenom.Text = dataGridView1.CurrentRow.Cells["Prenom"].Value?.ToString();
            txtEmail.Text = dataGridView1.CurrentRow.Cells["Email"].Value?.ToString();
        }

        private void ViderChamps()
        {
            txtNom.Text = "";
            txtPrenom.Text = "";
            txtEmail.Text = "";
        }
    }
}
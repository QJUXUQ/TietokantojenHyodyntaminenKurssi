using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

namespace Opiskelijat
{
    public partial class Form1 : Form
    {
        SqlConnection dbconnection = new SqlConnection(@"Data Source=" + @"(localdb)\MSSQLLocalDB" + ";Initial Catalog=Opiskelijat");
        List<Ryhma> opiskelijaryhma = new List<Ryhma>();

        public Form1()
        {

            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            if (dbconnection.State != ConnectionState.Open)
            {
                dbconnection.Open();
            }
            GetStudents();
            GetGroups();



        }
        private void GetStudents()
        {
            
            List<Opiskelijat> opiskelijanimet = new List<Opiskelijat>();

            dataGridView1.Columns.Clear();
            opiskelijanimet.Clear();

            using SqlCommand opiskelija = new SqlCommand("SELECT * FROM Opiskelija", dbconnection);
            using var reader = opiskelija.ExecuteReader();
            while (reader.Read())
            {
                Opiskelijat nimet = new Opiskelijat()
                {
                    OpiskelijaName = reader["Nimi"].ToString(),
                    OpiskelijaLastName = reader["Sukunimi"].ToString(),
                    OpiskelijaId = Convert.ToInt32(reader["Id"])
                };
                opiskelijanimet.Add(nimet);
            }
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Add("Id","Id");
            dataGridView1.Columns["Id"].DataPropertyName = "OpiskelijaId";
            dataGridView1.Columns["Id"].Visible = false;
            dataGridView1.Columns.Add("nimi", "Etunimi");
            dataGridView1.Columns.Add("sukunimi", "Sukunimi");
            dataGridView1.Columns["nimi"].DataPropertyName = "OpiskelijaName";
            dataGridView1.Columns["sukunimi"].DataPropertyName = "OpiskelijaLastName";
            dataGridView1 .DataSource = opiskelijanimet;    
        }
        private void GetGroups()
        {

            using SqlCommand ryhmat = new SqlCommand("SELECT * FROM Ryhma", dbconnection);
            using var reader = ryhmat.ExecuteReader();
            while (reader.Read())
            {
                Ryhma ryhmanimet = new Ryhma()
                {
                    ryhmanimi = reader["Nimi"].ToString(),
                    ID = Convert.ToInt32(reader["Id"])
                };
                opiskelijaryhma.Add(ryhmanimet);
            }
            for (int i = 0; i < opiskelijaryhma.Count; i++)
            {
                comboBox1.Items.Add(opiskelijaryhma[i].ryhmanimi);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {


            Opiskelijat buttonadd = new Opiskelijat();
            string Etunimi = textBox1.Text;
            string Sukunimi = textBox2.Text;
            string ryhmalista = comboBox1.Text;
            int newId;
            Ryhma matchRyhma = opiskelijaryhma.FirstOrDefault(r => r.ryhmanimi == ryhmalista);
            using (SqlTransaction transaction = dbconnection.BeginTransaction())
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = dbconnection;
                    cmd.Transaction = transaction;
                    try
                    {

                        cmd.CommandText = "INSERT INTO Opiskelija(Nimi,Sukunimi) OUTPUT INSERTED.Id VALUES (@Etunimi,@Sukunimi)";
                        cmd.Parameters.AddWithValue("@Etunimi", Etunimi);
                        cmd.Parameters.AddWithValue("@Sukunimi", Sukunimi);
                        newId = (int)cmd.ExecuteScalar();

                        cmd.CommandText = "INSERT INTO OpiskelijanRyhma(OpiskelijaId,RyhmaId) VALUES (@oppilasid,@ryhmaid)";
                        cmd.Parameters.AddWithValue("@oppilasid", newId);
                        cmd.Parameters.AddWithValue("@ryhmaid", matchRyhma.ID);
                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                        MessageBox.Show("Uusi opiskelija lisätty!");
                    }
                    catch (Exception ex)
                    {
                        cmd.Transaction.Rollback();
                        throw ex;
                    }
                }
            }

            Form1_Load(sender, e);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 1)
            {
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                if (selectedRow.DataBoundItem is Opiskelijat opiskelijat)
                {
                    using (SqlCommand cmd = new SqlCommand("DELETE FROM Opiskelija WHERE Id = @Id", dbconnection))
                    {
                        cmd.Parameters.AddWithValue("@Id", opiskelijat.OpiskelijaId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            else 
            {
                MessageBox.Show("Olet valinnut liikaa tai liian vähän poistettavaa");
            }
            Form1_Load(sender, e);
        }
    }
}
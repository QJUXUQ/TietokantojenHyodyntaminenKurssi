using Auto.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;



namespace Autokauppa.model
{
    public class DatabaseHallinta
    {

        SqlConnection dbYhteys = new SqlConnection(@"Data Source=" + @"(localdb)\MSSQLLocalDB ;Initial Catalog=Autokauppa"); //tietokannan sijainti

        List<AutoMalli> kaikkiMallit = new List<AutoMalli>(); //tekee listan automalleista
        

        public bool connectDatabase() //tarkistaa saako yhteyden tietokantaan
        {


            try
            {
                using (dbYhteys)
                {
                    dbYhteys.Open(); //yrittää yhteyttä 
                }




                return true;
            }
            catch (Exception e) //jos ei onnistu
            {
                Console.WriteLine("Virheilmoitukset:" + e);
                dbYhteys.Close();
                return false;

            }

        }

        public void disconnectDatabase() //tietokannan yhteyden sulkeminen
        {
            if (dbYhteys.State == ConnectionState.Open)
            {
                dbYhteys.Close();
            }
            else
            {
                Console.WriteLine("Ei yhteyttä suljettavaksi.");
            }

        }

        public bool saveAutoIntoDatabase(Auto newAuto) //palauttaa boolin jos tallennus onnistui
        {
            connectionOpen();
            using (SqlTransaction transaction = dbYhteys.BeginTransaction()) //aloittaa uuden transactionin tietokantaan
            {
                using (SqlCommand cmd = new SqlCommand()) //luo uuden SQL komennon
                {
                    cmd.Connection = dbYhteys;
                    cmd.Transaction = transaction; //yhdistetään transactioon ja tietokantayhteyteen
                    try
                    {
                        cmd.CommandText = "INSERT INTO auto (Hinta,Rekisteri_paivamaara,Moottorin_tilavuus,Mittarilukema,AutonMerkkiID,AutonMalliID,VaritID,PolttoaineID) VALUES (@Hinta,@Rekisteri_paivamaara,@Moottorin_tilavuus,@Mittarilukema,@AutonMerkkiID,@AutonMalliID,@VaritID,@PolttoaineID)";
                        cmd.Parameters.AddWithValue("@Hinta", newAuto.hinta);
                        cmd.Parameters.AddWithValue("@Rekisteri_paivamaara", newAuto.rekisteripaivays);
                        cmd.Parameters.AddWithValue("@Moottorin_tilavuus", newAuto.moottoritila);
                        cmd.Parameters.AddWithValue("@Mittarilukema", newAuto.mittariluku);
                        cmd.Parameters.AddWithValue("@AutonMerkkiID", newAuto.MerkkiId);
                        cmd.Parameters.AddWithValue("@AutonMalliID", newAuto.MalliId);
                        cmd.Parameters.AddWithValue("@VaritID", newAuto.VariId);
                        cmd.Parameters.AddWithValue("@PolttoaineID", newAuto.PolttoaineId); //asettaa tiedot paikoilleensa
                        cmd.ExecuteNonQuery(); //suorittaa insert into lauseen

                        transaction.Commit(); //palauttaa true jos onnistuu
                        
                        disconnectDatabase();

                        return true;



                    }
                    catch (Exception e)
                    {
                        transaction.Rollback(); //peruu transactionin
                        throw e;
                        return false;
                    }
                }
            }



        }

        public List<AutoMerkki> getAllAutoMakersFromDatabase() //hakee auton tekijät
        {
            connectionOpen();
            List<AutoMerkki> palaute = new List<AutoMerkki>(); //uusi automerkki lista
            if (dbYhteys.State == ConnectionState.Open)
            {
                using SqlCommand cmd = new SqlCommand("SELECT * FROM AutonMerkki", dbYhteys);
                var dataadapter = new SqlDataAdapter(cmd);
                using var reader = cmd.ExecuteReader();
                while (reader.Read()) //lukee automerkki haun tulokset ja lisää ne listaan
                {
                    AutoMerkki merkki = new AutoMerkki()
                    {
                        MerkkiId = Convert.ToInt32(reader["ID"]), // asettaa tietokannan automerkin tiedot automerkin listaan
                        MerkkiNimi = reader["Merkki"].ToString()

                    };
                    palaute.Add(merkki); //lisätään listaan
                }
            }
            else { Console.WriteLine("Virhe tekijöiden haussa."); }

            disconnectDatabase();
            return palaute; //palauttaa automerkkilistan 


        }


        public List<AutoMalli> getAutoModelsByMakerId(int makerId) //ottaa argumentiksi auton tekijän id

        {
            List<AutoMalli> palaute = new List<AutoMalli>();
            foreach (AutoMalli malli in kaikkiMallit) //katsooautomallin merkki id sama kuin automallin merkki id
            {
                if (malli.autonmerkkiId == makerId) //jos on sama merkki id molemmilla
                {
                    palaute.Add(malli);
                } else if (makerId==0) //palauttaa kaikki jos on 0
                {
                    palaute.Add(malli);
                }
            
    
            }

            return palaute;

        }
        public void GetCarModels() //haetaan automallit
        {
            kaikkiMallit.Clear(); //tyhjennetään lista ettei tule kopioita
            connectionOpen();


            try
            {
                using SqlCommand cmd = new SqlCommand("SELECT * FROM AutonMallit", dbYhteys);
                var dataadapter = new SqlDataAdapter(cmd);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    AutoMalli malli = new AutoMalli()
                    {
                        malliId = Convert.ToInt32(reader["ID"]),
                        malliNimi = reader["Auton_mallin_nimi"].ToString(),
                        autonmerkkiId = Convert.ToInt32(reader["AutonMerkkiID"])

                    };
                    kaikkiMallit.Add(malli);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Mallien haku ei onnistunut");
                throw ex;
            }
            disconnectDatabase();

        }
        private void connectionOpen() //avataan yhteys
        {
            if (dbYhteys.State == ConnectionState.Closed)
            {
                dbYhteys.Open();
            }
        }

       public List<Vari> GetAllColours() 
        {
            connectionOpen();
            List<Vari> colours = new List<Vari>();
            if (dbYhteys.State == ConnectionState.Open)
            {
                using SqlCommand cmd = new SqlCommand("SELECT * FROM Varit", dbYhteys);
                var dataadapter = new SqlDataAdapter(cmd);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Vari colour = new Vari()
                    {
                        variId = Convert.ToInt32(reader["ID"]),
                        variNimi = reader["Varin_nimi"].ToString()

                    };
                    colours.Add(colour);
                }
            }
            else { Console.WriteLine("Virhe värien haussa."); }

            disconnectDatabase() ;

            return colours;
        }

        public List<Polttoaine> GetAllGas()
        {
            connectionOpen();
            List<Polttoaine> poltto = new List<Polttoaine>();
            if (dbYhteys.State == ConnectionState.Open)
            {
                using SqlCommand cmd = new SqlCommand("SELECT * FROM Polttoaine", dbYhteys);
                var dataadapter = new SqlDataAdapter(cmd);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Polttoaine gas = new Polttoaine()
                    {
                        polttoId = Convert.ToInt32(reader["ID"]),
                        polttoaineNimi = reader["Polttoaineen_nimi"].ToString()

                    };
                    poltto.Add(gas);
                }
            }
            else { Console.WriteLine("Virhe polttoaineiden haussa."); }

            disconnectDatabase();
            return poltto;
        }

        public List<Auto> GetAllCars() //haetaan kaikki autot
        {
            connectionOpen();
            List<Auto> auto = new List<Auto>();
            if (dbYhteys.State == ConnectionState.Open)
            {
                using SqlCommand cmd = new SqlCommand("SELECT * FROM auto ORDER BY Hinta, ID ASC", dbYhteys); //haetaan hinnan mukaan halvimmasta suurempaan
                var dataadapter = new SqlDataAdapter(cmd);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Auto car = new Auto()
                    {
                        autoId = Convert.ToInt32(reader["ID"]),
                        hinta = Convert.ToDecimal(reader["Hinta"]),
                        rekisteripaivays= Convert.ToDateTime(reader["Rekisteri_paivamaara"]),
                        moottoritila= Convert.ToDecimal(reader["Moottorin_tilavuus"]),
                        mittariluku = Convert.ToInt32(reader["Mittarilukema"]),
                        MerkkiId = Convert.ToInt32(reader["AutonMerkkiID"]),
                        MalliId = Convert.ToInt32(reader["AutonMalliID"]),
                        VariId = Convert.ToInt32(reader["VaritID"]),
                        PolttoaineId = Convert.ToInt32(reader["PolttoaineID"]),

                    };
                    auto.Add(car);
                }
            }
            else { Console.WriteLine("Virhe autojen haussa."); }

            disconnectDatabase();
            return auto;
        }

    }
}

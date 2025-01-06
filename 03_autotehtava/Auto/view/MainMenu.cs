using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Autokauppa.model;
using Autokauppa.controller;
using Auto.model;

namespace Autokauppa.view
{
    public partial class MainMenu : Form
    {
        string tbHintaDefTxt = "Hinta"; //tekstiboksin aloitusteksti
        string tbTilavuusDefTxt = "Moottorin tilavuus";
        string tbMittarilukemaDefTxt = "Mittarilukema";
        decimal Hinta; //käyttäjän asettamat tiedot käsittelyä varten -->v
        DateTime rekPaiva;
        decimal moottorinTilavuus;
        int mittarinLukema;
        int merkkiId;
        int malliId;
        int variId;
        int polttoaineId;
        List<Autokauppa.model.Auto> kaikkiAutot;
        int bb = -1;

        KaupanLogiikka registerHandler; //kauppalogiikan olio nimeltä registerhandler

        public MainMenu()
        {
            registerHandler = new KaupanLogiikka();
            InitializeComponent();


        }

        private void btnLisaa_Click(object sender, EventArgs e) //lisätään auton tiedot
        {
            if (AreFieldsFull()) //tarkistaa onko muutokset tehty
            {
                Autokauppa.model.Auto auto = new Autokauppa.model.Auto()
                {
                    PolttoaineId = polttoaineId, //asettaa käyttäjän antamat tiedot auton tietoihin 
                    VariId = variId,
                    MalliId = malliId,
                    MerkkiId = merkkiId,
                    mittariluku = mittarinLukema,
                    moottoritila = moottorinTilavuus,
                    rekisteripaivays = rekPaiva,
                    hinta = Hinta

                };
                registerHandler.saveAuto(auto); //asettaa autoluokan olion lisätyillä tiedoilla
                btnTyhjays_Click(sender, e); //tyhjentää kaikki kentät
            }
            else { MessageBox.Show("Tallennus epäonnistui"); }

        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cbMerkki_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbMalli.Items.Clear(); //tyhjentää mallin comboboksin
            AutoMerkki autoMerkki = registerHandler.getAllAutoMakers().FirstOrDefault(a => a.MerkkiNimi == cbMerkki.Text); //käyttää Linq hakutapaa jossa etsitään automerkeistä merkkinimeä mikä vastaa comboboksissa olevaa nimeä
            merkkiId = autoMerkki.MerkkiId;
            foreach (AutoMalli malli in registerHandler.getAutoModels(autoMerkki.MerkkiId)) //autonmerkin nimellä hakee autonmerkin id jolla haetaan auton mallit sille kuuluvalle merkille
            {
                cbMalli.Items.Add(malli.malliNimi); //lisää saatujen mallien nimet cbmalli boksiin
            }
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

            tbHinta.Text = tbHintaDefTxt; //asetetaan aloitustekstit kyseisille tekstibokseille -->v
            tbTilavuus.Text = tbTilavuusDefTxt;
            tbMittarilukema.Text = tbMittarilukemaDefTxt;
            registerHandler.GetModels(); //hakee auton mallit
            foreach (var merkki in registerHandler.getAllAutoMakers()) //hakee kaikki auton merkit 
            {
                cbMerkki.Items.Add(merkki.MerkkiNimi);
            }
            foreach (var malli in registerHandler.getAutoModels(0)) //ottaa argumentikseen 0 koska sillä palautetaan kaikki auton mallit
            {
                cbMalli.Items.Add(malli.malliNimi);
            }
            foreach (var colour in registerHandler.getAllVari()) //värit --->v
            {
                cbVari.Items.Add(colour.variNimi);
            }
            foreach (var poltto in registerHandler.getAllPolttoaine())
            {
                cbPolttoaine.Items.Add(poltto.polttoaineNimi);
            }
            kaikkiAutot = registerHandler.getAllAuto(); //hakee kaikki autot tietokannasta ja pistää ne listaan kaikkiautot

        }

        private void testaaTietokantaaToolStripMenuItem_Click(object sender, EventArgs e) //kokeillaan, onko yhteys tietokantaan
        {
            if (registerHandler.TestDatabaseConnection())
            {
                MessageBox.Show("Yhteys on luotu.");
            }
            else
            {
                MessageBox.Show("Yhteys on jo saatu.");
            }
        }

        private void tbId_TextChanged(object sender, EventArgs e)
        {

        }

        private void tbHinta_TextChanged(object sender, EventArgs e)
        {
            if (Decimal.TryParse(tbHinta.Text, out decimal val)) //jos laitettu teksti kelpaa decimaaliluvuksi laitetaan se hinnaksi --->v
            {
                Hinta = val;
            }

        }

        private void tbHinta_GotFocus(object sender, EventArgs e) //tyhjentää boksin jos siinä on asetettu alkuarvo --->v
        {
            if (tbHinta.Text == tbHintaDefTxt)
            {
                tbHinta.Text = string.Empty;
            }
        }

        private void tbTilavuus_TextChanged(object sender, EventArgs e)
        {
            if (Decimal.TryParse(tbTilavuus.Text, out decimal val))
            {
                moottorinTilavuus = val;
            }
        }
        private void tbTilavuus_GotFocus(object sender, EventArgs e)
        {
            if (tbTilavuus.Text == tbTilavuusDefTxt)
            {
                tbTilavuus.Text = string.Empty;
            }
        }
        private void tbMittarilukema_TextChanged(object sender, EventArgs e)
        {
            if (Int32.TryParse(tbMittarilukema.Text, out int val)) //kokonaislukuna kelpaava
            {
                mittarinLukema = val;
            }
        }
        private void tbMittarilukema_GotFocus(object sender, EventArgs e)
        {
            if (tbMittarilukema.Text == tbMittarilukemaDefTxt)
            {
                tbMittarilukema.Text = string.Empty;
            }
        }
        private void cbMalli_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoMalli autoMalli = registerHandler.getAutoModels(0).FirstOrDefault(a => a.malliNimi == cbMalli.Text); //----^ merkkiId
            malliId = autoMalli.malliId;
        }

        private void cbVari_SelectedIndexChanged(object sender, EventArgs e)
        {
            Vari vari = registerHandler.getAllVari().FirstOrDefault(a => a.variNimi == cbVari.Text);
            variId = vari.variId;
        }

        private void cbPolttoaine_SelectedIndexChanged(object sender, EventArgs e)
        {
            Polttoaine polttoaine = registerHandler.getAllPolttoaine().FirstOrDefault(a => a.polttoaineNimi == cbPolttoaine.Text);
            polttoaineId = polttoaine.polttoId;
        }

        private void dtpPaiva_ValueChanged(object sender, EventArgs e) //ottaa päivämäärän ja asettaa sen rekisteripäiväksi, vaatii että päivämäärä on muutettu rekisteröimiseen
        {
            rekPaiva = dtpPaiva.Value.Date;
        }

        private void btnTyhjays_Click(object sender, EventArgs e) //tyhjentää kaikki boksit
        {
            cbMalli.Text = string.Empty;
            cbMerkki.Text = string.Empty;
            cbPolttoaine.Text = string.Empty;
            cbVari.Text = string.Empty;
            tbHinta.Text = string.Empty;
            tbId.Text = string.Empty;
            tbTilavuus.Text = string.Empty;
            tbMittarilukema.Text = string.Empty;
        }
        private bool AreFieldsFull() //tarkistaa onko käyttäjä lisännyt tietoa kaikkiin bokseihin
        {
            if (cbPolttoaine.Text != string.Empty &&
                cbMerkki.Text != string.Empty &&
            cbVari.Text != string.Empty &&
            cbMalli.Text != string.Empty &&
            tbHinta.Text != string.Empty && tbHinta.Text != tbHintaDefTxt && //ei ole tyhjä tai asetettu arvo
            tbMittarilukema.Text != string.Empty && tbMittarilukema.Text != tbMittarilukemaDefTxt &&
            tbTilavuus.Text != string.Empty && tbTilavuus.Text != tbTilavuusDefTxt) { return true; }
            else { return false; } //jos ei ole kaikki arvot täytetty, palauttaa false
        }

        private void btnSeuraava_Click(object sender, EventArgs e) //näyttää seuraavan auton autolistassa
        {
            bb++;
            if (bb == kaikkiAutot.Count) //jos on listan lopussa, niin näyttää ensimmäisen auton
            {
                bb = 0;
            }
            naytaAuto();
        }

        private void btnEdellinen_Click(object sender, EventArgs e) //näyttää edelliset autot
        {
            bb--;
            if (bb < 0)
            {
                bb = kaikkiAutot.Count - 1;
            }
            naytaAuto() ;
           
        }
        private void naytaAuto() //näyttää haettujen autojen tiedot
        {
            AutoMerkki autoMerkki = registerHandler.getAllAutoMakers().FirstOrDefault(a => a.MerkkiId == kaikkiAutot[bb].MerkkiId);
            cbMerkki.Text = autoMerkki.MerkkiNimi.ToString();
            AutoMalli autoMalli = registerHandler.getAutoModels(0).FirstOrDefault(a => a.malliId == kaikkiAutot[bb].MalliId);
            cbMalli.Text = autoMalli.malliNimi.ToString();
            Polttoaine polttoaine = registerHandler.getAllPolttoaine().FirstOrDefault(a => a.polttoId == kaikkiAutot[bb].PolttoaineId);
            cbPolttoaine.Text = polttoaine.polttoaineNimi.ToString();
            Vari vari = registerHandler.getAllVari().FirstOrDefault(a => a.variId == kaikkiAutot[bb].VariId);
            cbVari.Text = vari.variNimi.ToString();
            tbHinta.Text = kaikkiAutot[bb].hinta.ToString();
            tbId.Text = kaikkiAutot[bb].autoId.ToString();
            tbTilavuus.Text = kaikkiAutot[bb].moottoritila.ToString();
            tbMittarilukema.Text = kaikkiAutot[bb].mittariluku.ToString();
            dtpPaiva.Text = kaikkiAutot[bb].rekisteripaivays.ToString();
        }
    }

}


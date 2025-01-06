using Autokauppa.model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Auto.model;


namespace Autokauppa.controller
{

    
    public class KaupanLogiikka
    {
        DatabaseHallinta dbModel = new DatabaseHallinta();

        public bool TestDatabaseConnection() //testaa yhteyttä
        {
            bool doesItWork = dbModel.connectDatabase();
            return doesItWork;
        }

        public bool saveAuto(model.Auto newAuto) //kutsuu databasehallinnasta, on välikätenä mainmenu.cs
        {
            bool didItGoIntoDatabase = dbModel.saveAutoIntoDatabase(newAuto);
            return didItGoIntoDatabase;
        }

        public List<AutoMerkki> getAllAutoMakers() {

            return dbModel.getAllAutoMakersFromDatabase();
        }

        public List<AutoMalli> getAutoModels(int makerId) {

            return dbModel.getAutoModelsByMakerId(makerId);
        }
        public void GetModels() 
        {
            dbModel.GetCarModels();
        }

        public List<Vari> getAllVari() 
        {
            return dbModel.GetAllColours();
        }

        public List<Polttoaine> getAllPolttoaine() 
        {
           return dbModel.GetAllGas();
        }
        public List<Autokauppa.model.Auto> getAllAuto() 
        {
            return dbModel.GetAllCars();
        }
    }
}

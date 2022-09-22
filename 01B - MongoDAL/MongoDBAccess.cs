using MongoDB.Driver;
using MongoDB.Bson;

using System.Collections;

//Data access layer (Mongo)
//mongo DB
namespace DB3
{
    class AidIDFinderClass {
        public string _id { get; set; }
        public int value { get; set; }

        public int getValue()
        {
            return value;
        }
    }

    public class MongoDBAccess : BaseDB
    {
        private string connectionString = "mongodb+srv://robinhood:fuckoff@task3.pg46pi5.mongodb.net/?retryWrites=true&w=majority";

        
        //database connect
        private IMongoDatabase dbConnect()
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            var database = client.GetDatabase("IcecreamDBM");
            return database;
        }
        public bool createTables()
        {
            try
            {
                deleteAllData();
                addIDinitilizer("Dishes");
                addIDinitilizer("Ingridents");
                addIDinitilizer("Sales");
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        private void deleteAllData()
        {
            var database = dbConnect();
            var dbList = database.ListCollectionNames().ToList();
            foreach (var db in dbList)
            {
                var collection = database.GetCollection<BsonDocument>(db.ToString());
                var filter = Builders<BsonDocument>.Filter.Empty;
                var result = collection.DeleteMany(filter);
            }
            
        }

        public int insertObject(Object obj)
        {
            int x = -1;
            if (obj is Sale)
            {
                x=insertSale((Sale)obj);
            }
            else if(obj is Ingrident)
            {
                x = insertIngrident((Ingrident)obj);
            }
            else if(obj is Dish)
            {
                x = insertDish((Dish)obj);
            }
            return x;
        }


        //in mongoDB, special id table was needed to handle the auto ID increment
        //id section
        private void addIDinitilizer(string table) //to make it by interface, rename to fillData
        {

            List<BsonDocument> documents = new List<BsonDocument>();

            //build list of all documents

            var document = new BsonDocument {
                {"_id",table},
                { "value", 1}
            };

            documents.Add(document);


            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("TablesID");

            try
            {
                collection.InsertOne(document);
            }
            catch
            {

            }
            
        }

        private void updateIDinitilizer(string table) //to make it by interface, rename to fillData
        {
            int currentID = readIDInitilizer(table);
            var filter = Builders<AidIDFinderClass>.Filter.Eq("_id", table);
            var update = Builders<AidIDFinderClass>.Update.Set("value", currentID + 1);

            //add them all to mongo

            var database = dbConnect();
            var collection = database.GetCollection<AidIDFinderClass>("TablesID");

            collection.UpdateOne(filter, update);
        }

        private int readIDInitilizer(string table)
        {
            var document = new BsonDocument {
                { "_id", table }
            };
            var database = dbConnect();
            var collection = database.GetCollection<AidIDFinderClass>("TablesID");
            AidIDFinderClass x = collection.Find(document).First();
            return x.getValue();

        }

        //

        //Sales
        private int insertSale(Sale s)
        {
            List<BsonDocument> documents = new List<BsonDocument>();

            //build list of all documents
            int currentID = readIDInitilizer("Sales");
            var document = new BsonDocument {
                {"_id",currentID},
                { "Price", s.getPrice()},
                {"OrderDate", s.getOrderDate().ToString()},
                {"OrderState", s.getOrderState() }
            };

            documents.Add(document);

            //add them all to mongo

            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("Sales");

            try
            {
                collection.InsertOne(document);
                updateIDinitilizer("Sales");
                return currentID;
            }
            catch
            {
                return -1;
            }
        }

        public List<Sale> readAllSales()
        {
            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("Sales");
            List<BsonDocument> x = collection.Find(new BsonDocument()).ToList();
            List<Sale> y= new List<Sale>();
            foreach (var item in x)
            {
                y.Add(new Sale((int)item.GetElement(0).Value, (int)item.GetElement(1).Value, DateTime.Parse(item.GetElement(2).Value.ToString()), (bool)item.GetElement(3).Value));
            }
            return y;
        }

        public Sale getSale(int id)
        {
            var document = new BsonDocument {
                { "_id", id }
            };
            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("Sales");
            BsonDocument x = collection.Find(document).First();
            Sale y = new Sale((int)x.GetElement(0).Value, (int)x.GetElement(1).Value, DateTime.Parse(x.GetElement(2).Value.ToString()), (bool)x.GetElement(3).Value);
            return y;
        }

        public int updateSale(Sale s)
        {
            int currentID = s.getID();
            var filter = Builders<Sale>.Filter.Eq("_id", currentID);
            var update = Builders<Sale>.Update.Set("Price", s.getPrice()).Set("OrderDate",s.getOrderDate())
                .Set("OrderState",s.getOrderState());

            try
            {
                var database = dbConnect();
                var collection = database.GetCollection<Sale>("Sales");
                collection.UpdateOne(filter, update);
                return currentID;
            }
            catch
            {
                return -1;
            }
            
        }

        public void deleteSale(int id)
        {
            try
            {
                var database = dbConnect();
                var collection = database.GetCollection<Sale>("Sales");
                var filter = Builders<Sale>.Filter.Eq("_id", id);
                collection.DeleteOne(filter);
            }
            catch
            {
            }
        }


        //Ingridents
        private int insertIngrident(Ingrident i)
        {
            List<BsonDocument> documents = new List<BsonDocument>();

            //build list of all documents
            int currentID = readIDInitilizer("Ingridents");
            var document = new BsonDocument {
                {"_id",currentID},
                { "Flavor", i.getFlavor()},
                {"IngridentType", i.getIngridentType()}
            };

            documents.Add(document);


            

            try
            {
                var database = dbConnect();
                var collection = database.GetCollection<BsonDocument>("Ingridents");
                collection.InsertOne(document);
                updateIDinitilizer("Ingridents");
                return currentID;
            }
            catch
            {
                return -1;
            }
        }

        public int updateIngrident(Ingrident i)
        {
            int currentID = i.getId();
            var filter = Builders<Ingrident>.Filter.Eq("_id", currentID);
            var update = Builders<Ingrident>.Update.Set("Flavor", i.getFlavor()).Set("IngridentType", i.getIngridentType());

            try
            {
                var database = dbConnect();
                var collection = database.GetCollection<Ingrident>("Ingridents");
                collection.UpdateOne(filter, update);
                return currentID;
            }
            catch
            {
                return -1;
            }

        }

        public void deleteIngrident(int id)
        {
            try
            {
                var database = dbConnect();
                var collection = database.GetCollection<Ingrident>("Sales");
                var filter = Builders<Ingrident>.Filter.Eq("_id", id);
                collection.DeleteOne(filter);
            }
            catch
            {
            }
        }

        public List<Ingrident> readAllIngridents(string type)
        {
            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("Ingridents");
            List<BsonDocument> x = collection.Find(new BsonDocument()).ToList();
            List<Ingrident> y = new List<Ingrident>();
            foreach (var item in x)
            {
                if (item.GetElement(2).Value.ToString() == type)
                {
                    y.Add(new Ingrident((int)item.GetElement(0).Value, item.GetElement(1).Value.ToString(), item.GetElement(2).Value.ToString()));
                }
            }
            return y;
        }

        public Ingrident getIngrident(int id)
        {
            var document = new BsonDocument {
                { "_id", id }
            };
            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("Ingridents");
            BsonDocument x = collection.Find(document).First();
            Ingrident y = new Ingrident((int)x.GetElement(0).Value, x.GetElement(1).Value.ToString(), x.GetElement(2).Value.ToString());
            return y;
        }


        //Dishes
        private int insertDish(Dish d)
        {
            List<BsonDocument> documents = new List<BsonDocument>();

            //build list of all documents
            int currentID = readIDInitilizer("Dishes");
            var document = new BsonDocument {
                {"_id",currentID},
                { "ID_ing", d.getID_ing()},
                {"ID_sale", d.getID_sale()},
                {"Amount",d.getAmount() }
            };

            documents.Add(document);

            //add them all to mongo

            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("Dishes");

            try
            {
                collection.InsertOne(document);
                updateIDinitilizer("Dishes");
                return currentID;
            }
            catch
            {
                return -1;
            }
        }

        public int updateDish(Dish d)
        {
            int currentID = d.getID();
            var filter = Builders<Dish>.Filter.Eq("_id", currentID);
            var update = Builders<Dish>.Update.Set("ID_ing", d.getID_ing()).Set("ID_sale", d.getID_sale())
                .Set("Amount",d.getAmount());

            try
            {
                var database = dbConnect();
                var collection = database.GetCollection<Dish>("Dishes");
                collection.UpdateOne(filter, update);
                return currentID;
            }
            catch
            {
                return -1;
            }

        }

        public void deleteDish(int id)
        {
            try
            {
                var database = dbConnect();
                var collection = database.GetCollection<Dish>("Sales");
                var filter = Builders<Dish>.Filter.Eq("_id", id);
                collection.DeleteOne(filter);
            }
            catch
            {
            }
        }

        public List<Dish> readAllDishes()
        {
            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("Dishes");
            List<BsonDocument> x = collection.Find(new BsonDocument()).ToList();
            List<Dish> y = new List<Dish>();
            foreach (var item in x)
            {
                y.Add(new Dish((int)item.GetElement(0).Value, (int)item.GetElement(1).Value, (int)item.GetElement(2).Value, (int)item.GetElement(3).Value));
            }
            return y;
        }

        public Dish getDish(int id)
        {
            var document = new BsonDocument {
                { "_id", id }
            };
            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("Dishes");
            BsonDocument x = collection.Find(document).First();
            Dish y = new Dish((int)x.GetElement(0).Value, (int)x.GetElement(1).Value, (int)x.GetElement(2).Value, (int)x.GetElement(3).Value);
            return y;
        }



        //task5
        public List<Sale> dateQuery(DateTime date)
        {
            List<Sale> result = new List<Sale>();

            var database = dbConnect();
            var collection = database.GetCollection<BsonDocument>("Sales");
            List<BsonDocument> x = collection.Find(new BsonDocument()).ToList();
            List<Sale> y = new List<Sale>();
            foreach (var item in x)
            {
                y.Add(new Sale((int)item.GetElement(0).Value, (int)item.GetElement(1).Value, DateTime.Parse(item.GetElement(2).Value.ToString()), (bool)item.GetElement(3).Value));
            }
            foreach (Sale item in y)
            {
                if(item.getOrderDate().Date == date.Date)
                {
                    result.Add(item);
                }
            }

            
            
            
            return result;
        }

        public ArrayList favoriteIngrident()
        {
            //for mongo, we couldnt complete this function in time :(
            return null;
        }
    }
}
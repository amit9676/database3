using MySqlX.XDevAPI.Common;
using System.Collections;
using System.Diagnostics.Metrics;

//logic layer, all the heavy calculations are in here!
namespace DB3
{
    public class Logic
    {
        BaseDB DB;
        public Logic(bool instruction)
        {
            //depend on user's input - choose mongoDB or SQL
            if (instruction)
                DB = new DBAccess();
            else
                DB = new MongoDBAccess();
        }
        public bool createTables()
        {
            //choose if to create (or re-create) the database
            try
            {
                if (!DB.createTables())
                    throw new Exception();
                return true;
            }
            catch {
                return false;
            }
            
        }
        public void fillTableWithRandomValues()
        {
            string[] flavors = {"lemon", "strawberries", "pineapple", "chocolate", "vanilla", "banana",
            "apple", "peanuts", "mekupelet", "Pistachio"};

            string[] toppings = { "maple", "hot chocolate", "peanuts" };

            string[] cups = { "normal cup", "special cup", "box" };

            foreach (string s in flavors)
            {
                Ingrident i = new Ingrident(-1,s, "flavor");
                DB.insertObject(i);
            }

            foreach (string s in toppings)
            {
                Ingrident i = new Ingrident(-1,s, "topping");
                DB.insertObject(i);
            }

            foreach (string s in cups)
            {
                Ingrident i = new Ingrident(-1, s, "cup");
                DB.insertObject(i);
            }
        }

        public List<Ingrident> getAllIngridents(string type)
        {
            return DB.readAllIngridents(type);
        }

        public List<Sale> getAllSales()
        {
            return DB.readAllSales();
        }

        public List<Dish> getAllDishes()
        {
            return DB.readAllDishes();
        }
        

        public Sale insertSale(Sale s)
        {
            int x = DB.insertObject(s);
            s = DB.getSale(x);
            return s;
        }

        public Ingrident insertIngrident(Ingrident i)
        {
            int x = DB.insertObject(i);
            i = DB.getIngrident(x);
            return i;
        }

        public Dish insertDish(Dish d)
        {
            int x = DB.insertObject(d);
            d = DB.getDish(x);
            return d;
        }

        public Sale updateSale(Sale s)
        {
            int x = DB.updateSale(s);
            s = DB.getSale(x);
            return s;

        }

        public Dish updateDish(Dish d)
        {
            int x = DB.updateDish(d);
            d = DB.getDish(x);
            return d;
        }

        public Ingrident updateIngrident(Ingrident i)
        {
            int x = DB.updateIngrident(i);
            i = DB.getIngrident(x);
            return i;
        }



        public void deleteSale(int id)
        {
            DB.deleteSale(id);

        }

        public void deleteDish(int id)
        {
            DB.deleteDish(id);

        }

        public void deleteIngrident(int id)
        {
            DB.deleteIngrident(id);

        }

        public Ingrident getIngrident(int id)
        {
            return DB.getIngrident(id);
        }


        //check the instructed conditions, if to allow toppings, and if so - which toppings.
        public bool[] toppingChecker(int amountOfBalls, int cupType, int[] flavors)
        {
            bool[] results = { false, true, true };
            if(amountOfBalls >= 2 || cupType != 1)
            {
                results[0] = true;
                foreach(int f in flavors)
                {
                    Ingrident ing = getIngrident(f);
                    if(ing.getFlavor() == "chocolate" || ing.getFlavor() == "mekupelet")
                    {
                        results[1] = false;
                    }
                    else if(ing.getFlavor() == "vanilla")
                    {
                        results[2] = false;
                    }
                }
                return results;
            }

            return new bool[] { false,false,false};
        }

        //create the order
        public Sale OrderCreate(List<Ingrident> flavors, List<Ingrident> toppings, int totalFlavorsCount, int totalToppingsCount, Ingrident cupType, Sale newSale)
        {
           
            int[] flavsAmounts = new int[totalFlavorsCount + totalToppingsCount];
            foreach(Ingrident f in flavors)
            {
                flavsAmounts[f.getId()-1]++;
            }
            foreach (Ingrident f in toppings)
            {
                flavsAmounts[f.getId()-1]++;
            }
            int cost = costCalculator(cupType, flavors.Count, toppings.Count);
            Sale s = new Sale(newSale.getID(), cost, newSale.getOrderDate(), true);
            int s_ID = DB.updateSale(s);
            s = DB.getSale(s_ID);
            for (int i = 0; i < flavsAmounts.Length; i++)
            {
                if (flavsAmounts[i] > 0)
                {
                    DB.insertObject(new Dish(-1, i + 1, s_ID, flavsAmounts[i]));
                }
            }
            DB.insertObject(new Dish(-1, cupType.getId(), s_ID, 1));
            return s;
        }

        //caulculate the order's cost according to the instructions
        private int costCalculator(Ingrident cupType, int amountOfBalls, int amountOfToppings)
        {
            int cost = 0;
            if(amountOfBalls  == 1)
            {
                cost = 7;
            }
            else
            {
                cost = amountOfBalls * 6;
            }
            if(cupType.getFlavor() == "special cup")
            {
                cost += 2;
            }
            else if(cupType.getFlavor() == "box")
            {
                cost += 5;
            }
            cost += 2 * amountOfToppings;
            return cost;
        }

        public string typeOfCup(int val)
        {
            if(val == 1)
            {
                return "normal cup";
            }
            else if(val == 2)
            {
                return "special cup";
            }
            else
            {
                return "box";
            }
        }


        //managerMode
        public int[] dailyData(DateTime input)
        {
            List<Sale> data = DB.dateQuery(input);
            int totalSum = 0;
            int counter = 0;
            int average = 0;

            foreach (Sale item in data)
            {
                counter++;
                totalSum += item.getPrice();
            }
            if (counter == 0)
                return new int[] { 0, 0, 0 };
            average = totalSum / counter;
            return new int[] {totalSum,counter,average};
        }

        public Dictionary<string,int> favoriteIngrident()
        {
            ArrayList data = DB.favoriteIngrident();
            try
            {


                Dictionary<string, int> flavors = new Dictionary<string, int>();
                Dictionary<string, int> toppings = new Dictionary<string, int>();
                Dictionary<string, int> cups = new Dictionary<string, int>();
                string[] bestDishes = new string[] { "none", "none", "none" };
                int[] bestAmounts = new int[3];

                foreach (Object[] row in data)
                {
                    if (row[6].ToString() == "flavor")
                    {
                        favoriteIngridentAid(flavors, row[5].ToString(), (int)row[3]);
                    }
                    else if (row[6].ToString() == "topping")
                    {
                        favoriteIngridentAid(toppings, row[5].ToString(), (int)row[3]);
                    }
                    else if (row[6].ToString() == "cup")
                    {
                        favoriteIngridentAid(cups, row[5].ToString(), (int)row[3]);
                    }


                }
                foreach (KeyValuePair<string, int> entry in flavors)
                {
                    if (entry.Value > bestAmounts[0])
                    {
                        bestAmounts[0] = entry.Value;
                        bestDishes[0] = entry.Key;
                    }
                }
                foreach (KeyValuePair<string, int> entry in toppings)
                {
                    if (entry.Value > bestAmounts[1])
                    {
                        bestAmounts[1] = entry.Value;
                        bestDishes[1] = entry.Key;
                    }
                }
                foreach (KeyValuePair<string, int> entry in cups)
                {
                    if (entry.Value > bestAmounts[2])
                    {
                        bestAmounts[2] = entry.Value;
                        bestDishes[2] = entry.Key;
                    }
                }
                Dictionary<string, int> res = new Dictionary<string, int>();
                res.Add(bestDishes[0], bestAmounts[0]);
                res.Add(bestDishes[1], bestAmounts[1]);
                res.Add(bestDishes[2], bestAmounts[2]);
                return res;
            }
            catch
            {
                return null;
            }
        }

        private void favoriteIngridentAid(Dictionary<string, int> di, string key, int amount)
        {
            if (di.ContainsKey(key))
            {
                di[key] += amount;
            }
            else
            {
                di.Add(key, amount);
            }
        }

        public int uncompletedSales()
        {
            List<Sale> allSales = this.getAllSales();
            int counter = 0;
            foreach (Sale sale in allSales)
            {
                if(sale.getOrderState() == false)
                {
                    counter++;
                }
            }
            return counter;
        }
    }
}
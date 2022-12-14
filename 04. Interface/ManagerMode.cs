
using System.Globalization;


namespace DB3
{
    //this class handles the additional data in application layer (question 6b,c,d)
    internal class ManagerMode
    {
        private Logic Logic;
        public ManagerMode(Logic logic)
        {
            this.Logic = logic;
        }

        //6b
        public void dailyReport()
        {
            Console.WriteLine("enter date in format day/month/year: ");
            Console.WriteLine("note: sending empty input will retrieve today's data");
            string date = "";
            bool dateReady = false;
            DateTime dt = DateTime.Now;
            while (!dateReady)
            {
                try
                {
                    date = Console.ReadLine(); //if empty input is entered - check for today!
                    if(date == "")
                    {
                        date = dt.ToString("d/M/yyyy");
                        break;
                    }
                    dt = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
                    dateReady = true;
                }
                catch
                {
                    Console.WriteLine("please enter valid date by the format");
                }
                
            }
            int[] result = Logic.dailyData(dt);
            Console.WriteLine("report for " + date + ": ");
            Console.WriteLine("amount of sales: " + result[1]);
            Console.WriteLine("total income: " + result[0]);
            Console.WriteLine("average cost for sale: " + result[2]);
        }

        //6d
        public void bestFlavor()
        {
            Dictionary<string, int> flav = Logic.favoriteIngrident();
            Console.WriteLine("the most favorite flavor is " + flav.Keys.ElementAt(0) + " with " + flav.Values.ElementAt(0) + " balls sold");
            Console.WriteLine("the most favorite topping is " + flav.Keys.ElementAt(1) + " with " + flav.Values.ElementAt(1) + " toppings sold");
            Console.WriteLine("the most favorite cup is " + flav.Keys.ElementAt(2) + " with " + flav.Values.ElementAt(2) + " cups sold");
        }


        //6c
        public void uncompletedSales()
        {
            int ucs = Logic.uncompletedSales();
            Console.WriteLine("currently there are " + ucs + " uncompleted sales");
        }
    }
}

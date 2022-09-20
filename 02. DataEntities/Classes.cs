namespace DB3
{
    public class Sale
    {
        int SaleID;
        int Price;
        DateTime OrderDate;
        bool OrderState;

        public Sale(int id, int price, DateTime orderDate, bool orderState)
        {
            this.SaleID = id;
            this.Price = price;
            this.OrderDate = orderDate;
            this.OrderState = orderState;
        }

        public int getID() { return this.SaleID; }

        public int getPrice() { return this.Price; }
        public DateTime getOrderDate() { return this.OrderDate; }
        public bool getOrderState() { return this.OrderState; }

        public override string ToString()
        {
            return this.SaleID + ": " + this.Price + " , " + this.OrderDate + " , " + this.OrderState;
        }
    }

    public class Ingrident
    {
        int IngID;
        string Flavor;
        string IngridentType;

        public Ingrident(int id,string flavor, string ingridentType)
        {
            this.IngID = id;
            this.IngridentType = ingridentType;
            this.Flavor = flavor;
        }

        public int getId() { return this.IngID; }
        public string getFlavor() { return this.Flavor; }
        public string getIngridentType() { return this.IngridentType; }

        public override string ToString()
        {
            return this.IngID + ": " + " , " + this.IngridentType + " , " + this.Flavor;
        }
    }

    public class Dish
    {
        int DishID;
        int ID_ing;
        int ID_sale;
        int Amount;

        public Dish(int id, int id_ing, int id_sale, int amount)
        {
            this.DishID = id;
            this.ID_ing = id_ing;
            this.ID_sale = id_sale;
            this.Amount = amount;
        }
        public int getID() { return this.DishID; }

        public int getID_ing() { return this.ID_ing; }
        public int getID_sale() { return this.ID_sale; }
        public int getAmount() { return this.Amount; }

        public override string ToString()
        {
            return this.DishID + ": " + this.ID_ing + " , " + this.ID_sale + " , " + this.Amount;
        }
    }
}
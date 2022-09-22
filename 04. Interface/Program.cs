//project was created by Amit Goffer (205541360) and Yehonatan Amosi (209542349)


//application layer (icecream selling section)
using DB3;


//determine if to use sql or mongo
bool relationalOrMongo = false;
Console.WriteLine("do you wish to use relational SQL Database, or NoSQL mongo Database (1=sql, 2=mongo)");
relationalOrMongo = inputMaker(1, 2, "please enter 1 or 2") == 1 ? true : false;
Logic logic = new Logic(relationalOrMongo);


void initilize() //let use to choose if to use existing data or reset it
{
    bool tableFill = false;
    try
    {
        tableFill = logic.createTables();
        if (!tableFill)
            throw new Exception();
    }
    catch
    {
        Console.WriteLine("couldnt complete operation, please check connection and try again");
        return;
    }

    try
    {
        if (tableFill)
            logic.fillTableWithRandomValues();
    }
    catch
    {
        Console.WriteLine("couldnt complete operation, please check connection and try again");
        return;
    }
}


//choose if to activate the function above
Console.WriteLine("do you want to reset database? (1=yes, 0=no)");
int dataReset = inputMaker(0, 1, "please enter 0 or 1");
if(dataReset == 1)
    initilize();

//load data from server
List<Ingrident> allFlavors = logic.getAllIngridents("flavor");
List<Ingrident> allCups = logic.getAllIngridents("cup");


//input handler function, make sure the user chooses what is allowed to him to choose
int inputMaker(int minValue, int maxValue, string specialInputMessage)
{
    int input = 0;
    bool trigger = false;
    do
    {
        try
        {
            input = Int32.Parse(Console.ReadLine());
            if (input > maxValue || input < minValue)
            {
                Console.WriteLine(specialInputMessage);
                continue;
            }
            trigger = true;
        }
        catch
        {
            Console.WriteLine("please enter valid input");
        }
    }
    while (!trigger);
    return input;
}

//allow use to choose what he wants to do
Console.WriteLine("what action you wish to make?");
Console.WriteLine("1. make an order.");
Console.WriteLine("2. view a report");
Console.WriteLine("3. display best seller flavor");
Console.WriteLine("4. check uncompleted sales");
int oOrr = inputMaker(1, 4, "please enter 1 to 4");
ManagerMode m = new ManagerMode(logic);
if (oOrr == 2)
{
    
    m.dailyReport();
    return;
}
else if (oOrr == 3)
{
    try
    {
        m.bestFlavor();
    }
    catch
    {
        Console.WriteLine("could not obtain data");
    }
    return;
}
else if(oOrr == 4)
{
    m.uncompletedSales();
    return;
}



//icecream selling section
int type_of_cup = 0;
int amount_of_ball = 0;
int type_of_Topping = 0;
List<Ingrident> selectedFlavors = new List<Ingrident>();
List<Ingrident> selectedToppings = new List<Ingrident>();
Ingrident selectedCup = null;
int[] flavors = new int[amount_of_ball];
List<Ingrident> toppings = logic.getAllIngridents("topping");
int doMore = 1;
do
{
    type_of_cup = 0;
    amount_of_ball = 0;
    type_of_Topping = 0;
    Sale newSale = logic.insertSale(new Sale(-1, 0, DateTime.Now, false));
    flavorSection();
    toppingSection();
    createOrder(newSale);
    Console.WriteLine("\ndo you wanna make another order?  (1=yes, 0=no)");
    doMore = inputMaker(0, 1, "please choose 0 or 1");
}
while (doMore == 1);

void flavorSection()
{
    Console.WriteLine("Please select your order");
    for (int i = 0; i < allCups.Count; i++)
    {
        //display cups from the server
        Console.WriteLine((i + 1) + " - " + (allCups[i] as Ingrident).getFlavor());

    }
    type_of_cup = inputMaker(1, 3, "please enter valid input");
    selectedCup = allCups[type_of_cup - 1];
    Console.WriteLine("\nselect amount of icecream's balls: ");
    int maxBallAmount = type_of_cup == 3 ? (Int32.MaxValue - 1) : 3;
    if (maxBallAmount == 3)
    {
        amount_of_ball = inputMaker(1, maxBallAmount, "amount of balls must be between 1 to 3");
    }
    else
    {
        amount_of_ball = inputMaker(1, maxBallAmount, "amount of balls must be between 1 or above");
    }

    flavors = new int[amount_of_ball];

    Console.WriteLine("our available icecream flavors:");
    for (int i = 0; i < allFlavors.Count; i++)
    {
        //display flavors from the server
        Console.WriteLine((i + 1) + " - " + (allFlavors[i] as Ingrident).getFlavor());

    }

    //accept user flavor choice
    Console.WriteLine();
    for (int i = 0; i < flavors.Length; i++)
    {
        Console.WriteLine("select flavor for icecream ball " + (i + 1));
        flavors[i] = inputMaker(1, allFlavors.Count, "please enter valid input");
    }

    selectedFlavors = new List<Ingrident>();
    foreach (int t in flavors)
    {
        selectedFlavors.Add(allFlavors[t - 1]);
    }
}


void toppingSection()
{
    bool[] canTopping = logic.toppingChecker(amount_of_ball, type_of_cup, flavors);
    toppings = logic.getAllIngridents("topping");

    //get toppings from server
    Dictionary<string, bool> myTopping = new Dictionary<string, bool>();
    foreach (Ingrident t in toppings)
    {
        myTopping.Add(t.getFlavor(), false);
    }


    int amount_of_toppings = 1;
    int availableTopping = toppings.Count;
    if (!canTopping[1])
    {
        availableTopping--;
    }
    if (!canTopping[2])
    {
        availableTopping--;
    }

    if (type_of_cup != 1)
    {
        Console.WriteLine("Enter amount of desired toppings: ");
        amount_of_toppings = inputMaker(0, availableTopping, "amount of toppings must be between 0 to " + availableTopping);
    }


    if (canTopping[0])
    {
        while (amount_of_toppings > 0)
        {
            Console.WriteLine("\nour avaiable topping: ");
            Console.WriteLine("amount of topping remaining: " + amount_of_toppings);
            Console.WriteLine("0 - no topping");
            for (int i = 0; i < toppings.Count; i++)
            {
                if (!myTopping[toppings[i].getFlavor()])
                {
                    if (toppings[i].getFlavor() != "maple" && toppings[i].getFlavor() != "hot chocolate")
                    {
                        Console.WriteLine((i + 1) + " - " + toppings[i].getFlavor());
                    }
                    else if (toppings[i].getFlavor() == "maple" && canTopping[2])
                    {
                        Console.WriteLine((i + 1) + " - " + toppings[i].getFlavor());
                    }
                    else if (toppings[i].getFlavor() == "hot chocolate" && canTopping[1])
                    {
                        Console.WriteLine(i + 1 + " - " + toppings[i].getFlavor());
                    }
                }
            }

            type_of_Topping = inputMaker(0, toppings.Count, "please enter values between 0 and " + toppings.Count);

            if (type_of_Topping == 0)
            {
                break;
            }
            Ingrident selectedTopping = toppings[type_of_Topping - 1];
            if ((selectedTopping.getFlavor() == "hot chocolate" && !canTopping[1]) || (selectedTopping.getFlavor() == "maple" && !canTopping[2]))
            {
                Console.WriteLine("this type of topping is unavailable, please choose available topping\n");
                continue;
            }
            if (myTopping[selectedTopping.getFlavor()])
            {
                Console.WriteLine("you already selected this topping, try another");
                continue;
            }
            amount_of_toppings--;
            myTopping[selectedTopping.getFlavor()] = true;
        }


    }

    selectedToppings = new List<Ingrident>();
    foreach (Ingrident t in toppings)
    {
        if (myTopping[t.getFlavor()])
        {
            selectedToppings.Add(t);
        }
    }
}


//create the order, and display all order details (components, price, ect..)
void createOrder(Sale newSale)
{
    Console.WriteLine("creating order...");
    Sale s;
    try
    {
        //question 6A - display order details
        s = logic.OrderCreate(selectedFlavors, selectedToppings, allFlavors.Count, toppings.Count, selectedCup, newSale);
        Console.WriteLine("order created, your order ID is " + s.getID() + ", the order cost is " + s.getPrice());
        Console.WriteLine("order time: " + s.getOrderDate().ToString("G"));
        Console.WriteLine("order contains cup: " + logic.typeOfCup(type_of_cup));
        Console.WriteLine("order's flavors: ");
        foreach (Ingrident ingr in selectedFlavors)
        {
            Console.Write(ingr.getFlavor() + ", ");
        }
        Console.WriteLine("\n\norder's topping: ");
        foreach (Ingrident ingr in selectedToppings)
        {
            Console.Write(ingr.getFlavor() + ", ");
        }
    }
    catch
    {
        Console.WriteLine("operation failed, please try again");
        return;
    }
}


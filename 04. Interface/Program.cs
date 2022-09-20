using System;
using System.Data;
using System.Diagnostics;//used for Stopwatch class


using System.Collections;
using DB3;
using Org.BouncyCastle.Utilities;

Logic logic = new Logic();
void initilize()
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

Console.WriteLine("do you want to reset database? (1=yes, 0=no)");
int dataReset = inputMaker(0, 1, "please enter 0 or 1");
if(dataReset == 1)
    initilize();

List<Ingrident> allFlavors = logic.getAllIngridents("flavor");


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
int type_of_cup = 0;
int amount_of_ball = 0;
int type_of_Topping = 0;
List<Ingrident> selectedFlavors = new List<Ingrident>();
List<Ingrident> selectedToppings = new List<Ingrident>();
int[] flavors = new int[amount_of_ball];
List<Ingrident> toppings = logic.getAllIngridents("topping");
int doMore = 1;
do
{
    type_of_cup = 0;
    amount_of_ball = 0;
    type_of_Topping = 0;
    flavorSection();
    toppingSection();
    createOrder();
    Console.WriteLine("\ndo you wanna make another order?  (1=yes, 0=no)");
    doMore = inputMaker(0, 1, "please choose 0 or 1");
}
while (doMore == 1);

void flavorSection()
{
    Console.WriteLine("Please select your order");
    Console.WriteLine("\nselect your cup:\n1 - normal cup\n2 - spacial cup\n3 - box");
    type_of_cup = inputMaker(1, 3, "please enter valid input");

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
        Console.WriteLine((i + 1) + " - " + (allFlavors[i] as Ingrident).getFlavor());

    }

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


void createOrder()
{
    Console.WriteLine("creating order...");
    Sale s;
    try
    {
        s = logic.OrderCreate(selectedFlavors, selectedToppings, allFlavors.Count, toppings.Count, type_of_cup);
        Console.WriteLine("order created, your order ID is " + s.getID() + ", the order cost is " + s.getPrice());
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


using System;
using Library.ECommerceApp;
using Library.ECommerceApp.Services;
using Newtonsoft.Json;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Ecommerce App: ");


            var Inventory = ProductServices.Current;    // set up Inventory list of products
            var ShoppingCart = CartService.Current;     // set up Shopping Cart list of products

            // User Type - Different Interfaces (Menus)
            Console.WriteLine("1. Customer");
            Console.WriteLine("2. Employee");
            Console.Write("Enter choice: ");


            var usertype = int.Parse(Console.ReadLine() ?? "0");

            Inventory.Load("SaveData.json");    // Loads Existing Data if User Saved
            ShoppingCart.Load("SaveCart.json");
            bool cont = true;   // if true Menu loop continues
            bool customer = true;
            if (usertype != 1) { customer = false; }


            // --------- EMPLOYEE - Inventory Menu (OPTION #2) ---------
            if (!customer)
            {
                while (cont)
                {
                    Console.WriteLine("\n");
                    var action = InventoryPrintMenu();  // print menu
                    Console.WriteLine("\n");

                    if (action == ActionType.Create)
                    {
                        // 1) CREATE - creates new product in inventory list

                        Console.WriteLine("You chose to Add an Product.");
                        var newProduct = new Product();     // make new product newProduct

                        FillProduct(newProduct);
                        Inventory.Create(newProduct);       // put newProduct in 

                    }
                    else if (action == ActionType.ListInventory)
                    {
                        // 2) PRINT INVENTORY
                        PrintInventory(Inventory);
                    }
                    else if (action == ActionType.Update)
                    {
                        // 3) Update - updates quantity/name/description of existing product in Inventory

                        Console.Write("Enter ID of Product you would like to update: ");
                        var choice = int.Parse(Console.ReadLine() ?? "0");

                        var prod = Inventory.Products.FirstOrDefault(p => p.Id == choice);
                        if (prod != null)
                        {
                            var p = UpdateProduct(prod);
                            Inventory.Update(p);
                        }
                        else
                        {
                            Console.Write("No Product with that ID.");
                        }
                    }
                    else if (action == ActionType.Delete)
                    {
                        // 4) Delete - Delete Product from inventory

                        Console.WriteLine("Enter Id of Product you would like to Delete: ");
                        var id = int.Parse(Console.ReadLine() ?? "0");
                        Inventory.Delete(id);

                    }

                    else if (action == ActionType.Search)
                    {
                        SearchInventory(Inventory);
                    }
                    else if (action == ActionType.Save)
                    {
                        // 6) Save - saves data using Json to "SaveData.json"
                        Inventory.Save("SaveData.json");
                    }
                    else if (action == ActionType.Load)
                    {
                        // 7) Load - Inventory by read text from text file
                        Inventory.Load("SaveData.json");
                    }
                    else if (action == ActionType.Exit)
                    {
                        // 8) - EXIT LOOP

                        Console.WriteLine("Exiting.");
                        cont = false;
                    }

                }// END WHILE LOOP - Inventory
            }
            else
            {
                // --------- CUSTOMER - Customer Menu (OPTION #1) ---------

                while (cont)
                {
                    Console.WriteLine("\n");
                    var action = CustomerPrintMenu();
                    Console.WriteLine("\n");

                    if (action == ActionType.Add)
                    {
                        // 1. Add Item to Cart

                        Console.WriteLine("\n\nAdd Item to Cart\n");
                        Console.Write("Enter Product Id: ");
                        var id = int.Parse(Console.ReadLine() ?? "0");

                        ShoppingCart.Add(id);   // takes away from inventory 
                    }
                    else if (action == ActionType.Remove)
                    {
                        // 2. Remove from cart

                        Console.WriteLine("\n\nDelete Item from Cart");
                        Console.Write("Enter Product Id: ");
                        var id = int.Parse(Console.ReadLine() ?? "0");

                        ShoppingCart.Remove(id);    // adds back to inventory

                    }
                    else if (action == ActionType.ListInventory)
                    {
                        PrintInventory(Inventory);
                    }
                    else if (action == ActionType.ShowCart)
                    {
                        // 4. List Current Shopping Cart
                        PrintCart(ShoppingCart);

                    }
                    else if (action == ActionType.Search)
                    {
                        // 5 - Search

                        Console.WriteLine("Where would you like to search:");
                        Console.WriteLine("1. Shopping Cart");
                        Console.WriteLine("2. Inventory");
                        Console.WriteLine("Type choice: ");
                        var choice = int.Parse(Console.ReadLine() ?? "0");

                        bool found = false;

                        if (choice == 1) // Search shopping Cart
                        {
                            Console.Write("Enter Name or Description of Item: ");
                            string nameDes = Console.ReadLine() ?? string.Empty;

                            Console.WriteLine("Search found in Cart: \n");

                            for (int i = 0; i < ShoppingCart.Cart.Count; i++)
                            {
                                var hold = ShoppingCart.Cart[i];
                                if (hold.Name.Contains(nameDes))    // checks Name
                                {
                                    Console.WriteLine($"{hold}");
                                    found = true;
                                }
                                else if (hold.Description.Contains(nameDes))    // checks Description
                                {
                                    Console.WriteLine($"{hold}");
                                    found = true;
                                }
                            }
                            if (found == false)  // Nothing found matching
                            {
                                Console.WriteLine("No products dound matching that Name or Description. \n");
                            }
                        }
                        else // Search Inventory
                        {
                            SearchInventory(Inventory);
                        }

                    }
                    else if (action == ActionType.Checkout)
                    {
                        // 5) CHECKOUT - checks out customer then exits program
                        ShoppingCart.Checkout();
                        cont = false;
                    }
                    else if (action == ActionType.Exit)
                    {
                        // 6 - EXIT LOOP

                        //Saves Inventory & Cart
                        Inventory.Save("SaveData.json");

                        Console.Write("Would you like to Save Cart?\n1) Yes\n2) No\nEnter choice: ");

                        var answer = int.Parse(Console.ReadLine() ?? "0");
                        if (answer == 1)
                        {
                            ShoppingCart.Save("SaveCart.json");
                        }
                        else
                        {
                            File.WriteAllText("SaveCart.json", null);
                        }
                        Console.WriteLine("Exiting.");
                        cont = false;
                    }

                }// END WHILE LOOP

            }
            Console.WriteLine("Thank you for Shopping with us!");

        }


        // -------------------------------- SEARCH --------------------------------

        private static void SearchInventory(ProductServices inventory)
        {
            bool found = false;

            Console.Write("Enter Name or Description of Item: ");
            string nameDes = Console.ReadLine() ?? string.Empty;
            if (nameDes != null)
            {

                Console.WriteLine("Searching Inventory... \n\n ");

                Console.WriteLine($"-------------------------------------------------------------------");
                Console.WriteLine($"------------------------  Inventory Search ------------------------");
                Console.WriteLine($"-------------------------------------------------------------------");
                Console.WriteLine($"\tID#\tName\tPrice\tQuantity\tDescription\n");

                for (int i = 0; i < inventory.Products.Count; i++)
                {
                    var hold = inventory.Products[i];
                    if (hold.Name.Contains(nameDes))    // checks Name
                    {
                        Console.WriteLine($"{hold}");
                        found = true;
                    }
                    else if (hold.Description.Contains(nameDes))    // checks Description
                    {
                        Console.WriteLine($"{hold}");
                        found = true;
                    }
                }

                if (found == false)  // Nothing found matching
                {
                    Console.WriteLine("\nNo products found matching that Name or Description. \n");
                }
            }
            else
            {
                Console.WriteLine("\nNo products found matching that Name or Description. \n");
            }
        }


        private static void FillProduct(Product? product)
        {
            // FillProduct - gets all compoents from user from new Product
            if (product == null)
            {
                return;
            }
            // Name
            Console.Write("\nEnter New Product Name: ");
            product.Name = Console.ReadLine() ?? string.Empty;

            // Description
            Console.Write("\nEnter Description of Product: ");
            product.Description = Console.ReadLine() ?? string.Empty;

            // Price
            Console.Write("\nPrice Per Unit: ");
            product.Price = double.Parse(Console.ReadLine() ?? "0");

            // Quantity
            Console.Write("\nNumber of Units Avalible: ");
            product.Quantity = int.Parse(Console.ReadLine() ?? "0");

            product.AssignedUser = 1;

        }


        // -------------------------   MENUS  -------------------------

        private static ActionType InventoryPrintMenu()
        {
            // CRUD - Create, Read, Update, & Delete
            Console.WriteLine("------ Employee Menu ------\n");
            Console.WriteLine("1. Create New Product"); // Create
            Console.WriteLine("2. Print Inventory");    // ListInventory
            Console.WriteLine("3. Update Product");     // Update
            Console.WriteLine("4. Delete Product");     // Delete
            Console.WriteLine("5. Search Product");     // Search
            Console.WriteLine("6. Save Products");      // Save
            Console.WriteLine("7. Load Products");      // Load
            Console.WriteLine("8. EXIT\n");             // Exit


            Console.Write("\nEnter choice: ");             // Exit
            var input = int.Parse(Console.ReadLine() ?? "0");

            while(true)
            {
                switch (input)
                {
                    case 1:
                        return ActionType.Create;
                    case 2:
                        return ActionType.ListInventory;
                    case 3:
                        return ActionType.Update;
                    case 4:
                        return ActionType.Delete;
                    case 5:
                        return ActionType.Search;
                    case 6:
                        return ActionType.Save;
                    case 7:
                        return ActionType.Load;
                    case 8:
                        return ActionType.Exit;
                    default:
                        Console.WriteLine("Invalid Choice, Try Again.\n");
                        return InventoryPrintMenu(); 
                }
            }
        } 

        private static ActionType CustomerPrintMenu()
        {
            // CRUD - Create, Read, Update, & Delete
            Console.WriteLine("------ Customer Menu ------\n");
            Console.WriteLine("1. Add Item to Cart");         // Add
            Console.WriteLine("2. Remove Item from Cart");    // Remove
            Console.WriteLine("3. List Current Inventory");   // ListInventory
            Console.WriteLine("4. Show Shopping Cart");       // ShowCart
            Console.WriteLine("5. Checkout");       // ShowCart
            Console.WriteLine("6. EXIT\n");                     // Exit
            Console.Write("Enter choice: ");
            var input = int.Parse(Console.ReadLine() ?? "0");

            while (true)
            {
                switch (input)
                {
                    case 1:
                        return ActionType.Add;
                    case 2:
                        return ActionType.Remove;
                    case 3:
                        return ActionType.ListInventory;
                    case 4:
                        return ActionType.ShowCart;
                    case 5:
                        return ActionType.Checkout;
                    case 6:
                        return ActionType.Exit;
                    default:
                        Console.WriteLine("Invalid Choice, Try Again.\n");
                        return CustomerPrintMenu();
                }
            }

        } // END Customer MENU

        private static Product UpdateProduct(Product prod)
        {
            bool updateDone = false;

            while (updateDone == false)
            {
                Console.WriteLine($"\nWhat would you like to update:\n\t1)Name\n\t2)Description\n\t3)Price Per Unit\n\t4)Quantity avalible");
                Console.Write("Enter choice: ");
                var spot = int.Parse(Console.ReadLine() ?? "0");

                switch (spot)
                {
                    case 1: // change Name
                        Console.Write("\nEnter New Name: ");
                        string n = Console.ReadLine() ?? string.Empty;
                        if (n == null) { break; }
                        prod.Name = n;
                        break;
                    case 2: // change Description
                        Console.Write("\nEnter New Description: ");
                        string d = Console.ReadLine() ?? string.Empty;
                        if (d == null) { break; }
                        prod.Description = d;
                        break;
                    case 3: // change Price (per unit)
                        Console.Write("\nEnter New Price per Unit: ");
                        var pri = double.Parse(Console.ReadLine() ?? "0");
                        prod.Price = pri;
                        break;
                    case 4: // change Quantity avalible
                        Console.Write("\nEnter New Quantity of Products Avalible: ");
                        var q = int.Parse(Console.ReadLine() ?? "0");
                        prod.Quantity = q;
                        break;
                    default:
                        Console.WriteLine("\nInvalid Choice.");
                        break;
                }
                Console.WriteLine($"\nProduct Changed:\nName = {prod.Name}\t Description = {prod.Description}\t Quantity = {prod.Quantity}\t Price = ${prod.Price}");
                Console.WriteLine($"\n\nWould you like to make more changes:\n\t1) Yes\n\t2) No");
                Console.Write("\nEnter choice: ");

                var x = int.Parse(Console.ReadLine() ?? "0");
                if (x == 2) { updateDone = true; }
            }
            return prod; 
        }


        // -------------------------   PRINT LISTS  -------------------------

        private static void PrintInventory(ProductServices inventory)   // Prints Inventory 
        {
            Console.WriteLine("Printing Inventory... ");

            if (inventory.Products.Count <= 0) { Console.WriteLine("\n***** INVENTORY EMPTY *****\n"); }
            else
            {
                Console.WriteLine($"---------------------------------------------------------------------");
                Console.WriteLine($"------------------------  Current Inventory  ------------------------");
                Console.WriteLine($"---------------------------------------------------------------------");
                Console.WriteLine($"\tID#:\tName:\tPrice:\tQuantity:\tDescription:");
                Console.WriteLine($"---------------------------------------------------------------------");

                for (int i = 0; i < inventory.Products.Count; i++)
                {
                    var Product = inventory.Products[i];
                    Console.WriteLine($"{i + 1})\t{Product}");
                }
            }
        }

        
        private static void PrintCart(CartService shoppingcart)     // Prints shopping cart
        {
            if (shoppingcart.Cart.Count <= 0)
            {
                Console.WriteLine("No Items in Shopping Cart.\n");
            }
            else
            {
                Console.WriteLine($"---------------------------------------------------------------------");
                Console.WriteLine($"----------------------  Current Shopping Cart  ----------------------");
                Console.WriteLine($"---------------------------------------------------------------------");
                Console.WriteLine($"\tID#\tName\tPrice\tQuantity\tDescription");
                Console.WriteLine($"---------------------------------------------------------------------");

                for (int i = 0; i < shoppingcart.Cart.Count; i++)
                {
                    var Product = shoppingcart.Cart[i];
                    Console.WriteLine($"{i + 1})\t{Product}");
                }
            }
        }

    } // END Iternal Program


    public enum ActionType
    {
        Create, ListInventory, Update, Delete, Exit, Add, Remove, ShowCart,
        Search, Save, Load, Checkout
      
    }
} 
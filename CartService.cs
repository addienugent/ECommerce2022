using System;
using Newtonsoft.Json;

namespace Library.ECommerceApp.Services
{
	public class CartService
	{
		private ProductServices inventoryList;
		private List<Product> cartProducts;

		private CartService()
		{
			cartProducts = new List<Product>();
			inventoryList = ProductServices.Current;
		}

		public List<Product> Cart
		{
			get { return cartProducts; }
		}

		private static CartService current;


		public static CartService Current   // singletons
		{
			get
			{
				if (current == null)
				{
					current = new CartService();
				}

				return current;
			}
		}


		public int NextId
		{
			get
			{
				if (!Cart.Any())
				{
					return 1;
				}
				// makes a list of Id # and gets max returns + 1 for next Id #
				return Cart.Select(p => p.Id).Max() + 1;
			}
		}


		public void Create(Product product)
		{
			//product.Id = NextId;
			product.Quantity = 0;
			cartProducts.Add(product);
		}



		public void Add(int id) // Adds item to cart
		{
			// get matching product from inventory list
			var prod = inventoryList.Products.FirstOrDefault(p => p.Id == id);
			
			Console.WriteLine($"Adding {prod}"); // DELETE
			if (prod == null)
            {
				Console.WriteLine("Product was not found in Inventory.");
				return;
            }

			// get quantity needed
			Console.WriteLine("How many would you like? ");
			var q = int.Parse(Console.ReadLine() ?? "0");

			if (prod.Quantity < q)
            {
				Console.WriteLine("Quantity unavaliable.");
				return;
			}

			prod.Quantity = prod.Quantity - q;
			Console.WriteLine($"Inventory Quantity = {prod.Quantity}");

			// UPDATES inventory list quantity by - q
			inventoryList.Update(prod);

			// ------------ SHOPPING CART ------------ 

			var cartprod = cartProducts.FirstOrDefault(p => p.Id == id);
			//cprod.Quantity = q;
			if (cartprod == null)	// Remove from cart if aleady existing and add current quantity to new 
			{
				var p = new Product { Id = prod.Id, Name = prod.Name, Price = prod.Price, Description = prod.Description, Quantity = q};
				cartProducts.Add(p);
			}
            else { cartprod.Quantity += q; }

			Console.WriteLine($"Successfull Added {q} units of {prod.Name} to Cart \n");
		}

		public void Remove(int id) // Adds item to cart
		{

			// ------------ SHOPPING CART ------------ 

			var cartprod = cartProducts.FirstOrDefault(p => p.Id == id);

			Console.WriteLine($"Removing..."); // DELETE
			if (cartprod == null)
			{
				Console.WriteLine("Product was not found in Shopping Cart.");
				return;
			}

			// get quantity needed
			Console.Write("How many would you like to remove? ");
			var q = int.Parse(Console.ReadLine() ?? "0");

			if (cartprod.Quantity < q)
			{
				Console.WriteLine("Quantity unavaliable.");
				return;
			}
			cartprod.Quantity -= q;
			if(cartprod.Quantity <= 0)
            {
				cartProducts.Remove(cartprod);
			}
			
			var iprod = inventoryList.Products.FirstOrDefault(p => p.Id == id);
			int newq = q;
			if (iprod != null)   // Remove from cart if aleady existing and add current quantity to new 
			{
				newq = iprod.Quantity + q;
			}
			var p = new Product { Id = iprod.Id, Name = iprod.Name, Price = iprod.Price, Description = iprod.Description, Quantity = newq };
			inventoryList.Update(p);

			Console.WriteLine($"Successfull Removed {q} units of {iprod.Name} to Cart \n");

			



			Console.WriteLine($"Successfull Removed {q} units of {cartprod.Name} to Cart \n");
		}

		public void Load(string fileName)
		{
			var productsJson = File.ReadAllText(fileName);
			cartProducts = JsonConvert.DeserializeObject<List<Product>>(productsJson) ?? new List<Product>();
		}

		public void Save(string fileName)
		{
			var productsJson = JsonConvert.SerializeObject(cartProducts);
			File.WriteAllText(fileName, productsJson);
		}

		public void Checkout()
        {
			Console.WriteLine($"--------------------------------------------------");
			Console.WriteLine($"-------------------  CHECKOUT  -------------------");
			Console.WriteLine($"--------------------------------------------------");
			Console.WriteLine("\n\tName:\tID#:\tPrice:\tQuantity:\n");
			for (int i = 0; i < cartProducts.Count; i++)
			{
				var prod = cartProducts[i];
				Console.WriteLine($"\t{prod.Name}\t{prod.Id}\t${prod.Price}\t{prod.Quantity}");
			}

			var total = 0.0;
			for(int i = 0; i < cartProducts.Count; i++)
			{
				//var iprod = inventoryList.Products.FirstOrDefault(p => p.Id == id);
				var prod = cartProducts[i];
                prod.TotalPrice = prod.Quantity * prod.Price;
				total += prod.TotalPrice;
			}

			var subtotal = total * 0.07;
			Console.WriteLine($"--------------------------------------------------");
			Console.WriteLine($"\tsubtotal:\t$ {total}");
			Console.WriteLine($"\tsales tax:\t$ {subtotal}");
			Console.WriteLine($"\n\tTotal:\t$ {total+subtotal}\n");
		}


	}
}


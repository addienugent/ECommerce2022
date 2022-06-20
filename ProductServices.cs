using Newtonsoft.Json;

namespace Library.ECommerceApp.Services
{

    public class ProductServices
	{

		private List<Product> ProductList;

		// Returns product list
		public List<Product> Products
		{
            get { return ProductList; }
		}


		public int NextId
        {
            get
            {
				if(!Products.Any())
                {
					return 1;
                }
				// makes a list of Id # and gets max returns + 1 for next Id #
				return Products.Select(p => p.Id).Max() + 1;
            }
        }


		private static ProductServices current;


		public static ProductServices Current	// singletons
        {
            get
            {
				if(current == null)
                {
					current = new ProductServices();
                }

				return current;
            }
        }


		// Creates list of products
		private ProductServices()
		{
			ProductList = new List<Product>();
		}

		
		// Creates new Product & adds to ProductList
		public void Create(Product product)
        {
			product.Id = NextId;
			ProductList.Add(product);
        }


		public void Update(Product? product)
        {
			if(product == null) { return; }

			var prod = ProductList.FirstOrDefault(p => p.Id == product.Id);

			if(prod != null)	//if item DNE then add if quantity > 0
            {
				ProductList.Remove(prod);
            }

			if (product.Quantity <= 0)
				return;

			ProductList.Add(product);
		}


		// Deletes product with id #
		public void Delete(int id)
        {
			var productToDelete = ProductList.FirstOrDefault(p => p.Id == id);
			if(productToDelete == null)
            {
				return;
            }
            ProductList.Remove(productToDelete);
        }


		public void Load(string fileName)
		{ 
			var productsJson = File.ReadAllText(fileName);
			ProductList = JsonConvert.DeserializeObject<List<Product>>(productsJson) ?? new List<Product>();
		}

		public void Save(string fileName)
        {
			var productsJson = JsonConvert.SerializeObject(ProductList);
			File.WriteAllText(fileName, productsJson);

		}

		// ----------------- Shopping Cart ----------------------

		public void AddUnit(int id)
		{
			// quantity of product + 1
			var addProduct = ProductList.FirstOrDefault(p => p.Id == id);
			if (addProduct == null)
			{
				return;
			}
			addProduct.Quantity = addProduct.Quantity + 1;
			//Update(addProduct);
		}

		public void RemoveUnit(int id)
		{
			// quanity product - 1
			var productToDelete = ProductList.FirstOrDefault(p => p.Id == id);
			if (productToDelete == null)
			{
				return;
			}
			productToDelete.Quantity = productToDelete.Quantity - 1;
			//Update(productToDelete);
		}
	}
}


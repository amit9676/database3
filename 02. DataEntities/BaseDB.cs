using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DB3
{
    //base interface for the DALS to use
    public interface BaseDB
    {
        public bool createTables();
        public int insertObject(Object obj);
        public List<Sale> readAllSales();
        public Sale getSale(int id);
        public int updateSale(Sale s);
        public void deleteSale(int id);
        public int updateIngrident(Ingrident i);
        public void deleteIngrident(int id);
        public List<Ingrident> readAllIngridents(string type);
        public Ingrident getIngrident(int id);
        public int updateDish(Dish d);
        public void deleteDish(int id);
        public List<Dish> readAllDishes();
        public Dish getDish(int id);
        public List<Sale> dateQuery(DateTime date);
        public ArrayList favoriteIngrident();
    }
}

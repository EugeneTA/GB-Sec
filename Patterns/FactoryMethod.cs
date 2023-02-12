using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
    public class FactoryMethod
    { 
        static void Main(string[] args)
        {
            var pr1 = ProductFactory.Create<ProductOne>();
            var pr2 = ProductFactory.Create<ProductTwo>();

        }

    }

    public abstract class Product
    {
        public abstract void PostConstruction();
    }

    public class ProductOne : Product
    {
        public ProductOne()
        {

        }

        public ProductOne(int a, int b)
        {

        }

        public override void PostConstruction()
        {
            Console.WriteLine($"Product One created");
        }
    }

    public class ProductTwo : Product
    {
        public ProductTwo()
        {

        }
        public ProductTwo(string c)
        {

        }

        public override void PostConstruction()
        {
            Console.WriteLine($"Product Two created");
        }
    }

    public static class ProductFactory
    {
        public static T Create<T>() where T: Product, new()
        {
            var t = new T();
            t.PostConstruction();
            return t;
        }
    }
}

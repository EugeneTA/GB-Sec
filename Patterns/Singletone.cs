

namespace Patterns
{
    public class Singletone
    {
        static void Main(string[] args)
        {
            var obj1 = SimpleSingleTone.GetInstance;
            var obj2 = SimpleSingleTone.GetInstance;

            Console.WriteLine(obj1);
            Console.WriteLine(obj2);
            Console.WriteLine(obj1.Equals(obj2));
        }
    }

    public class SimpleSingleTone
    {
        private static SimpleSingleTone _simpleSingleTone;

        private SimpleSingleTone() { }

        public static SimpleSingleTone GetInstance
        {
            get 
            { 
                if (_simpleSingleTone == null)
                {
                    _simpleSingleTone = new SimpleSingleTone();
                }
                return _simpleSingleTone; 
            }
        }

        public override string ToString()
        {
            return $"Hello from Singletone";
        }
    }
}

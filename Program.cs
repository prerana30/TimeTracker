using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*namespace TimeTracker
{
    internal class Program
    {
        static void Main(string[] args)
        {


            Contractor contract = new Contractor();
            contract.costperhours();
            contract.StartTimer();
            contract.calculatingcost();
            

        }
    }
}*/


namespace TimeTracker
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter you full name");
            string fullname= Console.ReadLine();
            Console.WriteLine("Enter your cost per hour: ");
            float costPerHour = float.Parse(Console.ReadLine());

            Contractor contractor = new Contractor(costPerHour);
            contractor.StartTimer();
        }
    }
}






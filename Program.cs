using System;

namespace TimeTracker
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter your cost per hour per project: ");
            float costPerHour;

            while (!float.TryParse(Console.ReadLine(), out costPerHour))
            {
                Console.WriteLine("Invalid input. Please enter a valid numeric value.");
            }

            Contractor contractor = new Contractor(costPerHour);
            contractor.StartTimer();
        }
    }
}

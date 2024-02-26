using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace TimeTracker
{
    public class Contractor
    {
        private float costPerHour;
        private const float pomodoroWorkDurationMinutes = 45;
        private const float pomodoroBreakDurationMinutes = 15;

        public Contractor(float costPerHour)
        {
            this.costPerHour = costPerHour;
        }

        public void StartTimer()
        {
            Console.WriteLine("Enter your full name:");
            string fullName = Console.ReadLine();

            Console.WriteLine("Enter your project name:");
            string projectName = Console.ReadLine();

            Console.WriteLine("Choose timer mode:");
            Console.WriteLine("1. Manual");
            Console.WriteLine("2. Automatic");
            Console.Write("Enter your choice (1 or 2): ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                ManualTimeTracking(fullName, projectName);
            }
            else if (choice == "2")
            {
                Console.Write("Enter the desired duration of automatic time tracking (in hours): ");
                if (float.TryParse(Console.ReadLine(), out float automaticDurationHours))
                {
                    AutomaticTimeTracking(fullName, projectName, automaticDurationHours);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number for duration.");
                }
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter 1 or 2.");
            }
        }

        private void ManualTimeTracking(string fullName, string projectName)
        {
            try
            {
                Console.WriteLine("Enter the start time (format:(2024-02-03 02:05:00) yyyy-MM-dd HH:mm:ss): ");
                DateTime startTime = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("Enter the end time (format: (2024-02-04 02:05:01) yyyy-MM-dd HH:mm:ss): ");
                DateTime endTime = DateTime.Parse(Console.ReadLine());

                TimeSpan duration = endTime - startTime;
                float totalCost = (float)duration.TotalHours * costPerHour;

                Console.WriteLine($"Total duration: {duration.TotalHours} hours");
                Console.WriteLine($"Total cost: NRS,{totalCost}");

                GeneratePDF(fullName, projectName, startTime, endTime, duration, totalCost);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid date/time format. Please enter the date/time in the specified format.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            Console.ReadLine();
        }

        private void AutomaticTimeTracking(string fullName, string projectName, float automaticDurationHours)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                DateTime endTime = startTime.AddHours(automaticDurationHours);
                TimeSpan pomodoroWorkDuration = TimeSpan.FromMinutes(pomodoroWorkDurationMinutes);
                TimeSpan pomodoroBreakDuration = TimeSpan.FromMinutes(pomodoroBreakDurationMinutes);

                Console.WriteLine($"Automatic timer started for {automaticDurationHours} hours from now.");

                float totalWorkHours = 0;

                while (DateTime.Now < endTime)
                {
                    Console.WriteLine($"Work session: {DateTime.Now} - {DateTime.Now.Add(pomodoroWorkDuration)}");
                    System.Threading.Thread.Sleep(pomodoroWorkDuration);
                    totalWorkHours += pomodoroWorkDurationMinutes / 60;

                    if (DateTime.Now < endTime)
                    {
                        Console.WriteLine($"Break: {DateTime.Now.Add(pomodoroWorkDuration)} - {DateTime.Now.Add(pomodoroWorkDuration + pomodoroBreakDuration)}");
                        System.Threading.Thread.Sleep(pomodoroBreakDuration);
                    }
                }

                Console.WriteLine($"Automatic timer stopped after {automaticDurationHours} hours.");

                float totalCost = totalWorkHours * costPerHour;
                Console.WriteLine($"Total duration: {totalWorkHours} hours");
                Console.WriteLine($"Total cost: ${totalCost}");
                GeneratePDF(fullName, projectName, startTime, endTime, endTime - startTime, totalCost);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            Console.ReadLine();
        }

        private void GeneratePDF(string fullName, string projectName, DateTime startTime, DateTime endTime, TimeSpan totalDuration, float totalCost)
        {
            try
            {
                string pdfFileName = $"CostReport {fullName} {projectName} {DateTime.Now.Hour}h.{DateTime.Now.Minute}m.{DateTime.Now.Second}s.pdf";

                using (PdfWriter writer = new PdfWriter(pdfFileName))
                {
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        Document document = new Document(pdf);

                        // Title
                        Paragraph title = new Paragraph("Time Tracking Cost Report").SetFontSize(20).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                        document.Add(title);

                        // Information
                        Paragraph information = new Paragraph()
                            .Add(new Text($"Full Name: {fullName}\n").SetBold())
                            .Add(new Text($"Project Name: {projectName}\n").SetBold())
                            .Add(new Text($"Start Time: {startTime.ToString("yyyy-MM-dd HH:mm:ss")}\n").SetBold())
                            .Add(new Text($"End Time: {endTime.ToString("yyyy-MM-dd HH:mm:ss")}\n").SetBold())
                            .Add(new Text($"Total Duration: {totalDuration.TotalHours:F2} hours\n").SetBold())
                            .Add(new Text($"Total Cost: NRS,{totalCost:F2}").SetBold());
                        document.Add(information);

                        // Footer
                        Paragraph footer = new Paragraph("Generated by TimeTracker App").SetFontSize(10).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                        document.Add(footer);
                    }
                }
                Console.ReadLine();
                Console.WriteLine($"PDF generated successfully at {Path.GetFullPath(pdfFileName)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while generating PDF: {ex.Message}");
            }
        }
    }
}

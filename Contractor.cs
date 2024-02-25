using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace TimeTracker
{
    public class Contractor
    {
        private float costPerHour { get; set; }
        private const float pomodoroWorkDurationMinutes = 45;
        private const float pomodoroBreakDurationMinutes = 15;

        public Contractor(float costPerHour)
        {
            this.costPerHour = costPerHour;
        }

        public void StartTimer()
        {
            Console.WriteLine("Choose timer mode:");
            Console.WriteLine("1. Manual");
            Console.WriteLine("2. Automatic");
            Console.Write("Enter your choice (1 or 2): ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                ManualTimeTracking();
            }
            else if (choice == "2")
            {
                Console.Write("Enter the desired duration of automatic time tracking (in hours): ");
                float automaticDurationHours = float.Parse(Console.ReadLine());
                AutomaticTimeTracking(automaticDurationHours);
            }
            else
            {
                Console.WriteLine("Invalid choice. Please enter 1 or 2.");
            }
        }

        private void ManualTimeTracking()
        {
            Console.WriteLine("Enter the start time (format: yyyy-MM-dd HH:mm:ss): ");
            DateTime startTime = DateTime.Parse(Console.ReadLine());

            Console.WriteLine("Enter the end time (format: yyyy-MM-dd HH:mm:ss): ");
            DateTime endTime = DateTime.Parse(Console.ReadLine());

            TimeSpan duration = endTime - startTime;
            float totalCost = (float)duration.TotalHours * costPerHour;

            Console.WriteLine($"Total duration: {duration.TotalHours} hours");
            Console.WriteLine($"Total cost: NRS,{totalCost}");
            Console.ReadLine();

            GeneratePDF(startTime, endTime, duration, totalCost);
            Console.ReadLine();
        }

        private void AutomaticTimeTracking(float automaticDurationHours)
        {
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.AddHours(automaticDurationHours);
            TimeSpan pomodoroWorkDuration = TimeSpan.FromMinutes(pomodoroWorkDurationMinutes); // TimeSpan is a struct for an interval
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
            GeneratePDF(startTime, endTime, endTime - startTime, totalCost);
            Console.ReadLine();
        }

        private void GeneratePDF(DateTime startTime, DateTime endTime, TimeSpan totalDuration, float totalCost)
        {
            string pdfFileName = $"CostReport {DateTime.Now.Hour}h.{DateTime.Now.Minute}m.{DateTime.Now.Second}s.pdf";

            try
            {
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
                            .Add(new Text($"Start Time: {startTime.ToString("yyyy-MM-dd HH:mm:ss")}\n").SetBold())
                            .Add(new Text($"End Time: {endTime.ToString("yyyy-MM-dd HH:mm:ss")}\n").SetBold())
                            .Add(new Text($"Total Duration: {totalDuration.TotalHours:F2} hours\n").SetBold())
                            .Add(new Text($"Total Cost: NRS,{totalCost:F2}").SetBold());
                        document.Add(information);

                        // Footer
                        Paragraph footer = new Paragraph("Geneated by TimeTracker App").SetFontSize(10).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                        document.Add(footer);
                    }
                }

                Console.WriteLine($"PDF generated successfully at {Path.GetFullPath(pdfFileName)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while generating PDF: {ex.Message}");
            }
        }


    }
}
    

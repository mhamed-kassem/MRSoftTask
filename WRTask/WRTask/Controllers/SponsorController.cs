using ClosedXML.Excel;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using WRTask.Models;


namespace WRTask.Controllers
{
    public class SponsorController : Controller
    {
        private MRTaskDBContext Context;
        List<Sponsor> SponsorsOnShow;
        static int Currentitemindex;
        Person SignedInUser;
        public SponsorController(MRTaskDBContext context)
        {
            Context = context;

            SponsorsOnShow = Context.Sponsors.ToList();

            //get user session here current user
            int id = 1;
            SignedInUser = Context.People.Find(id);

        }
        public IActionResult Index()
        {
            return View();
        }
        //-------------------------------
        public IActionResult ShowAll()
        {
            SponsorsOnShow = Context.Sponsors.ToList();
            return View(SponsorsOnShow);
        }
        //-------------------------------
        public ActionResult Create()
        {
            return View();
        }
        //--------
        [HttpPost]
        public ActionResult Create(Sponsor sponsor)
        {
            if (ModelState.IsValid)
            {   
                //chech that Code Not Existed before
                if (Context.Sponsors.Find(sponsor.Code) == null)
                {
                    sponsor.CreatedByPersonId = SignedInUser.Id;
                    if (sponsor.Authorized)
                    {
                        sponsor.AuthorizedByPersonId = SignedInUser.Id;
                    }

                    Context.Sponsors.Add(sponsor);
                    Context.SaveChanges();

                    TempData["Message"] = "Sponsor: " + sponsor.Name + " Created Successfully";
                    
                    return RedirectToAction("Index");
                }
                //if reached here then code is repeated 
                TempData["CodeEXisted"] ="Code "+ sponsor.Code + " Already Existed";
            }

            return View(sponsor);
        }
        //-----------------------------------
        public ActionResult Update(int Id)
        {   
            Sponsor updateSponsor = Context.Sponsors.Find(Id);

            if (updateSponsor != null)
            {   
                //chech Authorization
                if (updateSponsor.Authorized)
                {
                    TempData["Message"] = updateSponsor.Name + " is Authorized, You can not update it.";

                    return RedirectToAction("Index");
                }

                return View(updateSponsor);
            }
            //Default Action to do 
            return RedirectToAction("Index");
        }
        //--------
        [HttpPost]
        public ActionResult Update(Sponsor sponsor)
        {
            if (ModelState.IsValid)
            {
                Sponsor DBSponsor = Context.Sponsors.Find(sponsor.Code);
                //check Authorization
                if (DBSponsor.Authorized)
                {
                    TempData["Message"] = DBSponsor.Name + " is Authorized, You can not update it.";

                    return RedirectToAction("Index");
                }
                //Chech Nullable to Avoid DB errors
                if (DBSponsor != null && sponsor != null)
                {
                    DBSponsor.Name = sponsor.Name;
                    DBSponsor.Authorized = sponsor.Authorized;
                    if (sponsor.Authorized)
                    {
                        sponsor.AuthorizedByPersonId = SignedInUser.Id;
                    }
                    Context.SaveChanges();

                    TempData["Message"] = DBSponsor.Name + " updated Successfully";
                }
                //Default Action to do 
                return RedirectToAction("Index");
            }

            //Go back to update view with the same date entered if it was invalid data
            return View(sponsor);

        }
        //-------------------------------------------

        public ActionResult Delete(int code)
        {
            Sponsor DBSponsor = Context.Sponsors.Find(code);
            //chech nullable & wrong Sponder code
            if (DBSponsor != null)
            {   
                //chech Authorizathion
                if (DBSponsor.Authorized)
                {
                    TempData["Message"] = DBSponsor.Name + " is Authorized, You can not delete it.";

                    return RedirectToAction("Index");
                }

                Context.Sponsors.Remove(DBSponsor);

                Context.SaveChanges();

                TempData["Message"] = DBSponsor.Name + " deleted Successfully";
            }
            //default Action
            return RedirectToAction("Index");
        }

        //------------------
        public ActionResult CodeSearch(int code) 
        {
            List<Sponsor> sponsorList = Context.Sponsors.Where(s=>s.Code==code).ToList();
            if (sponsorList.Count < 1)
            {
                return Ok("No Results");              
            }

            SponsorsOnShow = sponsorList;

            return View("ShowAll",sponsorList);
        }
        public ActionResult NameSearch(string name )
        {
            List<Sponsor> sponsorsList = Context.Sponsors.Where(s => s.Name.Contains(name)).ToList();
            if (sponsorsList.Count<1)
            {
                return Ok("No Results");
            }
            SponsorsOnShow = sponsorsList;
            return View("ShowAll", sponsorsList);
        }

        public ActionResult Details(int code)
        {
            Sponsor sponsor = Context.Sponsors.Find(code);
            if (sponsor != null)
            {
                Currentitemindex = SponsorsOnShow.IndexOf(sponsor);
                return View(sponsor);
            }
            return RedirectToAction("Index");
        }
        public ActionResult Previous() 
        {
            Currentitemindex--;
            if (Currentitemindex < 0)
            {
                Currentitemindex = SponsorsOnShow.Count-1;
            }
            
            return View("Details", SponsorsOnShow[Currentitemindex]);        
            
        }
        public ActionResult Next()
        {
            Currentitemindex +=1;
            if (Currentitemindex >= SponsorsOnShow.Count)
            {
                Currentitemindex = 0;
            }
            
            return View("Details", SponsorsOnShow[Currentitemindex]);
            
        }

        public IActionResult PrintToPDF()
        {  
            string path = "D:/workTask/SponsorReport.pdf";

            PdfWriter writer = new PdfWriter(path);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);


            Paragraph header = new Paragraph("Sponsors Report")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20);
            document.Add(header);

            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            // Table --------------
            Table table = new Table(5, true);
            table.SetBackgroundColor(ColorConstants.LIGHT_GRAY)
               .SetTextAlignment(TextAlignment.CENTER);
            
            Cell Code = new Cell(1, 1).Add(new Paragraph("Code")).SetFontSize(20);
            table.AddCell(Code);

            Cell SponSorName = new Cell(1, 1).Add(new Paragraph("SponSor Name")).SetFontSize(20);
            table.AddCell(SponSorName);
            
            Cell ISAuthorized = new Cell(1, 1).Add(new Paragraph("IS Authorized?")).SetFontSize(20);
            table.AddCell(ISAuthorized);
            
            Cell CreatedBy = new Cell(1, 1).Add(new Paragraph("Created By")).SetFontSize(20);
            table.AddCell(CreatedBy);
            
            Cell AuthorizedBy = new Cell(1, 1).Add(new Paragraph("Authorized By")).SetFontSize(20); 
            table.AddCell(AuthorizedBy);

            //inserting property values to table rows
            foreach (Sponsor item in SponsorsOnShow)
            {
                string Creator = item.CreatedByPerson != null ? item.CreatedByPerson.Name : "";
                string Authorizor = item.AuthorizedByPerson != null ? item.AuthorizedByPerson.Name : "";

                table.AddCell(new Cell(1, 1).Add(new Paragraph(item.Code.ToString())).SetFontSize(12));
                table.AddCell(new Cell(1, 1).Add(new Paragraph(item.Name)).SetFontSize(12));
                table.AddCell(new Cell(1, 1).Add(new Paragraph(item.Authorized.ToString())).SetFontSize(12));
                table.AddCell(new Cell(1, 1).Add(new Paragraph(Creator)).SetFontSize(12));
                table.AddCell(new Cell(1, 1).Add(new Paragraph(Authorizor)).SetFontSize(12));

                
            }

            document.Add(table);


            document.Close();

            return RedirectToAction("Index");
        }//printToPDF

       


        public IActionResult PrintToExel()
        {
            var workbook = new XLWorkbook();
            workbook.AddWorksheet("sheetName");
            var ws = workbook.Worksheet("sheetName");

            //header
            ws.Cell("A1").Value = "Code";
            ws.Cell("B1").Value = "Name";
            ws.Cell("C1").Value = "Is Authorized?";
            ws.Cell("D1").Value = "Created By";
            ws.Cell("E1").Value = "Authorized By";

            int row = 2;
            foreach (Sponsor item in SponsorsOnShow)
            {
                ws.Cell("A" + row.ToString()).Value = item.Code.ToString();
                ws.Cell("B" + row.ToString()).Value = item.Name;
                ws.Cell("C" + row.ToString()).Value = item.Authorized.ToString();
                ws.Cell("D" + row.ToString()).Value = item.CreatedByPerson!=null?item.CreatedByPerson.Name:"";
                ws.Cell("E" + row.ToString()).Value = item.AuthorizedByPerson != null ? item.AuthorizedByPerson.Name : "";
                row++;
            }
           
            workbook.SaveAs("D:/workTask/SponsorsExcel.xlsx");


            return RedirectToAction("Index");
        }
    }
}

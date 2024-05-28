using System.Net.Mail;
using System.Net;
using OfficeOpenXml;
using Quartz;
using Construction.Models;
namespace LosevStadium.Jobs
{
    public class ReportSender : IJob
    {
        string file_path_template;
        string file_path_report;
        private readonly OobjectDBContext _context;
        private readonly IWebHostEnvironment _appEnvironment;
        public ReportSender (OobjectDBContext context, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
        }
        public void PrepareReport()
        {
            //будем использовть библитотеку не для коммерческого использования
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            //открываем файл с шаблоном
            using (ExcelPackage excelPackage = new ExcelPackage(file_path_template))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "Лиинючев Н.А.";
                excelPackage.Workbook.Properties.Title = "Список объектов";
                excelPackage.Workbook.Properties.Subject = "Объекты";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet =
               excelPackage.Workbook.Worksheets["Oobject"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 3;
                List<Oobject> Oobjects = _context.Oobjects.ToList();
                foreach (Oobject oobject in Oobjects)
                {
                    worksheet.Cells[startLine, 1].Value = startLine - 2;
                    worksheet.Cells[startLine, 2].Value = oobject.OobjectID;
                    worksheet.Cells[startLine, 3].Value = oobject.Title;
                    worksheet.Cells[startLine, 4].Value = oobject.Adress;
                    worksheet.Cells[startLine, 5].Value = oobject.Type;
                    worksheet.Cells[startLine, 6].Value = oobject.Status;  
                    startLine++;
                }
                //созраняем в новое место
                excelPackage.SaveAs(file_path_report);
            }
        }
        public async Task Execute(IJobExecutionContext context)
        {
            // Путь к файлу с шаблоном
            string path = @"/Reports/report_template.xlsx";
            //Путь к файлу с результатом
            string result = @"/Reports/report.xlsx";
            file_path_template = _appEnvironment.WebRootPath + path;
            file_path_report = _appEnvironment.WebRootPath + result;
            try
            {
                if (File.Exists(file_path_report))
                    File.Delete(file_path_report);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            PrepareReport();
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("nekit_mokin@mail.ru", "Nikita");
            // кому отправляем
            MailAddress to = new MailAddress("nekit_mokin@mail.ru");
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Тест";
            // текст письма
            m.Body = "<h2>Письмо-тест работы smtp-клиента</h2>";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("nekit_mokin@mail.ru",
            "123456789-=");
            smtp.EnableSsl = true;
            // вкладываем файл в письмо
            m.Attachments.Add(new Attachment(file_path_report));
            // отправляем асинхронно
            await smtp.SendMailAsync(m);
            m.Dispose();
        }
    }
}

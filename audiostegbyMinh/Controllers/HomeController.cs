using audiostegbyMinh.Models;
using Google.Apis.Drive.v3;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace audiostegbyMinh.Controllers
{
    public class HomeController : Controller
    {

        private Functions file;
        private audioSteg sh;
        private string message;
        [HttpGet]
        public ActionResult GetGoogleDriveFiles()
        {
            return View(GoogleDriveFilesRepository.GetDriveFiles());
        }

        //Check watermark
        public ActionResult Check()
        {
            return View();
        }

        //Khi check xong
        [HttpPost]
        public ActionResult Checked(HttpPostedFileBase file)
        {
            string signature = GoogleDriveFilesRepository.checkWatermark(file);
            ViewBag.Message = signature;
            return View();
        }

        //Xóa file only for admin
        [HttpPost]
        public ActionResult DeleteFile(GoogleDriveFiles file)
        {
            GoogleDriveFilesRepository.DeleteFile(file);
            return RedirectToAction("GetGoogleDriveFiles");
        }
        //Upload only for admin
        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            GoogleDriveFilesRepository.FileUpload(file);
            return RedirectToAction("GetGoogleDriveFiles");
        }
        //Download và watermark vào file wav
        public void DownloadFile(string id)
        {
            string FilePath = GoogleDriveFilesRepository.DownloadGoogleFile(id);
            //string FilePath1 = GoogleDriveFilesRepository.DownloadGoogleFile(id);
            file = new Functions(new FileStream(FilePath, FileMode.Open, FileAccess.Read));
            sh = new audioSteg(file);

            message = "Copyright by Hoang Anh Minh, Download at: " + DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy zzzz");
            sh.waterMessage(message);
            file.writeFile(FilePath);
            //string FolderPath = System.Web.HttpContext.Current.Server.MapPath("/WavSteg/");
            //FilesResource.GetRequest request = service.Files.Get(id);
            //string FileName = request.Execute().Name;
            //string FilePath1 = System.IO.Path.Combine(FolderPath, FileName);
            //Stream sourceStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
            //FileStream destinationStream = new FileStream(FilePath1, FileMode.Create);
            //WaveStream audioStream = null;
            //audioStream = new WaveStream(sourceStream, destinationStream);
            //create a stream that contains the message, preceeded by its length
            //Stream messageStream = GetMessageStream();
            //if(messageStream != null)
            //{
            //    messageStream.Close();
            //}
            //if (audioStream != null) { audioStream.Close(); }
            //if (sourceStream != null) { sourceStream.Close(); }

            //Giau tin vao audio
            //WaveUtility utility = new WaveUtility(audioStream, destinationStream);
            //utility.Hide(messageStream);
            //if (destinationStream != null) { destinationStream.Close(); }
            Response.ContentType = "application /zip";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + Path.GetFileName(FilePath));
            Response.WriteFile(System.Web.HttpContext.Current.Server.MapPath("~/GoogleDriveFiles/" + Path.GetFileName(FilePath)));
            Response.End();
            Response.Flush();
        }
        //private Stream GetMessageStream()
        //{
        //    message = "Copyright by Hoang Anh Minh, Download at: " + DateTime.Now.ToString("HH:mm:ss dd-MM-yyyy zzzz");
        //    BinaryWriter messageWriter = new BinaryWriter(new MemoryStream());
        //    messageWriter.Write(Encoding.ASCII.GetBytes(message));
        //    messageWriter.Seek(0, SeekOrigin.Begin);
        //    return messageWriter.BaseStream;
        //}
    }
}
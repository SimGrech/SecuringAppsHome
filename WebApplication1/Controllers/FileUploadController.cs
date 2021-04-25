using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Utility;

namespace WebApplication1.Controllers
{
    public class FileUploadController : Controller
    {
        public IActionResult Upload(IFormFile file)
        {
            //you get who is the logged in user
            //User.Identity.Name

            //fetch the public key for the logged in user

        //    var encryptedfileMemoryStream = Encryption.HybridEncrypt(..., publicKey);

         //   System.IO.File.WriteAllBytes(..., encryptedfileMemoryStream.ToArray());
//

            return View();
        }


        //TO CHANGE WITH ENCRYPTION!
        /*
        public IActionResult Download(string id)
        {
            //1. get who is the owner of the file with id = id
            //2. you fetch the private key
            //3. call the HybridDecrypt 
            MemoryStream toDownload;// = HybridDecrypt(...)


            return File(toDownload, "application/octet-stream", Guid.NewGuid() + ".pdf");

        }
        */
    }
}

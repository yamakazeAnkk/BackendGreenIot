using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreenIotApi.Services.InterfaceService
{
    public interface IFirebaseStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string folderName);
        

        
    }
}
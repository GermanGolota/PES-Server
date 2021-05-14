using Application.Contracts.Service;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Services
{
    public class ChatImageService : IChatImageService
    {
        private const string IMAGES_FOLDER_NAME = "ChatImages";

        private readonly IWebHostEnvironment _webHost;

        public ChatImageService(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }

        private string GetBaseImagesLocation()
        {
            return Path.Combine(_webHost.ContentRootPath, IMAGES_FOLDER_NAME);
        }

        public string GetRelativeImageLocation(Guid chatId)
        {
            var root = GetBaseImagesLocation();
            var directoryLocation = Path.Combine(root, chatId.ToString());
            string result;
            if (Directory.Exists(directoryLocation))
            {
                var fileName = GetImageFileName(directoryLocation);
                if (String.IsNullOrEmpty(fileName))
                {
                    result = GetDefaultImageLocation();
                }
                else
                {
                    result = Path.Combine(directoryLocation, fileName);
                }
            }
            else
            {
                result = GetDefaultImageLocation();
            }
            return result;
        }

        private string GetImageFileName(string directoryLocation)
        {
            string result = "";
            var imageDirectory = new DirectoryInfo(directoryLocation);
            var images = imageDirectory.GetFiles();
            foreach (var image in images)
            {
                string name = Path.GetFileNameWithoutExtension(image.Name);
                if (name.Equals("image"))
                {
                    result = image.Name;
                    break;
                }
            }
            return result;
        }

        private string GetDefaultImageLocation()
        {
            return Path.Combine(GetBaseImagesLocation(), "default.png");
        }
    }
}

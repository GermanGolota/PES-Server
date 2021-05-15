using Application.Contracts.Service;
using Application.DTOs.Chat;
using Core.Exceptions;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebAPI.Services
{
    public class ChatImageService : IChatImageService
    {
        private const string IMAGES_FOLDER_NAME = "ChatImages";
        private const string IMAGE_FILE_NAME = "image";
        private static readonly List<string> _supportedExtensions = new List<string> { "png", "jpg", "jpeg" };

        private readonly IWebHostEnvironment _webHost;

        public ChatImageService(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }

        private string GetBaseImagesLocation()
        {
            return Path.Combine(_webHost.WebRootPath, IMAGES_FOLDER_NAME);
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
                    result = GetRelativeDefaultImageLocation();
                }
                else
                {
                    result = $"/{IMAGES_FOLDER_NAME}/{chatId}/{fileName}";
                }
            }
            else
            {
                result = GetRelativeDefaultImageLocation();
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

        private string GetRelativeDefaultImageLocation()
        {
            return $"/{IMAGES_FOLDER_NAME}/default.png";
        }

        public async Task UpdateChatsImage(Guid chatId, ChatImageUpdateRequest request)
        {
            if (_supportedExtensions.Contains(request.FileExtension))
            {
                var folderLocation = Path.Combine(GetBaseImagesLocation(), chatId.ToString());
                CreateDirectoryIfNotExist(folderLocation);
                var imageLocation = Path.Combine(folderLocation, $"{IMAGE_FILE_NAME}.{request.FileExtension}");
                if (File.Exists(imageLocation))
                {
                    File.Delete(imageLocation);
                }
                using (FileStream image = new FileStream(imageLocation, FileMode.Create))
                {
                    await request.ImageStream.CopyToAsync(image);
                }
            }
            else
            {
                throw new ExpectedException(ExceptionMessages.UnsupportedFileFormat);
            }
        }

        private void CreateDirectoryIfNotExist(string location)
        {
            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }
        }
    }
}

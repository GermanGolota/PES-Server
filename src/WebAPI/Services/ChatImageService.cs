using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application.Contracts.Service;
using Application.DTOs.Chat;
using Core.Exceptions;
using Microsoft.AspNetCore.Hosting;

namespace WebAPI.Services;

public class ChatImageService : IChatImageService
{
    private const string IMAGES_FOLDER_NAME = "ChatImages";
    private const string IMAGE_FILE_NAME = "image";
    private static readonly List<string> _supportedExtensions = new() { "png", "jpg", "jpeg", "gif" };

    private readonly IWebHostEnvironment _webHost;

    public ChatImageService(IWebHostEnvironment webHost)
    {
        _webHost = webHost;
    }

    public string GetRelativeImageLocation(Guid chatId)
    {
        string root = GetBaseImagesLocation();
        string directoryLocation = Path.Combine(root, chatId.ToString());
        string result;
        if (Directory.Exists(directoryLocation))
        {
            string fileName = GetImageFileName(directoryLocation);
            if (string.IsNullOrEmpty(fileName))
                result = GetRelativeDefaultImageLocation();
            else
                result = $"/{IMAGES_FOLDER_NAME}/{chatId}/{fileName}";
        }
        else
        {
            result = GetRelativeDefaultImageLocation();
        }

        return result;
    }

    public async Task UpdateChatsImage(Guid chatId, ChatImageUpdateRequest request)
    {
        if (_supportedExtensions.Contains(request.FileExtension))
        {
            string folderLocation = Path.Combine(GetBaseImagesLocation(), chatId.ToString());
            CreateDirectoryIfNotExist(folderLocation);
            string imageLocation = Path.Combine(folderLocation, $"{IMAGE_FILE_NAME}.{request.FileExtension}");
            DeleteChatImage(chatId, imageLocation);

            using (FileStream image = new(imageLocation, FileMode.Create))
            {
                await request.ImageStream.CopyToAsync(image);
            }
        }
        else
        {
            throw new ExpectedException(ExceptionMessages.UnsupportedFileFormat);
        }
    }

    private string GetBaseImagesLocation()
    {
        return Path.Combine(_webHost.WebRootPath, IMAGES_FOLDER_NAME);
    }

    private string GetAbsoluteImageLocation(Guid chatId)
    {
        string root = GetBaseImagesLocation();
        string directoryLocation = Path.Combine(root, chatId.ToString());
        string result;
        if (Directory.Exists(directoryLocation))
        {
            string fileName = GetImageFileName(directoryLocation);
            if (string.IsNullOrEmpty(fileName))
                result = GetAbsoluteDefaultImageLocation();
            else
                result = Path.Combine(root, chatId.ToString(), fileName);
        }
        else
        {
            result = GetAbsoluteDefaultImageLocation();
        }

        return result;
    }

    private string GetImageFileName(string directoryLocation)
    {
        string result = "";
        DirectoryInfo imageDirectory = new DirectoryInfo(directoryLocation);
        FileInfo[] images = imageDirectory.GetFiles();
        foreach (FileInfo image in images)
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

    private string GetAbsoluteDefaultImageLocation()
    {
        return Path.Combine(GetBaseImagesLocation(), "default.png");
    }

    private void DeleteChatImage(Guid chatId, string imageLocation)
    {
        DeleteFileIfExists(imageLocation);

        string chatImage = GetAbsoluteImageLocation(chatId);
        string defaultImage = GetAbsoluteDefaultImageLocation();
        if (chatImage != defaultImage) DeleteFileIfExists(chatImage);
    }

    private void DeleteFileIfExists(string location)
    {
        if (File.Exists(location)) File.Delete(location);
    }

    private void CreateDirectoryIfNotExist(string location)
    {
        if (!Directory.Exists(location)) Directory.CreateDirectory(location);
    }
}
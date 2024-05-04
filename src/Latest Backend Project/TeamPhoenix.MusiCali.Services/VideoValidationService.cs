using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Security;

// for checking resolution, we need to use 3rd party library like MediaToolKit, need to write DAR report
// currently we only check for the type, and size

namespace TeamPhoenix.MusiCali.Services
{
    public class VideoValidationService
    {
        private readonly IConfiguration configuration;
        private readonly LoggerService loggerService;
        private long maxFileSize;
        private string[] allowedExtensions;

        public VideoValidationService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.loggerService = loggerService;
            maxFileSize = configuration.GetValue<long>("VideoSettings:MaxFileSize");
            allowedExtensions = configuration.GetSection("VideoSettings:AllowedFormats").Get<string[]>();
        }

        public bool IsValidVideoFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > maxFileSize)
            {
                loggerService.LogFeature("username", "File size exceeds maximum limit of " + maxFileSize + " bytes");
                return false; // File is too large need to read username for logger to work
            }

            string fileExtension = Path.GetExtension(filePath).ToLower();
            if (Array.IndexOf(allowedExtensions, fileExtension) == -1)
            {
                loggerService.LogFeature("username", "File type " + fileExtension + " is not supported");
                return false; // File format not supported
            }

            return true; // File size and format are valid
        }
    }
}


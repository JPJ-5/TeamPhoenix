using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using TeamPhoenix.MusiCali.DataAccessLayer.Models;
using TeamPhoenix.MusiCali.Logging;
using TeamPhoenix.MusiCali.Security;

namespace TeamPhoenix.MusiCali.Services
{
    public class ImageValidationService
    {
        private readonly IConfiguration configuration;
        private readonly LoggerService loggerService;
        private readonly Dictionary<string, List<byte[]>> allowedFormatSignatures;
        private readonly long maxFileSize;

        public ImageValidationService(IConfiguration configuration, LoggerService loggerService)
        {
            this.configuration = configuration;
            this.loggerService = loggerService;
            allowedFormatSignatures = LoadAllowedFormatSignatures();
            maxFileSize = configuration.GetValue<long>("ImageSettings:MaxFileSize");
        }

        private Dictionary<string, List<byte[]>> LoadAllowedFormatSignatures()
        {
            var signatures = new Dictionary<string, List<byte[]>>();
            var formatsConfig = configuration.GetSection("ImageSettings:AllowedFormats").Get<IDictionary<string, string[]>>();
            foreach (var format in formatsConfig)
            {
                signatures[format.Key] = format.Value.Select(hex => StringToByteArray(hex)).ToList();
            }
            return signatures;
        }

        private byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public bool IsValidImageFormat(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Length > maxFileSize)
            {
                loggerService.LogFeature("username", "File size exceeds maximum limit of " + maxFileSize + " bytes");
                return false; // File is too large need to read username for logger to work
            }

            byte[] headerBytes = new byte[4];
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (stream.Read(headerBytes, 0, headerBytes.Length) != headerBytes.Length)
                    {
                        return false; // Not able to read the header, so can't validate
                    }
                }
            }
            catch (Exception ex)
            {
                loggerService.LogFeature("username", "Failed to read file header: " + ex.Message);
                return false;            // read the username for logger to work
            }

            // Check if header matches any of the allowed signatures
            return allowedFormatSignatures.Any(format =>
                format.Value.Any(signature => HeaderMatches(headerBytes, signature)));
        }

        private bool HeaderMatches(byte[] headerBytes, byte[] signature)
        {
            if (signature.Length > headerBytes.Length)
                return false;

            for (int i = 0; i < signature.Length; i++)
            {
                if (headerBytes[i] != signature[i])
                    return false;
            }

            return true;
        }
    }
}

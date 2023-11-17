using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Configuration;
using MoodleBot.Business.Entity;
using MoodleBot.Common;
using MoodleBot.Persistent.Entity;
using MoodleBot.Persistent.ExternalService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MoodleBot.Business.Moodle
{
    public class MoodleCertificate : IMoodleCertificate
    {
        #region Properties
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly ICertificate _certificate;
        #endregion

        #region Consturctor
        public MoodleCertificate(IConfiguration configuration, ILogger logger, ICertificate certificate)
        {
            _logger = logger;
            _configuration = configuration;
            _certificate = certificate;
        }
        #endregion

        #region Public Method
        public async Task<CertificateDetailMessage> PrepareCourseCertificate(long userId, long courseId)
        {
            var certificateDetails = await GetCertificateDetail(userId, courseId);
            return await PrepareCourseCertificate(certificateDetails, userId, courseId);
        }

        public async Task<CertificateDetailMessage> PrepareCourseCertificate(List<MoodleCertificateDetail> certificateDetails, long userId, long courseId)
        {
            var certificateDetailMessage = new CertificateDetailMessage
            {
                CertificateUrl = new List<string>(),
                IsCertificateAvailable = false
            };

            try
            {
                if (certificateDetails?.Count > 0)
                {
                    foreach (var certificate in certificateDetails)
                    {
                        var certificateUrl = await CreateCertificatePdfFile(certificate, userId);
                        if (certificateUrl.IsNotNullOrEmpty())
                        {
                            certificateDetailMessage.CertificateUrl.Add(certificateUrl);
                            certificateDetailMessage.IsCertificateAvailable = true;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"PrepareCourseCertificate: Got an error while creating course certificate for userId {userId} and courseId {courseId}", exception, null, userId);
            }

            return certificateDetailMessage;
        }

        public async Task<List<MoodleCertificateDetail>> GetCertificateDetail(long userId, long courseId)
        {
            List<MoodleCertificateDetail> certificateDetail = null;

            try
            {
                if (userId > 0 && courseId > 0)
                {
                    certificateDetail = await _certificate.GetCourseCertificateDetail(userId, courseId);
                    if (certificateDetail?.Count > 0)
                    {
                        _logger.Info($"GetCertificateDetail: Successfully get the certificate detail for courseId: {courseId}", null, userId);
                    }
                }
                else
                {
                    _logger.Info("GetCertificateDetail: UserId or CourseId is not found.");
                }
            }
            catch (Exception exception)
            {
                _logger.Error($"GetCertificateDetail: Got an error while get the certificate data for courseId: {courseId}.", exception, null, userId);
            }

            return certificateDetail;
        }
        #endregion

        #region Private Method
        private async Task<string> CreateCertificatePdfFile(MoodleCertificateDetail certificateDetail, long userId)
        {
            var pdfFileUrl = string.Empty;
            var pdfFileLocation = await CreatePDFFile(certificateDetail, userId);
            if (pdfFileLocation.IsNotNullOrEmpty())
            {
                pdfFileUrl = await GenerateFileHostUrl(pdfFileLocation, userId, certificateDetail.FullName);
            }

            return pdfFileUrl;
        }

        #endregion

        #region Create PDF file
        private async Task<string> CreatePDFFile(MoodleCertificateDetail certificateDetail, long userId)
        {
            var pdfFileLocation = string.Empty;
            try
            {
                var fileLocation = $@"CertificateData/{GenerateFileName(userId, certificateDetail.FullName)}";
                var file = new FileInfo(fileLocation);

                if (!Directory.Exists(file.DirectoryName))
                {
                    Directory.CreateDirectory(file.DirectoryName);
                }
                
                var pdfWriter = new PdfWriter(file);
                var pdfDoc = new PdfDocument(pdfWriter);
                var landscape = PageSize.A4.Rotate();
                pdfDoc.SetDefaultPageSize(new PageSize(landscape.GetWidth() - 32, landscape.GetHeight()));

                var document = new Document(pdfDoc);
                var imageData = ImageDataFactory.Create(certificateDetail.CertificateImage);
                var pdfImage = new Image(imageData);
                if (!string.IsNullOrEmpty(certificateDetail.Title)) {
                    pdfImage = await AddDetailOnImage(pdfDoc, pdfImage, certificateDetail.Title.ToUpper(), PdfFontFactory.CreateFont(@"CertificateData/Font/Lato-Bold.ttf"), 20, certificateDetail.TitlePosX, certificateDetail.TitlePosY, true);
                }
                if (!string.IsNullOrEmpty(certificateDetail.Description)) {
                    pdfImage = await AddDetailOnImage(pdfDoc, pdfImage, certificateDetail.Description.ToUpper(), PdfFontFactory.CreateFont(@"CertificateData/Font/Lato-Black.ttf"), 15, certificateDetail.DescriptionPosX, certificateDetail.DescriptionPosY, false);
                }
                pdfImage = await AddDetailOnImage(pdfDoc, pdfImage, certificateDetail.FullName.ToUpper(), PdfFontFactory.CreateFont(@"CertificateData/Font/Lato-ThinItalic.ttf"), 20, certificateDetail.FullnamePosX, certificateDetail.FullnamePosY, false);
                if (!string.IsNullOrEmpty(certificateDetail.Course)) {
                    pdfImage = await AddDetailOnImage(pdfDoc, pdfImage, certificateDetail.Course, PdfFontFactory.CreateFont(@"CertificateData/Font/Lato-Bold.ttf"), 15, certificateDetail.CoursePosX, certificateDetail.CoursePosY, false);
                }
                if (!string.IsNullOrEmpty(certificateDetail.CompletionDate)) {
                    pdfImage = await AddDetailOnImage(pdfDoc, pdfImage, certificateDetail.CompletionDate, PdfFontFactory.CreateFont(@"CertificateData/Font/Lato-Bold.ttf"), 15, certificateDetail.CompletiondatePosX, certificateDetail.CompletiondatePosY, false);
                }
                var table = new Table(UnitValue.CreatePercentArray(1)).UseAllAvailableWidth().SetWidth(UnitValue.CreatePercentValue(100));
                table.AddCell(new Cell().Add(pdfImage.SetAutoScale(true).SetWidth(UnitValue.CreatePercentValue(100))));
                document.Add(table);
                document.Close();
                pdfFileLocation = fileLocation;
            }
            catch (Exception exception)
            {
                _logger.Error("CreatePDFFile: Got an error while generating pdf file.", exception);
            }

            return pdfFileLocation;
        }

        private async Task<Image> AddDetailOnImage(PdfDocument pdfDoc, Image image, String watermark, PdfFont pdfFont, float fontSize, float x, float y, bool Isbold)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var width = image.GetImageScaledWidth();
                    var height = image.GetImageScaledHeight();
                    var template = new PdfFormXObject(new Rectangle(width, height));

                    //convert x and y from mm to px and subtruct the width and height from it.
                    float posx = (x * 3.7795275f);
                    float posy = height - (y * 3.7795275f);

                    new Canvas(template, pdfDoc).
                          Add(image).
                          SetFontColor(DeviceGray.BLACK).SetFont(pdfFont).
                          SetFontSize(fontSize).
                          ShowTextAligned(watermark, posx, posy, TextAlignment.CENTER);

                    if (Isbold)
                    {
                        new Canvas(template, pdfDoc).SetBold();
                    }

                    return new Image(template);
                });
            }
            catch (Exception exception)
            {
                _logger.Error("", exception);
            }

            return null;
        }

        private async Task<string> GenerateFileHostUrl(string filePath, long userId, string certificateName)
        {
            var fileUrl = string.Empty;

            try
            {
                var blobName = GenerateFileName(userId, certificateName);
                var blobServiceClient = new BlobServiceClient(_configuration.GetBlobStorageConfig("ConnectionString"));
                var blobContainer = blobServiceClient.GetBlobContainerClient(_configuration.GetBlobStorageConfig("ContainerName"));
                await blobContainer.CreateIfNotExistsAsync(PublicAccessType.Blob);
                var blobClient = blobContainer.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();

                var fileStream = new FileStream(filePath, FileMode.Open);
                
                // upload the file
                await blobClient.UploadAsync(fileStream);
                fileUrl = blobClient.GenerateSasUri(new BlobSasBuilder(BlobContainerSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1))).AbsoluteUri;
                fileStream.Close();
            }
            catch (Exception exception)
            {
                _logger.Error("GenerateFileHostUrl: Error while uploading file to BLOB", exception);
            }

            return fileUrl;
        }

        private string GenerateFileName(long userId, string certificateName)
        {
            return $@"coursecertificate/{userId}/{certificateName.ToLower().Replace(" ", "-")}.pdf";
        }
        #endregion
    }
}

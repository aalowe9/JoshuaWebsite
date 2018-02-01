using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Amazon.Internal;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace joshuaSite.Utilities
{
    public class S3Helper
    {

        public static void TestUpload()
        {
            string fileToBackup = HttpContext.Current.Server.MapPath("~/img/close.png"); // test file
            string myBucketName = "joshua-web-alexa-bucket"; //your s3 bucket name goes here
            string s3DirectoryName = "UploadsFromAlexa";
            string s3FileName = @"testfile2.png";


            //SaveToS3(fileToBackup, myBucketName, s3DirectoryName, s3FileName);
        }

        public static async Task<bool> SaveToS3(Stream file, string bucketName, string subDirectoryInBucket, string fileNameInS3)
        {
            BasicAWSCredentials credentials = new BasicAWSCredentials(ConfigurationManager.AppSettings["S3_AccessKey"], ConfigurationManager.AppSettings["S3_SecretKey"]);
            AmazonS3Client client = new AmazonS3Client(credentials, RegionEndpoint.EUWest1);

            TransferUtility utility = new TransferUtility(client);
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();
            request.CannedACL = S3CannedACL.PublicRead;
            if (subDirectoryInBucket == "" || subDirectoryInBucket == null)
            {
                request.BucketName = bucketName; //no subdirectory just bucket name
            }
            else
            {   // subdirectory and bucket name
                request.BucketName = bucketName + @"/" + subDirectoryInBucket;
            }
            request.Key = fileNameInS3; //file name up in S3
            
            request.InputStream = file; //local file name
            
            await utility.UploadAsync(request); //commensing the transfer

            return true;
        }
    }
}
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace twitch_alexa_skill.Services
{
    public static class Blobs
    {
        //public static async Task<BlobContainerClient> CreateBlobClient()
        //{
        //    return new BlobContainerClient(System.Environment.GetEnvironmentVariable("TABLE_CONNECTION_STRING"), "files");
        //}

        //public static Task SaveNewBlob(string content) 
        //{
        //    var blobclient = new BlobClient(System.Environment.GetEnvironmentVariable("TABLE_CONNECTION_STRING"), "files", "text.txt");
            

        //    var bytes = Encoding.UTF8.GetBytes(content);

        //    using (var ms = new MemoryStream(bytes)) 
        //    {
        //           blobclient.Upload(ms);
        //    }
               

        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Net.Http;

namespace ClientHTTP
{
    public class Client
    {
        public Dictionary<int, string> LoadedToServerFiles = new Dictionary<int, string>();
        public string requestedFileName = "";
        public string requestedFileSize = "";
        public byte[] requestedFileContent;

        const string httpServerURL = "http://localhost:9097/";

        const int MaxOneFileSize = 10485760;        //10МБ
        const int MaxAllFilesSize = 52428800;       //50МБ
        List<string> AcceptableExtensions = new List<string>() { ".doc", ".docx", ".txt", ".pdf", ".jpeg", ".jpg", ".png" };
        public int commonSizeOfLoadedFiles = 0;

        HttpClient client;
        public Client()
        {
            client = new HttpClient();
        }

        MultipartFormDataContent PrepareFileContentToSending(string filePath)
        {
            FileStream inputFile = new FileStream(filePath, FileMode.Open);
            int amount = (int)inputFile.Length;
            byte[] buffer = new byte[amount];
            try
            {
                inputFile.Read(buffer, 0, amount);
            }
            finally
            {
                inputFile.Close();
            }

            var byteArrayContent = new ByteArrayContent(buffer);
            var multipleDataContent = new MultipartFormDataContent();
            multipleDataContent.Add(byteArrayContent);
            return multipleDataContent;
        }

        public string CheckFileSizeAndExtension(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            if (!AcceptableExtensions.Contains(extension))
                return "Данное расширение файла " + extension + " не поддерживается!";

            var fileInfo = new FileInfo(filePath);
            int fileSize = (int)fileInfo.Length;
            if (fileSize > MaxOneFileSize)
                return "Размер загружаемого файла " + fileSize.ToString() + "байт превышает лимит в " + MaxOneFileSize.ToString() + " байт";

            int commonSize = commonSizeOfLoadedFiles + fileSize;
            if (commonSize > MaxAllFilesSize)
                return "Общий размер файлов в " + commonSize.ToString() + "байт превышает лимит в " + MaxAllFilesSize.ToString() + " байт";

            commonSizeOfLoadedFiles = commonSize;
            return "";
        }

        public async Task<int> LoadFileToServer(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string fileExtension = Path.GetExtension(fileName);
            HttpRequestMessage httpLoadRequestMessage = new HttpRequestMessage(HttpMethod.Post, httpServerURL + fileName);
            MultipartFormDataContent multipartFormDataContent = PrepareFileContentToSending(filePath);
            httpLoadRequestMessage.Content = multipartFormDataContent;
            httpLoadRequestMessage.Headers.Add("RealFileName", fileName);

            int additionalUniqueValue = 1;
            int resultFileID = -1;
            HttpResponseMessage httpLoadResponseMessage = await client.SendAsync(httpLoadRequestMessage);
            string resultFileIDstring = await httpLoadResponseMessage.Content.ReadAsStringAsync();
            resultFileID = int.Parse(resultFileIDstring);
            if (resultFileID == -1)
            {
                while (resultFileID == -1)
                {
                    string newFileName = fileName + "(" + additionalUniqueValue.ToString() + ")" + fileExtension;
                    HttpRequestMessage httpLoadRepeatRequestMessage = new HttpRequestMessage(HttpMethod.Post, httpServerURL + newFileName);
                    httpLoadRepeatRequestMessage.Content = PrepareFileContentToSending(filePath);
                    httpLoadRepeatRequestMessage.Headers.Add("RealFileName", fileName);
                    additionalUniqueValue++;

                    HttpResponseMessage httpLoadRepeatResponseMessage = await client.SendAsync(httpLoadRepeatRequestMessage);
                    string repeatedResultFileIDstring = await httpLoadRepeatResponseMessage.Content.ReadAsStringAsync();
                    resultFileID = int.Parse(repeatedResultFileIDstring);
                }
            }
            return resultFileID;
        }

        public async Task<string> DeleteLoadedToServerFile(int fileID)
        {
            HttpRequestMessage httpDeleteRequestMessage = new HttpRequestMessage(HttpMethod.Delete, httpServerURL + fileID);
            HttpResponseMessage httpDeleteResponseMessage = await client.SendAsync(httpDeleteRequestMessage);
            if (httpDeleteResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                LoadedToServerFiles.Remove(fileID);
                return "Файл успешно удалён!";
            }
            else if (httpDeleteResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return "Не удалось найти файл!";
            }
            else
                return "Что-то пошло не так!";     
        }

        public async Task<string> GetFileInformation(int fileID)
        {
            HttpRequestMessage httpGetInfoRequestMessage = new HttpRequestMessage(HttpMethod.Head, httpServerURL + fileID);
            HttpResponseMessage httpGetInfoResponseMessage = await client.SendAsync(httpGetInfoRequestMessage);
            if (httpGetInfoResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                IEnumerable<String> fileName = httpGetInfoResponseMessage.Headers.GetValues("FileName");
                requestedFileName = fileName.First();
                IEnumerable<String> fileSize = httpGetInfoResponseMessage.Headers.GetValues("FileSize");
                requestedFileSize = fileSize.First();
                return "Информация о файле успешно представлена!";
            }
            else if (httpGetInfoResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                requestedFileName = "";
                requestedFileSize = "";
                return "Не удалось найти файл!";
            }
            else
                return "Что-то пошло не так!";
        }

        public async Task<string> GetFileToSave(int fileID)
        {
            HttpRequestMessage httpSaveFileRequestMessage = new HttpRequestMessage(HttpMethod.Get, httpServerURL + fileID);
            HttpResponseMessage httpSaveFileResponseMessage = await client.SendAsync(httpSaveFileRequestMessage);
            if (httpSaveFileResponseMessage.StatusCode == System.Net.HttpStatusCode.OK)
            {
                requestedFileContent = await httpSaveFileResponseMessage.Content.ReadAsByteArrayAsync();
                return "";
            }
            else if (httpSaveFileResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return "Не удалось найти файл!";
            }
            else
                return "Что-то пошло не так!";
        }
    }
}

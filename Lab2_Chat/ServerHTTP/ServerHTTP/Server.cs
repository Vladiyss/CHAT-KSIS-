﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Web;
using System.IO;

namespace ServerHTTP
{
    class Server
    {
        HttpListener HTTPListener;
        Dictionary<int, string> Files = new Dictionary<int, string>();
        Dictionary<int, string> RealFilesFromClient = new Dictionary<int, string>();
        int currentFileID = 0;
        const int failedToSaveFile = -1;

        public const string pathToSaveFiles = "G:/4term/KSIS_LK/";
        const int HttpPort = 9097;
        string HTTPListenerPrefix = "http://*:" + HttpPort.ToString() + "/";

        public Server()
        {
            HTTPListener = new HttpListener();
            Thread listeningThread = new Thread(Listening);
            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        public static void StartServer()
        {
            var server = new Server();
            Console.ReadLine();
        }

        bool IsFileWithThisIDExists(int fileID)
        {
            if (Files.ContainsKey(fileID))
                return true;
            else
                return false;
        }

        byte[] GetRequestedFileContent(int fileId)
        {
            string filePath = pathToSaveFiles + Files[fileId];
            byte[] fileContent;
            FileStream fileToGetContent = new FileStream(filePath, FileMode.Open);
            try
            {
                fileContent = new byte[fileToGetContent.Length];
                fileToGetContent.Read(fileContent, 0, (int)fileToGetContent.Length);
            }
            finally
            {
                fileToGetContent.Close();
            }
            return fileContent;
        }

        void ManageHttpGETRequest(HttpListenerContext httpListenerContext)
        {
            var fileId = int.Parse(httpListenerContext.Request.Url.LocalPath.Substring(1));
            if (IsFileWithThisIDExists(fileId))
            {
                httpListenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
                byte[] requestedFileContent = GetRequestedFileContent(fileId);
                httpListenerContext.Response.OutputStream.Write(requestedFileContent, 0, requestedFileContent.Length);

                Console.WriteLine("Содержимое файла с ID " + fileId.ToString() + " передано на скачивание");
            }
            else
            {
                httpListenerContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            httpListenerContext.Response.OutputStream.Close();
        }

        void ManageHttpHEADRequest(HttpListenerContext httpListenerContext)
        {
            var fileId = int.Parse(httpListenerContext.Request.Url.LocalPath.Substring(1));
            if (IsFileWithThisIDExists(fileId))
            {
                httpListenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
                httpListenerContext.Response.Headers.Add("FileName", RealFilesFromClient[fileId]);

                string filePath = pathToSaveFiles + Files[fileId];
                FileStream fileToGetSize = new FileStream(filePath, FileMode.Open);
                long fileSize = fileToGetSize.Length;
                fileToGetSize.Close();
                httpListenerContext.Response.Headers.Add("FileSize", fileSize.ToString());
            }
            else
            {
                httpListenerContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            httpListenerContext.Response.OutputStream.Close();
        }


        bool IsFileWithThisNameExists(string fileName)
        {
            foreach (var name in Files)
            {
                if (fileName == name.Value)
                    return true;
            }
            return false;
        }

        byte[] GetFileContent(HttpListenerContext httpListenerContext)
        {
            var streamReader = new StreamReader(httpListenerContext.Request.InputStream, httpListenerContext.Request.ContentEncoding);
            string content = streamReader.ReadToEnd();
            return httpListenerContext.Request.ContentEncoding.GetBytes(content);
        }

        int AddFile(string fileName, byte[] fileContent)
        {
            string filePath = pathToSaveFiles + fileName;
            FileStream outputFile = new FileStream(filePath, FileMode.Create);
            try
            {
                outputFile.Write(fileContent, 0, fileContent.Length);
            }
            finally
            {
                outputFile.Close();
            }

            Files.Add(currentFileID, fileName);
            currentFileID++;
            return currentFileID - 1;
        }

        void ManageHttpPOSTRequest(HttpListenerContext httpListenerContext)
        {
            string fileName = httpListenerContext.Request.Url.LocalPath.Substring(1);
            //string fileName = httpListenerContext.Request.Headers.Get("FileName");
            if (IsFileWithThisNameExists(fileName))
            {
                httpListenerContext.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                httpListenerContext.Response.OutputStream.Write(Encoding.ASCII.GetBytes(failedToSaveFile.ToString()), 0,
                Encoding.ASCII.GetBytes(failedToSaveFile.ToString()).Length);
                Console.WriteLine("Имя " + fileName + "уже существует");
            }
            else
            {
                byte[] fileContent = GetFileContent(httpListenerContext);
                int newFileID = AddFile(fileName, fileContent);
                RealFilesFromClient.Add(newFileID, httpListenerContext.Request.Headers.Get("RealFileName"));

                httpListenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
                httpListenerContext.Response.OutputStream.Write(Encoding.ASCII.GetBytes(newFileID.ToString()), 0,
                Encoding.ASCII.GetBytes(newFileID.ToString()).Length);
                Console.WriteLine("Файл " + fileName + "был загружен на сервер под ID " + newFileID.ToString());
            }
            httpListenerContext.Response.OutputStream.Close();
        }


        void ManageHttpDELETERequest(HttpListenerContext httpListenerContext)
        {
            var fileId = int.Parse(httpListenerContext.Request.Url.LocalPath.Substring(1));
            if (IsFileWithThisIDExists(fileId))
            {
                httpListenerContext.Response.StatusCode = (int)HttpStatusCode.OK;
                var filePath = pathToSaveFiles + Files[fileId];
                File.Delete(filePath);
                Files.Remove(fileId);
                RealFilesFromClient.Remove(fileId);
                Console.WriteLine("Файл с ID " + fileId + " успешно удалён!");
            }
            else
            {
                httpListenerContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            httpListenerContext.Response.OutputStream.Close();
        }

        void DefineHttpMethod(HttpListenerContext httpListenerContext)
        {
            Console.WriteLine("Получили метод " + httpListenerContext.Request.HttpMethod);
            if (httpListenerContext.Request.HttpMethod == "GET")
            {
                ManageHttpGETRequest(httpListenerContext);
            }
            else if (httpListenerContext.Request.HttpMethod == "HEAD")
            {
                ManageHttpHEADRequest(httpListenerContext);
            }
            else if (httpListenerContext.Request.HttpMethod == "POST")
            {
                ManageHttpPOSTRequest(httpListenerContext);
            }
            else if (httpListenerContext.Request.HttpMethod == "DELETE")
            {
                ManageHttpDELETERequest(httpListenerContext);
            }
            else
                Console.WriteLine("Такой не обрабатывается!");
        }

        void Listening()
        {
            HTTPListener.Prefixes.Add(HTTPListenerPrefix);
            HTTPListener.Start();
            Console.WriteLine("Сервер HTTP слушает");

            while (true)
            {
                HttpListenerContext httpListenerContext = HTTPListener.GetContext();
                DefineHttpMethod(httpListenerContext);
            }
        }
    }
}

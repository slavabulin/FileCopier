using System;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileCopier;

namespace FileCopierTests
{
    [TestClass]
    public class FileCopierTests
    {
        [TestMethod]
        public void ConfigParser_GetFilePath_WrongFilePath_shouldFail()
        {
            //Передадим на вход GetFilePath пустую строчку
            //должны получить исключение
            ConfigParser parser = new ConfigParser();
            try
            {
                parser.GetFilePath(String.Empty);
            }
            catch (Exception)
            {
                //если получили - тест пройден
                return;
            }            
            Assert.Fail();
        }

        [TestMethod]
        public void ConfigParser_GetFilePath_RightFilePath_shouldPass()
        {
            //------------------------------------------
            //подготовим файл конфигурации, который дадим на вход ConfigParser

            string filename = "file.txt";
            string configname = "config.xml";
            FileData fileDataFromParser = new FileData();
            
            FileData fileData = new FileData();
            TextWriter writer = new StreamWriter(configname);

            fileData.paths = new FileData.Path[1];
            for (int i = 0; i < fileData.paths.Length; i++)
            {
                fileData.paths[i] = new FileData.Path();
                fileData.paths[i].FileName = filename;
                fileData.paths[i].From = "c:/";
                fileData.paths[i].To = "d:/";
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileData));
            try
            {
                xmlSerializer.Serialize(writer, fileData);
            }
            catch (ApplicationException e)
            {
                Console.WriteLine("Не могу сериализовать файл конфигурации; " + e.Message);
                Assert.Fail();
            }

            writer.Dispose();
            //------------------------------------------

            ConfigParser parser = new ConfigParser();
            

            try
            {
                //сравним то, что было дано на вход парсеру и то, что вышло
                //если значения одинаковы - тест пройден
                fileDataFromParser.paths = parser.GetFilePath(configname);
                if (fileData.paths[0].FileName == fileDataFromParser.paths[0].FileName
                    && fileData.paths[0].From == fileDataFromParser.paths[0].From
                    && fileData.paths[0].To == fileDataFromParser.paths[0].To)
                    return;
                else
                {
                    Assert.Fail();
                }
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                //убираем созданное для теста
                if(File.Exists("config.xml"))
                {
                    File.Delete("config.xml");
                }
            }
        }

        [TestMethod]
        public void FileManager_CopyFile_RightPath_shouldPass()
        {
            //создадим директории откуда, куда копировать и файл уоторый копировать
            DirectoryInfo dirFrom = null, dirTo = null;
            FileStream fs = null;
            string filename = "file.txt";

            try
            {
                string copyFrom = Guid.NewGuid().ToString();
                string copyTo = Guid.NewGuid().ToString();
                dirFrom = Directory.CreateDirectory(copyFrom);
                dirTo = Directory.CreateDirectory(copyTo);
                
                fs = File.Create(copyFrom + "/" + filename);
                fs.Dispose();
                FileData fileData = new FileData();
                fileData.paths = new FileData.Path[1];
                fileData.paths[0] = new FileData.Path();
                fileData.paths[0].FileName = filename;
                fileData.paths[0].From = copyFrom;
                fileData.paths[0].To = copyTo;
                //копируем, если возникают исключения - тест не пройден
                try
                {
                    FileManager mngr = new FileManager();
                    mngr.CopyFile(fileData.paths, true);
                }
                catch (Exception e)
                {
                    Assert.Fail();
                }
            }
                catch(Exception e)
            {
                Assert.Fail();
            }
            finally
            {
                //убираем созданное для теста
                if (dirFrom.Exists) dirFrom.Delete(true);
                if (dirTo.Exists) dirTo.Delete(true);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace FileCopier
{
    class Program
    {
        static void Main(string[] args)
        {
            //Имя файла, в котором содержится конфигурация копируемых файлов
            const string configPath = "cfg.xml";
            //флаг, указывающий, перезаписывать ли файл, если файл с таким именем уже есть в целевой директории
            bool overwriteExistingFile = false;

            Logger.InitLogger();

            //При запуске с ключом "-o" - программа перезаписывает уже существующие файлы.
            //При запуске без ключа не перезаписывает файл и пишет в лог ошибку, что такой файл уже существует.
            if (args.Contains("-o")) overwriteExistingFile = true;

            FileData.Path[] filePaths = new ConfigParser().GetFilePath(configPath);
            FileManager fileManager = new FileManager();

            fileManager.CopyFile(filePaths, overwriteExistingFile);
            Logger.Log.Info("Выполнение программы завершено.");
        }
    }

    public class ConfigParser
    {
        public FileData.Path[] GetFilePath(string cfgFilePath)
        {
            FileData fileData = new FileData();

            using (FileStream fs = new FileStream(cfgFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(FileData));
                try
                {
                    fileData = (FileData)xmlSerializer.Deserialize(fs);
                }
                catch (InvalidOperationException g)
                {
                    Logger.Log.Error("Ошибка десериализации файла конфигурации - " + g.Message + "\n Выполнение программы прервано.");
                    throw g;
                }
            }
            return fileData.paths;
        }
    }

    public class FileManager
    {
        public void CopyFile(FileData.Path[] paths, bool overwrite)
        {
            Parallel.ForEach(paths, path =>
            {
                var fullPathFrom = new StringBuilder().Append(path.From).Append(path.FileName).ToString();
                var fullPathTo = new StringBuilder().Append(path.To).Append(path.FileName).ToString();
                try
                {
                    //Копирование работает как с локальной файловой системой, так и с рашаренными сетевыми папками
                    //поддерживает адреса вида "\\123.123.123.123\folder\myfile.txt"
                    //и "c:\folder\myfile.txt"
                    System.IO.File.Copy(fullPathFrom, fullPathTo, overwrite);
                }
                //перехватываются ошибки копирования, ошибки путей, ошибки имени файла
                //не останавливая выполнения параллельных потоков
                catch (Exception e)
                {
                    Logger.Log.Error("Ошибка копирования файла - " + path.FileName +  " - " + e.Message);
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Globalization;

namespace iBOX_UpdateCamBase
{
    internal class Program
    {
        private static WebClient FirstWebClient = new WebClient();
        static void Main(string[] args)
        {
            var Culture = new CultureInfo("ru-RU");
            string DeviceName = "iBOX Magnetic WiFi GPS Dual";
            string CamBaseURL = "https://iboxstore.ru/upload/update/baza/regis/iBOX_Magnetic_WiFi_GPS_Dual/edog_data.bin";
            Write("Проверка наличия обновлений базы камер " + DeviceName + " начнётся через", false);
            for (int i = 5; i > 0; i--)
            {
                Console.WriteLine(i.ToString() + "...");
                System.Threading.Thread.Sleep(1000);
            }
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + "edog_data.bin.upd"))
            {
                DownloadFile(CamBaseURL, Directory.GetCurrentDirectory() + "\\" + "edog_data.bin.upd");
            }
            else
            {
                Write("Занято имя для загружаемого файла (edog_data.bin.upd)", true);
                ApplicationExit();
            }
            if (File.Exists(Directory.GetCurrentDirectory() + "\\" + "edog_data.bin"))
            {
                if (CalculateSHA256(Directory.GetCurrentDirectory() + "\\" + "edog_data.bin") != CalculateSHA256(Directory.GetCurrentDirectory() + "\\" + "edog_data.bin.upd"))
                {
                    Write("Загружена новая версия базы камер", true);
                    string DateFile = File.GetLastWriteTime(Directory.GetCurrentDirectory() + "\\" + "edog_data.bin").ToString();
                    DateFile = DateFile.Replace(" ", "_");
                    DateFile = DateFile.Replace(":", "-");
                    File.Move(Directory.GetCurrentDirectory() + "\\" + "edog_data.bin", Directory.GetCurrentDirectory() + "\\" + "edog_data" + "_" + DateFile + ".bin");
                    File.Move(Directory.GetCurrentDirectory() + "\\" + "edog_data.bin.upd", Directory.GetCurrentDirectory() + "\\" + "edog_data.bin");
                }
                else
                {
                    File.Delete(Directory.GetCurrentDirectory() + "\\" + "edog_data.bin.upd");
                    Write("Обновления базы камер отсутствуют", true);
                }
            }
            else
            {
                File.Move(Directory.GetCurrentDirectory() + "\\" + "edog_data.bin.upd", Directory.GetCurrentDirectory() + "\\" + "edog_data.bin");
                Write("Старой версии базы камер не найдено, загружена последняя версия", true);
            }
            ApplicationExit();
        }
        private static void Write(string text, bool toSelect)
        {
            if (toSelect) Console.WriteLine("--------");
            Console.WriteLine(text);
            if (toSelect) Console.WriteLine("--------");
        }
        private static void DownloadFile(string DwnldLnk, string Path)
        {
            Console.WriteLine("Начало загрузки файла");
            FirstWebClient.DownloadFile(DwnldLnk, Path);
            Console.WriteLine("Загрузка файла завершена");
        }
        private static string CalculateSHA256(string path)
        {
            using (SHA256 FirstSHA256 = SHA256.Create())
            {
                using (FileStream FirstFileStream = new FileStream(path, FileMode.Open))
                {
                    try
                    {
                        FirstFileStream.Position = 0;
                        byte[] HashValue = FirstSHA256.ComputeHash(FirstFileStream);
                        return PrintByteArray(HashValue);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine($"I/O Exception: {e.Message}");
                        return string.Empty;
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.WriteLine($"Access Exception: {e.Message}");
                        return string.Empty;
                    }
                }
            }
        }
        private static string PrintByteArray(byte[] array)
        {
            string hash = string.Empty;
            for (int i = 0; i < array.Length; i++)
            {
                hash += $"{array[i]:X2}";
            }
            return hash;
        }

        private static void ApplicationExit()
        {
            Write("Введите любой символ для выхода...", false);
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace NiceHashMiner.Configs.ConfigJsonFile
{
    public abstract class ConfigFile<T> where T : class
    {
        // statics/consts
        private const string TagFormat = "ConfigFile<{0}>";

        private readonly string _confFolder; // = @"configs\";
        private readonly string _tag;

        private void CheckAndCreateConfigsFolder()
        {
            try
            {
                if (Directory.Exists(_confFolder) == false)
                {
                    Directory.CreateDirectory(_confFolder);
                }
            }
            catch { }
        }

        // member stuff
        protected string FilePath;

        protected string FilePathOld;

        protected ConfigFile(string iConfFolder, string fileName, string fileNameOld)
        {
            _confFolder = iConfFolder;
            if (fileName.Contains(_confFolder))
            {
                FilePath = fileName;
            }
            else
            {
                FilePath = _confFolder + fileName;
            }
            if (fileNameOld.Contains(_confFolder))
            {
                FilePathOld = fileNameOld;
            }
            else
            {
                FilePathOld = _confFolder + fileNameOld;
            }
            _tag = string.Format(TagFormat, typeof(T).Name);
        }

        public bool IsFileExists()
        {
            return File.Exists(FilePath);
        }

        public T ReadFile()
        {
            CheckAndCreateConfigsFolder();
            T file = null;
            try
            {
                if (File.Exists(FilePath))
                {
                    file = JsonConvert.DeserializeObject<T>(File.ReadAllText(FilePath), Globals.JsonSettings);
                }
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint(_tag, $"ReadFile {FilePath}: exception {ex}");
                file = null;
            }
            return file;
        }

        public void Commit(T file)
        {
            CheckAndCreateConfigsFolder();
            if (file == null)
            {
                Helpers.ConsolePrint(_tag, $"Commit for FILE {FilePath} IGNORED. Passed null object");
                return;
            }
            try
            {
                WriteAllTextWithBackup(FilePath, JsonConvert.SerializeObject(file, Formatting.Indented));
            }
            catch (Exception ex)
            {
                 Helpers.ConsolePrint(_tag, $"Commit {FilePath}: exception {ex.ToString()}");
            }
        }

        public static void WriteAllTextWithBackup(string FilePath, string contents)
        {
            string path = FilePath;
            var tempPath = FilePath + ".tmp";
            // create the backup name
            var backup = FilePath + ".backup";

            // delete any existing backups
            try
            {
                if (File.Exists(backup))
                    File.Delete(backup);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }

            // get the bytes
            var data = Encoding.ASCII.GetBytes(contents);

            // write the data to a temp file
            try
            {
                var tempFile = File.Create(tempPath, 4096, FileOptions.WriteThrough);
                    tempFile.Write(data, 0, data.Length);
                tempFile.Flush();
                tempFile.Close();
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("File.Create", ex.ToString());
            }

            //copy file
            try
            {
                if (File.Exists(path)) File.Delete(path);
                System.IO.File.Move(tempPath, path);
                //File.Copy(tempPath, path, false);
            }
            catch (Exception ex)
            {
                Helpers.ConsolePrint("WriteAllTextWithBackup", ex.ToString());
            }
            return;
        }

        public void CreateBackup()
        {
            //Helpers.ConsolePrint(_tag, $"Backing up {FilePath} to {FilePathOld}..");
            try
            {
                if (File.Exists(FilePathOld))
                    File.Delete(FilePathOld);
                File.Copy(FilePath, FilePathOld, true);
            }
            catch { }
        }
        public void RestoreFromBackup()
        {
            Helpers.ConsolePrint(_tag, $"Restoring from {FilePathOld} to {FilePath}..");
            try
            {
                if (File.Exists(FilePath))
                    File.Delete(FilePath);
                File.Copy(FilePathOld, FilePath, true);
            }
            catch { }
        }
    }
}

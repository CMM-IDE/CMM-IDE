using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IDE_UI.Helper
{
    class FileHelper
    {
        private static string filter = "CMM代码文件（*.cmm)|*.cmm";


        public static async Task<string> ReadStringFromFileAsync(string path)
        {
            using (var sr = new StreamReader(path)) {
                    return await sr.ReadToEndAsync();
            }
        }

        public static string PickFileAsync()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == false) {
                return null;
            }

            return dialog.FileName;
        }

        public static string SaveFileAsync()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == false) {
                return null;
            }

            return dialog.FileName;
        }

        public static async Task<bool> WriteFileAsync(string path, string code)
        {
            try {
                
                using (StreamWriter outputFile = new StreamWriter(path)) {
                    await outputFile.WriteAsync(code);
                    return true;
                }
            }
            catch {
                return false;
            }
        }



    }
}

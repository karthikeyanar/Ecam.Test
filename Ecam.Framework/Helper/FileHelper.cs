using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecam.Framework {
    public class FileHelper {

        public static string GetValidFileName(string rawFileName) {

            string fileName = Path.GetFileNameWithoutExtension(rawFileName);

            string ext = Path.GetExtension(rawFileName);

            //special chars not allowed in filename  

            string specialChars = @"\/:*?""<>|#%&.,{}~";

            //Replace special chars in raw filename with empty spaces to make it valid  

            Array.ForEach(specialChars.ToCharArray(),specialChar => fileName = fileName.Replace(specialChar,' '));

            fileName = fileName.Replace(" ","_");//Recommended to remove the empty spaces in filename  

            return fileName + ext;

        }  
    }
}

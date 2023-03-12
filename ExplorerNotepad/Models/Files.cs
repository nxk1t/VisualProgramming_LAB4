using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerNotepad.Models
{
    public class Files : Explorer
    {
        public Files(string Name) : base(Name)
        {
            SourceName = Name;
            Image = "Assets/iconFile.png";
        }

        public Files(FileInfo fileName) : base(fileName.Name)
        {
            SourceName = fileName.FullName;
            Image = "Assets/iconFile.png";
        }
    }
}

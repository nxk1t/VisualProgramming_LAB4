using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerNotepad.Models
{
    public class Directories : Explorer
    {
        public Directories(string Name) : base(Name)
        {
            SourceName = Name;
            Image = "Assets/iconFolder.png";
        }

        public Directories(DirectoryInfo directoryName) : base(directoryName.Name)
        {
            SourceName = directoryName.FullName;
            Image = "Assets/iconFolder.png"; 
        }
    }
}

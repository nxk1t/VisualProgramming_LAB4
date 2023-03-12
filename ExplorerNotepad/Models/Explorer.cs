using ExplorerNotepad.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExplorerNotepad.Models
{
    public abstract class Explorer
    {
        public Explorer(string Name)
        {
            Header = Name;
        }

        public string Header { get; set; }
        public string Image { get; set; }
        public string SourceName { get; set; }
    }
}

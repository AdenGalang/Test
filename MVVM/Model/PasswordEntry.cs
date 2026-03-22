using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManaAlpha.MVVM.Model
{
    public class PasswordEntry
    {
        public string? Title { get; set; }      // e.g. "Gmail"
        public string? Username { get; set; }   // e.g. "ncd@gmail.com"
        public string? Password { get; set; }   // e.g. "hunter2"
    }
}


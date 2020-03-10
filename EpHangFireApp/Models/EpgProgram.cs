using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EpHangFireApp.Models
{
    public class EpgProgram
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime Finish { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Hash { get; set; }
        public string ChannelId { get; set; }

    }
}

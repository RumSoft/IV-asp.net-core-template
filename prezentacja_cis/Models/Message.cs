using System;
using System.Collections.Generic;

namespace prezentacja_cis.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; }

        public virtual Room Room { get; set; }
    }

    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Message> Messsages { get; set; }
    }
}
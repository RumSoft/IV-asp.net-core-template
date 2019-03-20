using System;
using System.Collections.Generic;
using AutoMapper;
using prezentacja_cis.Controllers;

namespace prezentacja_cis.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Text { get; set; }

        public virtual Room Room { get; set; }
    }

    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Message> Messsages { get; set; }
    }

    public class ChatAutoMapperProfile : Profile
    {
        public ChatAutoMapperProfile()
        {
            CreateMap<MessageEntity, Message>()
                .ForMember(prop => prop.Username, opt => opt.MapFrom(src => src.User))
                .ForMember(prop => prop.Text, opt => opt.MapFrom(src => src.Message))
                .ForAllOtherMembers(opt => opt.Ignore());

            CreateMap<Message, MessageEntity>()
                .ForMember(prop => prop.User, opt => opt.MapFrom(src => src.Username))
                .ForMember(prop => prop.Message, opt => opt.MapFrom(src => src.Text));
        }
    }
}




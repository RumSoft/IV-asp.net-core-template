using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentValidation;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prezentacja_cis.Models;

namespace prezentacja_cis.Controllers
{
    [Route("[controller]")]
    public class ChatController
    {
        [HttpGet("{roomId}")]
        public IEnumerable<MessageEntity> Messages(int roomId)
        {
            using (var ctx = new ChatContext())
            {
                var room = ctx.Rooms
                    .Include(x => x.Messsages)
                    .FirstOrDefault(r => r.Id == roomId);
                if (room == null)
                    return new MessageEntity[0];

                return room.Messsages.Select(Mapper.Map<MessageEntity>);
            }
        }

        [HttpGet]
        public IEnumerable<object> Rooms()
        {
            using (var ctx = new ChatContext())
            {
                return ctx.Rooms.ToList().Select((x, i) => new {x.Id, x.Name});
            }
        }

        [HttpPost("{roomId}/[action]")]
        public IActionResult SendMessage(int roomId, MessageEntity m)
        {
            using (var ctx = new ChatContext())
            {
                var room = ctx.Rooms.Include(x => x.Messsages).FirstOrDefault(r => r.Id == roomId);
                if (room == null)
                    return new BadRequestResult();

                var message = Mapper.Map<Message>(m);
                room.Messsages.Add(message);
                ctx.SaveChanges();

                return new OkResult();
            }
        }

        [HttpPost("[action]")]
        public IActionResult AddRoom(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return new BadRequestResult();

            using (var ctx = new ChatContext())
            {
                if (ctx.Rooms.Any(x => x.Name == name))
                    return new BadRequestResult();

                ctx.Rooms.Add(new Room
                {
                    Messsages = new List<Message>(),
                    Name = name
                });
                ctx.SaveChanges();
                return new OkResult();
            }
        }
    }

    [Validator(typeof(MessageEntityValidator))]
    public class MessageEntity
    {
        public string User { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class MessageEntityValidator : AbstractValidator<MessageEntity>
    {
        public MessageEntityValidator()
        {
            RuleFor(x => x.Message).NotEmpty();
            RuleFor(x => x.User).NotEmpty().MinimumLength(5);
        }
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
                .ForMember(prop => prop.Message, opt => opt.MapFrom(src => src.Text))
                .ForMember(prop => prop.SentAt, opt => opt.MapFrom(src => src.SentAt));
        }
    }
}
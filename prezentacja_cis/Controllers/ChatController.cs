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
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<object> Rooms()
        {
            using (var ctx = new ChatContext())
            {
                return ctx.Rooms.AsNoTracking()
                    .ToList()
                    .Select(x => new
                    {
                        x.Id,
                        x.Name
                    });
            }
        }

        [HttpGet("{roomId}")]
        public IEnumerable<MessageEntity> Messages(int roomId)
        {
            using (var ctx = new ChatContext())
            {
                var room = ctx.Rooms.AsNoTracking()
                    .Include(x => x.Messsages)
                    .FirstOrDefault(r => r.Id == roomId);
                if (room == null)
                    return new MessageEntity[0];

                return room.Messsages.Select(Mapper.Map<MessageEntity>);
            }
        }

        [HttpPost("{roomId}/[action]")]
        public IActionResult SendMessage(int roomId, MessageEntity m)
        {
            using (var ctx = new ChatContext())
            {
                var room = ctx.Rooms
                    .Include(x => x.Messsages)
                    .FirstOrDefault(r => r.Id == roomId);
                if (room == null)
                    return BadRequest();

                var message = Mapper.Map<Message>(m);
                room.Messsages.Add(message);
                ctx.SaveChanges();

                return Ok();
            }
        }

        [HttpPost("[action]")]
        public IActionResult AddRoom(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest();

            using (var ctx = new ChatContext())
            {
                if (ctx.Rooms.Any(x => x.Name == name))
                    return BadRequest();

                ctx.Rooms.Add(new Room
                {
                    Messsages = new List<Message>(),
                    Name = name
                });
                ctx.SaveChanges();
                return Ok();
            }
        }
    }

    [Validator(typeof(MessageEntityValidator))]
    public class MessageEntity
    {
        public string User { get; set; }
        public string Message { get; set; }
    }

    public class MessageEntityValidator : AbstractValidator<MessageEntity>
    {
        public MessageEntityValidator()
        {
            RuleFor(x => x.User).NotEmpty().MinimumLength(5);
            RuleFor(x => x.Message).NotEmpty();
        }
    }
}
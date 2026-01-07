using AutoMapper;
using MailNotification.Application.Queries;
using MailNotification.Domain.Entities;
namespace MailNotification.Application.Mappings
{
    public class MailMappingProfile : Profile
    {
        public MailMappingProfile()
        {
            CreateMap<MailLog, MailLogDto>();
        }
    }
}

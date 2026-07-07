using Application.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.User
{
    public class UploadAvatarCommand : IRequest<string>
    {
        public Stream AvatarUploadStream { get; set; }
        public string Mime {  get; set; }

        public string Extension { get; set; }
        
        public string Name { get; set; }

        public long SizeInBytes { get; set; }
        public Guid UserId { get; set; }
    }
}

using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.User
{
    public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand>
    {
        private readonly IUnitOfWork uow;
        private readonly UserManager<Domain.Entities.User> userManager;

        public DeleteStudentCommandHandler(IUnitOfWork uow, UserManager<Domain.Entities.User> userManager)
        {
            this.uow = uow; this.userManager = userManager;
        }

        public async Task Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            Domain.Entities.User user = await uow.UserRepository.Query().Where(u=>u.Id == request.StudentId).Include(u=>u.AvatarFile).FirstAsync(cancellationToken);
            
            if((await userManager.GetRolesAsync(user)).Any(role => !role.Equals("student", StringComparison.CurrentCultureIgnoreCase)))
                throw new InvalidOperationException("Cannot delete a user that isn't exclusively a student!");
            
            user.AvatarFileId = null;
            var avatar = user.AvatarFile;

            if(avatar != null)
                uow.FileMetadataRepository.Delete(avatar);
            
            user.AvatarFile = null;
            
            await userManager.DeleteAsync(user);
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}

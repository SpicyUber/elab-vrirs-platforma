using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.CourseEnrollment
{
    public class UnenrollUsingUserIdCommandHandler : IRequestHandler<UnenrollUsingUserIdCommand>
    {
        private readonly IUnitOfWork uow;

        public UnenrollUsingUserIdCommandHandler(IUnitOfWork uow) 
        {
            this.uow = uow;
        }

        public async Task Handle(UnenrollUsingUserIdCommand request, CancellationToken cancellationToken)
        {
            var enrollment = await uow.CourseEnrollmentRepository.Query()
                                                                 .Where(ce => ce.UserId == request.UserId && ce.CourseId == request.CourseId)
                                                                 .FirstAsync(cancellationToken);

            uow.CourseEnrollmentRepository.Delete(enrollment);
            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}

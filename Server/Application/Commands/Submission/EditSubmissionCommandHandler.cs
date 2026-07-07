using Application.DTOs.Submission;
using Domain.Enums;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Submission
{
    public class EditSubmissionCommandHandler : IRequestHandler<EditSubmissionCommand, SubmissionInfo>
    {
        private readonly IUnitOfWork uow;

        public EditSubmissionCommandHandler(IUnitOfWork uow) => this.uow = uow;

        public async Task<SubmissionInfo> Handle(EditSubmissionCommand request, CancellationToken cancellationToken)
        {
            var submission
                = await uow.SubmissionRepository
                .Query()
                .Where(s => s.Id == request.Id)
                .Include(s => s.Assignment)
                .Include(s => s.Student)
                .FirstAsync(cancellationToken);

            if(submission.Status != SubmissionStatus.Draft)
                throw new InvalidOperationException("Cannot edit a submitted submission!");

            submission.Title = request.Title;
            submission.Description = request.Description;

            if(request.Publish)
                submission.Status = SubmissionStatus.Submitted;

            await uow.SaveChangesAsync(cancellationToken);

            return new SubmissionInfo(submission);
        }
    }
}

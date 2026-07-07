using Application.DTOs.Assignment;
using Domain.Enums;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Assignment
{
    public class EditAssignmentCommandHandler : IRequestHandler<EditAssignmentCommand, AssignmentInfo>
    {
        private readonly IUnitOfWork uow;

        public EditAssignmentCommandHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<AssignmentInfo> Handle(EditAssignmentCommand request, CancellationToken cancellationToken)
        {
            var assignment = await uow.AssignmentRepository.Query()
                                                           .Include(a => a.CreatedByUser)
                                                           .Include(a => a.Course)
                                                           .Where(a => a.Id == request.AssignmentId)
                                                           .FirstOrDefaultAsync(cancellationToken);

            if(assignment == null) throw new InvalidOperationException("Assignment not found.");

            try
            {
                assignment.Title = request.Title;
                assignment.Description = request.Description;

                assignment.Location = request.Location;
                assignment.Category = request.Category;

                assignment.Status = request.Publish ? AssignmentStatus.Published : AssignmentStatus.Draft;

                assignment.AllowMultipleAttempts = request.AllowMultipleAttempts;
                assignment.AllowProjectUpload = request.AllowProjectUpload;

                assignment.OpensAt = request.OpensAt;
                assignment.DueAt = request.DueAt;

                assignment.MaxPoints = request.MaxPoints;
                assignment.MinPoints = request.MinPoints;

                await uow.SaveChangesAsync(cancellationToken);
            }
            catch(Exception)
            {
                throw new InvalidOperationException("Error saving changes.");
            }

            return new(assignment);
        }
    }
}

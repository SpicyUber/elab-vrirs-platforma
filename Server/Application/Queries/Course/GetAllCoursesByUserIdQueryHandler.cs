using Application.DTOs.Course;
using Domain.Enums;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Course
{
    public class GetAllCoursesByUserIdQueryHandler : IRequestHandler<GetAllCoursesByUserIdQuery, List<CourseInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllCoursesByUserIdQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<CourseInfo>> Handle(GetAllCoursesByUserIdQuery request, CancellationToken cancellationToken)
        {
            return await uow.CourseRepository.Query()
                .Include(c => c.CreatedByUser)
                .Include(c => c.Enrollments)
                .Where(c => 
                    c.Enrollments.Any(e => e.UserId == request.UserId 
                    && e.Status == EnrollmentStatus.Active 
                    && (request.Role == null || request.Role.Equals(e.EnrollmentRole)))
                    && c.IsActive)
                .Select(c => new CourseInfo(c))
                .ToListAsync(cancellationToken);
        }
    }
}

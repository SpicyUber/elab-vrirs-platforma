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
    public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, List<CourseInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllCoursesQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<CourseInfo>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            return await uow.CourseRepository.Query()
            .Include(c => c.CreatedByUser)
            .Include(c => c.Enrollments)
            .Select(c => new CourseInfo(c))
            .ToListAsync(cancellationToken);

        }
    }
}

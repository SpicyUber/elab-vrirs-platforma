using Application.DTOs.Course;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Course
{
    public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, CourseInfo>
    {
        private readonly IUnitOfWork uow;

        public GetCourseQueryHandler(IUnitOfWork uow) => this.uow = uow;

        public async Task<CourseInfo> Handle(GetCourseQuery request, CancellationToken cancellationToken)
        {
            return new CourseInfo(uow.CourseRepository.GetById(request.Id));
        }
    }
}

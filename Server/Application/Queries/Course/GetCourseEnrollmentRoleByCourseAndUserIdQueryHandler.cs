using Application.DTOs.CourseEnrollment;
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
    public class GetCourseEnrollmentRoleByCourseAndUserIdQueryHandler : IRequestHandler<GetCourseEnrollmentRoleByCourseAndUserIdQuery, EnrollmentRole>
    {
        private readonly IUnitOfWork uow;

        public GetCourseEnrollmentRoleByCourseAndUserIdQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<EnrollmentRole> Handle(GetCourseEnrollmentRoleByCourseAndUserIdQuery request, CancellationToken cancellationToken)
        {
            return (await uow.CourseEnrollmentRepository.Query().Where((e) => e.CourseId == request.CourseId && e.UserId == request.UserId).FirstAsync(cancellationToken)).EnrollmentRole;
        }
    }
}

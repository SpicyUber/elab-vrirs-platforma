using Application.DTOs.ProjectAsset;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.ProjectAsset
{
    public class GetAllProjectAssetsBySubmissionIdQueryHandler : IRequestHandler<GetAllProjectAssetsBySubmissionIdQuery, List<ProjectAssetInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllProjectAssetsBySubmissionIdQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<ProjectAssetInfo>> Handle(GetAllProjectAssetsBySubmissionIdQuery request, CancellationToken cancellationToken)
        {
            return await uow.ProjectAssetRepository
                .Query()
                .Where(a => a.SubmissionId == request.Id)
                .Select(a => new ProjectAssetInfo(a))
                .ToListAsync(cancellationToken);
        }
    }
}

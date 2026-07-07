using Application.DTOs.AssignmentAsset;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.AssignmentAsset
{
    public class GetAllAssignmentAssetsByAssignmentIdQueryHandler : IRequestHandler<GetAllAssignmentAssetsByAssignmentIdQuery, List<AssignmentAssetInfo>>
    {
        private readonly IUnitOfWork uow;

        public GetAllAssignmentAssetsByAssignmentIdQueryHandler(IUnitOfWork uow)
        {
            this.uow = uow;
        }

        public async Task<List<AssignmentAssetInfo>> Handle(GetAllAssignmentAssetsByAssignmentIdQuery request, CancellationToken cancellationToken)
        {
            return await uow.AssignmentAssetRepository.Query()
                                                      .Include(a => a.FileMetadata)
                                                      .Where(a => a.AssignmentId == request.Id)
                                                      .Select(a => new AssignmentAssetInfo(a))
                                                      .ToListAsync(cancellationToken);
        }
    }
}

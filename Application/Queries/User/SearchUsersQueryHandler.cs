using Application.DTOs.User;
using Infrastructure.Persistence.FileService.Interfaces;
using Infrastructure.Persistence.UnitOfWork.Interface;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.User
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, UserSearchResultPage>
    {
        private readonly IUnitOfWork uow;
        private readonly IFileService<Stream> fileService;
        private readonly UserManager<Domain.Entities.User> userManager;

        public SearchUsersQueryHandler(IUnitOfWork uow, IFileService<Stream> fileService, UserManager<Domain.Entities.User> userManager)
        {
            this.uow = uow;
            this.fileService = fileService;
            this.userManager = userManager;
        }

        public async Task<UserSearchResultPage> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Domain.Entities.User> userQuery = uow.UserRepository.Query()
                                              .Include(u => u.AvatarFile)
                                              .OrderBy(u => u.Email);

            if(!string.IsNullOrWhiteSpace(request.SearchParams.FullName))
                userQuery = userQuery.Where(u => u.FullName == request.SearchParams.FullName);

            if(!string.IsNullOrWhiteSpace(request.SearchParams.Index))
                userQuery = userQuery.Where(u => u.IndexNumber == request.SearchParams.Index);

            if(!string.IsNullOrWhiteSpace(request.SearchParams.Email))
                userQuery = userQuery.Where(u => u.Email == request.SearchParams.Email);

            int userCount = await userQuery.CountAsync(cancellationToken);

            request.SearchParams.EntiresPerPage = Math.Clamp(request.SearchParams.EntiresPerPage, 1, 20);

            int totalPages = Math.Max(
                1,
                (userCount + request.SearchParams.EntiresPerPage - 1)
                / request.SearchParams.EntiresPerPage);

            request.SearchParams.PageNumber = Math.Clamp(request.SearchParams.PageNumber, 1, totalPages);

            var users = await userQuery.Skip((request.SearchParams.PageNumber - 1) * request.SearchParams.EntiresPerPage)
                                       .Take(request.SearchParams.EntiresPerPage)
                                       .ToListAsync(cancellationToken);

            List<UserProfileInfo> results = new();

            foreach(var u in users)
            {
                results.Add(new(u, await GetAvatarInBase64(u, cancellationToken), string.Join(',', await userManager.GetRolesAsync(u))));
            }

            return new()
            {
                UserProfiles = results,
                EntiresPerPage = request.SearchParams.EntiresPerPage,
                MaxPages = totalPages,
                PageNumer = request.SearchParams.PageNumber
            };
        }

        private async Task<string> GetAvatarInBase64(Domain.Entities.User user, CancellationToken cancellationToken)
        {
            if(user?.AvatarFile?.StoragePath == null) return null;

            using Stream stream = await fileService.DownloadAsync(user.AvatarFile.StoragePath, cancellationToken);
            using MemoryStream memoryStream = new();

            await stream.CopyToAsync(memoryStream, cancellationToken);

            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}

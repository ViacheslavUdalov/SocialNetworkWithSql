using ASP.SecondSocialWithSQL.DTOS;
using ASP.SecondSocialWithSQL.Entities;
using ASP.SecondSocialWithSQL.Helpers;
using ASP.SecondSocialWithSQL.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ASP.SecondSocialWithSQL.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public UserRepository(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public void Update(AppUser user)
    {
        _dataContext.Entry(user).State = EntityState.Modified;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _dataContext.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _dataContext.Users
            .Include(p => p.Photos)
            .ToListAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _dataContext.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _dataContext.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<PageList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
        //AsQueryable() - это метод, который преобразует коллекцию в IQueryable.
        //IQueryable представляет собой интерфейс для выполнения запросов к данным,
        //который позволяет оптимизировать запросы к источнику данных.
        var query = _dataContext.Users.AsQueryable();
          
       query = query.Where(u => u.UserName != userParams.CurrentUsername);
       query = query.Where(u => u.Gender == userParams.Gender);

       var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
       var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
       
       query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

       query = userParams.OrderBy switch
       {
           "created" => query.OrderByDescending(u => u.Created),
           _ => query.OrderByDescending(u => u.LastActive)
       };
       
       return await PageList<MemberDto>
           //ProjectTo() является частью библиотеки AutoMapper и используется для проекции объектов на другой тип.
           //Метод AsNoTracking() позволяет отключить отслеживание изменений для
           //определенного запроса. Это означает, что Entity Framework не
           //будет отслеживать изменения, которые вы вносите в объекты, полученные из этого запроса.
           //Это может улучшить производительность и уменьшить использование памяти
           .CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(),
           userParams.PageNumber, userParams.PageSize);
    }

    public async Task<MemberDto> GetMemberAsync(string username)
    {
        return await _dataContext.Users.Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }
}
using ASP.SecondSocialWithSQL.Extenstions;
using ASP.SecondSocialWithSQL.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASP.SecondSocialWithSQL.Helpers;
//интерфейс IAsyncActionFilter для создания фильтра, который выполняет дополнительную логику перед выполнением контроллерного действия
public class LogUserActivity : IAsyncActionFilter
{
    //ActionExecutingContext предоставляет доступ к данным
    //, связанным с текущим запросом и действием, что позволяет выполнять дополнительную логику перед выполнением действия.
    // ActionExecutionDelegate next - это делегат, который представляет следующий этап в цепочке выполнения фильтров.
    // Он используется для передачи контроль на следующий фильтр или контроллерное действие. 
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // после того, как action был выполнен
        var resultContext = await next();
        
        // перед выполнением запроса, проверяем, авторизовае ли пользователь
        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.GetUserId();
        // получение репозитория из сервисов приложения.
        var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
        var user = await repo.GetUserByIdAsync(userId);
        user.LastActive = DateTime.Now;
        await repo.SaveAllAsync();

    }
}
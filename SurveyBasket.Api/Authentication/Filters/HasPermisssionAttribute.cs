namespace SurveyBasket.Api.Authentication.Filters;

public class HasPermisssionAttribute(string permission) : AuthorizeAttribute(permission)
{

}
